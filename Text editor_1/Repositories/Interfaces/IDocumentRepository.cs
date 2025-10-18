using System.Collections.Generic;
using TextEditorMK.Models;

namespace TextEditorMK.Repositories.Interfaces
{
    public interface IDocumentRepository
    {
        Document GetById(int id);
        List<Document> GetAll();
        void Add(Document document);
        void Update(Document document);
        void Delete(int id);
        Document GetByPath(string filePath);
    }
}


