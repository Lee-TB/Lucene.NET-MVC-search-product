using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace search_product_mvc.Services;

public class LuceneWriter
{
    private readonly IndexWriter _writer;
    private readonly Analyzer _analyzer;
    public LuceneWriter()
    {
        var directory = FSDirectory.Open("Lucene_Index");
        _analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
        var indexConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, _analyzer);
        _writer = new IndexWriter(directory, indexConfig);
    }

    public IndexWriter GetWriter()
    {
        return _writer;
    }

    public Analyzer GetAnalyzer()
    {
        return _analyzer;
    }
}