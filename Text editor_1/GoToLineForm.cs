using System;
using System.Drawing;
using System.Windows.Forms;
using TextEditorMK.Models; // ? Додаємо для EditorTheme

namespace Text_editor_1
{

    public partial class GoToLineForm : Form
    {
        public int LineNumber { get; private set; }
        public bool IsValidInput { get; private set; }

        private readonly int _totalLines;

        public GoToLineForm(int totalLines, int currentLine = 1)
        {
            _totalLines = totalLines;
            InitializeComponent();
            
            
            lineNumberTextBox.Text = currentLine.ToString();
            lineNumberTextBox.SelectAll();
            
           
            infoLabel.Text = $"Line number (1 - {totalLines}):";
        }


        public void ApplyTheme(EditorTheme theme)
        {
            if (theme == null) return;

            try
            {

                this.BackColor = theme.BackgroundColor;
                this.ForeColor = theme.ForegroundColor;


                lineNumberTextBox.BackColor = theme.TextBoxBackColor;
                lineNumberTextBox.ForeColor = theme.TextBoxForeColor;


                okButton.BackColor = theme.ButtonBackColor;
                okButton.ForeColor = theme.ButtonForeColor;
                cancelButton.BackColor = theme.ButtonBackColor;
                cancelButton.ForeColor = theme.ButtonForeColor;


                infoLabel.ForeColor = theme.ForegroundColor;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying theme to GoToLineForm: {ex.Message}");
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void lineNumberTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != '\r')
            {
                e.Handled = true;
            }
            

            if (e.KeyChar == '\r')
            {
                okButton.PerformClick();
            }
        }

        private void lineNumberTextBox_TextChanged(object sender, EventArgs e)
        {

            okButton.Enabled = ValidateInput(false);
        }

        private bool ValidateInput(bool showErrors = true)
        {
            string input = lineNumberTextBox.Text.Trim();
            
            if (string.IsNullOrEmpty(input))
            {
                if (showErrors)
                    MessageBox.Show("Please enter a line number.", "Invalid Input", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(input, out int lineNumber))
            {
                if (showErrors)
                    MessageBox.Show("Please enter a valid number.", "Invalid Input", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (lineNumber < 1 || lineNumber > _totalLines)
            {
                if (showErrors)
                    MessageBox.Show($"Line number must be between 1 and {_totalLines}.", "Invalid Range", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            LineNumber = lineNumber;
            IsValidInput = true;
            return true;
        }

        private void GoToLineForm_Load(object sender, EventArgs e)
        {
            lineNumberTextBox.Focus();
        }
    }
}