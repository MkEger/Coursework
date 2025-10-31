using System;
using System.Collections.Generic;
using TextEditorMK.Models;
using TextEditorMK.Repositories.Interfaces;

namespace TextEditorMK.Repositories.Implementations
{
    /// <summary>
    /// MySQL נואכ³חאצ³ÿ הכÿ EncodingRepository
    /// </summary>
    public class MySqlEncodingRepository : IEncodingRepository
    {
        private readonly MySqlDatabaseHelper _dbHelper;

        public MySqlEncodingRepository()
        {
            _dbHelper = new MySqlDatabaseHelper();
        }

        public TextEncoding GetById(int id)
        {
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM TextEncodings WHERE Id = @Id";

                    using (var command = new MySql.Data.MySqlClient.MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new TextEncoding
                                {
                                    Id = reader.GetInt32("Id"),
                                    Name = reader.GetString("Name"),
                                    CodePage = reader.GetString("CodePage"),
                                    IsDefault = reader.GetBoolean("IsDefault")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MySQL Encoding GetById Error: {ex.Message}");
            }
            return null;
        }

        public List<TextEncoding> GetAll()
        {
            var encodings = new List<TextEncoding>();
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM TextEncodings ORDER BY Name";

                    using (var command = new MySql.Data.MySqlClient.MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            encodings.Add(new TextEncoding
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                CodePage = reader.GetString("CodePage"),
                                IsDefault = reader.GetBoolean("IsDefault")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MySQL Encoding GetAll Error: {ex.Message}");
            }
            return encodings;
        }

        public TextEncoding GetDefault()
        {
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM TextEncodings WHERE IsDefault = TRUE LIMIT 1";

                    using (var command = new MySql.Data.MySqlClient.MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new TextEncoding
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                CodePage = reader.GetString("CodePage"),
                                IsDefault = reader.GetBoolean("IsDefault")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MySQL Encoding GetDefault Error: {ex.Message}");
            }

            // Fallback to default
            return new TextEncoding
            {
                Id = 1,
                Name = "UTF-8",
                CodePage = "utf-8",
                IsDefault = true
            };
        }

        public TextEncoding GetByCodePage(string codePage)
        {
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM TextEncodings WHERE CodePage = @CodePage LIMIT 1";

                    using (var command = new MySql.Data.MySqlClient.MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CodePage", codePage);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new TextEncoding
                                {
                                    Id = reader.GetInt32("Id"),
                                    Name = reader.GetString("Name"),
                                    CodePage = reader.GetString("CodePage"),
                                    IsDefault = reader.GetBoolean("IsDefault")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MySQL Encoding GetByCodePage Error: {ex.Message}");
            }
            return null;
        }
    }

    /// <summary>
    /// MySQL נואכ³חאצ³ÿ הכÿ EditorSettingsRepository
    /// </summary>
    public class MySqlEditorSettingsRepository : IEditorSettingsRepository
    {
        private readonly MySqlDatabaseHelper _dbHelper;

        public MySqlEditorSettingsRepository()
        {
            _dbHelper = new MySqlDatabaseHelper();
        }

        public EditorSettings GetCurrent()
        {
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM EditorSettings ORDER BY Id DESC LIMIT 1";

                    using (var command = new MySql.Data.MySqlClient.MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new EditorSettings
                            {
                                Id = reader.GetInt32("Id"),
                                FontFamily = reader.GetString("FontFamily"),
                                FontSize = reader.GetInt32("FontSize"),
                                Theme = reader.GetString("Theme"),
                                WordWrap = reader.GetBoolean("WordWrap"),
                                ShowLineNumbers = reader.GetBoolean("ShowLineNumbers"),
                                WindowWidth = reader.IsDBNull("WindowWidth") ? 800 : reader.GetInt32("WindowWidth"),
                                WindowHeight = reader.IsDBNull("WindowHeight") ? 600 : reader.GetInt32("WindowHeight")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MySQL Settings GetCurrent Error: {ex.Message}");
            }

            // Return default settings
            return new EditorSettings();
        }

        public void Update(EditorSettings settings)
        {
            if (settings == null) return;

            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();

                    // Check if settings exist
                    string checkQuery = "SELECT COUNT(*) FROM EditorSettings WHERE Id = @Id";
                    using (var checkCommand = new MySql.Data.MySqlClient.MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Id", settings.Id);
                        var count = Convert.ToInt32(checkCommand.ExecuteScalar());

                        string query;
                        if (count > 0)
                        {
                            // Update existing
                            query = @"UPDATE EditorSettings SET 
                                FontFamily = @FontFamily, 
                                FontSize = @FontSize, 
                                Theme = @Theme, 
                                WordWrap = @WordWrap, 
                                ShowLineNumbers = @ShowLineNumbers,
                                WindowWidth = @WindowWidth,
                                WindowHeight = @WindowHeight
                                WHERE Id = @Id";
                        }
                        else
                        {
                            // Insert new
                            query = @"INSERT INTO EditorSettings 
                                (FontFamily, FontSize, Theme, WordWrap, ShowLineNumbers, WindowWidth, WindowHeight) 
                                VALUES (@FontFamily, @FontSize, @Theme, @WordWrap, @ShowLineNumbers, @WindowWidth, @WindowHeight);
                                SELECT LAST_INSERT_ID();";
                        }

                        using (var command = new MySql.Data.MySqlClient.MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", settings.Id);
                            command.Parameters.AddWithValue("@FontFamily", settings.FontFamily);
                            command.Parameters.AddWithValue("@FontSize", settings.FontSize);
                            command.Parameters.AddWithValue("@Theme", settings.Theme);
                            command.Parameters.AddWithValue("@WordWrap", settings.WordWrap);
                            command.Parameters.AddWithValue("@ShowLineNumbers", settings.ShowLineNumbers);
                            command.Parameters.AddWithValue("@WindowWidth", settings.WindowWidth);
                            command.Parameters.AddWithValue("@WindowHeight", settings.WindowHeight);

                            if (count == 0)
                            {
                                var result = command.ExecuteScalar();
                                settings.Id = Convert.ToInt32(result);
                            }
                            else
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MySQL Settings Update Error: {ex.Message}");
            }
        }
    }
}