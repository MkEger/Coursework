using System.Drawing;

namespace TextEditorMK.Models
{
    public class EditorTheme
    {
        public string Name { get; set; }
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
        public Color MenuBackColor { get; set; }
        public Color MenuForeColor { get; set; }
        public Color StatusBarBackColor { get; set; }
        public Color StatusBarForeColor { get; set; }
        public Color SelectionBackColor { get; set; }
        public Color SelectionForeColor { get; set; }
        
        public Color ButtonBackColor { get; set; }
        public Color ButtonForeColor { get; set; }
        public Color ButtonBorderColor { get; set; }
        public Color TextBoxBackColor { get; set; }
        public Color TextBoxForeColor { get; set; }
        public Color BorderColor { get; set; }
        public Color HoverColor { get; set; }

        public static EditorTheme Light => new EditorTheme
        {
            Name = "Light",
            BackgroundColor = Color.White,
            ForegroundColor = Color.Black,
            MenuBackColor = Color.FromArgb(240, 240, 240),
            MenuForeColor = Color.Black,
            StatusBarBackColor = Color.FromArgb(230, 230, 230),
            StatusBarForeColor = Color.Black,
            SelectionBackColor = Color.FromArgb(51, 153, 255),
            SelectionForeColor = Color.White,
            ButtonBackColor = Color.FromArgb(225, 225, 225),
            ButtonForeColor = Color.Black,
            ButtonBorderColor = Color.FromArgb(173, 173, 173),
            TextBoxBackColor = Color.White,
            TextBoxForeColor = Color.Black,
            BorderColor = Color.FromArgb(173, 173, 173),
            HoverColor = Color.FromArgb(229, 241, 251)
        };

        public static EditorTheme Dark => new EditorTheme
        {
            Name = "Dark",
            BackgroundColor = Color.FromArgb(37, 37, 38),     
            ForegroundColor = Color.FromArgb(241, 241, 241),  
            MenuBackColor = Color.FromArgb(45, 45, 48),       
            MenuForeColor = Color.FromArgb(241, 241, 241),    
            StatusBarBackColor = Color.FromArgb(0, 122, 204), 
            StatusBarForeColor = Color.White,
            SelectionBackColor = Color.FromArgb(51, 153, 255),
            SelectionForeColor = Color.White,
            ButtonBackColor = Color.FromArgb(62, 62, 64),
            ButtonForeColor = Color.FromArgb(241, 241, 241),
            ButtonBorderColor = Color.FromArgb(104, 104, 104),
            TextBoxBackColor = Color.FromArgb(51, 51, 55),
            TextBoxForeColor = Color.FromArgb(241, 241, 241),
            BorderColor = Color.FromArgb(63, 63, 70),
            HoverColor = Color.FromArgb(80, 80, 80)
        };

        public static EditorTheme Blue => new EditorTheme
        {
            Name = "Blue",
            BackgroundColor = Color.FromArgb(245, 250, 255),
            ForegroundColor = Color.FromArgb(0, 40, 80),
            MenuBackColor = Color.FromArgb(230, 240, 250),
            MenuForeColor = Color.FromArgb(0, 40, 80),
            StatusBarBackColor = Color.FromArgb(70, 130, 200),
            StatusBarForeColor = Color.White,
            SelectionBackColor = Color.FromArgb(100, 160, 220),
            SelectionForeColor = Color.White,
            ButtonBackColor = Color.FromArgb(220, 235, 250),
            ButtonForeColor = Color.FromArgb(0, 40, 80),
            ButtonBorderColor = Color.FromArgb(100, 160, 220),
            TextBoxBackColor = Color.White,
            TextBoxForeColor = Color.FromArgb(0, 40, 80),
            BorderColor = Color.FromArgb(100, 160, 220),
            HoverColor = Color.FromArgb(200, 225, 250)
        };

        public static EditorTheme Green => new EditorTheme
        {
            Name = "Green",
            BackgroundColor = Color.FromArgb(250, 255, 250),
            ForegroundColor = Color.FromArgb(0, 60, 0),
            MenuBackColor = Color.FromArgb(240, 250, 240),
            MenuForeColor = Color.FromArgb(0, 60, 0),
            StatusBarBackColor = Color.FromArgb(60, 150, 60),
            StatusBarForeColor = Color.White,
            SelectionBackColor = Color.FromArgb(120, 200, 120),
            SelectionForeColor = Color.White,
            ButtonBackColor = Color.FromArgb(235, 250, 235),
            ButtonForeColor = Color.FromArgb(0, 60, 0),
            ButtonBorderColor = Color.FromArgb(120, 200, 120),
            TextBoxBackColor = Color.White,
            TextBoxForeColor = Color.FromArgb(0, 60, 0),
            BorderColor = Color.FromArgb(120, 200, 120),
            HoverColor = Color.FromArgb(225, 245, 225)
        };

        public static EditorTheme Markdown => new EditorTheme
        {
            Name = "Markdown",
            BackgroundColor = Color.FromArgb(248, 249, 250),
            ForegroundColor = Color.FromArgb(36, 41, 46),
            MenuBackColor = Color.FromArgb(246, 248, 250),
            MenuForeColor = Color.FromArgb(36, 41, 46),
            StatusBarBackColor = Color.FromArgb(0, 116, 217),
            StatusBarForeColor = Color.White,
            SelectionBackColor = Color.FromArgb(0, 116, 217),
            SelectionForeColor = Color.White,
            ButtonBackColor = Color.FromArgb(240, 242, 244),
            ButtonForeColor = Color.FromArgb(36, 41, 46),
            ButtonBorderColor = Color.FromArgb(0, 116, 217),
            TextBoxBackColor = Color.White,
            TextBoxForeColor = Color.FromArgb(36, 41, 46),
            BorderColor = Color.FromArgb(209, 213, 218),
            HoverColor = Color.FromArgb(232, 236, 240)
        };

        public static EditorTheme[] GetAllThemes()
        {
            return new[] { Light, Dark, Blue, Green, Markdown };
        }

        public static EditorTheme GetByName(string name)
        {
            switch (name)
            {
                case "Dark": return Dark;
                case "Blue": return Blue;
                case "Green": return Green;
                case "Markdown": return Markdown;
                default: return Light;
            }
        }
    }
}