using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;

namespace LibraryData
{
    public class BookRepository
    {
        private readonly string connectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=MyDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30";

        // Adding book using parameterized query
        public void AddBook(Book book)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            string query = "INSERT INTO Books (Title, Author, Stock) VALUES (@title, @author, @stock)";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@title", book.Title);
            cmd.Parameters.AddWithValue("@author", book.Author);
            cmd.Parameters.AddWithValue("@stock", book.Stock);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // Retrieve every book as a List<Book>
        public List<Book> GetAllBooks()
        {
            List<Book> books = new List<Book>();
            SqlConnection conn = new SqlConnection(connectionString);
            string query = "SELECT BookId, Title, Author, Stock FROM Books";

            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Book book = new Book
                {
                    BookId = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Author = reader.GetString(2),
                    Stock = reader.GetInt32(3)
                };
                books.Add(book);
            }

            return books;
        }

        // Update book stock copies
        public void UpdateStock(int bookId, int newStock)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            string query = "UPDATE Books SET Stock = @stock WHERE BookId = @bookId";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@stock", newStock);
            cmd.Parameters.AddWithValue("@bookId", bookId);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // Remove a book from the database
        public void DeleteBook(int bookId)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            string query = "DELETE FROM Books WHERE BookId = @bookId";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@bookId", bookId);

            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}