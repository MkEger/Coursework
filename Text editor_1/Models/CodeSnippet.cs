using System;

namespace TextEditorMK.Models
{
    public class CodeSnippet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Trigger { get; set; }
        public string Language { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Category { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public int UsageCount { get; set; }

        public CodeSnippet()
        {
            CreatedDate = DateTime.Now;
            UsageCount = 0;
        }

        public void UpdateUsage()
        {
            LastUsedDate = DateTime.Now;
            UsageCount++;
        }
    }
}