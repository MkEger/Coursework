using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TextEditorMK.Models;
using TextEditorMK.Services;
using TextEditorMK.Observers;

namespace TextEditorMK.Commands
{
    /// <summary>
    /// Command Pattern - базовий інтерфейс для команд
    /// </summary>
    public interface ICommand
    {
        void Execute();
        bool CanExecute();
        string Description { get; }
        CommandCategory Category { get; }
    }

    public enum CommandCategory
    {
        File,
        Edit,
        View,
        Tools,
        Help
    }

    /// <summary>
    /// Базовий клас для команд з підтримкою скасування
    /// </summary>
    public abstract class UndoableCommand : ICommand
    {
        public abstract void Execute();
        public abstract void Undo();
        public virtual bool CanExecute() => true;
        public abstract string Description { get; }
        public virtual CommandCategory Category => CommandCategory.Edit;
    }

    /// <summary>
    /// Invoker для управління командами з підтримкою Undo/Redo
    /// </summary>
    public class CommandInvoker : IDisposable
    {
        private readonly Stack<UndoableCommand> _undoStack = new Stack<UndoableCommand>();
        private readonly Stack<UndoableCommand> _redoStack = new Stack<UndoableCommand>();
        private readonly int _maxHistorySize;

        public event EventHandler<CommandExecutedEventArgs> CommandExecuted;
        public event EventHandler<CommandFailedEventArgs> CommandFailed;
        public event EventHandler<CommandUndoneEventArgs> CommandUndone;
        public event EventHandler<CommandRedoneEventArgs> CommandRedone;

        public CommandInvoker(int maxHistorySize = 100)
        {
            _maxHistorySize = maxHistorySize;
        }

        public void ExecuteCommand(ICommand command)
        {
            if (command == null) return;

            try
            {
                if (!command.CanExecute())
                {
                    System.Diagnostics.Debug.WriteLine($"Command cannot be executed: {command.Description}");
                    return;
                }

                command.Execute();

                // Додати до історії якщо це UndoableCommand
                if (command is UndoableCommand undoableCommand)
                {
                    _undoStack.Push(undoableCommand);
                    _redoStack.Clear(); // Очистити redo при виконанні нової команди

                    // Обмежити розмір історії
                    while (_undoStack.Count > _maxHistorySize)
                    {
                        var oldestCommand = new UndoableCommand[_undoStack.Count];
                        _undoStack.CopyTo(oldestCommand, 0);
                        _undoStack.Clear();
                        
                        for (int i = 1; i < oldestCommand.Length; i++)
                        {
                            _undoStack.Push(oldestCommand[oldestCommand.Length - 1 - i]);
                        }
                    }
                }

                OnCommandExecuted(command);
            }
            catch (Exception ex)
            {
                OnCommandFailed(command, ex);
            }
        }

        public void Undo()
        {
            if (_undoStack.Count == 0) return;

            try
            {
                var command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);

                OnCommandUndone(command);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Undo failed: {ex.Message}");
            }
        }

        public void Redo()
        {
            if (_redoStack.Count == 0) return;

            try
            {
                var command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);

                OnCommandRedone(command);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Redo failed: {ex.Message}");
            }
        }

        public bool CanUndo() => _undoStack.Count > 0;
        public bool CanRedo() => _redoStack.Count > 0;
        public int UndoCount() => _undoStack.Count;
        public int RedoCount() => _redoStack.Count;

        public void ClearHistory()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        // Events
        protected virtual void OnCommandExecuted(ICommand command)
        {
            CommandExecuted?.Invoke(this, new CommandExecutedEventArgs(command));
        }

        protected virtual void OnCommandFailed(ICommand command, Exception exception)
        {
            CommandFailed?.Invoke(this, new CommandFailedEventArgs(command, exception));
        }

        protected virtual void OnCommandUndone(UndoableCommand command)
        {
            CommandUndone?.Invoke(this, new CommandUndoneEventArgs(command));
        }

        protected virtual void OnCommandRedone(UndoableCommand command)
        {
            CommandRedone?.Invoke(this, new CommandRedoneEventArgs(command));
        }

        public void Dispose()
        {
            ClearHistory();
        }
    }

    // Event Args
    public class CommandExecutedEventArgs : EventArgs
    {
        public ICommand Command { get; }
        public DateTime ExecutedAt { get; }

        public CommandExecutedEventArgs(ICommand command)
        {
            Command = command;
            ExecutedAt = DateTime.Now;
        }
    }

    public class CommandFailedEventArgs : EventArgs
    {
        public ICommand Command { get; }
        public Exception Exception { get; }
        public DateTime FailedAt { get; }

        public CommandFailedEventArgs(ICommand command, Exception exception)
        {
            Command = command;
            Exception = exception;
            FailedAt = DateTime.Now;
        }
    }

    public class CommandUndoneEventArgs : EventArgs
    {
        public UndoableCommand Command { get; }
        public DateTime UndoneAt { get; }

        public CommandUndoneEventArgs(UndoableCommand command)
        {
            Command = command;
            UndoneAt = DateTime.Now;
        }
    }

    public class CommandRedoneEventArgs : EventArgs
    {
        public UndoableCommand Command { get; }
        public DateTime RedoneAt { get; }

        public CommandRedoneEventArgs(UndoableCommand command)
        {
            Command = command;
            RedoneAt = DateTime.Now;
        }
    }

    // ===== FILE COMMANDS =====

    public class NewDocumentCommand : ICommand
    {
        private readonly MainForm _mainForm;
        private readonly DocumentService _documentService;

        public NewDocumentCommand(MainForm mainForm, DocumentService documentService)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        }

        public string Description => "Create New Document";
        public CommandCategory Category => CommandCategory.File;

        public void Execute()
        {
            _mainForm.CreateNewDocument();
        }

        public bool CanExecute() => true;
    }

    public class OpenDocumentCommand : ICommand
    {
        private readonly MainForm _mainForm;
        private readonly DocumentService _documentService;

        public OpenDocumentCommand(MainForm mainForm, DocumentService documentService)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        }

        public string Description => "Open Document";
        public CommandCategory Category => CommandCategory.File;

        public void Execute()
        {
            _mainForm.OpenDocument();
        }

        public bool CanExecute() => true;
    }

    public class SaveDocumentCommand : ICommand
    {
        private readonly MainForm _mainForm;
        private readonly DocumentService _documentService;
        private readonly Document _document;

        public SaveDocumentCommand(MainForm mainForm, DocumentService documentService, Document document)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
            _document = document;
        }

        public string Description => "Save Document";
        public CommandCategory Category => CommandCategory.File;

        public void Execute()
        {
            _mainForm.SaveDocument();
        }

        public bool CanExecute()
        {
            return _document != null && !string.IsNullOrEmpty(_document.Content);
        }
    }

    public class SaveAsDocumentCommand : ICommand
    {
        private readonly MainForm _mainForm;
        private readonly DocumentService _documentService;
        private readonly Document _document;

        public SaveAsDocumentCommand(MainForm mainForm, DocumentService documentService, Document document)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
            _document = document;
        }

        public string Description => "Save Document As";
        public CommandCategory Category => CommandCategory.File;

        public void Execute()
        {
            _mainForm.SaveAsDocument();
        }

        public bool CanExecute()
        {
            return _document != null;
        }
    }

    public class ShowRecentFilesCommand : ICommand
    {
        private readonly MainForm _mainForm;
        private readonly DocumentService _documentService;

        public ShowRecentFilesCommand(MainForm mainForm, DocumentService documentService)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        }

        public string Description => "Show Recent Files";
        public CommandCategory Category => CommandCategory.File;

        public void Execute()
        {
            _mainForm.ShowRecentFiles();
        }

        public bool CanExecute() => true;
    }

    public class ExitApplicationCommand : ICommand
    {
        private readonly MainForm _mainForm;

        public ExitApplicationCommand(MainForm mainForm)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
        }

        public string Description => "Exit Application";
        public CommandCategory Category => CommandCategory.File;

        public void Execute()
        {
            _mainForm.Close();
        }

        public bool CanExecute() => true;
    }

    // ===== EDIT COMMANDS =====

    public class FindTextCommand : ICommand
    {
        private readonly MainForm _mainForm;
        private readonly RichTextBox _textBox;

        public FindTextCommand(MainForm mainForm, RichTextBox textBox)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
        }

        public string Description => "Find Text";
        public CommandCategory Category => CommandCategory.Edit;

        public void Execute()
        {
            var findDialog = new FindDialog(_textBox);
            findDialog.Show();
        }

        public bool CanExecute() => !string.IsNullOrEmpty(_textBox.Text);
    }

    public class ReplaceTextCommand : ICommand
    {
        private readonly MainForm _mainForm;
        private readonly RichTextBox _textBox;

        public ReplaceTextCommand(MainForm mainForm, RichTextBox textBox)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
        }

        public string Description => "Replace Text";
        public CommandCategory Category => CommandCategory.Edit;

        public void Execute()
        {
            var replaceDialog = new ReplaceDialog(_textBox);
            replaceDialog.Show();
        }

        public bool CanExecute() => !string.IsNullOrEmpty(_textBox.Text);
    }

    // ===== VIEW COMMANDS =====

    public class ChangeFontCommand : ICommand
    {
        private readonly MainForm _mainForm;
        private readonly RichTextBox _textBox;
        private readonly EditorSettings _settings;
        private readonly IEditorSettingsRepository _settingsRepository;

        public ChangeFontCommand(MainForm mainForm, RichTextBox textBox, EditorSettings settings, IEditorSettingsRepository settingsRepository)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));
        }

        public string Description => "Change Font";
        public CommandCategory Category => CommandCategory.View;

        public void Execute()
        {
            var fontDialog = new FontDialog();
            fontDialog.Font = _textBox.Font;

            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                _textBox.Font = fontDialog.Font;
                
                // Зберегти в налаштуваннях
                _settings.FontFamily = fontDialog.Font.FontFamily.Name;
                _settings.FontSize = (int)fontDialog.Font.Size;
                _settingsRepository.Update(_settings);
            }
        }

        public bool CanExecute() => true;
    }

    public class ToggleWordWrapCommand : ICommand
    {
        private readonly MainForm _mainForm;
        private readonly RichTextBox _textBox;
        private readonly EditorSettings _settings;
        private readonly IEditorSettingsRepository _settingsRepository;

        public ToggleWordWrapCommand(MainForm mainForm, RichTextBox textBox, EditorSettings settings, IEditorSettingsRepository settingsRepository)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));
        }

        public string Description => "Toggle Word Wrap";
        public CommandCategory Category => CommandCategory.View;

        public void Execute()
        {
            _textBox.WordWrap = !_textBox.WordWrap;
            _settings.WordWrap = _textBox.WordWrap;
            _settingsRepository.Update(_settings);
        }

        public bool CanExecute() => true;
    }

    // ===== TOOLS COMMANDS =====

    public class ShowStatisticsCommand : ICommand
    {
        private readonly MainForm _mainForm;
        private readonly DocumentStatisticsObserver _statisticsObserver;

        public ShowStatisticsCommand(MainForm mainForm, DocumentStatisticsObserver statisticsObserver)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
            _statisticsObserver = statisticsObserver ?? throw new ArgumentNullException(nameof(statisticsObserver));
        }

        public string Description => "Show Document Statistics";
        public CommandCategory Category => CommandCategory.Tools;

        public void Execute()
        {
            var stats = _statisticsObserver.GetStatisticsReport();
            MessageBox.Show(stats, "Document Statistics", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public bool CanExecute() => _statisticsObserver != null;
    }

    // ===== PLACEHOLDER DIALOG CLASSES =====
    // Ці класи потрібно реалізувати окремо

    public class FindDialog : Form
    {
        private readonly RichTextBox _textBox;

        public FindDialog(RichTextBox textBox)
        {
            _textBox = textBox;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Find";
            this.Size = new System.Drawing.Size(400, 150);
            this.StartPosition = FormStartPosition.CenterParent;
            
            // Тут потрібно додати контроли для пошуку
        }
    }

    public class ReplaceDialog : Form
    {
        private readonly RichTextBox _textBox;

        public ReplaceDialog(RichTextBox textBox)
        {
            _textBox = textBox;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Replace";
            this.Size = new System.Drawing.Size(400, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            
            // Тут потрібно додати контроли для заміни
        }
    }
}