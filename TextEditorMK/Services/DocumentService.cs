using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TextEditorMK.Models;
using TextEditorMK.Repositories.Interfaces;

namespace TextEditorMK.Services
{
    /// <summary>
    /// Сервіс для управління документами з бізнес-логікою
    /// </summary>
    public class DocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IRecentFileRepository _recentFileRepository;
        private readonly IEncodingRepository _encodingRepository;
        
        // Кеш для оптимізації
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
            _recentFileRepository = recentFileRepository ?? throw new ArgumentNullException(nameof(recentFileRepository));
            _encodingRepository = encodingRepository ?? throw new ArgumentNullException(nameof(encodingRepository));

            _documentCache = new Dictionary<int, Document>();
            _recentFileCache = new Dictionary<string, RecentFile>();
            
            LoadRecentFilesCache();
        }

        // ===== DOCUMENT OPERATIONS =====

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
                // Перевірити чи документ вже відкритий
                var existingDoc = GetDocumentByPath(filePath);
                if (existingDoc != null)
                {
                    return existingDoc;
                }

                var document = DocumentFactory.CreateFromFile(filePath);
                document.TextEncoding = DetectOrGetDefaultEncoding(filePath);

                AddDocument(document);
                AddToRecentFiles(filePath, document.FileName);

                OnDocumentAdded(document);
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

                // Создать резервную копию если файл существует
                if (File.Exists(targetPath))
                {
                    CreateBackup(targetPath);
                }

                // Записать файл с правильным енкодингом
                var encoding = document.TextEncoding?.CodePage == "utf-8" 
                    ? System.Text.Encoding.UTF8 
                    : System.Text.Encoding.Default;

                File.WriteAllText(targetPath, document.Content, encoding);

                // Обновити властивості документа
                document.FilePath = targetPath;
                document.FileName = Path.GetFileName(targetPath);
                document.MarkAsSaved();
                document.ModifiedAt = DateTime.Now;

                // Зберегти в репозиторій
                UpdateDocument(document);
                AddToRecentFiles(targetPath, document.FileName);

                OnDocumentSaved(document);
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
                _documentCache[document.Id] = document; // Обновить кеш
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
                // Сначала проверить кеш
                if (_documentCache.ContainsKey(id))
                {
                    return _documentCache[id];
                }

                // Загрузить из репозитория
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

        // ===== RECENT FILES OPERATIONS =====

        public void AddToRecentFiles(string filePath, string fileName)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(fileName))
                return;

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
            }
            catch (Exception ex)
            {
                // Не бросаем исключение для recent files, просто логируем
                System.Diagnostics.Debug.WriteLine($"Failed to add to recent files: {ex.Message}");
            }
        }

        public List<RecentFile> GetRecentFiles(int count = 10)
        {
            try
            {
                var recentFiles = _recentFileRepository.GetRecent(count);
                
                // Фильтруем существующие файлы
                return recentFiles.Where(f => File.Exists(f.FilePath)).ToList();
            }
            catch (Exception ex)
            {
                throw new DocumentServiceException($"Failed to get recent files: {ex.Message}", ex);
            }
        }

        public void ClearRecentFiles()
        {
            try
            {
                var allRecentFiles = _recentFileRepository.GetAll();
                foreach (var file in allRecentFiles)
                {
                    _recentFileRepository.Delete(file.Id);
                }
                
                _recentFileCache.Clear();
            }
            catch (Exception ex)
            {
                throw new DocumentServiceException($"Failed to clear recent files: {ex.Message}", ex);
            }
        }

        // ===== UTILITY METHODS =====

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
                // Простая логика определения енкодинга по расширению
                var extension = Path.GetExtension(filePath).ToLower();
                
                return extension switch
                {
                    ".txt" => _encodingRepository.GetByCodePage("utf-8") ?? _encodingRepository.GetDefault(),
                    ".json" => _encodingRepository.GetByCodePage("utf-8") ?? _encodingRepository.GetDefault(),
                    ".xml" => _encodingRepository.GetByCodePage("utf-8") ?? _encodingRepository.GetDefault(),
                    _ => _encodingRepository.GetDefault()
                };
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
                
                // Ограничить количество резервных копий
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

        // ===== EVENTS =====

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

        // ===== STATISTICS =====

        public DocumentServiceStatistics GetStatistics()
        {
            try
            {
                var allDocuments = GetAllDocuments();
                var recentFiles = GetRecentFiles(100);

                return new DocumentServiceStatistics
                {
                    TotalDocuments = allDocuments.Count,
                    TotalCharacters = allDocuments.Sum(d => d.Content?.Length ?? 0),
                    TotalRecentFiles = recentFiles.Count,
                    CacheSize = _documentCache.Count,
                    MostRecentFile = recentFiles.FirstOrDefault(),
                    LargestDocument = allDocuments.OrderByDescending(d => d.Content?.Length ?? 0).FirstOrDefault()
                };
            }
            catch (Exception ex)
            {
                throw new DocumentServiceException($"Failed to get statistics: {ex.Message}", ex);
            }
        }
    }

    // ===== SUPPORTING CLASSES =====

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

        public override string ToString()
        {
            return $"Documents: {TotalDocuments}, Characters: {TotalCharacters:N0}, " +
                   $"Recent Files: {TotalRecentFiles}, Cache: {CacheSize}";
        }
    }
}