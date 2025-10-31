using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TextEditorMK.Models;
using TextEditorMK.Repositories.Interfaces;
using TextEditorMK.Helpers; // ? Додаємо using для ThemeHelper

namespace Text_editor_1
{
    public partial class SettingsForm : Form
    {
        private readonly IEditorSettingsRepository _settingsRepository;
        private EditorSettings _currentSettings;
        private EditorTheme _previewTheme;

        public EditorSettings UpdatedSettings { get; private set; }
        public bool SettingsChanged { get; private set; }

        public SettingsForm(IEditorSettingsRepository settingsRepository, EditorSettings currentSettings)
        {
            InitializeComponent();
            _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));
            _currentSettings = currentSettings?.Clone() ?? new EditorSettings();
            
            
            ValidateSettings(_currentSettings);
            
            InitializeSettings();
            
            // ? Використовуємо ThemeHelper для застосування теми
            try
            {
                ApplyPreviewTheme();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"?? Initial preview theme error: {ex.Message}");
                // Встановити дефолтну тему при помилці
                try
                {
                    if (txtPreview != null)
                    {
                        txtPreview.Font = new Font("Consolas", 12);
                        txtPreview.Text = "Settings Preview (Safe Mode)";
                    }
                }
                catch
                {
                    // Ігноруємо помилки fallback
                }
            }
        }

        /// <summary>
        /// Валідація налаштувань перед використанням
        /// </summary>
        private void ValidateSettings(EditorSettings settings)
        {
            if (settings == null) return;

            // Валідація FontSize
            if (settings.FontSize <= 0 || settings.FontSize > 72)
            {
                settings.FontSize = 12;
            }

            // Валідація FontFamily
            if (string.IsNullOrEmpty(settings.FontFamily))
            {
                settings.FontFamily = "Consolas";
            }

            // Валідація Theme
            if (string.IsNullOrEmpty(settings.Theme))
            {
                settings.Theme = "Light";
            }

            // Валідація AutoSaveInterval
            if (settings.AutoSaveInterval <= 0 || settings.AutoSaveInterval > 300)
            {
                settings.AutoSaveInterval = 30;
            }

            // Валідація розмірів вікна
            if (settings.WindowWidth <= 0 || settings.WindowWidth > 2000)
            {
                settings.WindowWidth = 800;
            }

            if (settings.WindowHeight <= 0 || settings.WindowHeight > 1500)
            {
                settings.WindowHeight = 600;
            }

            // Валідація MaxRecentFiles
            if (settings.MaxRecentFiles <= 0 || settings.MaxRecentFiles > 50)
            {
                settings.MaxRecentFiles = 10;
            }
        }

        private void InitializeSettings()
        {
            try
            {
                
                cmbFontFamily.Items.AddRange(new[] { "Consolas", "Courier New", "Arial", "Times New Roman", "Segoe UI" });
                cmbFontFamily.Text = _currentSettings.FontFamily;

                
                numFontSize.Minimum = 8;
                numFontSize.Maximum = 72;
                
                
                int safeFontSize = Math.Max(8, Math.Min(72, _currentSettings.FontSize));
                if (safeFontSize <= 0) safeFontSize = 12; // Додаткова перевірка
                
                numFontSize.Value = safeFontSize;

                // Теми
                var themes = EditorTheme.GetAllThemes();
                cmbTheme.Items.AddRange(themes.Select(t => t.Name).ToArray());
                cmbTheme.Text = _currentSettings.Theme;

                // Налаштування тексту
                chkWordWrap.Checked = _currentSettings.WordWrap;
                chkShowLineNumbers.Checked = _currentSettings.ShowLineNumbers;
                
                // Автозбереження
                chkAutoSave.Checked = _currentSettings.AutoSave;
                numAutoSaveInterval.Minimum = 10;
                numAutoSaveInterval.Maximum = 300;
                numAutoSaveInterval.Value = Math.Max(10, Math.Min(300, _currentSettings.AutoSaveInterval));

                // Енкодування
                cmbDefaultEncoding.Items.AddRange(new[] { "UTF-8", "UTF-16", "Windows-1251" });
                cmbDefaultEncoding.Text = _currentSettings.DefaultEncoding;

                // Недавні файли
                numMaxRecentFiles.Minimum = 5;
                numMaxRecentFiles.Maximum = 50;
                numMaxRecentFiles.Value = Math.Max(5, Math.Min(50, _currentSettings.MaxRecentFiles));

                // UI елементи
                chkShowStatusBar.Checked = _currentSettings.ShowStatusBar;
                chkShowToolbar.Checked = _currentSettings.ShowToolbar;

                // Розмір вікна
                numWindowWidth.Minimum = 400;
                numWindowWidth.Maximum = 2000;
                numWindowWidth.Value = Math.Max(400, Math.Min(2000, _currentSettings.WindowWidth));

                numWindowHeight.Minimum = 300;
                numWindowHeight.Maximum = 1500;
                numWindowHeight.Value = Math.Max(300, Math.Min(1500, _currentSettings.WindowHeight));
                
                System.Diagnostics.Debug.WriteLine($"? Settings initialized: FontSize={numFontSize.Value}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Error initializing settings: {ex.Message}");
                
                // Встановити безпечні дефолтні значення при помилці
                try
                {
                    numFontSize.Minimum = 8;
                    numFontSize.Maximum = 72;
                    numFontSize.Value = 12;
                    cmbFontFamily.Text = "Consolas";
                    cmbTheme.Text = "Light";
                }
                catch
                {
                    // Ігноруємо помилки fallback
                }
            }
        }

        private void ApplyPreviewTheme()
        {
            _previewTheme = EditorTheme.GetByName(cmbTheme.Text);
            
            try
            {
                // ? Використовуємо ThemeHelper для застосування теми
                ThemeHelper.ApplyThemeToForm(this, _previewTheme);

                // Попередній перегляд тексту
                if (txtPreview != null)
                {
                    // ? Безпечне отримання розміру шрифту з валідацією
                    int fontSize = (int)numFontSize.Value;
                    if (fontSize <= 0 || fontSize > 72)
                    {
                        fontSize = 12; // Дефолтний розмір
                    }
                    
                    string fontFamily = string.IsNullOrEmpty(cmbFontFamily.Text) ? "Consolas" : cmbFontFamily.Text;
                    
                    try
                    {
                        txtPreview.Font = new Font(fontFamily, fontSize);
                        txtPreview.Text = $"Preview of {_previewTheme.Name} theme\nFont: {fontFamily} {fontSize}pt\nThis is how your editor will look.\n\n**Bold text** and *italic text* examples";
                    }
                    catch (Exception ex)
                    {
                        // Fallback на дефолтний шрифт при помилці
                        txtPreview.Font = new Font("Consolas", 12);
                        txtPreview.Text = $"Preview of {_previewTheme.Name} theme\nFont: Consolas 12pt (fallback)\nThis is how your editor will look.";
                        System.Diagnostics.Debug.WriteLine($"?? Font preview error: {ex.Message}");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"? Theme preview applied: {_previewTheme.Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Theme preview error: {ex.Message}");
            }
        }

        private void cmbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ApplyPreviewTheme();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"?? Theme preview error: {ex.Message}");
            }
        }

        private void cmbFontFamily_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ApplyPreviewTheme();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"?? Font family preview error: {ex.Message}");
            }
        }

        private void numFontSize_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                
                if (numFontSize.Value >= numFontSize.Minimum && numFontSize.Value <= numFontSize.Maximum)
                {
                    ApplyPreviewTheme();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"?? Font size preview error: {ex.Message}");
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                // ? Валідація даних перед збереженням
                if (numFontSize.Value <= 0 || numFontSize.Value > 72)
                {
                    MessageBox.Show("Font size must be between 1 and 72.", "Invalid Font Size", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (numAutoSaveInterval.Value < 10 || numAutoSaveInterval.Value > 300)
                {
                    MessageBox.Show("Auto-save interval must be between 10 and 300 seconds.", "Invalid Interval", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(cmbFontFamily.Text))
                {
                    MessageBox.Show("Please select a font family.", "Invalid Font", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Зберегти налаштування з валідованими значеннями
                _currentSettings.FontFamily = cmbFontFamily.Text;
                _currentSettings.FontSize = (int)numFontSize.Value;
                _currentSettings.Theme = cmbTheme.Text;
                _currentSettings.WordWrap = chkWordWrap.Checked;
                _currentSettings.ShowLineNumbers = chkShowLineNumbers.Checked;
                _currentSettings.AutoSave = chkAutoSave.Checked;
                _currentSettings.AutoSaveInterval = (int)numAutoSaveInterval.Value;
                _currentSettings.DefaultEncoding = cmbDefaultEncoding.Text;
                _currentSettings.MaxRecentFiles = (int)numMaxRecentFiles.Value;
                _currentSettings.ShowStatusBar = chkShowStatusBar.Checked;
                _currentSettings.ShowToolbar = chkShowToolbar.Checked;
                _currentSettings.WindowWidth = (int)numWindowWidth.Value;
                _currentSettings.WindowHeight = (int)numWindowHeight.Value;

                _settingsRepository.Update(_currentSettings);
                UpdatedSettings = _currentSettings;
                SettingsChanged = true;
                
                MessageBox.Show("Settings saved successfully!", "Settings", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Reset all settings to default values?", "Confirm Reset", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                try
                {
                    _currentSettings.Reset();
                    ValidateSettings(_currentSettings); 
                    InitializeSettings();
                    
                    // ? Безпечний виклик попереднього перегляду
                    try
                    {
                        ApplyPreviewTheme();
                    }
                    catch (Exception previewEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"?? Preview error after reset: {previewEx.Message}");
                    }
                    
                    MessageBox.Show("Settings reset to default values.", "Settings Reset", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error resetting settings: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtPreview_TextChanged(object sender, EventArgs e)
        {

        }
    }
}