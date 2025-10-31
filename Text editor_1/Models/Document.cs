using System;
using System.Text;
using TextEditorMK.Models;

namespace TextEditorMK.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int EncodingId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
        public bool IsSaved { get; set; } = false;

        public TextEncoding TextEncoding { get; set; }

        public void SetContent(string content)
        {
            Content = content;
            ModifiedAt = DateTime.Now;
            IsSaved = false;
        }

        public void MarkAsSaved()
        {
            IsSaved = true;
            ModifiedAt = DateTime.Now;
        }

        public void MarkAsModified()
        {
            IsSaved = false;
            ModifiedAt = DateTime.Now;
        }
    }
}

