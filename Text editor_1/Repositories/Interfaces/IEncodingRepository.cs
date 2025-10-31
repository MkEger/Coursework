using System.Collections.Generic;
using TextEditorMK.Models;

namespace TextEditorMK.Repositories.Interfaces
{
    public interface IEncodingRepository
    {
        TextEncoding GetById(int id);
        List<TextEncoding> GetAll();
        TextEncoding GetDefault();
        TextEncoding GetByCodePage(string codePage);
        TextEncoding GetByName(string name);
    }
}



