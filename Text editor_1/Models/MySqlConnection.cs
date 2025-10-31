using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using TextEditorMK.Models;

namespace TextEditorMK.Data
{
    public class MySqlDatabaseHelper
    {
        private readonly string _connectionString;

        public MySqlDatabaseHelper()
        {
            _connectionString = "Server=localhost;Port=3306;Database=texteditor_db;Uid=root;Pwd=root;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                // Створити базу даних якщо не існує
                TestAndCreateDatabase();

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    // ✅ Додати базові дані тільки якщо таблиці порожні
                    SeedDataIfEmpty(connection);

                    MessageBox.Show("Database connected successfully! Your data is preserved.", "MySQL Ready",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error: {ex.Message}\n\nFalling back to in-memory storage.",
                    "MySQL Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                throw; 
            }
        }

        private void TestAndCreateDatabase()
        {
            // Підключення без вказання конкретної БД
            string connectionWithoutDb = "Server=localhost;Port=3306;Uid=root;Pwd=root;";

            using (var connection = new MySqlConnection(connectionWithoutDb))
            {
                connection.Open();

                string createDbQuery = "CREATE DATABASE IF NOT EXISTS texteditor_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci";
                using (var command = new MySqlCommand(createDbQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Додати базові дані тільки якщо таблиці порожні (зберегти існуючі дані!)
        /// </summary>
        private void SeedDataIfEmpty(MySqlConnection connection)
        {
            // Перевірити чи TextEncodings порожня
            string checkEncodings = "SELECT COUNT(*) FROM TextEncodings";
            using (var cmd = new MySqlCommand(checkEncodings, connection))
            {
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count == 0)
                {
                    string insertEncodings = @"
                        INSERT INTO TextEncodings (Name, CodePage, IsDefault) VALUES 
                        ('UTF-8', 'utf-8', TRUE),
                        ('UTF-16 LE', 'utf-16', FALSE),
                        ('Windows-1251', 'windows-1251', FALSE)";

                    using (var insertCmd = new MySqlCommand(insertEncodings, connection))
                    {
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }

            // Перевірити чи EditorSettings порожня
            string checkSettings = "SELECT COUNT(*) FROM EditorSettings";
            using (var cmd = new MySqlCommand(checkSettings, connection))
            {
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count == 0)
                {
                    string insertSettings = @"
                        INSERT INTO EditorSettings (FontFamily, FontSize, Theme, WordWrap, ShowLineNumbers) 
                        VALUES ('Consolas', 12, 'Light', FALSE, TRUE)";

                    using (var insertCmd = new MySqlCommand(insertSettings, connection))
                    {
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }

            // ✅ НЕ додаємо тестові RecentFiles - залишаємо існуючі дані!
            // RecentFiles зберігають реальні дані користувача
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public bool TestConnection()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
