using System;
using System.Drawing;
using System.Windows.Forms;
using TextEditorMK.Models;

namespace Text_editor_1
{

    public partial class SnippetEditForm : Form
    {
        public CodeSnippet Snippet { get; private set; }
        private readonly bool _isNew;

        public SnippetEditForm(CodeSnippet snippet, bool isNew = false)
        {
            Snippet = snippet ?? throw new ArgumentNullException(nameof(snippet));
            _isNew = isNew;
            
            InitializeComponent();
            LoadSnippetData();
        }


        public void ApplyTheme(EditorTheme theme)
        {
            if (theme == null) return;

            try
            {
                this.BackColor = theme.BackgroundColor;
                this.ForeColor = theme.ForegroundColor;

                var textBoxes = new[] { nameTextBox, triggerTextBox, languageTextBox, descriptionTextBox, categoryTextBox, codeTextBox };
                foreach (var textBox in textBoxes)
                {
                    textBox.BackColor = theme.TextBoxBackColor;
                    textBox.ForeColor = theme.TextBoxForeColor;
                }

                var buttons = new[] { okButton, cancelButton, insertVariableButton, previewButton };
                foreach (var button in buttons)
                {
                    button.BackColor = theme.ButtonBackColor;
                    button.ForeColor = theme.ButtonForeColor;
                }

                var labels = new[] { nameLabel, triggerLabel, languageLabel, descriptionLabel, categoryLabel, codeLabel };
                foreach (var label in labels)
                {
                    label.ForeColor = theme.ForegroundColor;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying theme to SnippetEditForm: {ex.Message}");
            }
        }

        private void LoadSnippetData()
        {
            nameTextBox.Text = Snippet.Name;
            triggerTextBox.Text = Snippet.Trigger;
            languageTextBox.Text = Snippet.Language;
            descriptionTextBox.Text = Snippet.Description;
            categoryTextBox.Text = Snippet.Category;
            codeTextBox.Text = Snippet.Code;
            
            this.Text = _isNew ? "New Snippet" : $"Edit Snippet - {Snippet.Name}";
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                SaveSnippetData();
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Please enter a snippet name.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(triggerTextBox.Text))
            {
                MessageBox.Show("Please enter a trigger word.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                triggerTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(codeTextBox.Text))
            {
                MessageBox.Show("Please enter snippet code.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                codeTextBox.Focus();
                return false;
            }

            return true;
        }

        private void SaveSnippetData()
        {
            Snippet.Name = nameTextBox.Text.Trim();
            Snippet.Trigger = triggerTextBox.Text.Trim();
            Snippet.Language = string.IsNullOrWhiteSpace(languageTextBox.Text) ? "text" : languageTextBox.Text.Trim();
            Snippet.Description = descriptionTextBox.Text.Trim();
            Snippet.Category = string.IsNullOrWhiteSpace(categoryTextBox.Text) ? "General" : categoryTextBox.Text.Trim();
            Snippet.Code = codeTextBox.Text;
        }

        private void insertVariableButton_Click(object sender, EventArgs e)
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("$1 - First placeholder").Click += (s, args) => InsertVariable("$1");
            menu.Items.Add("$2 - Second placeholder").Click += (s, args) => InsertVariable("$2");
            menu.Items.Add("$0 - Final cursor position").Click += (s, args) => InsertVariable("$0");
            menu.Items.Add("-");
            menu.Items.Add("$DATE - Current date").Click += (s, args) => InsertVariable("$DATE");
            menu.Items.Add("$TIME - Current time").Click += (s, args) => InsertVariable("$TIME");
            menu.Items.Add("$USER - Username").Click += (s, args) => InsertVariable("$USER");
            
            menu.Show(insertVariableButton, new Point(0, insertVariableButton.Height));
        }

        private void InsertVariable(string variable)
        {
            int start = codeTextBox.SelectionStart;
            codeTextBox.SelectedText = variable;
            codeTextBox.SelectionStart = start + variable.Length;
            codeTextBox.Focus();
        }

        private void previewButton_Click(object sender, EventArgs e)
        {
            string preview = codeTextBox.Text;
            
            preview = preview.Replace("$DATE", DateTime.Now.ToString("yyyy-MM-dd"));
            preview = preview.Replace("$TIME", DateTime.Now.ToString("HH:mm:ss"));
            preview = preview.Replace("$USER", Environment.UserName);
            preview = preview.Replace("$1", "[placeholder1]");
            preview = preview.Replace("$2", "[placeholder2]");
            preview = preview.Replace("$0", "[cursor]");
            
            MessageBox.Show(preview, "Snippet Preview", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SnippetEditForm_Load(object sender, EventArgs e)
        {
            nameTextBox.Focus();
        }
    }
}