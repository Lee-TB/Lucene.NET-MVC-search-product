using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using search_product_mvc.Models;

namespace search_product_mvc.Services;

public class LuceneService : ILuceneService
{
    private readonly IndexWriter _writer;
    private readonly IndexSearcher _searcher;
    private readonly Analyzer _analyzer;
    private readonly MultiFieldQueryParser _multiFieldQueryParser;
    public LuceneService()
    {
        var directory = FSDirectory.Open("Lucene_Index");
        _analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
        var indexConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, _analyzer);
        _writer = new IndexWriter(directory, indexConfig);
        _searcher = new IndexSearcher(_writer.GetReader(applyAllDeletes: true));
        var fields = new[] { nameof(Product.Title), nameof(Product.Description) };
        _multiFieldQueryParser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, fields, _analyzer);
    }

    public void Add(Product product)
    {
        var doc = new Document() {
            new StringField(nameof(Product.Id), product.Id.ToString(), Field.Store.YES),
            new TextField(nameof(Product.Title), product.Title.ToString(), Field.Store.YES),
            new TextField(nameof(Product.Description), product.Description ?? "", Field.Store.YES),
            new StringField(nameof(Product.Price), product.Price.ToString(), Field.Store.YES)
        };
        _writer.AddDocument(doc);
    }

    public void AddRange(IEnumerable<Product> products)
    {
        foreach (var product in products)
        {
            var doc = new Document() {
                new StringField(nameof(Product.Id), product.Id.ToString(), Field.Store.YES),
                new TextField(nameof(Product.Title), product.Title.ToString(), Field.Store.YES),
                new TextField(nameof(Product.Description), product.Description ?? "", Field.Store.YES),
                new StringField(nameof(Product.Price), product.Price.ToString(), Field.Store.YES)
            };
            _writer.AddDocument(doc);
        }
    }

    public IEnumerable<Product> Search(string query, int maxHits = 10)
    {
        var luceneQuery = _multiFieldQueryParser.Parse(query);
        var topDocs = _searcher.Search(luceneQuery, maxHits);
        var hits = topDocs.ScoreDocs;
        var products = new List<Product>();
        foreach (var hit in hits)
        {
            var doc = _searcher.Doc(hit.Doc);
            var product = new Product
            {
                Id = Guid.Parse(doc.Get(nameof(Product.Id))),
                Title = doc.Get(nameof(Product.Title)),
                Description = doc.Get(nameof(Product.Description)),
                Price = decimal.Parse(doc.Get(nameof(Product.Price)))
            };
            products.Add(product);
        }
        return products;
    }

    public void Commit()
    {
        _writer.Commit();
    }

    public void Clear()
    {
        _writer.DeleteAll();
    }
}