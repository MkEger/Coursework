using TextEditorMK.Models;
using TextEditorMK.Repositories.Interfaces;

namespace TextEditorMK.Repositories.Implementations
{
    public class EditorSettingsRepository : IEditorSettingsRepository
    {
        private EditorSettings _settings = new EditorSettings();

        public EditorSettings GetCurrent() => _settings;

        public void Update(EditorSettings settings)
        {
            _settings = settings;
        }
    }
}
