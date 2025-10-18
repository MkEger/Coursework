using System.Collections.Generic;
using TextEditorMK.Models;

namespace TextEditorMK.Repositories.Interfaces
{
    public interface IRecentFileRepository
    {
        RecentFile GetById(int id);
        List<RecentFile> GetAll();
        List<RecentFile> GetRecent(int count);
        void AddOrUpdate(RecentFile file);
        void Delete(int id);
    }
}
