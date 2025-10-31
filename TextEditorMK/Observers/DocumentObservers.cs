using System;
using System.ComponentModel;
using System.IO;
using System.Timers;
using TextEditorMK.Models;
using TextEditorMK.Services;

namespace TextEditorMK.Observers
{
    /// <summary>
    /// Базовий клас для спостерігачів документа
    /// </summary>
    public abstract class DocumentObserver
    {
        protected Document _document;
        protected bool _isSubscribed = false;

        public DocumentObserver(Document document)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
            Subscribe();
        }

        public virtual void Subscribe()
        {
            if (!_isSubscribed && _document != null)
            {
                _document.ContentChanged += OnContentChanged;
                _document.StatusChanged += OnStatusChanged;
                _document.PropertyChanged += OnPropertyChanged;
                _document.VersionChanged += OnVersionChanged;
                _isSubscribed = true;
            }
        }

        public virtual void Unsubscribe()
        {
            if (_isSubscribed && _document != null)
            {
                _document.ContentChanged -= OnContentChanged;
                _document.StatusChanged -= OnStatusChanged;
                _document.PropertyChanged -= OnPropertyChanged;
                _document.VersionChanged -= OnVersionChanged;
                _isSubscribed = false;
            }
        }

        protected abstract void OnContentChanged(object sender, DocumentChangedEventArgs e);
        protected abstract void OnStatusChanged(object sender, DocumentStatusChangedEventArgs e);
        protected abstract void OnPropertyChanged(object sender, PropertyChangedEventArgs e);
        protected virtual void OnVersionChanged(object sender, DocumentVersionChangedEventArgs e) { }

        public virtual void Dispose()
        {
            Unsubscribe();
        }
    }

    /// <summary>
    /// Спостерігач для автоматичного збереження
    /// </summary>
    public class DocumentAutoSaveObserver : DocumentObserver
    {
        private readonly DocumentService _documentService;
        private readonly System.Timers.Timer _autoSaveTimer;
        private bool _hasUnsavedChanges = false;
        private readonly int _autoSaveIntervalMs;

        public DocumentAutoSaveObserver(Document document, DocumentService documentService, int intervalMs = 30000) 
            : base(document)
        {
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
            _autoSaveIntervalMs = intervalMs;
            
            _autoSaveTimer = new System.Timers.Timer(_autoSaveIntervalMs);
            _autoSaveTimer.Elapsed += AutoSaveTimer_Elapsed;
            _autoSaveTimer.AutoReset = true;
            _autoSaveTimer.Start();
        }

        protected override void OnContentChanged(object sender, DocumentChangedEventArgs e)
        {
            _hasUnsavedChanges = true;
            
            // Перезапустити таймер
            _autoSaveTimer.Stop();
            _autoSaveTimer.Start();
            
            System.Diagnostics.Debug.WriteLine($"[AutoSave] Content changed, reset timer");
        }

        protected override void OnStatusChanged(object sender, DocumentStatusChangedEventArgs e)
        {
            if (e.Message.Contains("saved"))
            {
                _hasUnsavedChanges = false;
                _autoSaveTimer.Stop();
                System.Diagnostics.Debug.WriteLine($"[AutoSave] Document saved, stopped timer");
            }
        }

        protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Document.IsSaved) && _document.IsSaved)
            {
                _hasUnsavedChanges = false;
                _autoSaveTimer.Stop();
            }
        }

        private void AutoSaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_hasUnsavedChanges && !string.IsNullOrEmpty(_document.FilePath))
            {
                try
                {
                    _documentService.SaveDocument(_document);
                    System.Diagnostics.Debug.WriteLine($"[AutoSave] Auto-saved: {_document.FileName}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[AutoSave] Failed: {ex.Message}");
                }
            }
        }

        public override void Dispose()
        {
            _autoSaveTimer?.Stop();
            _autoSaveTimer?.Dispose();
            base.Dispose();
        }
    }

    /// <summary>
    /// Спостерігач для збору статистики
    /// </summary>
    public class DocumentStatisticsObserver : DocumentObserver
    {
        public int CharacterCount { get; private set; }
        public int WordCount { get; private set; }
        public int LineCount { get; private set; }
        public int ParagraphCount { get; private set; }
        public DateTime LastModified { get; private set; }
        public TimeSpan EditingTime { get; private set; }

        private DateTime _editingStartTime = DateTime.Now;
        private DateTime _lastEditTime = DateTime.Now;

        public DocumentStatisticsObserver(Document document) : base(document)
        {
            UpdateStatistics();
        }

        protected override void OnContentChanged(object sender, DocumentChangedEventArgs e)
        {
            UpdateStatistics();
            UpdateEditingTime();
        }

        protected override void OnStatusChanged(object sender, DocumentStatusChangedEventArgs e)
        {
            LastModified = DateTime.Now;
        }

        protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Document.Content))
            {
                UpdateStatistics();
            }
        }

        private void UpdateStatistics()
        {
            if (_document?.Content == null)
            {
                CharacterCount = WordCount = LineCount = ParagraphCount = 0;
                return;
            }

            CharacterCount = _document.Content.Length;
            
            // Підрахунок слів
            if (string.IsNullOrWhiteSpace(_document.Content))
            {
                WordCount = 0;
            }
            else
            {
                var words = _document.Content.Split(new[] { ' ', '\n', '\r', '\t' }, 
                    StringSplitOptions.RemoveEmptyEntries);
                WordCount = words.Length;
            }

            // Підрахунок рядків
            LineCount = _document.Content.Split(new[] { '\n' }, StringSplitOptions.None).Length;
            
            // Підрахунок параграфів
            if (string.IsNullOrWhiteSpace(_document.Content))
            {
                ParagraphCount = 0;
            }
            else
            {
                var paragraphs = _document.Content.Split(new[] { "\n\n", "\r\n\r\n" }, 
                    StringSplitOptions.RemoveEmptyEntries);
                ParagraphCount = paragraphs.Length;
            }
        }

        private void UpdateEditingTime()
        {
            var now = DateTime.Now;
            if (now - _lastEditTime < TimeSpan.FromMinutes(5)) // Тільки якщо менше 5 хвилин між редагуванням
            {
                EditingTime = EditingTime.Add(now - _lastEditTime);
            }
            _lastEditTime = now;
        }

        public string GetStatisticsReport()
        {
            return $"Characters: {CharacterCount:N0}\n" +
                   $"Words: {WordCount:N0}\n" +
                   $"Lines: {LineCount:N0}\n" +
                   $"Paragraphs: {ParagraphCount:N0}\n" +
                   $"Editing time: {EditingTime:hh\\:mm\\:ss}\n" +
                   $"Last modified: {LastModified:yyyy-MM-dd HH:mm:ss}";
        }
    }

    /// <summary>
    /// Спостерігач для збереження історії змін
    /// </summary>
    public class DocumentHistoryObserver : DocumentObserver
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly int _maxHistorySize;
        private readonly System.Collections.Generic.Queue<DocumentSnapshot> _history;

        public DocumentHistoryObserver(Document document, IDocumentRepository documentRepository, int maxHistorySize = 50) 
            : base(document)
        {
            _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
            _maxHistorySize = maxHistorySize;
            _history = new System.Collections.Generic.Queue<DocumentSnapshot>();
        }

        protected override void OnContentChanged(object sender, DocumentChangedEventArgs e)
        {
            // Зберегти знімок документа
            var snapshot = _document.CreateSnapshot();
            _history.Enqueue(snapshot);

            // Обмежити розмір історії
            while (_history.Count > _maxHistorySize)
            {
                _history.Dequeue();
            }

            System.Diagnostics.Debug.WriteLine($"[History] Saved snapshot, history size: {_history.Count}");
        }

        protected override void OnStatusChanged(object sender, DocumentStatusChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[History] Status: {e.Message}");
        }

        protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Можна логувати зміни властивостей
        }

        protected override void OnVersionChanged(object sender, DocumentVersionChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[History] Version changed from {e.OldVersion} to {e.NewVersion}");
        }

        public System.Collections.Generic.IEnumerable<DocumentSnapshot> GetHistory()
        {
            return _history.ToArray();
        }

        public int GetHistoryCount()
        {
            return _history.Count;
        }
    }
}