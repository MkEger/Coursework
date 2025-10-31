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
        
        public int WindowWidth { get; set; } = 800;
        public int WindowHeight { get; set; } = 600;
        public bool AutoSave { get; set; } = true;
        public int AutoSaveInterval { get; set; } = 30; 
        public string DefaultEncoding { get; set; } = "UTF-8";
        public bool ShowStatusBar { get; set; } = true;
        public bool ShowToolbar { get; set; } = true;
        public string RecentFilesPath { get; set; } = "";
        public int MaxRecentFiles { get; set; } = 10;

        public void Reset()
        {
            FontFamily = "Consolas";
            FontSize = 12; 
            Theme = "Light";
            WordWrap = false;
            ShowLineNumbers = true;
            WindowWidth = 800;
            WindowHeight = 600;
            AutoSave = true;
            AutoSaveInterval = 30; 
            DefaultEncoding = "UTF-8";
            ShowStatusBar = true;
            ShowToolbar = true;
            MaxRecentFiles = 10;
        }

        public EditorSettings Clone()
        {
            return new EditorSettings
            {
                Id = this.Id,
                FontFamily = this.FontFamily,
                FontSize = this.FontSize,
                Theme = this.Theme,
                WordWrap = this.WordWrap,
                ShowLineNumbers = this.ShowLineNumbers,
                WindowWidth = this.WindowWidth,
                WindowHeight = this.WindowHeight,
                AutoSave = this.AutoSave,
                AutoSaveInterval = this.AutoSaveInterval,
                DefaultEncoding = this.DefaultEncoding,
                ShowStatusBar = this.ShowStatusBar,
                ShowToolbar = this.ShowToolbar,
                RecentFilesPath = this.RecentFilesPath,
                MaxRecentFiles = this.MaxRecentFiles
            };
        }

        public override string ToString()
        {
            return $"EditorSettings: {FontFamily} {FontSize}pt, Theme: {Theme}, " +
                   $"Window: {WindowWidth}x{WindowHeight}, AutoSave: {AutoSave}";
        }
    }
}
