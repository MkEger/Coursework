# Text Editor MK - Professional Course Project

## �� ��������� ������

### ������� ������:
- **Windows OS** (Windows 7 SP1 ��� �����)
- **.NET Framework 4.7.2** ��� ����� �����
- **Visual Studio 2017/2019/2022** 
- **MySQL 8.0+** (�����������)

### ����� �������:

1. **���������� ����������:**
```bash
git clone https://github.com/MkEger/Text-editor_1
cd Text-editor_1
```

2. **³������� �������:**
   - ³������� ���� `Text editor_1.sln` � Visual Studio

3. **����� �� ������:**
   - �������� **F5** ��� ������� � �������������
   - ��� **Ctrl+F5** ��� ������� ��� ������������

### ������������ MySQL ����������:

**�������:** MySQL ������� ����� ��� ������� "������ �����". ��� MySQL �������� ������ ����������.

#### �� �������� ������������ ����������:
1. **������� ���� `MySqlDatabaseHelper`** � ������
2. **��� ����� connection string � �����������:**

� ������ `MySqlRecentFileRepository.cs` �� `MySqlDocumentRepository.cs` ������� ����� � �����������:

```csharp
// �̲Ͳ�� ֲ ��������� �� ��ί:
private readonly string _connectionString = 
    "Server=localhost;Database=texteditor_db;Uid=your_username;Pwd=your_password;";

// ��� � ����� GetConnection():
private MySqlConnection GetConnection()
{
    return new MySqlConnection("Server=localhost;Database=texteditor_db;Uid=root;Pwd=your_password;");
}
```

#### ��������� ��� �����:
- **Server**: ������ MySQL ������� (�������� `localhost`)
- **Database**: ����� ���� ����� (������� `texteditor_db`)
- **Uid**: ��'� ����������� MySQL (�������� `root`)
- **Pwd**: ������ ��� MySQL �����������

#### ��������� ���� �����:
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

**�������:** �������� ������ �� ������������ ������ ���� ���������� �� �����. MySQL ���������� �����������.

---

*������ ���������: ������� 2024*