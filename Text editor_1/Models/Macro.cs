using System;
using System.Collections.Generic;

namespace TextEditorMK.Models
{
    /// <summary>
    /// ������ ������� ��� ������������� ��
    /// </summary>
    public class Macro
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<MacroAction> Actions { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public int UsageCount { get; set; }
        public string KeyboardShortcut { get; set; }

        public Macro()
        {
            Actions = new List<MacroAction>();
            CreatedDate = DateTime.Now;
            UsageCount = 0;
        }

        /// <summary>
        /// ������� ���������� ������������
        /// </summary>
        public void UpdateUsage()
        {
            LastUsedDate = DateTime.Now;
            UsageCount++;
        }
    }

    /// <summary>
    /// ĳ� �������
    /// </summary>
    public class MacroAction
    {
        public int Id { get; set; }
        public MacroActionType Type { get; set; }
        public string Data { get; set; }
        public int Position { get; set; }
        public DateTime Timestamp { get; set; }

        public MacroAction()
        {
            Timestamp = DateTime.Now;
        }
    }

    /// <summary>
    /// ���� �� �������
    /// </summary>
    public enum MacroActionType
    {
        Insert,
        Delete,
        MoveCursor,
        KeyPress,
        SelectText,
        Format
    }
}