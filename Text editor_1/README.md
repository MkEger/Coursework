# Text Editor MK - Professional Course Project

## Як запустити проект

### Системні вимоги:
- **Windows OS** (Windows 7 SP1 або новіша)
- **.NET Framework 4.7.2** або новіша версія
- **Visual Studio 2017/2019/2022** 
- **MySQL 8.0+** (опціонально)

### Кроки запуску:

1. **Клонування репозиторію:**
```bash
git clone https://github.com/MkEger/Text-editor_1
cd Text-editor_1
```

2. **Відкриття проекту:**
   - Відкрийте файл `Text editor_1.sln` у Visual Studio

3. **Збірка та запуск:**
   - Натисніть **F5** для запуску з налагодженням
   - Або **Ctrl+F5** для запуску без налагодження

### Налаштування MySQL підключення:

**ВАЖЛИВО:** MySQL потрібна тільки для функції "Недавні файли". Без MySQL програма працює повноцінно.

#### Де змінювати налаштування підключення:
1. **Знайдіть клас `MySqlDatabaseHelper`** у проекті
2. **Або змініть connection string у репозиторіях:**

У файлах `MySqlRecentFileRepository.cs` та `MySqlDocumentRepository.cs` знайдіть рядки з підключенням:

```csharp
// ЗМІНІТЬ ЦІ ПАРАМЕТРИ НА СВОЇ:
private readonly string _connectionString = 
    "Server=localhost;Database=texteditor_db;Uid=your_username;Pwd=your_password;";

// АБО у методі GetConnection():
private MySqlConnection GetConnection()
{
    return new MySqlConnection("Server=localhost;Database=texteditor_db;Uid=root;Pwd=your_password;");
}
```

#### Параметри для заміни:
- **Server**: Адреса MySQL сервера (зазвичай `localhost`)
- **Database**: Назва бази даних (створіть `texteditor_db`)
- **Uid**: Ім'я користувача MySQL (зазвичай `root`)
- **Pwd**: Пароль для MySQL користувача

#### Створення бази даних:
```sql
CREATE DATABASE texteditor_db;
USE texteditor_db;

CREATE TABLE RecentFiles (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FilePath VARCHAR(500) NOT NULL,
    FileName VARCHAR(255) NOT NULL,
    LastOpenedAt DATETIME NOT NULL,
    OpenCount INT DEFAULT 1
);

CREATE TABLE Documents (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FileName VARCHAR(255) NOT NULL,
    FilePath VARCHAR(500),
    Content LONGTEXT,
    EncodingId INT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    ModifiedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    IsSaved BOOLEAN DEFAULT FALSE
);
```

**Примітка:** Програма готова до використання відразу після клонування та збірки. MySQL підключення опціональне.

---

*Останнє оновлення: Грудень 2024*