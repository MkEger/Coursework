using System;
using System.Collections.Generic;
using TextEditorMK.Repositories.Interfaces;
using TextEditorMK.Repositories.Implementations;

namespace TextEditorMK.Factories
{
    /// <summary>
    /// Abstract Factory Pattern - Створення сімейств репозиторіїв
    /// </summary>
    public interface IRepositoryFactory
    {
        IDocumentRepository CreateDocumentRepository();
        IRecentFileRepository CreateRecentFileRepository();
        IEncodingRepository CreateEncodingRepository();
        IEditorSettingsRepository CreateSettingsRepository();
        string GetFactoryType();
    }

    /// <summary>
    /// MySQL Factory - для роботи з базою даних
    /// </summary>
    public class MySqlRepositoryFactory : IRepositoryFactory
    {
        public IDocumentRepository CreateDocumentRepository()
        {
            return new MySqlDocumentRepository();
        }

        public IRecentFileRepository CreateRecentFileRepository()
        {
            return new MySqlRecentFileRepository();
        }

        public IEncodingRepository CreateEncodingRepository()
        {
            return new MySqlEncodingRepository();
        }

        public IEditorSettingsRepository CreateSettingsRepository()
        {
            return new MySqlEditorSettingsRepository();
        }

        public string GetFactoryType() => "MySQL Database";
    }

    /// <summary>
    /// In-Memory Factory - для роботи в пам'яті (fallback)
    /// </summary>
    public class InMemoryRepositoryFactory : IRepositoryFactory
    {
        public IDocumentRepository CreateDocumentRepository()
        {
            return new DocumentRepository();
        }

        public IRecentFileRepository CreateRecentFileRepository()
        {
            return new RecentFileRepository();
        }

        public IEncodingRepository CreateEncodingRepository()
        {
            return new EncodingRepository();
        }

        public IEditorSettingsRepository CreateSettingsRepository()
        {
            return new EditorSettingsRepository();
        }

        public string GetFactoryType() => "In-Memory Storage";
    }

    /// <summary>
    /// Provider для вибору фабрики
    /// </summary>
    public static class RepositoryFactoryProvider
    {
        public static IRepositoryFactory GetFactory()
        {
            try
            {
                // Спроба підключення до MySQL
                using (var connection = new MySql.Data.MySqlClient.MySqlConnection(
                    "Server=localhost;Database=texteditor_mk;Uid=root;Pwd=password123;"))
                {
                    connection.Open();
                    connection.Close();
                }

                return new MySqlRepositoryFactory();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MySQL not available: {ex.Message}");
                return new InMemoryRepositoryFactory();
            }
        }

        public static IRepositoryFactory GetMySqlFactory()
        {
            return new MySqlRepositoryFactory();
        }

        public static IRepositoryFactory GetInMemoryFactory()
        {
            return new InMemoryRepositoryFactory();
        }
    }

    /// <summary>
    /// Cache Factory - декоратор для кешування
    /// </summary>
    public class CachedRepositoryFactory : IRepositoryFactory
    {
        private readonly IRepositoryFactory _baseFactory;
        private readonly Dictionary<Type, object> _cache = new Dictionary<Type, object>();

        public CachedRepositoryFactory(IRepositoryFactory baseFactory)
        {
            _baseFactory = baseFactory ?? throw new ArgumentNullException(nameof(baseFactory));
        }

        public IDocumentRepository CreateDocumentRepository()
        {
            return GetOrCreate<IDocumentRepository>(() => _baseFactory.CreateDocumentRepository());
        }

        public IRecentFileRepository CreateRecentFileRepository()
        {
            return GetOrCreate<IRecentFileRepository>(() => _baseFactory.CreateRecentFileRepository());
        }

        public IEncodingRepository CreateEncodingRepository()
        {
            return GetOrCreate<IEncodingRepository>(() => _baseFactory.CreateEncodingRepository());
        }

        public IEditorSettingsRepository CreateSettingsRepository()
        {
            return GetOrCreate<IEditorSettingsRepository>(() => _baseFactory.CreateSettingsRepository());
        }

        public string GetFactoryType() => $"Cached {_baseFactory.GetFactoryType()}";

        private T GetOrCreate<T>(Func<T> factory) where T : class
        {
            var type = typeof(T);
            if (_cache.ContainsKey(type))
            {
                return (T)_cache[type];
            }

            var instance = factory();
            _cache[type] = instance;
            return instance;
        }
    }
}       