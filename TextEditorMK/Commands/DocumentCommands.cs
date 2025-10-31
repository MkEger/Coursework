using System;
using System.Windows.Forms;
using Text_editor_1;

namespace TextEditorMK.Commands
{
    /// <summary>
    /// ������� ��������� ������ ���������
    /// </summary>
    public class NewDocumentCommand : BaseCommand
    {
        public NewDocumentCommand(MainForm mainForm) : base(mainForm) { }

        public override string Description => "Create New Document";

        public override void Execute()
        {
            _mainForm.CreateNewDocument();
        }
    }

    /// <summary>
    /// ������� �������� ���������
    /// </summary>
    public class OpenDocumentCommand : BaseCommand
    {
        public OpenDocumentCommand(MainForm mainForm) : base(mainForm) { }

        public override string Description => "Open Document";

        public override void Execute()
        {
            _mainForm.OpenDocument();
        }
    }

    /// <summary>
    /// ������� ���������� ���������
    /// </summary>
    public class SaveDocumentCommand : BaseCommand
    {
        public SaveDocumentCommand(MainForm mainForm) : base(mainForm) { }

        public override string Description => "Save Document";

        public override void Execute()
        {
            _mainForm.SaveDocument();
        }

        public override bool CanExecute()
        {
            return _mainForm.HasCurrentDocument() && !string.IsNullOrEmpty(_mainForm.GetCurrentDocumentContent());
        }
    }

    /// <summary>
    /// ������� ������ ������� �����
    /// </summary>
    public class ShowRecentFilesCommand : BaseCommand
    {
        public ShowRecentFilesCommand(MainForm mainForm) : base(mainForm) { }

        public override string Description => "Show Recent Files";

        public override void Execute()
        {
            _mainForm.ShowRecentFiles();
        }
    }

    /// <summary>
    /// ������� ������ � ��������
    /// </summary>
    public class ExitApplicationCommand : BaseCommand
    {
        public ExitApplicationCommand(MainForm mainForm) : base(mainForm) { }

        public override string Description => "Exit Application";

        public override void Execute()
        {
            if (_mainForm.HasUnsavedChanges())
            {
                var result = MessageBox.Show(
                    "You have unsaved changes. Do you want to save before exit?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _mainForm.SaveDocument();
                }
                else if (result == DialogResult.Cancel)
                {
                    return; // �� ���������
                }
            }

            _mainForm.Close();
        }
    }
}