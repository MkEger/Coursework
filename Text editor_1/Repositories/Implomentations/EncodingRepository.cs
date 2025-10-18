using System.Collections.Generic;
using System.Linq;

using TextEditorMK.Repositories.Interfaces;
using TextEditorMK.Models;

namespace TextEditorMK.Repositories.Implementations
{
    public class EncodingRepository : IEncodingRepository
    {
        private readonly List<TextEncoding> _encodings = new List<TextEncoding>()
        {
            new TextEncoding { Id = 1, Name = "UTF-8", CodePage = "utf-8", IsDefault = true },
            new TextEncoding { Id = 2, Name = "UTF-16 LE", CodePage = "utf-16", IsDefault = false }
        };

        public TextEncoding GetById(int id) => _encodings.FirstOrDefault(e => e.Id == id);

        public List<TextEncoding> GetAll() => _encodings;

        public TextEncoding GetDefault() => _encodings.FirstOrDefault(e => e.IsDefault);

        public TextEncoding GetByCodePage(string codePage) =>
            _encodings.FirstOrDefault(e => e.CodePage == codePage);
    }
}