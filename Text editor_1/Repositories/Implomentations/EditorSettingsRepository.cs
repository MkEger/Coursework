using TextEditorMK.Models;
using TextEditorMK.Repositories.Interfaces;

namespace TextEditorMK.Repositories.Implementations
{
    public class EditorSettingsRepository : IEditorSettingsRepository
    {
        private EditorSettings _settings = new EditorSettings();

        public EditorSettings GetCurrent() 
        {
            // ✅ Валідація перед поверненням
            ValidateSettings(_settings);
            return _settings;
        }

        public void Update(EditorSettings settings)
        {
            if (settings == null)
            {
                _settings = new EditorSettings();
                return;
            }

            // ✅ Валідація перед збереженням
            ValidateSettings(settings);
            _settings = settings;
        }

        /// <summary>
        /// Валідація налаштувань для запобігання некоректним значенням
        /// </summary>
        private void ValidateSettings(EditorSettings settings)
        {
            if (settings == null) return;

            // Валідація FontSize
            if (settings.FontSize <= 0 || settings.FontSize > 72)
            {
                settings.FontSize = 12;
                System.Diagnostics.Debug.WriteLine($"⚠️ EditorSettingsRepository: Invalid FontSize corrected to 12");
            }

            // Валідація FontFamily
            if (string.IsNullOrEmpty(settings.FontFamily))
            {
                settings.FontFamily = "Consolas";
                System.Diagnostics.Debug.WriteLine($"⚠️ EditorSettingsRepository: Empty FontFamily corrected to Consolas");
            }

            // Валідація Theme
            if (string.IsNullOrEmpty(settings.Theme))
            {
                settings.Theme = "Light";
                System.Diagnostics.Debug.WriteLine($"⚠️ EditorSettingsRepository: Empty Theme corrected to Light");
            }

            // Валідація AutoSaveInterval
            if (settings.AutoSaveInterval <= 0)
            {
                settings.AutoSaveInterval = 30;
                System.Diagnostics.Debug.WriteLine($"⚠️ EditorSettingsRepository: Invalid AutoSaveInterval corrected to 30");
            }

            // Валідація розмірів вікна
            if (settings.WindowWidth <= 0)
            {
                settings.WindowWidth = 800;
            }

            if (settings.WindowHeight <= 0)
            {
                settings.WindowHeight = 600;
            }

            // Валідація MaxRecentFiles
            if (settings.MaxRecentFiles <= 0)
            {
                settings.MaxRecentFiles = 10;
            }
        }
    }
}
