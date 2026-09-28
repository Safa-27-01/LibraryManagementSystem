using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace LibraryData
{
    public class BackupHelper
    {
        // 1. Save a list of books as a JSON file
        public void ExportBooksToJson(List<Book> books, string filePath)
        {
            string jsonString = JsonSerializer.Serialize(books, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
        }

        // 2. Read a JSON file and return it back as a list of books
        public List<Book> ImportBooksFromJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<Book>();
            }

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Book>>(jsonString) ?? new List<Book>();
        }
    }
}