using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TextEditorMK.Models;
using TextEditorMK.Services;

namespace Text_editor_1
{

    public partial class BookmarksForm : Form
    {
        private readonly BookmarkService _bookmarkService;
        private readonly RichTextBox _textBox;

        public BookmarksForm(BookmarkService bookmarkService, RichTextBox textBox)
        {
            _bookmarkService = bookmarkService ?? throw new ArgumentNullException(nameof(bookmarkService));
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
            
            InitializeComponent();
            LoadBookmarks();
        }


        public void ApplyTheme(EditorTheme theme)
        {
            if (theme == null) return;

            try
            {

                this.BackColor = theme.BackgroundColor;
                this.ForeColor = theme.ForegroundColor;

                bookmarksListView.BackColor = theme.TextBoxBackColor;
                bookmarksListView.ForeColor = theme.TextBoxForeColor;

   
                var buttons = new[] { goToButton, deleteButton, clearAllButton, closeButton, refreshButton };
                foreach (var button in buttons)
                {
                    button.BackColor = theme.ButtonBackColor;
                    button.ForeColor = theme.ButtonForeColor;
                }

        
                statusLabel.ForeColor = theme.ForegroundColor;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying theme to BookmarksForm: {ex.Message}");
            }
        }

        private void LoadBookmarks()
        {
            bookmarksListView.Items.Clear();
            
            var bookmarks = _bookmarkService.GetAllBookmarks();
            
            foreach (var bookmark in bookmarks)
            {
                var item = new ListViewItem(bookmark.LineNumber.ToString());
                item.SubItems.Add(bookmark.Description);
                item.SubItems.Add(bookmark.CreatedDate.ToString("HH:mm:ss"));
                item.Tag = bookmark;
                
                bookmarksListView.Items.Add(item);
            }
            

            UpdateButtonStates();
            

            statusLabel.Text = $"{bookmarks.Count} bookmark(s)";
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = bookmarksListView.SelectedItems.Count > 0;
            bool hasBookmarks = bookmarksListView.Items.Count > 0;
            
            goToButton.Enabled = hasSelection;
            deleteButton.Enabled = hasSelection;
            clearAllButton.Enabled = hasBookmarks;
        }

        private void goToButton_Click(object sender, EventArgs e)
        {
            if (bookmarksListView.SelectedItems.Count > 0)
            {
                var bookmark = (Bookmark)bookmarksListView.SelectedItems[0].Tag;
                
                try
                {

                    bookmark.UpdateAccess();
                    

                    GoToLine(bookmark.LineNumber);
                    
                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error navigating to bookmark: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (bookmarksListView.SelectedItems.Count > 0)
            {
                var bookmark = (Bookmark)bookmarksListView.SelectedItems[0].Tag;
                
                var result = MessageBox.Show(
                    $"Delete bookmark at line {bookmark.LineNumber}?", 
                    "Confirm Delete", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        _bookmarkService.RemoveBookmark(bookmark.LineNumber);
                        LoadBookmarks();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting bookmark: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void clearAllButton_Click(object sender, EventArgs e)
        {
            if (bookmarksListView.Items.Count > 0)
            {
                var result = MessageBox.Show(
                    $"Delete all {bookmarksListView.Items.Count} bookmarks?", 
                    "Confirm Clear All", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        _bookmarkService.ClearAllBookmarks();
                        LoadBookmarks();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error clearing bookmarks: {ex.Message}", "Error", 
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

        private void bookmarksListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void bookmarksListView_DoubleClick(object sender, EventArgs e)
        {
            if (bookmarksListView.SelectedItems.Count > 0)
            {
                goToButton.PerformClick();
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            LoadBookmarks();
        }

        private void GoToLine(int lineNumber)
        {
            try
            {
                if (lineNumber > _textBox.Lines.Length)
                {
                    MessageBox.Show($"Line {lineNumber} doesn't exist. Document has only {_textBox.Lines.Length} lines.", 
                        "Line Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int charIndex = _textBox.GetFirstCharIndexFromLine(lineNumber - 1);
                if (charIndex >= 0)
                {
                    _textBox.SelectionStart = charIndex;
                    _textBox.SelectionLength = 0;
                    _textBox.ScrollToCaret();
                    _textBox.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to line {lineNumber}: {ex.Message}", "Navigation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BookmarksForm_Load(object sender, EventArgs e)
        {
            bookmarksListView.Focus();
        }
    }
}