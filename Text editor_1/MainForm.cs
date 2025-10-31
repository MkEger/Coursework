using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TextEditorMK.Models;
using TextEditorMK.Repositories.Implementations;
using TextEditorMK.Repositories.Interfaces;
using TextEditorMK.Services;

namespace Text_editor_1
{
    /// <summary>
    /// Головна форма текстового редактора з 4 шаблонами проектування:
    /// 1. Abstract Factory - для вибору типу репозиторіїв
    /// 2. Command Pattern - для обробки команд меню
    /// 3. Factory Method - для створення документів
    /// 4. Observer Pattern - для відслідковування змін документа
    /// + Service Layer - для бізнес-логіки
    /// </summary>
    public partial class MainForm : Form
    {
        #region Fields

        // Repository Pattern - абстракція доступу до даних
        private IDocumentRepository _documentRepository;
        private IEncodingRepository _encodingRepository;
        private IRecentFileRepository _recentFileRepository;
        private IEditorSettingsRepository _settingsRepository;

        // Service Layer - бізнес-логіка
        private DocumentService _documentService;
        private TextEditorMK.Services.EditorExtensionsService _extensionsService; // ✅ Додаємо сервіс розширень

        // Поточний стан
        private Document _currentDocument;
        private EditorSettings _currentSettings;

        // Observer Pattern - автозбереження
        private Timer _autoSaveTimer;
        
        // Markdown підсвітка
        private Timer _markdownHighlightTimer;
        
        // ✅ Прапорець для контролю Markdown підсвітки
        private bool _markdownHighlightEnabled = true;
        
        // ✅ Додаткові поля для оптимізації
        private bool _isHighlighting = false;
        private string _lastHighlightedText = string.Empty;

        #endregion

        #region Constructor

        public MainForm()
        {
            InitializeComponent();
            InitializeRepositories();
            InitializeDocumentService();
            InitializeExtensionsService(); // ✅ Ініціалізація сервісу розширень
            InitializeAutoSave();
            CreateNewDocument();
        }

        #endregion

        #region Abstract Factory Pattern

        /// <summary>
        /// Abstract Factory Pattern - вибір типу репозиторіїв на основі доступності БД
        /// </summary>
        private void InitializeRepositories()
        {
            try
            {
                // ОБОВ'ЯЗКОВЕ підключення до MySQL для недавніх файлів
                _recentFileRepository = new MySqlRecentFileRepository();
                _documentRepository = new MySqlDocumentRepository();
                
                // Інші репозиторії можуть бути in-memory
                _encodingRepository = new EncodingRepository();
                _settingsRepository = new EditorSettingsRepository();
                
                MessageBox.Show(
                    "? Successfully connected to MySQL database!\n" +
                    "?? Recent files will be stored in database permanently.\n" +
                    "?? All your recent files will persist between sessions.",
                    "Database Connection - MySQL Ready", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"? MySQL database connection FAILED!\n\n" +
                    $"Error: {ex.Message}\n\n" +
                    $"?? IMPORTANT: Recent files CANNOT be saved without MySQL!\n" +
                    $"?? Please check your MySQL connection and restart the application.\n\n" +
                    $"The application will continue but recent files will NOT be saved.",
                    "Critical Database Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                
                // Для недавніх файлів БЕЗ fallback - показуємо що БД недоступна
                _recentFileRepository = null;
                _documentRepository = new DocumentRepository(); // Fallback для документів
                _encodingRepository = new EncodingRepository();
                _settingsRepository = new EditorSettingsRepository();
            }
        }

        /// <summary>
        /// Ініціалізація DocumentService з підпискою на події
        /// </summary>
        private void InitializeDocumentService()
        {
            _documentService = new DocumentService(_documentRepository, _recentFileRepository, _encodingRepository);
            
            // Підписатися на події DocumentService
            _documentService.DocumentAdded += OnDocumentServiceEvent;
            _documentService.DocumentSaved += OnDocumentServiceEvent;
            _documentService.DocumentDeleted += OnDocumentServiceEvent;
        }

        private void OnDocumentServiceEvent(object sender, DocumentServiceEventArgs e)
        {
            ShowStatusMessage($"{e.Action}: {e.Document.FileName}", 2000);
            System.Diagnostics.Debug.WriteLine($"[DocumentService] {e.Action}: {e.Document.FileName} at {e.Timestamp}");
        }

        /// <summary>
        /// ✅ Ініціалізація сервісу розширених функцій
        /// </summary>
        private void InitializeExtensionsService()
        {
            try
            {
                _extensionsService = new TextEditorMK.Services.EditorExtensionsService(richTextBox1);
                
                // Підписуємося на події розширень
                _extensionsService.MacroStarted += OnExtensionEvent;
                _extensionsService.MacroStopped += OnExtensionEvent;
                _extensionsService.SnippetInserted += OnExtensionEvent;
                _extensionsService.BookmarkToggled += OnExtensionEvent;
                
                System.Diagnostics.Debug.WriteLine("✅ Extensions service initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Failed to initialize extensions service: {ex.Message}");
                MessageBox.Show($"Warning: Advanced features may not work properly.\nError: {ex.Message}", 
                    "Extensions Initialization Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Обробник подій розширень для показу статусних повідомлень
        /// </summary>
        private void OnExtensionEvent(object sender, TextEditorMK.Services.ExtensionEventArgs e)
        {
            try
            {
                ShowStatusMessage($"{e.ActionType}: {e.Details}", 2000);
                System.Diagnostics.Debug.WriteLine($"[Extension] {e.ActionType}: {e.Details} at {e.Timestamp}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error handling extension event: {ex.Message}");
            }
        }

        #endregion

        #region Event Handlers

        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdateTitle();
            LoadSettings();
            UpdateStatusBar();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (HasUnsavedChanges())
            {
                var result = MessageBox.Show(
                    "You have unsaved changes. Do you want to save before closing?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Yes:
                        SaveDocument();
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }
        }

        /// <summary>
        /// Observer Pattern - реагування на зміни тексту
        /// </summary>
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (_currentDocument != null && richTextBox1 != null)
            {
                // ✅ Запобігти зворотним викликам під час підсвітки
                if (_isHighlighting) return;

                // Оновити документ через сервіс
                _currentDocument.SetContent(richTextBox1.Text);
                _documentService.UpdateDocument(_currentDocument);
                
                // ✅ Застосувати підсвітку тільки якщо вона увімкнена і це Markdown файл
                if (_markdownHighlightEnabled && 
                    !string.IsNullOrEmpty(_currentDocument.FilePath) && 
                    Path.GetExtension(_currentDocument.FilePath).ToLower() == ".md")
                {
                    // ✅ Перевірити чи текст змінився з останньої підсвітки
                    if (_lastHighlightedText == richTextBox1.Text) return;
                    
                    // ✅ Розумна затримка залежно від розміру тексту та типу зміни
                    int textLength = richTextBox1.Text.Length;
                    int delay;
                    
                    if (textLength > 10000)
                        delay = 2000;  // Великий файл - довга затримка
                    else if (textLength > 5000)
                        delay = 1500;  // Середній файл
                    else if (textLength > 1000)
                        delay = 800;   // Невеликий файл
                    else
                        delay = 400;   // Маленький файл - швидка реакція
                    
                    // ✅ Перезавантажити таймер з новою затримкою
                    if (_markdownHighlightTimer != null)
                    {
                        _markdownHighlightTimer.Stop();
                        _markdownHighlightTimer.Interval = delay;
                        _markdownHighlightTimer.Start();
                    }
                    else
                    {
                        _markdownHighlightTimer = new Timer();
                        _markdownHighlightTimer.Interval = delay;
                        _markdownHighlightTimer.Tick += MarkdownHighlightTimer_Tick;
                        _markdownHighlightTimer.Start();
                    }
                }
                
                // Оновити UI (це теж можна оптимізувати)
                UpdateTitle();
                UpdateStatusBar();
            }
        }

        /// <summary>
        /// ✅ Окремий обробник таймера для кращого контролю
        /// </summary>
        private void MarkdownHighlightTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                _markdownHighlightTimer?.Stop();
                
                // ✅ Подвійна перевірка та захист від повторних викликів
                if (_markdownHighlightEnabled && 
                    !_isHighlighting &&
                    !string.IsNullOrEmpty(_currentDocument?.FilePath) && 
                    Path.GetExtension(_currentDocument.FilePath).ToLower() == ".md" &&
                    _lastHighlightedText != richTextBox1.Text)
                {
                    ApplyMarkdownHighlighting();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Markdown highlight timer error: {ex.Message}");
            }
        }

        #endregion

        #region Command Pattern - Menu Handlers

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteNewDocumentCommand();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteOpenDocumentCommand();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteSaveDocumentCommand();
        }

        private void recentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteShowRecentFilesCommand();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteExitApplicationCommand();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecuteShowSettingsCommand();
        }

        #endregion

        #region Command Implementations

        private void ExecuteNewDocumentCommand()
        {
            try
            {
                CreateNewDocument();
                ShowStatusMessage("New document created", 2000);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create new document: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExecuteOpenDocumentCommand()
        {
            try
            {
                OpenDocument();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open document: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExecuteSaveDocumentCommand()
        {
            try
            {
                SaveDocument();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save document: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExecuteShowRecentFilesCommand()
        {
            try
            {
                ShowRecentFiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening recent files: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExecuteShowSettingsCommand()
        {
            try
            {
                ShowSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening settings: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExecuteExitApplicationCommand()
        {
            this.Close();
        }

        #endregion

        #region Document Operations (using DocumentService)

        /// <summary>
        /// Створення нового документа через DocumentService
        /// </summary>
        public void CreateNewDocument()
        {
            try
            {
                _currentDocument = _documentService.CreateNewDocument();

                if (richTextBox1 != null)
                {
                    richTextBox1.Clear();
                }

                UpdateTitle();
                UpdateStatusBar();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create new document: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Відкриття документа через DocumentService
        /// </summary>
        public void OpenDocument()
        {
            var openDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|Markdown files (*.md)|*.md|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _currentDocument = _documentService.OpenDocument(openDialog.FileName);

                    if (richTextBox1 != null)
                    {
                        richTextBox1.Text = _currentDocument.Content;
                        
                        // ✅ Оновити мову в сервісі розширень
                        if (_extensionsService != null)
                        {
                            _extensionsService.DetectLanguageFromFile(openDialog.FileName);
                        }
                        
                        // ✅ Застосувати підсвітку синтаксиса для Markdown
                        if (Path.GetExtension(openDialog.FileName).ToLower() == ".md")
                        {
                            ApplyMarkdownHighlighting();
                        }
                        else
                        {
                            ClearSyntaxHighlighting();
                        }
                    }

                    UpdateTitle();
                    UpdateStatusBar();
                    ShowStatusMessage($"Opened: {_currentDocument.FileName}", 2000);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to open file: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Збереження документа через DocumentService
        /// </summary>
        public void SaveDocument()
        {
            if (_currentDocument == null) return;

            try
            {
                if (string.IsNullOrEmpty(_currentDocument.FilePath))
                {
                    SaveAsDocument();
                    return;
                }

                _documentService.SaveDocument(_currentDocument);
                
                UpdateTitle();
                ShowStatusMessage($"Saved: {_currentDocument.FileName}", 2000);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save document: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Збереження документа як новий файл
        /// </summary>
        public void SaveAsDocument()
        {
            if (_currentDocument == null) return;

            var saveDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|Markdown files (*.md)|*.md|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = _currentDocument.FileName
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _documentService.SaveDocument(_currentDocument, saveDialog.FileName);
                    
                    // ✅ Застосувати підсвітку якщо це Markdown файл
                    if (Path.GetExtension(saveDialog.FileName).ToLower() == ".md")
                    {
                        ApplyMarkdownHighlighting();
                    }
                    else
                    {
                        ClearSyntaxHighlighting();
                    }
                    
                    UpdateTitle();
                    ShowStatusMessage($"Saved as: {_currentDocument.FileName}", 2000);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to save document as: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Показ недавніх файлів
        /// </summary>
        public void ShowRecentFiles()
        {
            try
            {
                // ✅ Передаємо поточні налаштування для застосування теми
                var recentForm = new RecentFilesForm(_recentFileRepository, _documentRepository, _encodingRepository, _currentSettings);
                if (recentForm.ShowDialog() == DialogResult.OK && recentForm.SelectedDocument != null)
                {
                    LoadDocument(recentForm.SelectedDocument);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to show recent files: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Показ налаштувань редактора
        /// </summary>
        public void ShowSettings()
        {
            try
            {
                var settingsForm = new SettingsForm(_settingsRepository, _currentSettings);
                if (settingsForm.ShowDialog() == DialogResult.OK && settingsForm.SettingsChanged)
                {
                    _currentSettings = settingsForm.UpdatedSettings;
                    ApplySettings(_currentSettings);
                    ShowStatusMessage("Settings applied successfully", 2000);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to show settings: {ex.Message}", ex);
            }
        }

        #endregion

        #region Utility Methods

        private void LoadDocument(Document document)
        {
            if (document == null) return;

            _currentDocument = document;

            if (richTextBox1 != null)
            {
                richTextBox1.Text = document.Content;
            }

            UpdateTitle();
            UpdateStatusBar();
        }

        private void UpdateTitle()
        {
            if (_currentDocument != null)
            {
                string status = _currentDocument.IsSaved ? "" : "*";
                string encoding = _currentDocument.TextEncoding?.Name ?? "UTF-8";
                this.Text = $"Text Editor MK - {_currentDocument.FileName}{status} [{encoding}]";
            }
            else
            {
                this.Text = "Text Editor MK - Professional Course Project";
            }
        }

        /// <summary>
        /// ✅ Оптимізоване Observer Pattern - оновлення статистики з обмеженням частоти
        /// </summary>
        private void UpdateStatusBar()
        {
            if (statusStrip1 == null || _currentDocument == null || _isHighlighting) return;

            try
            {
                // ✅ Кешовані значення для уникнення повторних обчислень
                var content = _currentDocument.Content ?? string.Empty;
                var charCount = content.Length;
                
                if (statusStrip1.Items.Count > 0)
                {
                    // ✅ Обчислювати слова та рядка тільки якщо текст не занадто великий
                    if (charCount < 10000)
                    {
                        var wordCount = string.IsNullOrWhiteSpace(content) ? 0 :
                            content.Split(new[] { ' ', '\n', '\r', '\t' }, 
                            StringSplitOptions.RemoveEmptyEntries).Length;
                        var lineCount = content.Split(new[] { '\n' }, 
                            StringSplitOptions.None).Length;
                        
                        statusStrip1.Items[0].Text = 
                            $"Chars: {charCount:N0} | Words: {wordCount:N0} | Lines: {lineCount:N0}";
                    }
                    else
                    {
                        // Для великих файлів показувати тільки символи
                        statusStrip1.Items[0].Text = $"Chars: {charCount:N0} | Large file";
                    }
                }

                // ✅ Позицію курсора обчислювати тільки якщо потрібно
                if (richTextBox1 != null && statusStrip1.Items.Count > 1 && !_isHighlighting)
                {
                    try
                    {
                        int line = richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart) + 1;
                        int column = richTextBox1.SelectionStart - 
                            richTextBox1.GetFirstCharIndexFromLine(line - 1) + 1;
                        statusStrip1.Items[1].Text = $"Ln {line}, Col {column}";
                    }
                    catch
                    {
                        // Ігнорувати помилки позиції курсора під час підсвітки
                        statusStrip1.Items[1].Text = "Ln -, Col -";
                    }
                }

                // Енкодування
                if (statusStrip1.Items.Count > 2)
                {
                    statusStrip1.Items[2].Text = _currentDocument.TextEncoding?.Name ?? "UTF-8";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating status bar: {ex.Message}");
            }
        }

        private void ShowStatusMessage(string message, int duration)
        {
            if (statusStrip1 != null && statusStrip1.Items.Count > 3)
            {
                statusStrip1.Items[3].Text = message;
                
                // Таймер для очищення повідомлення
                var messageTimer = new Timer();
                messageTimer.Interval = duration;
                messageTimer.Tick += (s, e) =>
                {
                    if (statusStrip1.Items.Count > 3)
                        statusStrip1.Items[3].Text = "Ready";
                    messageTimer.Stop();
                    messageTimer.Dispose();
                };
                messageTimer.Start();
            }
        }

        private void LoadSettings()
        {
            try
            {
                _currentSettings = _settingsRepository.GetCurrent();
                
                // ✅ Перевірка чи налаштування коректні
                if (_currentSettings == null)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ No settings found, creating default");
                    _currentSettings = new EditorSettings();
                }

                // ✅ Валідація критичних параметрів
                if (_currentSettings.FontSize <= 0 || _currentSettings.FontSize > 72)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Invalid FontSize {_currentSettings.FontSize}, resetting to 12");
                    _currentSettings.FontSize = 12;
                }

                if (string.IsNullOrEmpty(_currentSettings.FontFamily))
                {
                    _currentSettings.FontFamily = "Consolas";
                }

                if (string.IsNullOrEmpty(_currentSettings.Theme))
                {
                    _currentSettings.Theme = "Light";
                }

                if (_currentSettings.AutoSaveInterval <= 0)
                {
                    _currentSettings.AutoSaveInterval = 30;
                }

                ApplySettings(_currentSettings);
                System.Diagnostics.Debug.WriteLine($"✅ Settings loaded: Font={_currentSettings.FontFamily} {_currentSettings.FontSize}pt");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Failed to load settings: {ex.Message}");
                
                // Створити дефолтні налаштування при помилці
                _currentSettings = new EditorSettings();
                ApplyDefaultSettings();
                
                // Спробувати зберегти дефолтні налаштування
                try
                {
                    _settingsRepository.Update(_currentSettings);
                    System.Diagnostics.Debug.WriteLine("✅ Saved default settings to repository");
                }
                catch (Exception saveEx)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Failed to save default settings: {saveEx.Message}");
                }
            }
        }

        private void ApplySettings(EditorSettings settings)
        {
            if (settings == null) return;

            try
            {
                // Застосувати тему
                var theme = EditorTheme.GetByName(settings.Theme);
                ApplyTheme(theme);

                // Застосувати налаштування шрифту з перевіркою розміру
                if (richTextBox1 != null)
                {
                    // ✅ Перевірка та виправлення розміру шрифту
                    int fontSize = settings.FontSize;
                    if (fontSize <= 0 || fontSize > 72)
                    {
                        fontSize = 12; // Дефолтний розмір
                        System.Diagnostics.Debug.WriteLine($"⚠️ Invalid font size {settings.FontSize}, using default 12");
                    }

                    string fontFamily = string.IsNullOrEmpty(settings.FontFamily) ? "Consolas" : settings.FontFamily;
                    
                    try
                    {
                        richTextBox1.Font = new System.Drawing.Font(fontFamily, fontSize);
                        richTextBox1.WordWrap = settings.WordWrap;
                    }
                    catch (Exception fontEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ Font error: {fontEx.Message}, using default font");
                        richTextBox1.Font = new System.Drawing.Font("Consolas", 12);
                        richTextBox1.WordWrap = false;
                    }
                }

                // Застосувати розмір вікна з перевіркою
                if (settings.WindowWidth > 400 && settings.WindowHeight > 300)
                {
                    this.Size = new System.Drawing.Size(settings.WindowWidth, settings.WindowHeight);
                }

                // Застосувати видимість компонентів
                if (statusStrip1 != null)
                {
                    statusStrip1.Visible = settings.ShowStatusBar;
                }

                // Оновити інтервал автозбереження з перевіркою
                if (_autoSaveTimer != null)
                {
                    int interval = settings.AutoSaveInterval;
                    if (interval < 10 || interval > 300)
                    {
                        interval = 30; // Дефолтний інтервал
                        System.Diagnostics.Debug.WriteLine($"⚠️ Invalid autosave interval {settings.AutoSaveInterval}, using default 30");
                    }

                    _autoSaveTimer.Interval = interval * 1000; // Convert to milliseconds
                    _autoSaveTimer.Enabled = settings.AutoSave;
                }

                System.Diagnostics.Debug.WriteLine($"✅ Settings applied successfully: Font={settings.FontFamily} {settings.FontSize}pt, Theme={settings.Theme}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Failed to apply settings: {ex.Message}");
                // Застосувати дефолтні налаштування при помилці
                ApplyDefaultSettings();
            }
        }

        /// <summary>
        /// Застосувати дефолтні налаштування при помилці
        /// </summary>
        private void ApplyDefaultSettings()
        {
            try
            {
                if (richTextBox1 != null)
                {
                    richTextBox1.Font = new System.Drawing.Font("Consolas", 12);
                    richTextBox1.WordWrap = false;
                }

                var defaultTheme = EditorTheme.Light;
                ApplyTheme(defaultTheme);

                if (_autoSaveTimer != null)
                {
                    _autoSaveTimer.Interval = 30000; // 30 seconds
                    _autoSaveTimer.Enabled = true;
                }

                System.Diagnostics.Debug.WriteLine("✅ Applied default settings due to error");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Failed to apply default settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Застосувати тему до інтерфейсу
        /// </summary>
        private void ApplyTheme(EditorTheme theme)
        {
            if (theme == null) return;

            try
            {
                // Основні кольори форми
                this.BackColor = theme.BackgroundColor;
                this.ForeColor = theme.ForegroundColor;

                // ✅ Застосувати до RichTextBox з повним скиданням форматування
                if (richTextBox1 != null)
                {
                    // Використати новий метод для повного скидання форматування
                    ResetTextFormatting(theme);
                    
                    // ✅ Якщо це Markdown файл, повторно застосувати підсвітку після короткої затримки
                    if (!string.IsNullOrEmpty(_currentDocument?.FilePath) && 
                        Path.GetExtension(_currentDocument.FilePath).ToLower() == ".md")
                    {
                        var themeTimer = new Timer();
                        themeTimer.Interval = 200; // Збільшуємо затримку для стабільності
                        themeTimer.Tick += (s, e) =>
                        {
                            ApplyMarkdownHighlighting();
                            themeTimer.Stop();
                            themeTimer.Dispose();
                        };
                        themeTimer.Start();
                    }
                }

                // ✅ Покращене застосування до меню
                if (menuStrip1 != null)
                {
                    menuStrip1.BackColor = theme.MenuBackColor;
                    menuStrip1.ForeColor = theme.MenuForeColor;
                    
                    // ✅ Додаткові налаштування для темної теми
                    if (theme.Name == "Dark")
                    {
                        menuStrip1.RenderMode = ToolStripRenderMode.Professional;
                        menuStrip1.Renderer = new DarkMenuRenderer(theme);
                    }
                    else
                    {
                        menuStrip1.RenderMode = ToolStripRenderMode.System;
                    }
                    
                    // Застосувати до всіх пунктів меню
                    foreach (ToolStripMenuItem item in menuStrip1.Items)
                    {
                        ApplyThemeToMenuItem(item, theme);
                    }
                }

                // Застосувати до статус бару
                if (statusStrip1 != null)
                {
                    statusStrip1.BackColor = theme.StatusBarBackColor;
                    statusStrip1.ForeColor = theme.StatusBarForeColor;
                    
                    // ✅ Додаткові налаштування для темної теми
                    if (theme.Name == "Dark")
                    {
                        statusStrip1.RenderMode = ToolStripRenderMode.Professional;
                        statusStrip1.Renderer = new DarkStatusStripRenderer(theme);
                    }
                    else
                    {
                        statusStrip1.RenderMode = ToolStripRenderMode.System;
                    }
                    
                    foreach (ToolStripStatusLabel label in statusStrip1.Items)
                    {
                        label.BackColor = theme.StatusBarBackColor;
                        label.ForeColor = theme.StatusBarForeColor;
                    }
                }

                System.Diagnostics.Debug.WriteLine($"✅ Applied theme: {theme.Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Failed to apply theme: {ex.Message}");
            }
        }

        /// <summary>
        /// Застосувати тему до пункту меню та його підпунктів
        /// </summary>
        private void ApplyThemeToMenuItem(ToolStripMenuItem item, EditorTheme theme)
        {
            if (item == null || theme == null) return;

            item.BackColor = theme.MenuBackColor;
            item.ForeColor = theme.MenuForeColor;

            // Рекурсивно застосувати до підпунктів
            foreach (ToolStripItem subItem in item.DropDownItems)
            {
                if (subItem is ToolStripMenuItem menuItem)
                {
                    ApplyThemeToMenuItem(menuItem, theme);
                }
                else if (subItem is ToolStripSeparator separator)
                {
                    separator.BackColor = theme.MenuBackColor;
                    separator.ForeColor = theme.BorderColor;
                }
            }
        }

        #endregion

        #region Markdown Syntax Highlighting

        /// <summary>
        /// ✅ Максимально оптимізована підсвітка синтаксису Markdown
        /// </summary>
        private void ApplyMarkdownHighlighting()
        {
            if (richTextBox1 == null || string.IsNullOrEmpty(richTextBox1.Text) || _isHighlighting)
                return;

            try
            {
                // ✅ Встановити прапорець для запобігання повторних викликів
                _isHighlighting = true;
                
                // Зберегти поточний текст для порівняння
                _lastHighlightedText = richTextBox1.Text;
                
                // Зберегти позицію курсора
                int cursorPosition = richTextBox1.SelectionStart;
                
                // ✅ Відключити всі оновлення та події
                richTextBox1.SuspendLayout();
                
                // ✅ Тимчасово відключити TextChanged щоб запобігти зворотним викликам
                richTextBox1.TextChanged -= richTextBox1_TextChanged;
                
                // ✅ Швидке очищення попередньої підсвітки
                ClearPreviousHighlightingOptimized();
                
                // ✅ Застосувати підсвітку тільки якщо текст не занадто великий
                if (richTextBox1.Text.Length < 50000) // Ліміт для продуктивності
                {
                    // Застосувати підсвітку в оптимізованому порядку
                    HighlightMarkdownCodeBlocks();
                    HighlightMarkdownInlineCode();
                    HighlightMarkdownHeaders();
                    HighlightMarkdownLinks();
                    HighlightMarkdownBoldText();
                    HighlightMarkdownItalicText();
                    HighlightMarkdownQuotes();
                    HighlightMarkdownLists();
                    HighlightMarkdownHorizontalRules();
                }
                
                // ✅ Відновити позицію курсора
                richTextBox1.SelectionStart = Math.Min(cursorPosition, richTextBox1.Text.Length);
                richTextBox1.SelectionLength = 0;
                
                System.Diagnostics.Debug.WriteLine($"✅ Optimized Markdown highlighting applied to {richTextBox1.Text.Length} chars");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Markdown highlighting error: {ex.Message}");
            }
            finally
            {
                try
                {
                    // ✅ Обов'язково відновити події та layout
                    richTextBox1.TextChanged += richTextBox1_TextChanged;
                    richTextBox1.ResumeLayout();
                    _isHighlighting = false;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Error restoring events: {ex.Message}");
                    _isHighlighting = false;
                }
            }
        }

        /// <summary>
        /// ✅ Оптимізоване очищення підсвітки
        /// </summary>
        private void ClearPreviousHighlightingOptimized()
        {
            try
            {
                // Отримати поточну тему швидше
                var currentTheme = _currentSettings?.Theme != null ? 
                    EditorTheme.GetByName(_currentSettings.Theme) : EditorTheme.Light;
                
                // ✅ Швидке очищення без зайвих операцій
                richTextBox1.SelectAll();
                richTextBox1.SelectionColor = currentTheme.TextBoxForeColor;
                richTextBox1.SelectionFont = richTextBox1.Font;
                richTextBox1.SelectionBackColor = Color.Empty;
                richTextBox1.SelectionLength = 0; // Скинути виділення
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Optimized clear error: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ Підсвітка блоків коду ```code```
        /// </summary>
        private void HighlightMarkdownCodeBlocks()
        {
            var codeColor = Color.FromArgb(100, 150, 100);
            var codeFont = new Font("Consolas", richTextBox1.Font.Size, FontStyle.Regular);
            
            // Покращений паттерн для блоків коду
            var pattern = @"```[\s\S]*?```";
            HighlightPattern(pattern, codeColor, FontStyle.Regular, codeFont, Color.FromArgb(240, 240, 240));
        }

        /// <summary>
        /// ✅ Підсвітка інлайн коду `code`
        /// </summary>
        private void HighlightMarkdownInlineCode()
        {
            var codeColor = Color.FromArgb(100, 150, 100);
            var codeFont = new Font("Consolas", richTextBox1.Font.Size, FontStyle.Regular);
            
            // Покращений паттерн для інлайн коду (не конфліктує з блоками)
            var pattern = @"(?<!`)(`[^`\r\n]+`)(?!`)";
            HighlightPattern(pattern, codeColor, FontStyle.Regular, codeFont, Color.FromArgb(245, 245, 245));
        }

        /// <summary>
        /// ✅ Покращена підсвітка заголовків
        /// </summary>
        private void HighlightMarkdownHeaders()
        {
            var text = richTextBox1.Text;
            var lines = text.Split('\n');
            int currentIndex = 0;

            for (int lineNum = 0; lineNum < lines.Length; lineNum++)
            {
                var line = lines[lineNum];
                var trimmedLine = line.Trim();
                
                if (trimmedLine.StartsWith("#") && trimmedLine.Length > 1 && trimmedLine[1] == ' ' || 
                    (trimmedLine.StartsWith("##") && (trimmedLine.Length == 2 || trimmedLine[2] == ' ')))
                {
                    // Підрахувати рівень заголовка
                    int headerLevel = 0;
                    for (int i = 0; i < trimmedLine.Length && trimmedLine[i] == '#'; i++)
                        headerLevel++;

                    if (headerLevel > 0 && headerLevel <= 6)
                    {
                        // Визначити колір залежно від рівня
                        Color headerColor = GetHeaderColor(headerLevel);
                        float fontSize = richTextBox1.Font.Size + (7 - headerLevel);
                        
                        richTextBox1.Select(currentIndex, line.Length);
                        richTextBox1.SelectionColor = headerColor;
                        richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 
                            fontSize, FontStyle.Bold);
                    }
                }
                
                currentIndex += line.Length;
                if (lineNum < lines.Length - 1) 
                    currentIndex += 1; // Додати символ нового рядка
            }
        }

        /// <summary>
        /// ✅ Отримати колір для заголовка залежно від рівня
        /// </summary>
        private Color GetHeaderColor(int level)
        {
            switch (level)
            {
                case 1: return Color.FromArgb(0, 80, 160);   // Темно-синій
                case 2: return Color.FromArgb(0, 100, 180);  // Синій
                case 3: return Color.FromArgb(0, 120, 200);  // Світло-синій
                case 4: return Color.FromArgb(60, 140, 220); // Блакитний
                case 5: return Color.FromArgb(80, 160, 240); // Світло-блакитний
                case 6: return Color.FromArgb(100, 180, 255);// Дуже світло-блакитний
                default: return Color.FromArgb(0, 100, 200);
            }
        }

        /// <summary>
        /// ✅ Покращена підсвітка жирного тексту **text**
        /// </summary>
        private void HighlightMarkdownBoldText()
        {
            var boldColor = Color.FromArgb(150, 50, 50);
            // Покращений паттерн який не конфліктує з іншими елементами
            var pattern = @"(?<!\*)\*\*(?!\s)([^\*\r\n]+?)(?<!\s)\*\*(?!\*)";
            HighlightPattern(pattern, boldColor, FontStyle.Bold);
        }

        /// <summary>
        /// ✅ Покращена підсвітка курсивного тексту *text*
        /// </summary>
        private void HighlightMarkdownItalicText()
        {
            var italicColor = Color.FromArgb(100, 100, 150);
            // Паттерн який не конфліктує з жирним текстом та списками
            var pattern = @"(?<!\*)\*(?!\*)(?!\s)([^\*\r\n]+?)(?<!\s)\*(?!\*)";
            HighlightPattern(pattern, italicColor, FontStyle.Italic);
        }

        /// <summary>
        /// ✅ Покращена підсвітка посилань [text](url)
        /// </summary>
        private void HighlightMarkdownLinks()
        {
            var linkColor = Color.FromArgb(0, 150, 200);
            
            // Звичайні посилання
            var linkPattern = @"\[([^\]]+)\]\([^)]+\)";
            HighlightPattern(linkPattern, linkColor, FontStyle.Underline);
            
            // Автоматичні посилання <url>
            var autoLinkPattern = @"<(https?://[^>]+)>";
            HighlightPattern(autoLinkPattern, linkColor, FontStyle.Underline);
        }

        /// <summary>
        /// ✅ Покращена підсвітка цитат > text
        /// </summary>
        private void HighlightMarkdownQuotes()
        {
            var quoteColor = Color.FromArgb(120, 120, 120);
            var text = richTextBox1.Text;
            var lines = text.Split('\n');
            int currentIndex = 0;

            for (int lineNum = 0; lineNum < lines.Length; lineNum++)
            {
                var line = lines[lineNum];
                var trimmedLine = line.TrimStart();
                
                if (trimmedLine.StartsWith(">"))
                {
                    richTextBox1.Select(currentIndex, line.Length);
                    richTextBox1.SelectionColor = quoteColor;
                    richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 
                        richTextBox1.Font.Size, FontStyle.Italic);
                    richTextBox1.SelectionBackColor = Color.FromArgb(250, 250, 250);
                }
                
                currentIndex += line.Length;
                if (lineNum < lines.Length - 1)
                    currentIndex += 1;
            }
        }

        /// <summary>
        /// ✅ Покращена підсвітка списків
        /// </summary>
        private void HighlightMarkdownLists()
        {
            var listColor = Color.FromArgb(150, 100, 50);
            
            // Маркіровані списки з покращеним паттерном
            var unorderedListPattern = @"^(\s*)[-*+](\s)";
            HighlightPattern(unorderedListPattern, listColor, FontStyle.Bold);
            
            // Номером списки
            var orderedListPattern = @"^(\s*)\d+\.(\s)";
            HighlightPattern(orderedListPattern, listColor, FontStyle.Bold);
        }

        /// <summary>
        /// ✅ Підсвітка горизонтальних ліній --- 
        /// </summary>
        private void HighlightMarkdownHorizontalRules()
        {
            var hrColor = Color.FromArgb(180, 180, 180);
            
            // Горизонтальні лінії (---, ***, ___)
            var hrPattern = @"^(\s*)([-*_]){3,}(\s*)$";
            HighlightPattern(hrPattern, hrColor, FontStyle.Bold);
        }

        // ✅ Кеш для регулярних виразів - підвищує продуктивність
        private static readonly System.Collections.Generic.Dictionary<string, System.Text.RegularExpressions.Regex> _regexCache = 
            new System.Collections.Generic.Dictionary<string, System.Text.RegularExpressions.Regex>();

        /// <summary>
        /// ✅ Максимально оптимізований метод для підсвітки з кешуванням регулярних виразів
        /// </summary>
        private void HighlightPattern(string pattern, Color color, FontStyle fontStyle, 
            Font customFont = null, Color? backgroundColor = null)
        {
            try
            {
                // ✅ Використовувати кеш для регулярних виразів
                if (!_regexCache.TryGetValue(pattern, out var regex))
                {
                    regex = new System.Text.RegularExpressions.Regex(pattern, 
                        System.Text.RegularExpressions.RegexOptions.Multiline | 
                        System.Text.RegularExpressions.RegexOptions.Compiled); // Compiled для швидкості
                    _regexCache[pattern] = regex;
                }

                var matches = regex.Matches(richTextBox1.Text);
                
                // ✅ Обмежити кількість матчів для продуктивності
                int matchCount = 0;
                const int maxMatches = 1000;

                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    if (++matchCount > maxMatches) break; // Запобігти залежанню на великих файлах
                    
                    if (match.Index >= 0 && match.Index + match.Length <= richTextBox1.Text.Length)
                    {
                        richTextBox1.Select(match.Index, match.Length);
                        richTextBox1.SelectionColor = color;
                        
                        if (backgroundColor.HasValue)
                        {
                            richTextBox1.SelectionBackColor = backgroundColor.Value;
                        }
                        
                        if (customFont != null)
                        {
                            richTextBox1.SelectionFont = customFont;
                        }
                        else
                        {
                            var currentFont = richTextBox1.SelectionFont ?? richTextBox1.Font;
                            richTextBox1.SelectionFont = new Font(currentFont.FontFamily, 
                                currentFont.Size, fontStyle);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Optimized pattern error for '{pattern}': {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ Швидке очищення підсвітки синтаксису
        /// </summary>
        private void ClearSyntaxHighlighting()
        {
            if (richTextBox1 == null) return;

            try
            {
                // Зберегти позицію курсора
                int cursorPosition = richTextBox1.SelectionStart;
                
                richTextBox1.SuspendLayout();
                
                // Очистити всі кольори та стилі
                richTextBox1.SelectAll();
                richTextBox1.SelectionColor = richTextBox1.ForeColor;
                richTextBox1.SelectionFont = richTextBox1.Font;
                richTextBox1.SelectionBackColor = Color.Empty;
                
                richTextBox1.ResumeLayout();
                
                // Відновити позицію курсора
                richTextBox1.SelectionStart = Math.Min(cursorPosition, richTextBox1.Text.Length);
                richTextBox1.SelectionLength = 0;
                
                // Встановити правильний колір виділення згідно з поточною темою
                if (_currentSettings != null)
                {
                    var currentTheme = EditorTheme.GetByName(_currentSettings.Theme);
                    if (currentTheme != null)
                    {
                        richTextBox1.SelectionBackColor = currentTheme.SelectionBackColor;
                    }
                }
                
                System.Diagnostics.Debug.WriteLine("✅ Syntax highlighting cleared successfully");
            }
            catch (Exception ex)
            {
                richTextBox1.ResumeLayout();
                System.Diagnostics.Debug.WriteLine($"❌ Clear highlighting error: {ex.Message}");
            }
        }

        #endregion

        #region Public Properties

        public bool HasCurrentDocument() => _currentDocument != null;
        
        public bool HasUnsavedChanges() => _currentDocument != null && !_currentDocument.IsSaved;
        
        public string GetCurrentDocumentContent() => _currentDocument?.Content ?? string.Empty;
        
        public Document GetCurrentDocument() => _currentDocument;

        /// <summary>
        /// Отримати статистику роботи з документами
        /// </summary>
        public DocumentServiceStatistics GetDocumentStatistics()
        {
            return _documentService?.GetStatistics();
        }

        #endregion

        #region Observer Pattern - Auto-Save

        /// <summary>
        /// Observer Pattern - автоматичне збереження
        /// </summary>
        private void InitializeAutoSave()
        {
            _autoSaveTimer = new Timer();
            _autoSaveTimer.Interval = 30000; // 30 секунд
            _autoSaveTimer.Tick += AutoSaveTimer_Tick;
            _autoSaveTimer.Start();
        }

        private void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            if (_currentDocument != null && 
                !_currentDocument.IsSaved && 
                !string.IsNullOrEmpty(_currentDocument.FilePath))
            {
                try
                {
                    // Створити резервну копію через DocumentService
                    var backupPath = _currentDocument.FilePath + ".autosave";
                    File.WriteAllText(backupPath, _currentDocument.Content);
                    
                    ShowStatusMessage("Auto-saved backup", 2000);
                }
                catch (Exception ex)
                {
                    ShowStatusMessage($"Auto-save failed: {ex.Message}", 5000);
                }
            }
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _autoSaveTimer?.Stop();
                _autoSaveTimer?.Dispose();
                
                _markdownHighlightTimer?.Stop();
                _markdownHighlightTimer?.Dispose();
                
                // ✅ Очистити кеш регулярних виразів
                _regexCache.Clear();
                
                // ✅ Звільнити ресурси сервісу розширень
                try
                {
                    if (_extensionsService != null)
                    {
                        _extensionsService.MacroStarted -= OnExtensionEvent;
                        _extensionsService.MacroStopped -= OnExtensionEvent;
                        _extensionsService.SnippetInserted -= OnExtensionEvent;
                        _extensionsService.BookmarkToggled -= OnExtensionEvent;
                        
                        // Сервіс розширень не має Dispose, але очищуємо посилання
                        _extensionsService = null;
                        System.Diagnostics.Debug.WriteLine("✅ Extensions service disposed");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Error disposing extensions service: {ex.Message}");
                }
                
                // Відписатися від подій DocumentService
                if (_documentService != null)
                {
                    _documentService.DocumentAdded -= OnDocumentServiceEvent;
                    _documentService.DocumentSaved -= OnDocumentServiceEvent;
                    _documentService.DocumentDeleted -= OnDocumentServiceEvent;
                }
                
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Handlers

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // ✅ Ctrl+H для ручного застосування підсвітки Markdown
            if (keyData == (Keys.Control | Keys.H))
            {
                if (!string.IsNullOrEmpty(_currentDocument?.FilePath) && 
                    Path.GetExtension(_currentDocument.FilePath).ToLower() == ".md")
                {
                    ApplyMarkdownHighlighting();
                    ShowStatusMessage("Markdown highlighting applied", 2000);
                    return true;
                }
                else
                {
                    ShowStatusMessage("Open a Markdown (.md) file first", 2000);
                    return true;
                }
            }

            // ✅ Ctrl+Shift+R для скидання форматування тексту
            if (keyData == (Keys.Control | Keys.Shift | Keys.R))
            {
                if (_currentSettings != null)
                {
                    var currentTheme = EditorTheme.GetByName(_currentSettings.Theme);
                    if (currentTheme != null)
                    {
                        ResetTextFormatting(currentTheme);
                        ShowStatusMessage("Text formatting reset", 2000);
                        return true;
                    }
                }
                ShowStatusMessage("Unable to reset formatting", 2000);
                return true;
            }

            // ✅ Ctrl+Shift+M для перемикання Markdown підсвітки
            if (keyData == (Keys.Control | Keys.Shift | Keys.M))
            {
                _markdownHighlightEnabled = !_markdownHighlightEnabled;
                
                if (_markdownHighlightEnabled)
                {
                    if (!string.IsNullOrEmpty(_currentDocument?.FilePath) && 
                        Path.GetExtension(_currentDocument.FilePath).ToLower() == ".md")
                    {
                        ApplyMarkdownHighlighting();
                    }
                    ShowStatusMessage("Markdown highlighting enabled", 2000);
                }
                else
                {
                    ClearSyntaxHighlighting();
                    ShowStatusMessage("Markdown highlighting disabled", 2000);
                }
                return true;
            }

            // ✅ Розширені скорочення для нових функцій
            if (_extensionsService != null && _extensionsService.IsExtensionsEnabled)
            {
                // Ctrl+B - Toggle bookmark
                if (keyData == (Keys.Control | Keys.B))
                {
                    try
                    {
                        _extensionsService.ToggleBookmarkAtCursor();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        ShowStatusMessage($"Bookmark error: {ex.Message}", 3000);
                    }
                }

                // F2 - Next bookmark
                if (keyData == Keys.F2)
                {
                    try
                    {
                        bool moved = _extensionsService.GoToNextBookmark();
                        if (!moved)
                        {
                            ShowStatusMessage("No more bookmarks", 2000);
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        ShowStatusMessage($"Navigation error: {ex.Message}", 3000);
                    }
                }

                // Shift+F2 - Previous bookmark
                if (keyData == (Keys.Shift | Keys.F2))
                {
                    try
                    {
                        bool moved = _extensionsService.GoToPreviousBookmark();
                        if (!moved)
                        {
                            ShowStatusMessage("No previous bookmarks", 2000);
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        ShowStatusMessage($"Navigation error: {ex.Message}", 3000);
                    }
                }

                // F12 - Show extensions info
                if (keyData == Keys.F12)
                {
                    try
                    {
                        var stats = _extensionsService.GetStatistics();
                        string info = $"Extensions Statistics\n\n{stats}";
                        MessageBox.Show(info, "Extensions Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        ShowStatusMessage($"Info error: {ex.Message}", 3000);
                    }
                }

                // Ctrl+K, Ctrl+S - Trigger snippet (example)
                if (keyData == (Keys.Control | Keys.K))
                {
                    ShowStatusMessage("Press S for snippet, M for macro commands", 2000);
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Text Formatting Reset

        /// <summary>
        /// ✅ Повністю скинути форматування тексту до базових налаштувань теми
        /// </summary>
        private void ResetTextFormatting(EditorTheme theme)
        {
            if (richTextBox1 == null || theme == null) return;

            try
            {
                // Зберегти позицію та текст
                int cursorPosition = richTextBox1.SelectionStart;
                string currentText = richTextBox1.Text;
                
                // Тимчасово відключити оновлення для продуктивності
                richTextBox1.SuspendLayout();
                
                // Очистити весь текст та форматування
                richTextBox1.Clear();
                
                // Встановити базові кольори
                richTextBox1.BackColor = theme.TextBoxBackColor;
                richTextBox1.ForeColor = theme.TextBoxForeColor;
                richTextBox1.SelectionBackColor = theme.SelectionBackColor;
                
                // Повернути текст
                richTextBox1.Text = currentText;
                
                // Відновити позицію курсора
                if (cursorPosition <= richTextBox1.Text.Length)
                {
                    richTextBox1.SelectionStart = cursorPosition;
                }
                richTextBox1.SelectionLength = 0;
                
                // Включити оновлення
                richTextBox1.ResumeLayout();
                
                System.Diagnostics.Debug.WriteLine("✅ Text formatting reset completed");
            }
            catch (Exception ex)
            {
                richTextBox1.ResumeLayout(); // Забезпечити відновлення layout при помилці
                System.Diagnostics.Debug.WriteLine($"❌ Text formatting reset error: {ex.Message}");
            }
        }

        #endregion

        #region Extended Edit Functions (Event Handlers Stubs)

        // ✅ Заглушки для обробників подій Edit меню (якщо будуть додані через Designer)
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Заглушка - можна реалізувати пізніше
            ShowStatusMessage("Undo function ready for implementation", 2000);
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Заглушка - можна реалізувати пізніше
            ShowStatusMessage("Redo function ready for implementation", 2000);
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (richTextBox1.SelectedText.Length > 0)
                {
                    richTextBox1.Cut();
                    ShowStatusMessage($"Cut {richTextBox1.SelectedText.Length} characters", 2000);
                }
                else
                {
                    ShowStatusMessage("No text selected to cut", 2000);
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Cut error: {ex.Message}", 3000);
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (richTextBox1.SelectedText.Length > 0)
                {
                    richTextBox1.Copy();
                    ShowStatusMessage($"Copied {richTextBox1.SelectedText.Length} characters", 2000);
                }
                else
                {
                    ShowStatusMessage("No text selected to copy", 2000);
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Copy error: {ex.Message}", 3000);
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    richTextBox1.Paste();
                    ShowStatusMessage("Text pasted", 2000);
                }
                else
                {
                    ShowStatusMessage("No text in clipboard", 2000);
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Paste error: {ex.Message}", 3000);
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.SelectAll();
                ShowStatusMessage($"Selected all text ({richTextBox1.Text.Length} chars)", 2000);
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Select all error: {ex.Message}", 3000);
            }
        }

        private void findReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Заглушка для Find & Replace
            ShowStatusMessage("Find & Replace function ready for implementation", 2000);
        }

        private void goToLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int totalLines = richTextBox1.Lines.Length;
                int currentLine = richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart) + 1;
                
                using (var goToForm = new GoToLineForm(totalLines, currentLine))
                {
                    // ✅ Застосувати поточну тему
                    if (_currentSettings != null)
                    {
                        var currentTheme = EditorTheme.GetByName(_currentSettings.Theme);
                        goToForm.ApplyTheme(currentTheme);
                    }

                    if (goToForm.ShowDialog() == DialogResult.OK && goToForm.IsValidInput)
                    {
                        GoToLineSimple(goToForm.LineNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Go to line error: {ex.Message}", 3000);
            }
        }

        #region Simple Helper Methods

        /// <summary>
        /// Простий перехід до рядка без складної логіки
        /// </summary>
        private void GoToLineSimple(int lineNumber)
        {
            try
            {
                if (lineNumber > richTextBox1.Lines.Length)
                {
                    ShowStatusMessage($"Line {lineNumber} doesn't exist. Max: {richTextBox1.Lines.Length}", 3000);
                    return;
                }

                int charIndex = richTextBox1.GetFirstCharIndexFromLine(lineNumber - 1);
                if (charIndex >= 0)
                {
                    richTextBox1.SelectionStart = charIndex;
                    richTextBox1.SelectionLength = 0;
                    richTextBox1.ScrollToCaret();
                    richTextBox1.Focus();
                    ShowStatusMessage($"Moved to line {lineNumber}", 2000);
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Navigation error: {ex.Message}", 3000);
            }
        }

        /// <summary>
        /// Простий метод застосування форматування
        /// </summary>
        private void ApplySimpleFormatting(FontStyle style)
        {
            try
            {
                if (richTextBox1.SelectedText.Length == 0)
                {
                    ShowStatusMessage("Please select text to format", 2000);
                    return;
                }

                Font currentFont = richTextBox1.SelectionFont ?? richTextBox1.Font;
                FontStyle newStyle = currentFont.Style.HasFlag(style) ? 
                    currentFont.Style & ~style : 
                    currentFont.Style | style;

                richTextBox1.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newStyle);
                
                string action = currentFont.Style.HasFlag(style) ? "Removed" : "Applied";
                ShowStatusMessage($"{action} {style} formatting", 2000);
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Formatting error: {ex.Message}", 3000);
            }
        }

        /// <summary>
        /// Простий метод зміни кодування
        /// </summary>
        private void ChangeEncodingSimple(string encodingName)
        {
            try
            {
                if (_currentDocument != null)
                {
                    // Знайти кодування в репозиторії
                    var encoding = _encodingRepository?.GetByName(encodingName);
                    if (encoding != null)
                    {
                        _currentDocument.TextEncoding = encoding;
                        UpdateTitle();
                        UpdateStatusBar();
                        ShowStatusMessage($"Encoding changed to {encodingName}", 2000);
                    }
                    else
                    {
                        ShowStatusMessage($"Encoding {encodingName} not available", 2000);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Encoding error: {ex.Message}", 3000);
            }
        }

        #endregion

        private void toggleBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_extensionsService != null)
                {
                    _extensionsService.ToggleBookmarkAtCursor();
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Bookmark error: {ex.Message}", 3000);
            }
        }

        private void nextBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_extensionsService != null)
                {
                    bool moved = _extensionsService.GoToNextBookmark();
                    if (!moved)
                    {
                        ShowStatusMessage("No more bookmarks", 2000);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Navigation error: {ex.Message}", 3000);
            }
        }

        private void prevBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_extensionsService != null)
                {
                    bool moved = _extensionsService.GoToPreviousBookmark();
                    if (!moved)
                    {
                        ShowStatusMessage("No previous bookmarks", 2000);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Navigation error: {ex.Message}", 3000);
            }
        }

        private void manageBookmarksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_extensionsService != null)
                {
                    using (var bookmarksForm = new BookmarksForm(_extensionsService.Bookmarks, richTextBox1))
                    {
                        // ✅ Застосувати поточну тему
                        if (_currentSettings != null)
                        {
                            var currentTheme = EditorTheme.GetByName(_currentSettings.Theme);
                            bookmarksForm.ApplyTheme(currentTheme);
                        }

                        bookmarksForm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening bookmarks manager: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void snippetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_extensionsService != null)
                {
                    using (var snippetsForm = new SnippetsForm(_extensionsService.Snippets, _extensionsService.CurrentLanguage))
                    {
                        // ✅ Застосувати поточну тему
                        if (_currentSettings != null)
                        {
                            var currentTheme = EditorTheme.GetByName(_currentSettings.Theme);
                            snippetsForm.ApplyTheme(currentTheme);
                        }

                        if (snippetsForm.ShowDialog() == DialogResult.OK && snippetsForm.SelectedSnippet != null)
                        {
                            // Вставити сніппет у поточну позицію курсора
                            int cursorPosition = richTextBox1.SelectionStart;
                            _extensionsService.Snippets.InsertSnippet(snippetsForm.SelectedSnippet, cursorPosition);
                            ShowStatusMessage($"Inserted snippet: {snippetsForm.SelectedSnippet.Name}", 2000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening snippets manager: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void extensionsInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_extensionsService != null)
                {
                    var stats = _extensionsService.GetStatistics();
                    string info = $"Extensions Information\n\n{stats}";
                    MessageBox.Show(info, "Extensions Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting extensions info: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }

    /// <summary>
    /// ✅ Спеціальний рендер для темного меню
    /// </summary>
    public class DarkMenuRenderer : ToolStripProfessionalRenderer
    {
        private readonly EditorTheme _theme;

        public DarkMenuRenderer(EditorTheme theme)
        {
            _theme = theme;
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected || e.Item.Pressed)
            {
                // Колір при наведенні/натисканні
                using (var brush = new SolidBrush(_theme.HoverColor))
                {
                    e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
                }
            }
            else
            {
                // Звичайний фон
                using (var brush = new SolidBrush(_theme.MenuBackColor))
                {
                    e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
                }
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = _theme.MenuForeColor;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            // Не малювати границю для темної теми
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            using (var pen = new Pen(_theme.BorderColor))
            {
                var bounds = e.Item.Bounds;
                e.Graphics.DrawLine(pen, bounds.Left + 20, bounds.Top + bounds.Height / 2, 
                                        bounds.Right - 5, bounds.Top + bounds.Height / 2);
            }
        }
    }

    /// <summary>
    /// ✅ Спеціальний рендер для темного статус бару
    /// </summary>
    public class DarkStatusStripRenderer : ToolStripProfessionalRenderer
    {
        private readonly EditorTheme _theme;

        public DarkStatusStripRenderer(EditorTheme theme)
        {
            _theme = theme;
        }

        protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
        {
            using (var brush = new SolidBrush(_theme.StatusBarBackColor))
            {
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using (var brush = new SolidBrush(_theme.StatusBarBackColor))
            {
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }
        }
    }
}