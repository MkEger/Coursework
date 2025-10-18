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
            // ВИПРАВЛЕНО: правильний рядок підключення
            _connectionString = "Server=localhost;Port=3306;Database=texteditor_db;Uid=root;Pwd=root;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                // Спочатку тестуємо підключення та створюємо базу даних
                TestAndCreateDatabase();

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    // Видаляємо старі проблемні таблиці
                    DropTablesIfNeeded(connection);

                    // Створюємо правильні таблиці
                    CreateTables(connection);

                    // Початкові дані
                    SeedData(connection);

                    MessageBox.Show("Database and tables created successfully!", "MySQL Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error: {ex.Message}\n\nFalling back to in-memory storage.",
                    "MySQL Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                throw; // Щоб Form1 перейшов на fallback
            }
        }

        private void TestAndCreateDatabase()
        {
            // Підключаємося без вказання бази даних
            string connectionWithoutDb = "Server=localhost;Port=3306;Uid=root;Pwd=root;";

            using (var connection = new MySqlConnection(connectionWithoutDb))
            {
                connection.Open();

                // Створюємо базу даних якщо не існує
                string createDbQuery = "CREATE DATABASE IF NOT EXISTS texteditor_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci";
                using (var command = new MySqlCommand(createDbQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void DropTablesIfNeeded(MySqlConnection connection)
        {
            try
            {
                // Відключаємо foreign key checks
                using (var cmd = new MySqlCommand("SET FOREIGN_KEY_CHECKS = 0", connection))
                {
                    cmd.ExecuteNonQuery();
                }

                // Видаляємо всі таблиці для чистого початку
                string[] tables = { "Documents", "RecentFiles", "EditorSettings", "TextEncodings" };
                foreach (string table in tables)
                {
                    try
                    {
                        using (var cmd = new MySqlCommand($"DROP TABLE IF EXISTS {table}", connection))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch { } // Ignore individual errors
                }

                // Включаємо foreign key checks
                using (var cmd = new MySqlCommand("SET FOREIGN_KEY_CHECKS = 1", connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch { } // Ignore cleanup errors
        }

        private void CreateTables(MySqlConnection connection)
        {
            string[] createTableQueries = {
                // 1. TextEncodings (незалежна таблиця)
                @"CREATE TABLE TextEncodings (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Name VARCHAR(50) NOT NULL DEFAULT 'UTF-8',
                    CodePage VARCHAR(20) NOT NULL DEFAULT 'utf-8',
                    IsDefault BOOLEAN NOT NULL DEFAULT FALSE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4",

                // 2. Documents (ГОЛОВНЕ ВИПРАВЛЕННЯ: Content LONGTEXT NULL без DEFAULT)
                @"CREATE TABLE Documents (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    FileName VARCHAR(255) NOT NULL DEFAULT '',
                    FilePath VARCHAR(500) NOT NULL DEFAULT '',
                    Content LONGTEXT NULL,
                    EncodingId INT NOT NULL DEFAULT 1,
                    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    ModifiedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    IsSaved BOOLEAN NOT NULL DEFAULT FALSE,
                    KEY idx_filepath (FilePath(255)),
                    KEY idx_modified (ModifiedAt)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4",

                // 3. RecentFiles
                @"CREATE TABLE RecentFiles (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    FilePath VARCHAR(500) NOT NULL DEFAULT '',
                    FileName VARCHAR(255) NOT NULL DEFAULT '',
                    LastOpenedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    OpenCount INT NOT NULL DEFAULT 0,
                    KEY idx_last_opened (LastOpenedAt)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4",

                // 4. EditorSettings
                @"CREATE TABLE EditorSettings (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    FontFamily VARCHAR(50) NOT NULL DEFAULT 'Consolas',
                    FontSize INT NOT NULL DEFAULT 12,
                    Theme VARCHAR(20) NOT NULL DEFAULT 'Light',
                    WordWrap BOOLEAN NOT NULL DEFAULT FALSE,
                    ShowLineNumbers BOOLEAN NOT NULL DEFAULT TRUE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4"
            };

            foreach (string query in createTableQueries)
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void SeedData(MySqlConnection connection)
        {
            // 1. Базові кодування
            string insertEncodings = @"
                INSERT INTO TextEncodings (Name, CodePage, IsDefault) VALUES 
                ('UTF-8', 'utf-8', TRUE),
                ('UTF-16 LE', 'utf-16', FALSE),
                ('Windows-1251', 'windows-1251', FALSE)";

            using (var cmd = new MySqlCommand(insertEncodings, connection))
            {
                cmd.ExecuteNonQuery();
            }

            // 2. Базові налаштування
            string insertSettings = @"
                INSERT INTO EditorSettings (FontFamily, FontSize, Theme, WordWrap, ShowLineNumbers) 
                VALUES ('Consolas', 12, 'Light', FALSE, TRUE)";

            using (var cmd = new MySqlCommand(insertSettings, connection))
            {
                cmd.ExecuteNonQuery();
            }

            // 3. Тестові нещодавні файли
            string insertRecentFiles = @"
                INSERT INTO RecentFiles (FilePath, FileName, LastOpenedAt, OpenCount) VALUES 
                ('C:/Documents/example1.txt', 'example1.txt', DATE_SUB(NOW(), INTERVAL 1 DAY), 5),
                ('C:/Documents/readme.txt', 'readme.txt', DATE_SUB(NOW(), INTERVAL 2 DAY), 3),
                ('C:/Projects/notes.txt', 'notes.txt', DATE_SUB(NOW(), INTERVAL 1 HOUR), 8),
                ('C:/Temp/draft.txt', 'draft.txt', DATE_SUB(NOW(), INTERVAL 3 HOUR), 2)";

            using (var cmd = new MySqlCommand(insertRecentFiles, connection))
            {
                cmd.ExecuteNonQuery();
            }
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
