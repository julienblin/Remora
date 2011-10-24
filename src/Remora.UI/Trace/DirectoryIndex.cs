using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace Remora.UI.Trace
{
    public class DirectoryIndex
    {
        private LuceneDirectory _directoryStore;
        private Analyzer _analyzer;

        public void AddToIndex(string path, Action<DirectoryIndex, string, int, int> progressCallback)
        {
            _directoryStore = new RAMDirectory();
            _analyzer = new StandardAnalyzer();
            var writer = new IndexWriter(_directoryStore, _analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            var traceFiles = System.IO.Directory.EnumerateFiles(path, "*.xml", SearchOption.TopDirectoryOnly).ToList();
            var currentIndex = 0;
            foreach (var traceFile in traceFiles)
            {
                using(var stream = File.OpenRead(traceFile))
                {
                    var serializableOperation = Remora.Core.Serialization.SerializableOperation.Deserialize(stream);
                    var doc = new Document();
                    doc.Add(new Field("id", serializableOperation.OperationId.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                    writer.AddDocument(doc);
                    progressCallback(this, traceFile, currentIndex, traceFiles.Count);
                    ++currentIndex;
                }
            }

            writer.Optimize();
            writer.Commit();
            writer.Close();
        }
    }
}
