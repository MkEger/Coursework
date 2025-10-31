using System;
using System.IO;
using TextEditorMK.Models;

namespace TextEditorMK.Factories
{
    /// <summary>
    /// Factory Method - для створення різних типів документів
    /// </summary>
    public static class DocumentFactory
    {
        /// <summary>
        /// Створити новий пустий документ
        /// </summary>
        public static Document CreateNewDocument()
        {
            return new Document
            {
                Id = GenerateId(),
                FileName = "Untitled.txt",
                FilePath = string.Empty,
                Content = string.Empty,
                EncodingId = 1,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                IsSaved = false
            };
        }

        /// <summary>
        /// Створити документ з файлу
        /// </summary>
        public static Document CreateFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            try
            {
                string content = File.ReadAllText(filePath);
                
                return new Document
                {
                    Id = GenerateId(),
                    FileName = Path.GetFileName(filePath),
                    FilePath = filePath,
                    Content = content,
                    EncodingId = 1,
                    CreatedAt = File.GetCreationTime(filePath),
                    ModifiedAt = File.GetLastWriteTime(filePath),
                    IsSaved = true
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create document from file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Створити документ з готовими даними (для RecentFiles)
        /// </summary>
        public static Document CreateFromRecentFile(RecentFile recentFile)
        {
            if (recentFile == null)
                throw new ArgumentNullException(nameof(recentFile));

            return CreateFromFile(recentFile.FilePath);
        }

        /// <summary>
        /// Створити копію документа
        /// </summary>
        public static Document CreateCopy(Document original)
        {
            if (original == null)
                throw new ArgumentNullException(nameof(original));

            return new Document
            {
                Id = GenerateId(), // Новий ID для копії
                FileName = $"Copy of {original.FileName}",
                FilePath = string.Empty, // Копія не має шляху
                Content = original.Content,
                EncodingId = original.EncodingId,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                IsSaved = false
            };
        }

        private static int GenerateId()
        {
            return new Random().Next(1, 10000);
        }
    }
}