using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using TextEditorMK.Models;
using TextEditorMK.Repositories.Interfaces;
using TextEditorMK.Data;

namespace TextEditorMK.Repositories.Implementations
{
    public class MySqlRecentFileRepository : IRecentFileRepository
    {
        private readonly MySqlDatabaseHelper _dbHelper;

        public MySqlRecentFileRepository()
        {
            _dbHelper = new MySqlDatabaseHelper();
        }

        public void AddOrUpdate(RecentFile file)
        {
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();
                    
                    string checkQuery = "SELECT Id, OpenCount FROM RecentFiles WHERE FilePath = @FilePath";
                    using (var checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@FilePath", file.FilePath);
                        using (var reader = checkCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
 
                                int existingId = reader.GetInt32("Id");
                                int currentCount = reader.GetInt32("OpenCount");
                                reader.Close();

                                string updateQuery = @"
                                    UPDATE RecentFiles 
                                    SET LastOpenedAt = NOW(), OpenCount = @OpenCount, FileName = @FileName
                                    WHERE Id = @Id";
                                
                                using (var updateCommand = new MySqlCommand(updateQuery, connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@Id", existingId);
                                    updateCommand.Parameters.AddWithValue("@OpenCount", currentCount + 1);
                                    updateCommand.Parameters.AddWithValue("@FileName", file.FileName);
                                    updateCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                reader.Close();

                                string insertQuery = @"
                                    INSERT INTO RecentFiles (FilePath, FileName, LastOpenedAt, OpenCount) 
                                    VALUES (@FilePath, @FileName, NOW(), 1)";
                                
                                using (var insertCommand = new MySqlCommand(insertQuery, connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@FilePath", file.FilePath);
                                    insertCommand.Parameters.AddWithValue("@FileName", file.FileName);
                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error saving recent file: {ex.Message}", 
                    "MySQL Error", System.Windows.Forms.MessageBoxButtons.OK, 
                    System.Windows.Forms.MessageBoxIcon.Warning);
            }
        }

        public List<RecentFile> GetRecent(int count)
        {
            var files = new List<RecentFile>();
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT * FROM RecentFiles 
                        ORDER BY LastOpenedAt DESC 
                        LIMIT @Count";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Count", count);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                files.Add(new RecentFile
                                {
                                    Id = reader.GetInt32("Id"),
                                    FilePath = reader.GetString("FilePath"),
                                    FileName = reader.GetString("FileName"),
                                    LastOpenedAt = reader.GetDateTime("LastOpenedAt"),
                                    OpenCount = reader.GetInt32("OpenCount")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error loading recent files: {ex.Message}", 
                    "MySQL Error", System.Windows.Forms.MessageBoxButtons.OK, 
                    System.Windows.Forms.MessageBoxIcon.Warning);
            }
            return files;
        }

        public RecentFile GetById(int id)
        {
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM RecentFiles WHERE Id = @Id";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new RecentFile
                                {
                                    Id = reader.GetInt32("Id"),
                                    FilePath = reader.GetString("FilePath"),
                                    FileName = reader.GetString("FileName"),
                                    LastOpenedAt = reader.GetDateTime("LastOpenedAt"),
                                    OpenCount = reader.GetInt32("OpenCount")
                                };
                            }
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public List<RecentFile> GetAll()
        {
            var files = new List<RecentFile>();
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM RecentFiles ORDER BY LastOpenedAt DESC";

                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            files.Add(new RecentFile
                            {
                                Id = reader.GetInt32("Id"),
                                FilePath = reader.GetString("FilePath"),
                                FileName = reader.GetString("FileName"),
                                LastOpenedAt = reader.GetDateTime("LastOpenedAt"),
                                OpenCount = reader.GetInt32("OpenCount")
                            });
                        }
                    }
                }
            }
            catch { }
            return files;
        }

        public void Delete(int id)
        {
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "DELETE FROM RecentFiles WHERE Id = @Id";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }
    }
}