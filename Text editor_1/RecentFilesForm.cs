using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TextEditorMK.Models;
using TextEditorMK.Repositories.Interfaces;
using TextEditorMK.Helpers; // ✅ Додаємо using для ThemeHelper

namespace Text_editor_1
{
    public partial class RecentFilesForm : Form
    {
        private readonly IRecentFileRepository _recentFileRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IEncodingRepository _encodingRepository;
        private readonly EditorSettings _currentSettings;
        
        public Document SelectedDocument { get; private set; }

        public RecentFilesForm(IRecentFileRepository recentFileRepository, 
                              IDocumentRepository documentRepository,
                              IEncodingRepository encodingRepository,
                              EditorSettings currentSettings = null)
        {
            InitializeComponent();
            _recentFileRepository = recentFileRepository;
            _documentRepository = documentRepository;
            _encodingRepository = encodingRepository;
            _currentSettings = currentSettings;
            
            // ✅ Застосувати тему якщо є налаштування
            if (_currentSettings != null)
            {
                ApplyTheme();
            }
            
            LoadRecentFiles();
        }

        /// <summary>
        /// Застосувати поточну тему до форми
        /// </summary>
        private void ApplyTheme()
        {
            try
            {
                if (_currentSettings == null) return;

                var theme = EditorTheme.GetByName(_currentSettings.Theme);
                if (theme == null) return;

                // ✅ Використовуємо ThemeHelper для застосування теми
                ThemeHelper.ApplyThemeToForm(this, theme);

                System.Diagnostics.Debug.WriteLine($"✅ Applied {theme.Name} theme to RecentFilesForm");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error applying theme to RecentFilesForm: {ex.Message}");
            }
        }

        private void LoadRecentFiles()
        {
            // ✅ Перевірка чи репозиторій доступний
            if (_recentFileRepository == null)
            {
                MessageBox.Show("Recent files are not available.\nMySQL database is not connected.", 
                    "Database Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var recentFiles = _recentFileRepository.GetRecent(10);
                
                listViewRecentFiles.Items.Clear();
                foreach (var file in recentFiles)
                {
                    var item = new ListViewItem(new[]
                    {
                        file.FileName,
                        file.FilePath,
                        file.LastOpenedAt.ToString("dd.MM.yyyy HH:mm"),
                        file.OpenCount.ToString()
                    });
                    item.Tag = file;
                    listViewRecentFiles.Items.Add(item);
                }

                System.Diagnostics.Debug.WriteLine($"✅ Loaded {recentFiles.Count} recent files");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recent files: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (listViewRecentFiles.SelectedItems.Count > 0)
            {
                var selectedFile = (RecentFile)listViewRecentFiles.SelectedItems[0].Tag;
                
                // Завантажити документ
                try
                {
                    // ✅ Використовуємо правильне кодування
                    var encoding = System.Text.Encoding.UTF8; // Дефолт
                    string content = System.IO.File.ReadAllText(selectedFile.FilePath, encoding);
                    
                    SelectedDocument = new Document
                    {
                        Id = GenerateNewId(),
                        FileName = selectedFile.FileName,
                        FilePath = selectedFile.FilePath,
                        Content = content,
                        TextEncoding = _encodingRepository.GetDefault(),
                        IsSaved = true
                    };
                    
                    // Оновити статистику
                    selectedFile.UpdateLastOpened();
                    _recentFileRepository?.AddOrUpdate(selectedFile);
                    
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening file: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnClearHistory_Click(object sender, EventArgs e)
        {
            if (_recentFileRepository == null)
            {
                MessageBox.Show("Cannot clear history - database not available.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = MessageBox.Show("Clear all recent files history?", 
                "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                try
                {
                    var allFiles = _recentFileRepository.GetAll();
                    foreach (var file in allFiles)
                    {
                        _recentFileRepository.Delete(file.Id);
                    }
                    LoadRecentFiles();
                    
                    MessageBox.Show("Recent files history cleared.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error clearing history: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private int GenerateNewId()
        {
            return new Random().Next(1, 10000);
        }
    }
}
