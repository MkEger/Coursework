using System;
using System.Windows.Forms;
using TextEditorMK.Services;
using TextEditorMK.Models;

namespace TextEditorMK.Tests
{
    
    public class ExtensionsServiceTest
    {
        public static void RunBasicTests()
        {
            Console.WriteLine("?? Starting Extensions Service Tests...\n");

            try
            {
                
                var richTextBox = new RichTextBox();
                richTextBox.Text = "public class TestClass\n{\n    // Test code\n}";

                
                var extensionsService = new EditorExtensionsService(richTextBox);
                
                
                extensionsService.DetectLanguageFromFile("test.cs");
                Console.WriteLine($"? Language detection: {extensionsService.CurrentLanguage}");

                
                extensionsService.Bookmarks.AddBookmark(2, "Test class definition");
                var bookmarks = extensionsService.Bookmarks.GetAllBookmarks();
                Console.WriteLine($"? Bookmarks: {bookmarks.Count} created");

                
                var snippets = extensionsService.Snippets.GetSnippetsForLanguage("csharp");
                Console.WriteLine($"? Snippets: {snippets.Count} available for C#");

                
                var macros = extensionsService.Macros.GetAllMacros();
                Console.WriteLine($"? Macros: {macros.Count} available");

                
                var stats = extensionsService.GetStatistics();
                Console.WriteLine($"? Statistics: {stats}");

                Console.WriteLine("\n?? All tests passed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Test failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
