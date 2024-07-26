using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace search_product_mvc.Services;

public class LuceneService<TEntity> : ILuceneService<TEntity> where TEntity : class
{
    private readonly IndexWriter _writer;
    private readonly Analyzer _analyzer;
    private readonly MultiFieldQueryParser _multiFieldQueryParser;
    public LuceneService()
    {
        var directory = FSDirectory.Open("Lucene_Index");
        _analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
        var indexConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, _analyzer);
        _writer = new IndexWriter(directory, indexConfig);

        var fields = new List<string>();
        Type entityType = typeof(TEntity);
        foreach (var p in entityType.GetProperties())
        {
            if (p.PropertyType == typeof(string))
            {
                fields.Add(p.Name);
            }
        }
        _multiFieldQueryParser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, fields.ToArray(), _analyzer);
    }

    public void Add(TEntity entity)
    {
        var doc = new Document();
        doc.Add(new StringField("EntityType", typeof(TEntity).Name, Field.Store.YES));
        foreach (var p in entity.GetType().GetProperties())
        {
            if (p.PropertyType == typeof(string))
            {
                doc.Add(new TextField(p.Name, p.GetValue(entity)?.ToString() ?? "", Field.Store.YES));
            }
            else
            {
                doc.Add(new StringField(p.Name, p.GetValue(entity)?.ToString() ?? "", Field.Store.YES));
            }
        }
        _writer.AddDocument(doc);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            Add(entity);
        }
    }

    public void Clear()
    {
        _writer.DeleteAll();
    }

    public void Commit()
    {
        _writer.Commit();
    }

    public IEnumerable<TEntity> Search(string query, int maxHits = 10)
    {
        var searcher = new IndexSearcher(_writer.GetReader(applyAllDeletes: true));
        var parsedQuery = _multiFieldQueryParser.Parse(query);
        var booleanQuery = new BooleanQuery
        {
            { parsedQuery, Occur.MUST },
            { new TermQuery(new Term("EntityType", typeof(TEntity).Name)), Occur.MUST }
        };
        var hits = searcher.Search(booleanQuery, maxHits);
        var results = new List<TEntity>();
        foreach (var hit in hits.ScoreDocs)
        {
            var doc = searcher.Doc(hit.Doc);
            var entity = Activator.CreateInstance<TEntity>();
            foreach (var p in entity.GetType().GetProperties())
            {
                if (p.PropertyType == typeof(string))
                {
                    p.SetValue(entity, doc.Get(p.Name));
                }
                else if (p.PropertyType == typeof(Guid))
                {
                    if (Guid.TryParse(doc.Get(p.Name), out Guid guid))
                    {
                        p.SetValue(entity, guid);
                    }
                }
                else
                {
                    // Parse the value to the corresponding type
                    var convertedValue = Convert.ChangeType(doc.Get(p.Name), p.PropertyType);
                    p.SetValue(entity, convertedValue);
                }
            }
            results.Add(entity);
        }
        return results;
    }
}