using System;
using System.Linq;
using System.Windows.Forms;
using TextEditorMK.Models;
using TextEditorMK.Repositories.Interfaces;

namespace Text_editor_1
{
    public partial class RecentFilesForm : Form
    {
        private readonly IRecentFileRepository _recentFileRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IEncodingRepository _encodingRepository;
        
        public Document SelectedDocument { get; private set; }

        public RecentFilesForm(IRecentFileRepository recentFileRepository, 
                              IDocumentRepository documentRepository,
                              IEncodingRepository encodingRepository)
        {
            InitializeComponent();
            _recentFileRepository = recentFileRepository;
            _documentRepository = documentRepository;
            _encodingRepository = encodingRepository;
            
            LoadRecentFiles();
        }

        private void LoadRecentFiles()
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
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (listViewRecentFiles.SelectedItems.Count > 0)
            {
                var selectedFile = (RecentFile)listViewRecentFiles.SelectedItems[0].Tag;
                
                // Завантажити документ
                try
                {
                    string content = System.IO.File.ReadAllText(selectedFile.FilePath);
                    
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
                    _recentFileRepository.AddOrUpdate(selectedFile);
                    
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка відкриття файлу: {ex.Message}", "Помилка",
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
            var result = MessageBox.Show("Очистити всю історію нещодавніх файлів?", 
                "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                var allFiles = _recentFileRepository.GetAll();
                foreach (var file in allFiles)
                {
                    _recentFileRepository.Delete(file.Id);
                }
                LoadRecentFiles();
            }
        }

        private int GenerateNewId()
        {
            return new Random().Next(1, 10000);
        }
    }
}
