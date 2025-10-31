using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TextEditorMK.Models;

namespace TextEditorMK.Services
{

    public class MacroService
    {
        private readonly RichTextBox _textBox;
        private readonly List<Macro> _macros;
        private bool _isRecording;
        private string _currentMacroName;
        private readonly List<MacroAction> _currentActions;

        public bool IsRecording => _isRecording;
        public string CurrentMacroName => _currentMacroName;

        public MacroService(RichTextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
            _macros = new List<Macro>();
            _currentActions = new List<MacroAction>();
        }

        public void StartRecording(string macroName)
        {
            if (_isRecording)
                throw new InvalidOperationException("Macro recording already in progress");

            _isRecording = true;
            _currentMacroName = macroName;
            _currentActions.Clear();
            
            System.Diagnostics.Debug.WriteLine($"? Started recording macro: {macroName}");
        }

        public void StopRecording()
        {
            if (!_isRecording)
                throw new InvalidOperationException("No macro recording in progress");

            var macro = new Macro
            {
                Name = _currentMacroName,
                Description = $"Recorded macro with {_currentActions.Count} actions",
                CreatedDate = DateTime.Now
            };

            _macros.RemoveAll(m => m.Name == _currentMacroName);
            _macros.Add(macro);

            _isRecording = false;
            _currentMacroName = null;
            _currentActions.Clear();

            System.Diagnostics.Debug.WriteLine($"? Stopped recording macro: {macro.Name}");
        }

        public void ExecuteMacro(string macroName)
        {
            var macro = _macros.FirstOrDefault(m => m.Name == macroName);
            if (macro == null)
                throw new ArgumentException($"Macro '{macroName}' not found");

            macro.UpdateUsage();
            System.Diagnostics.Debug.WriteLine($"? Executed macro: {macroName}");
        }

        public List<Macro> GetAllMacros()
        {
            return new List<Macro>(_macros);
        }

        public void DeleteMacro(string macroName)
        {
            int removed = _macros.RemoveAll(m => m.Name == macroName);
            if (removed == 0)
                throw new ArgumentException($"Macro '{macroName}' not found");
        }
    }
}