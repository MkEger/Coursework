using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TextEditorMK.Models;
using TextEditorMK.Repositories.Interfaces;
using TextEditorMK.Factories;

namespace TextEditorMK.Services
{
    public class DocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IRecentFileRepository _recentFileRepository;
        private readonly IEncodingRepository _encodingRepository;

        private readonly Dictionary<int, Document> _documentCache;
        private readonly Dictionary<string, RecentFile> _recentFileCache;

        public event EventHandler<DocumentServiceEventArgs> DocumentAdded;
        public event EventHandler<DocumentServiceEventArgs> DocumentSaved;
        public event EventHandler<DocumentServiceEventArgs> DocumentDeleted;

        public DocumentService(
            IDocumentRepository documentRepository,
            IRecentFileRepository recentFileRepository,
            IEncodingRepository encodingRepository)
        {
            _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
            _recentFileRepository = recentFileRepository;
            _encodingRepository = encodingRepository ?? throw new ArgumentNullException(nameof(encodingRepository));

            _documentCache = new Dictionary<int, Document>();
            _recentFileCache = new Dictionary<string, RecentFile>();

            if (_recentFileRepository != null)
            {
                LoadRecentFilesCache();
                System.Diagnostics.Debug.WriteLine("DocumentService initialized with MySQL for recent files");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("DocumentService initialized WITHOUT recent files support (MySQL unavailable)");
            }
        }

        public Document CreateNewDocument(string fileName = "Untitled.txt")
        {
            try
            {
                var document = DocumentFactory.CreateNewDocument();
                document.FileName = fileName;
                document.TextEncoding = _encodingRepository.GetDefault();

                AddDocument(document);

                OnDocumentAdded(document);
                return document;
            }
            catch (Exception ex)
            {
                throw new DocumentServiceException($"Failed to create new document: {ex.Message}", ex);
            }
        }

        public Document OpenDocument(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            try
            {
                var existingDoc = GetDocumentByPath(filePath);
                if (existingDoc != null)
                {
                    return existingDoc;
                }

                var detectedEncoding = DetectEncodingFromFile(filePath);
                string content = File.ReadAllText(filePath, detectedEncoding);

                var document = DocumentFactory.CreateFromFile(filePath);
                document.Content = content;
                document.TextEncoding = GetEncodingBySystemEncoding(detectedEncoding);

                AddDocument(document);
                AddToRecentFiles(filePath, document.FileName);

                OnDocumentAdded(document);
                System.Diagnostics.Debug.WriteLine($"Document opened with {detectedEncoding.EncodingName} encoding");
                return document;
            }
            catch (Exception ex)
            {
                throw new DocumentServiceException($"Failed to open document: {ex.Message}", ex);
            }
        }

        public void SaveDocument(Document document, string filePath = null)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            try
            {
                string targetPath = filePath ?? document.FilePath;

                if (string.IsNullOrEmpty(targetPath))
                    throw new ArgumentException("File path is required for saving");

                if (File.Exists(targetPath))
                {
                    CreateBackup(targetPath);
                }

                System.Text.Encoding encoding = System.Text.Encoding.UTF8;

                if (document.TextEncoding != null)
                {
                    switch (document.TextEncoding.CodePage.ToLower())
                    {
                        case "utf-8":
                            encoding = System.Text.Encoding.UTF8;
                            break;
                        case "utf-16":
                            encoding = System.Text.Encoding.Unicode;
                            break;
                        case "windows-1251":
                            encoding = System.Text.Encoding.GetEncoding("windows-1251");
                            break;
                        default:
                            encoding = System.Text.Encoding.UTF8;
                            break;
                    }
                }

                File.WriteAllText(targetPath, document.Content, encoding);

                document.FilePath = targetPath;
                document.FileName = Path.GetFileName(targetPath);
                document.MarkAsSaved();
                document.ModifiedAt = DateTime.Now;

                UpdateDocument(document);
                AddToRecentFiles(targetPath, document.FileName);

                OnDocumentSaved(document);
                System.Diagnostics.Debug.WriteLine($"Document saved with {encoding.EncodingName} encoding");
            }
            catch (Exception ex)
            {
                throw new DocumentServiceException($"Failed to save document: {ex.Message}", ex);
            }
        }

        public void AddDocument(Document document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            try
            {
                _documentRepository.Add(document);
                _documentCache[document.Id] = document;
            }
            catch (Exception ex)
            {
                throw new DocumentServiceException($"Failed to add document: {ex.Message}", ex);
            }
        }

        public void UpdateDocument(Document document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            try
            {
                document.ModifiedAt = DateTime.Now;
                _documentRepository.Update(document);
                _documentCache[document.Id] = document;
            }
            catch (Exception ex)
            {
                throw new DocumentServiceException($"Failed to update document: {ex.Message}", ex);
            }
        }

        public void DeleteDocument(int documentId)
        {
            try
            {
                var document = GetDocumentById(documentId);
                if (document != null)
                {
                    _documentRepository.Delete(documentId);
                    _documentCache.Remove(documentId);
                    OnDocumentDeleted(document);
                }
            }
            catch (Exception ex)
            {
                throw new DocumentServiceException($"Failed to delete document: {ex.Message}", ex);
            }
        }

        public Document GetDocumentById(int id)
        {
            try
            {
                if (_documentCache.ContainsKey(id))
                {
                    return _documentCache[id];
                }

                var document = _documentRepository.GetById(id);
                if (document != null)
                {
                    _documentCache[id] = document;
                }

                return document;
            }
            catch (Exception ex)
            {
                throw new DocumentServiceException($"Failed to get document by ID: {ex.Message}", ex);
            }
        }

        public Document GetDocumentByPath(string filePath)
        {
            try
            {
                return _documentRepository.GetByPath(filePath);
            }
            catch (Exception ex)
            {
                throw new DocumentServiceException($"Failed to get document by path: {ex.Message}", ex);
            }
        }

        public List<Document> GetAllDocuments()
        {
            try
            {
                return _documentRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new DocumentServiceException($"Failed to get all documents: {ex.Message}", ex);
            }
        }

        public List<Document> SearchDocuments(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return GetAllDocuments();

            try
            {
                var allDocuments = GetAllDocuments();
                var searchTermLower = searchTerm.ToLower();

                return allDocuments.Where(doc =>
                    doc.FileName.ToLower().Contains(searchTermLower) ||
                    doc.FilePath.ToLower().Contains(searchTermLower) ||
                    doc.Content.ToLower().Contains(searchTermLower)
                ).ToList();
            }
            catch (Exception ex)
            {
                throw new DocumentServiceException($"Failed to search documents: {ex.Message}", ex);
            }
        }

        public void AddToRecentFiles(string filePath, string fileName)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(fileName))
                return;

            if (_recentFileRepository == null)
            {
                System.Diagnostics.Debug.WriteLine("Recent files NOT saved - MySQL database not available!");
                return;
            }

            try
            {
                var recentFile = _recentFileCache.ContainsKey(filePath)
                    ? _recentFileCache[filePath]
                    : new RecentFile
                    {
                        Id = GenerateId(),
                        FilePath = filePath,
                        FileName = fileName
                    };

                recentFile.UpdateLastOpened();

                _recentFileRepository.AddOrUpdate(recentFile);
                _recentFileCache[filePath] = recentFile;

                System.Diagnostics.Debug.WriteLine($"Recent file saved to MySQL: {fileName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save recent file to MySQL: {ex.Message}");
            }
        }

        public List<RecentFile> GetRecentFiles(int count = 10)
        {
            if (_recentFileRepository == null)
            {
                System.Diagnostics.Debug.WriteLine("Cannot load recent files - MySQL database not available!");
                return new List<RecentFile>();
            }

            try
            {
                var recentFiles = _recentFileRepository.GetRecent(count);

                var existingFiles = recentFiles.Where(f => File.Exists(f.FilePath)).ToList();

                System.Diagnostics.Debug.WriteLine($"Loaded {existingFiles.Count} recent files from MySQL");
                return existingFiles;
            }
            catch (Exception ex)
            {
                throw new DocumentServiceException($"Failed to get recent files from MySQL: {ex.Message}", ex);
            }
        }

        public void ClearRecentFiles()
        {
            if (_recentFileRepository == null)
            {
                System.Diagnostics.Debug.WriteLine("Cannot clear recent files - MySQL database not available!");
                return;
            }

            try
            {
                var allRecentFiles = _recentFileRepository.GetAll();
                foreach (var file in allRecentFiles)
                {
                    _recentFileRepository.Delete(file.Id);
                }

                _recentFileCache.Clear();
                System.Diagnostics.Debug.WriteLine($"Cleared {allRecentFiles.Count} recent files from MySQL");
            }
            catch (Exception ex)
            {
                throw new DocumentServiceException($"Failed to clear recent files from MySQL: {ex.Message}", ex);
            }
        }

        private void LoadRecentFilesCache()
        {
            try
            {
                var recentFiles = _recentFileRepository.GetAll();
                foreach (var file in recentFiles)
                {
                    _recentFileCache[file.FilePath] = file;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load recent files cache: {ex.Message}");
            }
        }

        private TextEncoding DetectOrGetDefaultEncoding(string filePath)
        {
            try
            {
                var systemEncoding = DetectEncodingFromFile(filePath);
                return GetEncodingBySystemEncoding(systemEncoding);
            }
            catch
            {
                return _encodingRepository.GetDefault();
            }
        }

        private System.Text.Encoding DetectEncodingFromFile(string filePath)
        {
            try
            {
                var bytes = File.ReadAllBytes(filePath);

                if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                    return System.Text.Encoding.UTF8;

                if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
                    return System.Text.Encoding.Unicode;

                if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
                    return System.Text.Encoding.BigEndianUnicode;

                var extension = Path.GetExtension(filePath).ToLower();

                switch (extension)
                {
                    case ".md":
                    case ".json":
                    case ".xml":
                        return System.Text.Encoding.UTF8;
                    default:
                        return System.Text.Encoding.UTF8;
                }
            }
            catch
            {
                return System.Text.Encoding.UTF8;
            }
        }

        private TextEncoding GetEncodingBySystemEncoding(System.Text.Encoding systemEncoding)
        {
            try
            {
                if (systemEncoding.Equals(System.Text.Encoding.UTF8))
                    return _encodingRepository.GetByCodePage("utf-8") ?? _encodingRepository.GetDefault();

                if (systemEncoding.Equals(System.Text.Encoding.Unicode))
                    return _encodingRepository.GetByCodePage("utf-16") ?? _encodingRepository.GetDefault();

                if (systemEncoding.CodePage == 1251)
                    return _encodingRepository.GetByCodePage("windows-1251") ?? _encodingRepository.GetDefault();

                return _encodingRepository.GetDefault();
            }
            catch
            {
                return _encodingRepository.GetDefault();
            }
        }

        private void CreateBackup(string filePath)
        {
            try
            {
                var backupPath = $"{filePath}.backup.{DateTime.Now:yyyyMMddHHmmss}";
                File.Copy(filePath, backupPath, true);

                CleanupBackups(filePath, 5);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to create backup: {ex.Message}");
            }
        }

        private void CleanupBackups(string originalPath, int maxBackups)
        {
            try
            {
                var directory = Path.GetDirectoryName(originalPath);
                var fileName = Path.GetFileName(originalPath);
                var backupPattern = $"{fileName}.backup.*";

                var backupFiles = Directory.GetFiles(directory, backupPattern)
                    .OrderByDescending(f => File.GetCreationTime(f))
                    .Skip(maxBackups);

                foreach (var backupFile in backupFiles)
                {
                    File.Delete(backupFile);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to cleanup backups: {ex.Message}");
            }
        }

        private int GenerateId()
        {
            return new Random().Next(1, int.MaxValue);
        }

        protected virtual void OnDocumentAdded(Document document)
        {
            DocumentAdded?.Invoke(this, new DocumentServiceEventArgs(document, "Added"));
        }

        protected virtual void OnDocumentSaved(Document document)
        {
            DocumentSaved?.Invoke(this, new DocumentServiceEventArgs(document, "Saved"));
        }

        protected virtual void OnDocumentDeleted(Document document)
        {
            DocumentDeleted?.Invoke(this, new DocumentServiceEventArgs(document, "Deleted"));
        }

        public DocumentServiceStatistics GetStatistics()
        {
            try
            {
                var allDocuments = GetAllDocuments();

                var recentFiles = _recentFileRepository != null
                    ? GetRecentFiles(100)
                    : new List<RecentFile>();

                return new DocumentServiceStatistics
                {
                    TotalDocuments = allDocuments.Count,
                    TotalCharacters = allDocuments.Sum(d => d.Content?.Length ?? 0),
                    TotalRecentFiles = recentFiles.Count,
                    CacheSize = _documentCache.Count,
                    MostRecentFile = recentFiles.FirstOrDefault(),
                    LargestDocument = allDocuments.OrderByDescending(d => d.Content?.Length ?? 0).FirstOrDefault(),
                    IsMySqlAvailable = _recentFileRepository != null
                };
            }
            catch (Exception ex)
            {
                throw new DocumentServiceException($"Failed to get statistics: {ex.Message}", ex);
            }
        }
    }

    public class DocumentServiceEventArgs : EventArgs
    {
        public Document Document { get; }
        public string Action { get; }
        public DateTime Timestamp { get; }

        public DocumentServiceEventArgs(Document document, string action)
        {
            Document = document;
            Action = action;
            Timestamp = DateTime.Now;
        }
    }

    public class DocumentServiceException : Exception
    {
        public DocumentServiceException(string message) : base(message) { }
        public DocumentServiceException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class DocumentServiceStatistics
    {
        public int TotalDocuments { get; set; }
        public long TotalCharacters { get; set; }
        public int TotalRecentFiles { get; set; }
        public int CacheSize { get; set; }
        public RecentFile MostRecentFile { get; set; }
        public Document LargestDocument { get; set; }
        public bool IsMySqlAvailable { get; set; }

        public override string ToString()
        {
            var dbStatus = IsMySqlAvailable ? "MySQL Available" : "No Database";
            return $"Documents: {TotalDocuments}, Characters: {TotalCharacters:N0}, " +
                   $"Recent Files: {TotalRecentFiles}, Cache: {CacheSize}, DB: {dbStatus}";
        }
    }
}