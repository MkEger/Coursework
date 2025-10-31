using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TextEditorMK.Models
{
    /// <summary>
    /// Observer Pattern - Document з подіями та сповіщеннями
    /// </summary>
    public class Document : INotifyPropertyChanged
    {
        private string _content = string.Empty;
        private bool _isSaved = false;
        private string _fileName = "Untitled.txt";
        private DateTime _modifiedAt = DateTime.Now;
        private int _version = 1;

        public int Id { get; set; }

        public string FileName
        {
            get => _fileName;
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FilePath { get; set; } = string.Empty;

        public string Content
        {
            get => _content;
            private set
            {
                if (_content != value)
                {
                    var oldContent = _content;
                    _content = value;
                    IsSaved = false;
                    ModifiedAt = DateTime.Now;
                    _version++;
                    
                    OnPropertyChanged();
                    OnContentChanged(new DocumentChangedEventArgs(this, oldContent, value));
                }
            }
        }

        public int EncodingId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime ModifiedAt
        {
            get => _modifiedAt;
            set
            {
                if (_modifiedAt != value)
                {
                    _modifiedAt = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSaved
        {
            get => _isSaved;
            set
            {
                if (_isSaved != value)
                {
                    _isSaved = value;
                    OnPropertyChanged();
                    OnStatusChanged(new DocumentStatusChangedEventArgs(this, 
                        value ? "Document saved" : "Document modified"));
                }
            }
        }

        public int Version => _version;
        public TextEncoding TextEncoding { get; set; }

        // ===== OBSERVER EVENTS =====
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DocumentChangedEventArgs> ContentChanged;
        public event EventHandler<DocumentStatusChangedEventArgs> StatusChanged;
        public event EventHandler<DocumentVersionChangedEventArgs> VersionChanged;

        // ===== PUBLIC METHODS =====

        public void SetContent(string content)
        {
            Content = content ?? string.Empty;
        }

        public void MarkAsSaved()
        {
            IsSaved = true;
        }

        public void MarkAsModified()
        {
            IsSaved = false;
            ModifiedAt = DateTime.Now;
        }

        public DocumentSnapshot CreateSnapshot()
        {
            return new DocumentSnapshot
            {
                Id = this.Id,
                FileName = this.FileName,
                FilePath = this.FilePath,
                Content = this.Content,
                Version = this.Version,
                CreatedAt = DateTime.Now
            };
        }

        // ===== NOTIFICATION METHODS =====

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnContentChanged(DocumentChangedEventArgs e)
        {
            ContentChanged?.Invoke(this, e);
            OnVersionChanged(new DocumentVersionChangedEventArgs(this, _version - 1, _version));
        }

        protected virtual void OnStatusChanged(DocumentStatusChangedEventArgs e)
        {
            StatusChanged?.Invoke(this, e);
        }

        protected virtual void OnVersionChanged(DocumentVersionChangedEventArgs e)
        {
            VersionChanged?.Invoke(this, e);
        }

        // ===== COMPARISON =====

        public override bool Equals(object obj)
        {
            return obj is Document document && Id == document.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{FileName} (v{Version}) - {(IsSaved ? "Saved" : "Modified")}";
        }
    }

    /// <summary>
    /// Аргументи події зміни вмісту документа
    /// </summary>
    public class DocumentChangedEventArgs : EventArgs
    {
        public Document Document { get; }
        public string OldContent { get; }
        public string NewContent { get; }
        public DateTime ChangeTime { get; }
        public int ContentLength { get; }
        public int LengthDifference { get; }

        public DocumentChangedEventArgs(Document document, string oldContent, string newContent)
        {
            Document = document;
            OldContent = oldContent ?? string.Empty;
            NewContent = newContent ?? string.Empty;
            ChangeTime = DateTime.Now;
            ContentLength = NewContent.Length;
            LengthDifference = ContentLength - OldContent.Length;
        }
    }

    /// <summary>
    /// Аргументи події зміни статусу документа
    /// </summary>
    public class DocumentStatusChangedEventArgs : EventArgs
    {
        public Document Document { get; }
        public string Message { get; }
        public DateTime Timestamp { get; }
        public string StatusType { get; }

        public DocumentStatusChangedEventArgs(Document document, string message, string statusType = "Info")
        {
            Document = document;
            Message = message;
            StatusType = statusType;
            Timestamp = DateTime.Now;
        }
    }

    /// <summary>
    /// Аргументи події зміни версії документа
    /// </summary>
    public class DocumentVersionChangedEventArgs : EventArgs
    {
        public Document Document { get; }
        public int OldVersion { get; }
        public int NewVersion { get; }
        public DateTime ChangeTime { get; }

        public DocumentVersionChangedEventArgs(Document document, int oldVersion, int newVersion)
        {
            Document = document;
            OldVersion = oldVersion;
            NewVersion = newVersion;
            ChangeTime = DateTime.Now;
        }
    }

    /// <summary>
    /// Снапшот документа для історії
    /// </summary>
    public class DocumentSnapshot
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Content { get; set; }
        public int Version { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}