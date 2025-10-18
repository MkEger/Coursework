namespace TextEditorMK.Models
{
    public class EditorSettings
    {
        public int Id { get; set; }
        public string FontFamily { get; set; } = "Consolas";
        public int FontSize { get; set; } = 12;
        public string Theme { get; set; } = "Light";
        public bool WordWrap { get; set; } = false;
        public bool ShowLineNumbers { get; set; } = true;

        public void Reset()
        {
            FontFamily = "Consolas";
            FontSize = 12;
            Theme = "Light";
            WordWrap = false;
            ShowLineNumbers = true;
        }
    }
}
