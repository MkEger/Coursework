using System;
using System.IO;
using System.Windows.Forms;
using TextEditorMK.Models;

namespace TextEditorMK.Services
{
    /// <summary>
    /// Service Layer Pattern - Координатор всіх розширених функцій редактора
    /// Інтегрує макроси, сніппети, закладки та інші функції
    /// </summary>
    public class EditorExtensionsService
    {
        private readonly RichTextBox _textBox;
        
        // Сервіси розширень
        public MacroService Macros { get; private set; }
        public SnippetService Snippets { get; private set; }
        public BookmarkService Bookmarks { get; private set; }
        
        // Поточні налаштування
        public string CurrentLanguage { get; private set; } = "text";
        public bool IsExtensionsEnabled { get; set; } = true;

        // Події для інтеграції з MainForm
        public event EventHandler<ExtensionEventArgs> MacroStarted;
        public event EventHandler<ExtensionEventArgs> MacroStopped;
        public event EventHandler<ExtensionEventArgs> SnippetInserted;
        public event EventHandler<ExtensionEventArgs> BookmarkToggled;

        public EditorExtensionsService(RichTextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
            InitializeServices();
        }

        private void InitializeServices()
        {
            try
            {
                Macros = new MacroService(_textBox);
                Snippets = new SnippetService(_textBox);
                Bookmarks = new BookmarkService(_textBox);

                System.Diagnostics.Debug.WriteLine("? Editor extensions initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Failed to initialize editor extensions: {ex.Message}");
                throw new Exception("Failed to initialize editor extensions", ex);
            }
        }

        /// <summary>
        /// Визначити мову програмування за розширенням файлу
        /// </summary>
        public void DetectLanguageFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                SetLanguage("text");
                return;
            }

            try
            {
                string extension = Path.GetExtension(filePath).ToLower();
                
                string language;
                switch (extension)
                {
                    case ".cs":
                        language = "csharp";
                        break;
                    case ".md":
                        language = "markdown";
                        break;
                    case ".html":
                    case ".htm":
                        language = "html";
                        break;
                    case ".js":
                        language = "javascript";
                        break;
                    case ".json":
                        language = "json";
                        break;
                    case ".xml":
                        language = "xml";
                        break;
                    case ".css":
                        language = "css";
                        break;
                    case ".sql":
                        language = "sql";
                        break;
                    default:
                        language = "text";
                        break;
                }

                SetLanguage(language);
                System.Diagnostics.Debug.WriteLine($"? Language detected: {language} from {extension}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Error detecting language: {ex.Message}");
                SetLanguage("text");
            }
        }

        /// <summary>
        /// Встановити мову програмування
        /// </summary>
        public void SetLanguage(string language)
        {
            CurrentLanguage = language?.ToLower() ?? "text";
            System.Diagnostics.Debug.WriteLine($"? Language set to: {CurrentLanguage}");
        }

        /// <summary>
        /// Розумна вставка сніппету на основі контексту
        /// </summary>
        public void TriggerSmartSnippet(string trigger)
        {
            if (!IsExtensionsEnabled || string.IsNullOrEmpty(trigger))
                return;

            try
            {
                var snippet = Snippets.FindSnippetByTrigger(trigger, CurrentLanguage);
                if (snippet != null)
                {
                    int cursorPosition = _textBox.SelectionStart;
                    Snippets.InsertSnippet(snippet, cursorPosition);
                    
                    OnSnippetInserted(snippet.Name, trigger);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Error inserting snippet: {ex.Message}");
            }
        }

        /// <summary>
        /// Почати запис макросу
        /// </summary>
        public void StartMacroRecording(string macroName)
        {
            if (!IsExtensionsEnabled)
                return;

            try
            {
                Macros.StartRecording(macroName);
                OnMacroStarted(macroName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Error starting macro recording: {ex.Message}");
            }
        }

        /// <summary>
        /// Зупинити запис макросу
        /// </summary>
        public void StopMacroRecording()
        {
            if (!IsExtensionsEnabled || !Macros.IsRecording)
                return;

            try
            {
                string macroName = Macros.CurrentMacroName;
                Macros.StopRecording();
                OnMacroStopped(macroName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Error stopping macro recording: {ex.Message}");
            }
        }

        /// <summary>
        /// Перемкнути закладку на поточному рядку
        /// </summary>
        public void ToggleBookmarkAtCursor()
        {
            if (!IsExtensionsEnabled)
                return;

            try
            {
                int currentLine = GetCurrentLineNumber();
                Bookmarks.ToggleBookmark(currentLine);
                OnBookmarkToggled(currentLine);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Error toggling bookmark: {ex.Message}");
            }
        }

        /// <summary>
        /// Перейти до наступної закладки
        /// </summary>
        public bool GoToNextBookmark()
        {
            if (!IsExtensionsEnabled)
                return false;

            try
            {
                int currentLine = GetCurrentLineNumber();
                int? nextLine = Bookmarks.GoToNextBookmark(currentLine);
                return nextLine.HasValue;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Error navigating to next bookmark: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Перейти до попередньої закладки
        /// </summary>
        public bool GoToPreviousBookmark()
        {
            if (!IsExtensionsEnabled)
                return false;

            try
            {
                int currentLine = GetCurrentLineNumber();
                int? prevLine = Bookmarks.GoToPreviousBookmark(currentLine);
                return prevLine.HasValue;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Error navigating to previous bookmark: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Отримати статистику використання розширень
        /// </summary>
        public EditorExtensionsStatistics GetStatistics()
        {
            return new EditorExtensionsStatistics
            {
                CurrentLanguage = CurrentLanguage,
                IsExtensionsEnabled = IsExtensionsEnabled,
                TotalMacros = Macros.GetAllMacros().Count,
                TotalSnippets = Snippets.GetSnippetsForLanguage(CurrentLanguage).Count,
                TotalBookmarks = Bookmarks.GetAllBookmarks().Count,
                IsRecordingMacro = Macros.IsRecording,
                CurrentMacroName = Macros.CurrentMacroName
            };
        }

        private int GetCurrentLineNumber()
        {
            try
            {
                return _textBox.GetLineFromCharIndex(_textBox.SelectionStart) + 1;
            }
            catch
            {
                return 1;
            }
        }

        // Події для інтеграції з UI
        protected virtual void OnMacroStarted(string macroName)
        {
            MacroStarted?.Invoke(this, new ExtensionEventArgs("MacroStarted", macroName));
        }

        protected virtual void OnMacroStopped(string macroName)
        {
            MacroStopped?.Invoke(this, new ExtensionEventArgs("MacroStopped", macroName));
        }

        protected virtual void OnSnippetInserted(string snippetName, string trigger)
        {
            SnippetInserted?.Invoke(this, new ExtensionEventArgs("SnippetInserted", $"{snippetName} ({trigger})"));
        }

        protected virtual void OnBookmarkToggled(int lineNumber)
        {
            BookmarkToggled?.Invoke(this, new ExtensionEventArgs("BookmarkToggled", $"Line {lineNumber}"));
        }
    }

    /// <summary>
    /// Аргументи подій для розширень редактора
    /// </summary>
    public class ExtensionEventArgs : EventArgs
    {
        public string ActionType { get; }
        public string Details { get; }
        public DateTime Timestamp { get; }

        public ExtensionEventArgs(string actionType, string details)
        {
            ActionType = actionType;
            Details = details;
            Timestamp = DateTime.Now;
        }
    }

    /// <summary>
    /// Статистика використання розширень
    /// </summary>
    public class EditorExtensionsStatistics
    {
        public string CurrentLanguage { get; set; }
        public bool IsExtensionsEnabled { get; set; }
        public int TotalMacros { get; set; }
        public int TotalSnippets { get; set; }
        public int TotalBookmarks { get; set; }
        public bool IsRecordingMacro { get; set; }
        public string CurrentMacroName { get; set; }

        public override string ToString()
        {
            return $"Language: {CurrentLanguage} | Macros: {TotalMacros} | " +
                   $"Snippets: {TotalSnippets} | Bookmarks: {TotalBookmarks}" +
                   (IsRecordingMacro ? $" | Recording: {CurrentMacroName}" : "");
        }
    }
}