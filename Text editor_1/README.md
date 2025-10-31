# Text Editor MK - Professional Course Project

## ?? Getting Started

### System Requirements:
- **Windows OS** (Windows 7 SP1 or higher)
- **.NET Framework 4.7.2** or newer version
- **Visual Studio 2017/2019/2022** 
- **MySQL 8.0+** (optional)

### Quick Start:

1. **Clone the repository:**
```bash
git clone https://github.com/MkEger/Text-editor_1
cd Text-editor_1
```

2. **Open the project:**
   - Open the `Text editor_1.sln` file in Visual Studio

3. **Build and run:**
   - Press **F5** to build and run with debugging
   - Or **Ctrl+F5** to run without debugging

### MySQL Database Configuration:

**Note:** MySQL database is optional for the "Recent Files" feature. The application works without MySQL configuration.

#### To enable database functionality:
1. **Remove the `MySqlDatabaseHelper`** from the project
2. **Or configure the connection string in the constructor:**

In files `MySqlRecentFileRepository.cs` and `MySqlDocumentRepository.cs`, modify the connection string:

```csharp
// CHANGE THIS CONNECTION STRING:
private readonly string _connectionString = 
    "Server=localhost;Database=texteditor_db;Uid=your_username;Pwd=your_password;";

// Or in the GetConnection() method:
private MySqlConnection GetConnection()
{
    return new MySqlConnection("Server=localhost;Database=texteditor_db;Uid=root;Pwd=your_password;");
}
```

#### Connection parameters:
- **Server**: MySQL server address (usually `localhost`)
- **Database**: Database name (create `texteditor_db`)
- **Uid**: MySQL username (usually `root`)
- **Pwd**: Password for MySQL user