using System.Collections.Generic;
using System.Linq;

using TextEditorMK.Models;
using TextEditorMK.Repositories.Interfaces;

namespace TextEditorMK.Repositories.Implementations
{
    public class RecentFileRepository : IRecentFileRepository
    {
        private readonly List<RecentFile> _recentFiles = new List<RecentFile>();

        public RecentFile GetById(int id) => _recentFiles.FirstOrDefault(r => r.Id == id);

        public List<RecentFile> GetAll() => _recentFiles;

        public List<RecentFile> GetRecent(int count) =>
            _recentFiles.OrderByDescending(r => r.LastOpenedAt).Take(count).ToList();

        public void AddOrUpdate(RecentFile file)
        {
            var existing = _recentFiles.FirstOrDefault(r => r.FilePath == file.FilePath);
            if (existing == null)
            {
                _recentFiles.Add(file);
            }
            else
            {
                existing.UpdateLastOpened();
            }
        }

        public void Delete(int id)
        {
            var file = GetById(id);
            if (file != null) _recentFiles.Remove(file);
        }
    }
}

