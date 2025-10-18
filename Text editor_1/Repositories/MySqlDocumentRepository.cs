using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using TextEditorMK.Models;
using TextEditorMK.Repositories.Interfaces;
using TextEditorMK.Data;

namespace TextEditorMK.Repositories.Implementations
{
    public class MySqlDocumentRepository : IDocumentRepository
    {
        private readonly MySqlDatabaseHelper _dbHelper;

        public MySqlDocumentRepository()
        {
            _dbHelper = new MySqlDatabaseHelper();
        }

        public void Add(Document document)
        {
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO Documents (FileName, FilePath, Content, EncodingId, CreatedAt, ModifiedAt, IsSaved) 
                        VALUES (@FileName, @FilePath, @Content, @EncodingId, @CreatedAt, @ModifiedAt, @IsSaved);
                        SELECT LAST_INSERT_ID();";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FileName", document.FileName);
                        command.Parameters.AddWithValue("@FilePath", document.FilePath);
                        command.Parameters.AddWithValue("@Content", document.Content ?? string.Empty);
                        command.Parameters.AddWithValue("@EncodingId", document.EncodingId);
                        command.Parameters.AddWithValue("@CreatedAt", document.CreatedAt);
                        command.Parameters.AddWithValue("@ModifiedAt", document.ModifiedAt);
                        command.Parameters.AddWithValue("@IsSaved", document.IsSaved);

                        var result = command.ExecuteScalar();
                        document.Id = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"MySQL Add Error: {ex.Message}",
                    "MySQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Update(Document document)
        {
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        UPDATE Documents 
                        SET FileName = @FileName, FilePath = @FilePath, Content = @Content, 
                            EncodingId = @EncodingId, ModifiedAt = @ModifiedAt, IsSaved = @IsSaved 
                        WHERE Id = @Id";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", document.Id);
                        command.Parameters.AddWithValue("@FileName", document.FileName);
                        command.Parameters.AddWithValue("@FilePath", document.FilePath);
                        command.Parameters.AddWithValue("@Content", document.Content ?? string.Empty);
                        command.Parameters.AddWithValue("@EncodingId", document.EncodingId);
                        command.Parameters.AddWithValue("@ModifiedAt", document.ModifiedAt);
                        command.Parameters.AddWithValue("@IsSaved", document.IsSaved);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"MySQL Update Error: {ex.Message}",
                    "MySQL Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public Document GetById(int id)
        {
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM Documents WHERE Id = @Id";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Document
                                {
                                    Id = reader.GetInt32("Id"),
                                    FileName = reader.GetString("FileName"),
                                    FilePath = reader.GetString("FilePath"),
                                    Content = reader["Content"] as string ?? string.Empty, // <-- ВИПРАВЛЕНО
                                    EncodingId = reader.GetInt32("EncodingId"),
                                    CreatedAt = reader.GetDateTime("CreatedAt"),
                                    ModifiedAt = reader.GetDateTime("ModifiedAt"),
                                    IsSaved = reader.GetBoolean("IsSaved")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting document: {ex.Message}", "MySQL Error");
            }
            return null;
        }

        public List<Document> GetAll()
        {
            var documents = new List<Document>();
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM Documents ORDER BY ModifiedAt DESC";

                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            documents.Add(new Document
                            {
                                Id = reader.GetInt32("Id"),
                                FileName = reader.GetString("FileName"),
                                FilePath = reader.GetString("FilePath"),
                                Content = reader["Content"] as string ?? string.Empty, // <-- ВИПРАВЛЕНО
                                EncodingId = reader.GetInt32("EncodingId"),
                                CreatedAt = reader.GetDateTime("CreatedAt"),
                                ModifiedAt = reader.GetDateTime("ModifiedAt"),
                                IsSaved = reader.GetBoolean("IsSaved")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting all documents: {ex.Message}", "MySQL Error");
            }
            return documents;
        }

        public void Delete(int id)
        {
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "DELETE FROM Documents WHERE Id = @Id";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting document: {ex.Message}", "MySQL Error");
            }
        }

        public Document GetByPath(string filePath)
        {
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM Documents WHERE FilePath = @FilePath ORDER BY ModifiedAt DESC LIMIT 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FilePath", filePath);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Document
                                {
                                    Id = reader.GetInt32("Id"),
                                    FileName = reader.GetString("FileName"),
                                    FilePath = reader.GetString("FilePath"),
                                    Content = reader["Content"] as string ?? string.Empty, // <-- ВИПРАВЛЕНО
                                    EncodingId = reader.GetInt32("EncodingId"),
                                    CreatedAt = reader.GetDateTime("CreatedAt"),
                                    ModifiedAt = reader.GetDateTime("ModifiedAt"),
                                    IsSaved = reader.GetBoolean("IsSaved")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting document by path: {ex.Message}", "MySQL Error");
            }
            return null;
        }
    }
}
