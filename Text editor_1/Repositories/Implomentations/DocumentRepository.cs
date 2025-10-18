using System.Collections.Generic;
using System.Linq;

using TextEditorMK.Models;
using TextEditorMK.Repositories.Interfaces;

namespace TextEditorMK.Repositories.Implementations
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly List<Document> _documents = new List<Document>();

        public void Add(Document document) => _documents.Add(document);

        public void Update(Document document)
        {
            var existing = GetById(document.Id);
            if (existing != null)
            {
                existing.FileName = document.FileName;
                existing.FilePath = document.FilePath;
                existing.Content = document.Content;
                existing.EncodingId = document.EncodingId;
                existing.ModifiedAt = document.ModifiedAt;
                existing.IsSaved = document.IsSaved;
            }
        }

        public void Delete(int id)
        {
            var doc = GetById(id);
            if (doc != null) _documents.Remove(doc);
        }

        public List<Document> GetAll() => _documents;

        public Document GetById(int id) => _documents.FirstOrDefault(d => d.Id == id);

        public Document GetByPath(string filePath) =>
            _documents.FirstOrDefault(d => d.FilePath == filePath);
    }
}