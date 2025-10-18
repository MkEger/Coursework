using TextEditorMK.Models;

namespace TextEditorMK.Repositories.Interfaces
{
    public interface IEditorSettingsRepository
    {
        EditorSettings GetCurrent();
        void Update(EditorSettings settings);
    }
}
