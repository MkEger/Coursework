using System;

namespace TextEditorMK.Models
{

    public class Bookmark
    {
        public int Id { get; set; }
        public int LineNumber { get; set; }
        public string Description { get; set; }
        public string LinePreview { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastAccessedDate { get; set; }
        public int AccessCount { get; set; }
        public bool IsActive { get; set; }

        public Bookmark()
        {
            CreatedDate = DateTime.Now;
            AccessCount = 0;
            IsActive = true;
        }

        public void UpdateAccess()
        {
            LastAccessedDate = DateTime.Now;
            AccessCount++;
        }
    }
}