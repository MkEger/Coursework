using System;

namespace TextEditorMK.Models
{
    public class RecentFile
    {
        public int Id { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime LastOpenedAt { get; set; } = DateTime.Now;
        public int OpenCount { get; set; } = 0;

        public void UpdateLastOpened()
        {
            LastOpenedAt = DateTime.Now;
            OpenCount++;
        }
    }
}

