using TextEditorMK.Models;
using TextEditorMK.Repositories.Interfaces;

namespace TextEditorMK.Repositories.Implementations
{
    public class EditorSettingsRepository : IEditorSettingsRepository
    {
        private EditorSettings _settings = new EditorSettings();

        public EditorSettings GetCurrent() 
        {
      
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


            ValidateSettings(settings);
            _settings = settings;
        }


        private void ValidateSettings(EditorSettings settings)
        {
            if (settings == null) return;


            if (settings.FontSize <= 0 || settings.FontSize > 72)
            {
                settings.FontSize = 12;
                System.Diagnostics.Debug.WriteLine($"⚠️ EditorSettingsRepository: Invalid FontSize corrected to 12");
            }

            if (string.IsNullOrEmpty(settings.FontFamily))
            {
                settings.FontFamily = "Consolas";
                System.Diagnostics.Debug.WriteLine($"⚠️ EditorSettingsRepository: Empty FontFamily corrected to Consolas");
            }


            if (string.IsNullOrEmpty(settings.Theme))
            {
                settings.Theme = "Light";
                System.Diagnostics.Debug.WriteLine($"⚠️ EditorSettingsRepository: Empty Theme corrected to Light");
            }


            if (settings.AutoSaveInterval <= 0)
            {
                settings.AutoSaveInterval = 30;
                System.Diagnostics.Debug.WriteLine($"⚠️ EditorSettingsRepository: Invalid AutoSaveInterval corrected to 30");
            }


            if (settings.WindowWidth <= 0)
            {
                settings.WindowWidth = 800;
            }

            if (settings.WindowHeight <= 0)
            {
                settings.WindowHeight = 600;
            }


            if (settings.MaxRecentFiles <= 0)
            {
                settings.MaxRecentFiles = 10;
            }
        }
    }
}
