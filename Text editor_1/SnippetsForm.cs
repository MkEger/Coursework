using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TextEditorMK.Models;
using TextEditorMK.Services;

namespace Text_editor_1
{

    public partial class SnippetsForm : Form
    {
        private readonly SnippetService _snippetService;
        private string _currentLanguage;
        private EditorTheme _currentTheme; 

        public CodeSnippet SelectedSnippet { get; private set; }

        public SnippetsForm(SnippetService snippetService, string currentLanguage = "text")
        {
            _snippetService = snippetService ?? throw new ArgumentNullException(nameof(snippetService));
            _currentLanguage = currentLanguage ?? "text";
            
            InitializeComponent();
            InitializeLanguageFilter();
            LoadSnippets();
        }


        public void ApplyTheme(EditorTheme theme)
        {
            if (theme == null) return;


            _currentTheme = theme;

            try
            {

                this.BackColor = theme.BackgroundColor;
                this.ForeColor = theme.ForegroundColor;


                snippetsListView.BackColor = theme.TextBoxBackColor;
                snippetsListView.ForeColor = theme.TextBoxForeColor;


                languageComboBox.BackColor = theme.TextBoxBackColor;
                languageComboBox.ForeColor = theme.TextBoxForeColor;


                previewTextBox.BackColor = theme.TextBoxBackColor;
                previewTextBox.ForeColor = theme.TextBoxForeColor;

  
                var buttons = new[] { insertButton, newButton, editButton, deleteButton, closeButton };
                foreach (var button in buttons)
                {
                    button.BackColor = theme.ButtonBackColor;
                    button.ForeColor = theme.ButtonForeColor;
                }

                languageLabel.ForeColor = theme.ForegroundColor;
                descriptionLabel.ForeColor = theme.ForegroundColor;
                statusLabel.ForeColor = theme.ForegroundColor;

                previewGroupBox.ForeColor = theme.ForegroundColor;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying theme to SnippetsForm: {ex.Message}");
            }
        }

 
        private void ApplyThemeToEditForm(SnippetEditForm editForm)
        {
            if (_currentTheme != null)
            {
                editForm.ApplyTheme(_currentTheme);
            }
        }

        private void InitializeLanguageFilter()
        {
            languageComboBox.Items.Clear();
            languageComboBox.Items.AddRange(new object[]
            {
                "All Languages",
                "text",
                "csharp",
                "markdown", 
                "html",
                "javascript",
                "css",
                "json",
                "xml",
                "sql"
            });
            
            int index = languageComboBox.Items.IndexOf(_currentLanguage);
            languageComboBox.SelectedIndex = index >= 0 ? index : 0;
        }

        private void LoadSnippets()
        {
            snippetsListView.Items.Clear();
            
            string filterLanguage = languageComboBox.SelectedItem?.ToString();
            if (filterLanguage == "All Languages") filterLanguage = null;
            
            var snippets = string.IsNullOrEmpty(filterLanguage) 
                ? _snippetService.GetSnippetsForLanguage("") 
                : _snippetService.GetSnippetsForLanguage(filterLanguage);
            
            foreach (var snippet in snippets)
            {
                var item = new ListViewItem(snippet.Name);
                item.SubItems.Add(snippet.Trigger);
                item.SubItems.Add(snippet.Language);
                item.SubItems.Add(snippet.Category ?? "General");
                item.SubItems.Add(snippet.UsageCount.ToString());
                item.Tag = snippet;
                
                snippetsListView.Items.Add(item);
            }
            
            UpdateButtonStates();
            statusLabel.Text = $"{snippets.Count} snippet(s) for {filterLanguage ?? "all languages"}";
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = snippetsListView.SelectedItems.Count > 0;
            
            insertButton.Enabled = hasSelection;
            editButton.Enabled = hasSelection;
            deleteButton.Enabled = hasSelection;
            
            if (hasSelection)
            {
                var snippet = (CodeSnippet)snippetsListView.SelectedItems[0].Tag;
                ShowSnippetPreview(snippet);
            }
            else
            {
                ClearSnippetPreview();
            }
        }

        private void ShowSnippetPreview(CodeSnippet snippet)
        {
            if (snippet == null) return;
            
            previewTextBox.Text = snippet.Code;
            descriptionLabel.Text = snippet.Description ?? "No description";
        }

        private void ClearSnippetPreview()
        {
            previewTextBox.Clear();
            descriptionLabel.Text = "Select a snippet to see preview";
        }

        private void insertButton_Click(object sender, EventArgs e)
        {
            if (snippetsListView.SelectedItems.Count > 0)
            {
                SelectedSnippet = (CodeSnippet)snippetsListView.SelectedItems[0].Tag;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            var newSnippet = new CodeSnippet
            {
                Name = "New Snippet",
                Trigger = "new",
                Language = _currentLanguage,
                Code = "// New snippet code here",
                Description = "New snippet description",
                Category = "General"
            };
            
            using (var editForm = new SnippetEditForm(newSnippet, isNew: true))
            {
                // Apply current theme
                ApplyThemeToEditForm(editForm);
                
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _snippetService.AddSnippet(editForm.Snippet);
                        LoadSnippets();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding snippet: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (snippetsListView.SelectedItems.Count > 0)
            {
                var snippet = (CodeSnippet)snippetsListView.SelectedItems[0].Tag;
                
                using (var editForm = new SnippetEditForm(snippet, isNew: false))
                {
                    // Apply current theme
                    ApplyThemeToEditForm(editForm);
                    
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            // Delete old and add updated
                            _snippetService.DeleteSnippet(snippet.Name);
                            _snippetService.AddSnippet(editForm.Snippet);
                            LoadSnippets();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error updating snippet: {ex.Message}", "Error", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (snippetsListView.SelectedItems.Count > 0)
            {
                var snippet = (CodeSnippet)snippetsListView.SelectedItems[0].Tag;
                
                var result = MessageBox.Show(
                    $"Delete snippet '{snippet.Name}'?", 
                    "Confirm Delete", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        _snippetService.DeleteSnippet(snippet.Name);
                        LoadSnippets();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting snippet: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void languageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSnippets();
        }

        private void snippetsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void snippetsListView_DoubleClick(object sender, EventArgs e)
        {
            if (snippetsListView.SelectedItems.Count > 0)
            {
                insertButton.PerformClick();
            }
        }

        private void SnippetsForm_Load(object sender, EventArgs e)
        {
            snippetsListView.Focus();
        }
    }
}