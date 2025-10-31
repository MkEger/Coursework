using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TextEditorMK.Models;

namespace TextEditorMK.Services
{

    public class SnippetService
    {
        private readonly RichTextBox _textBox;
        private readonly List<CodeSnippet> _snippets;

        public SnippetService(RichTextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
            _snippets = new List<CodeSnippet>();
            
            InitializeDefaultSnippets();
        }

        public List<CodeSnippet> GetSnippetsForLanguage(string language)
        {
            if (string.IsNullOrEmpty(language))
                return new List<CodeSnippet>(_snippets);

            return _snippets.Where(s => s.Language.Equals(language, StringComparison.OrdinalIgnoreCase))
                           .OrderBy(s => s.Name)
                           .ToList();
        }

        public void InsertSnippet(CodeSnippet snippet, int cursorPosition)
        {
            if (snippet == null)
                throw new ArgumentNullException(nameof(snippet));

            try
            {
                string expandedCode = ExpandSnippet(snippet);
                

                var currentFont = _textBox.Font;
                var currentColor = _textBox.ForeColor;
                
                _textBox.SelectionStart = cursorPosition;
                _textBox.SelectionLength = 0;

                _textBox.SelectionFont = currentFont;
                _textBox.SelectionColor = currentColor;
                _textBox.SelectionBackColor = _textBox.BackColor;
                
                _textBox.SelectedText = expandedCode;
                

                _textBox.SelectionStart = cursorPosition + expandedCode.Length;
                _textBox.SelectionLength = 0;

                snippet.UpdateUsage();
                System.Diagnostics.Debug.WriteLine($"? Inserted snippet: {snippet.Name}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting snippet '{snippet.Name}': {ex.Message}", ex);
            }
        }

        public void AddSnippet(CodeSnippet snippet)
        {
            if (snippet == null)
                throw new ArgumentNullException(nameof(snippet));

            _snippets.RemoveAll(s => s.Name == snippet.Name && s.Language == snippet.Language);
            snippet.CreatedDate = DateTime.Now;
            _snippets.Add(snippet);
        }

        public void DeleteSnippet(string snippetName)
        {
            int removed = _snippets.RemoveAll(s => s.Name == snippetName);
            if (removed == 0)
                throw new ArgumentException($"Snippet '{snippetName}' not found");
        }

        public CodeSnippet FindSnippetByTrigger(string trigger, string language)
        {
            if (string.IsNullOrWhiteSpace(trigger))
                return null;

            return _snippets.FirstOrDefault(s => 
                s.Trigger.Equals(trigger, StringComparison.OrdinalIgnoreCase) && 
                (string.IsNullOrEmpty(language) || s.Language.Equals(language, StringComparison.OrdinalIgnoreCase)));
        }

        private string ExpandSnippet(CodeSnippet snippet)
        {
            if (snippet == null)
                return string.Empty;

            string code = snippet.Code;
            
            code = code.Replace("$DATE", DateTime.Now.ToString("yyyy-MM-dd"));
            code = code.Replace("$TIME", DateTime.Now.ToString("HH:mm:ss"));
            code = code.Replace("$USER", Environment.UserName);

            return code;
        }

        private void InitializeDefaultSnippets()
        {

            _snippets.AddRange(new[]
            {
                new CodeSnippet
                {
                    Name = "class",
                    Trigger = "class",
                    Language = "csharp",
                    Description = "Create a new class",
                    Code = "public class $1\n{\n    $0\n}",
                    Category = "Class"
                },
                new CodeSnippet
                {
                    Name = "method", 
                    Trigger = "method",
                    Language = "csharp",
                    Description = "Create a new method",
                    Code = "public $1 $2($3)\n{\n    $0\n}",
                    Category = "Method"
                }
            });

            System.Diagnostics.Debug.WriteLine($"? Initialized {_snippets.Count} default snippets");
        }
    }
}