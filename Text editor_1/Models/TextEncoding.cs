namespace TextEditorMK.Models
{
    public class TextEncoding
    {
        public int Id { get; set; }
        public string Name { get; set; } = "UTF-8";
        public string CodePage { get; set; } = "utf-8";
        public bool IsDefault { get; set; } = false;

        public static TextEncoding DetectFromBytes(byte[] data)
        {
            if (data.Length >= 3 && data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF)
                return new TextEncoding { Name = "UTF-8", CodePage = "utf-8" };

            if (data.Length >= 2 && data[0] == 0xFF && data[1] == 0xFE)
                return new TextEncoding { Name = "UTF-16 LE", CodePage = "utf-16" };

            return new TextEncoding { Name = "UTF-8", CodePage = "utf-8", IsDefault = true };
        }
    }
}

