using System;
using System.IO;
using System.Windows.Forms;
using TextEditorMK.Models;
using TextEditorMK.Repositories.Implementations;
using TextEditorMK.Repositories.Interfaces;

namespace Text_editor_1
{
    public partial class MainForm : Form
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IEncodingRepository _encodingRepository;
        private readonly IRecentFileRepository _recentFileRepository;
        private readonly IEditorSettingsRepository _settingsRepository;

        private Document _currentDocument;

        public MainForm()
        {
            InitializeComponent();

            // Initialize repositories with MySQL support
            try
            {
                _documentRepository = new MySqlDocumentRepository();
                _recentFileRepository = new MySqlRecentFileRepository(); // <-- ТЕПЕР MYSQL!
                
                MessageBox.Show("Connected to MySQL database successfully!", "Database", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"MySQL not available: {ex.Message}\nUsing in-memory storage.", 
                    "Database Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                _documentRepository = new DocumentRepository();
                _recentFileRepository = new RecentFileRepository(); // Fallback
            }

            // Keep other repositories as in-memory for now
            _encodingRepository = new EncodingRepository();
            _settingsRepository = new EditorSettingsRepository();

            CreateNewDocument();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdateTitle();
            LoadSettings();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (_currentDocument != null && richTextBox1 != null)
            {
                _currentDocument.SetContent(richTextBox1.Text);
                _documentRepository.Update(_currentDocument);
                UpdateTitle();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewDocument();
            if (richTextBox1 != null)
            {
                richTextBox1.Clear();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDocument();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveDocument();
        }

        private void recentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var recentForm = new RecentFilesForm(_recentFileRepository, _documentRepository, _encodingRepository);
                if (recentForm.ShowDialog() == DialogResult.OK && recentForm.SelectedDocument != null)
                {
                    LoadDocument(recentForm.SelectedDocument);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening recent files: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentDocument != null && !_currentDocument.IsSaved)
            {
                var result = MessageBox.Show("You have unsaved changes. Do you want to save before exit?",
                    "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SaveDocument();
                }
                else if (result == DialogResult.Cancel)
                {
                    return; // Don't close
                }
            }
            this.Close();
        }

        private void CreateNewDocument()
        {
            _currentDocument = new Document
            {
                Id = GenerateNewId(),
                FileName = "Untitled.txt",
                FilePath = string.Empty,
                Content = string.Empty,
                EncodingId = 1,
                TextEncoding = _encodingRepository.GetDefault()
            };

            _documentRepository.Add(_currentDocument);
            if (richTextBox1 != null)
            {
                richTextBox1.Clear();
            }
            UpdateTitle();
        }

        private void OpenDocument()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string content = File.ReadAllText(openFileDialog1.FileName);

                    _currentDocument = new Document
                    {
                        Id = GenerateNewId(),
                        FileName = Path.GetFileName(openFileDialog1.FileName),
                        FilePath = openFileDialog1.FileName,
                        Content = content,
                        TextEncoding = _encodingRepository.GetDefault(),
                        IsSaved = true
                    };

                    _documentRepository.Add(_currentDocument);
                    if (richTextBox1 != null)
                    {
                        richTextBox1.Text = content;
                    }

                    // Add to recent files
                    var recentFile = new RecentFile
                    {
                        Id = GenerateNewId(),
                        FileName = _currentDocument.FileName,
                        FilePath = _currentDocument.FilePath
                    };
                    _recentFileRepository.AddOrUpdate(recentFile);

                    UpdateTitle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening file: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveDocument()
        {
            if (_currentDocument == null) return;

            if (string.IsNullOrEmpty(_currentDocument.FilePath))
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    _currentDocument.FilePath = saveFileDialog1.FileName;
                    _currentDocument.FileName = Path.GetFileName(saveFileDialog1.FileName);
                }
                else
                {
                    return;
                }
            }

            try
            {
                File.WriteAllText(_currentDocument.FilePath, _currentDocument.Content);
                _currentDocument.IsSaved = true;
                _documentRepository.Update(_currentDocument);
                UpdateTitle();

                MessageBox.Show("File saved successfully!", "Save",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDocument(Document document)
        {
            if (document == null) return;

            _currentDocument = document;
            if (richTextBox1 != null)
            {
                richTextBox1.Text = document.Content;
            }
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            if (_currentDocument != null)
            {
                string status = _currentDocument.IsSaved ? "" : "*";
                this.Text = $"Text Editor MK - {_currentDocument.FileName}{status}";
            }
        }

        private void LoadSettings()
        {
            try
            {
                var settings = _settingsRepository.GetCurrent();
                if (richTextBox1 != null)
                {
                    richTextBox1.Font = new System.Drawing.Font(settings.FontFamily, settings.FontSize);
                    richTextBox1.WordWrap = settings.WordWrap;
                }
            }
            catch
            {
                // Use default settings if error
            }
        }

        private int GenerateNewId()
        {
            return new Random().Next(1, 10000);
        }
    }
}