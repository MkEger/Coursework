using System;

namespace TextEditorMK.Commands
{
    /// <summary>
    /// Command Pattern - інкапсулює операції як об'єкти
    /// </summary>
    public interface ICommand
    {
        void Execute();
        bool CanExecute();
        string Description { get; }
    }

    public abstract class BaseCommand : ICommand
    {
        protected readonly MainForm _mainForm;

        protected BaseCommand(MainForm mainForm)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
        }

        public abstract void Execute();
        public virtual bool CanExecute() => true;
        public abstract string Description { get; }
    }
}