using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TextEditorMK.Models;

namespace TextEditorMK.Services
{
    /// <summary>
    /// Service Layer Pattern - Сервіс для управління закладками
    /// </summary>
    public class BookmarkService
    {
        private readonly RichTextBox _textBox;
        private readonly List<Bookmark> _bookmarks;

        public BookmarkService(RichTextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
            _bookmarks = new List<Bookmark>();
        }

        public void AddBookmark(int lineNumber, string description = null)
        {
            if (lineNumber < 1)
                throw new ArgumentException("Line number must be greater than 0");

            if (HasBookmark(lineNumber))
                throw new InvalidOperationException($"Bookmark already exists on line {lineNumber}");

            var bookmark = new Bookmark
            {
                LineNumber = lineNumber,
                Description = description ?? $"Bookmark at line {lineNumber}",
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            _bookmarks.Add(bookmark);
            _bookmarks.Sort((b1, b2) => b1.LineNumber.CompareTo(b2.LineNumber));

            System.Diagnostics.Debug.WriteLine($"? Added bookmark at line {lineNumber}");
        }

        public void RemoveBookmark(int lineNumber)
        {
            var bookmark = _bookmarks.FirstOrDefault(b => b.LineNumber == lineNumber);
            if (bookmark == null)
                throw new ArgumentException($"No bookmark found at line {lineNumber}");

            _bookmarks.Remove(bookmark);
        }

        public void ToggleBookmark(int lineNumber, string description = null)
        {
            if (HasBookmark(lineNumber))
            {
                RemoveBookmark(lineNumber);
            }
            else
            {
                AddBookmark(lineNumber, description);
            }
        }

        public int? GoToNextBookmark(int currentLine)
        {
            var nextBookmark = _bookmarks
                .Where(b => b.IsActive && b.LineNumber > currentLine)
                .OrderBy(b => b.LineNumber)
                .FirstOrDefault();

            if (nextBookmark != null)
            {
                GoToLine(nextBookmark.LineNumber);
                return nextBookmark.LineNumber;
            }

            return null;
        }

        public int? GoToPreviousBookmark(int currentLine)
        {
            var prevBookmark = _bookmarks
                .Where(b => b.IsActive && b.LineNumber < currentLine)
                .OrderByDescending(b => b.LineNumber)
                .FirstOrDefault();

            if (prevBookmark != null)
            {
                GoToLine(prevBookmark.LineNumber);
                return prevBookmark.LineNumber;
            }

            return null;
        }

        public List<Bookmark> GetAllBookmarks()
        {
            return _bookmarks.Where(b => b.IsActive).OrderBy(b => b.LineNumber).ToList();
        }

        public void ClearAllBookmarks()
        {
            _bookmarks.Clear();
        }

        public bool HasBookmark(int lineNumber)
        {
            return _bookmarks.Any(b => b.IsActive && b.LineNumber == lineNumber);
        }

        private void GoToLine(int lineNumber)
        {
            try
            {
                if (lineNumber > _textBox.Lines.Length)
                    return;

                int charIndex = _textBox.GetFirstCharIndexFromLine(lineNumber - 1);
                if (charIndex >= 0)
                {
                    _textBox.SelectionStart = charIndex;
                    _textBox.SelectionLength = 0;
                    _textBox.ScrollToCaret();
                    _textBox.Focus();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navigating to line {lineNumber}: {ex.Message}");
            }
        }
    }
}