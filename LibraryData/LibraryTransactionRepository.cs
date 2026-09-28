using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;

namespace LibraryData
{
    public class LibraryTransactionRepository
    {
        private readonly string connectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=MyDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30";

        // Part C: Issue a Book Using a Transaction
        public bool IssueBook(int bookId, int memberId)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                // 1. Check current stock of the book
                string checkStockQuery = "SELECT Stock FROM Books WHERE BookId = @bookId";
                SqlCommand checkCmd = new SqlCommand(checkStockQuery, conn, transaction);
                checkCmd.Parameters.AddWithValue("@bookId", bookId);

                object result = checkCmd.ExecuteScalar();
                if (result == null)
                {
                    transaction.Rollback();
                    return false; // Book does not exist
                }

                int currentStock = Convert.ToInt32(result);
                if (currentStock <= 0)
                {
                    transaction.Rollback();
                    return false; // No copies left — tests the rollback requirement!
                }

                // 2. Decrease stock by 1
                string updateStockQuery = "UPDATE Books SET Stock = Stock - 1 WHERE BookId = @bookId";
                SqlCommand updateCmd = new SqlCommand(updateStockQuery, conn, transaction);
                updateCmd.Parameters.AddWithValue("@bookId", bookId);
                updateCmd.ExecuteNonQuery();

                // 3. Insert record into IssuedBooks table
                string insertIssueQuery = "INSERT INTO IssuedBooks (BookId, MemberId, IssueDate, ReturnDate) VALUES (@bookId, @memberId, @issueDate, NULL)";
                SqlCommand insertCmd = new SqlCommand(insertIssueQuery, conn, transaction);
                insertCmd.Parameters.AddWithValue("@bookId", bookId);
                insertCmd.Parameters.AddWithValue("@memberId", memberId);
                insertCmd.Parameters.AddWithValue("@issueDate", DateTime.Now);
                insertCmd.ExecuteNonQuery();

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }

        // Part D: Return a Book Using a Transaction
        public bool ReturnBook(int issueId)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                // 1. Find the active issue record and get its BookId
                string getIssueQuery = "SELECT BookId FROM IssuedBooks WHERE IssueId = @issueId AND ReturnDate IS NULL";
                SqlCommand getCmd = new SqlCommand(getIssueQuery, conn, transaction);
                getCmd.Parameters.AddWithValue("@issueId", issueId);

                object result = getCmd.ExecuteScalar();
                if (result == null)
                {
                    transaction.Rollback();
                    return false; // Invalid issue ID or already returned
                }

                int bookId = Convert.ToInt32(result);

                // 2. Update ReturnDate in IssuedBooks
                string updateIssueQuery = "UPDATE IssuedBooks SET ReturnDate = @returnDate WHERE IssueId = @issueId";
                SqlCommand updateIssueCmd = new SqlCommand(updateIssueQuery, conn, transaction);
                updateIssueCmd.Parameters.AddWithValue("@returnDate", DateTime.Now);
                updateIssueCmd.Parameters.AddWithValue("@issueId", issueId);
                updateIssueCmd.ExecuteNonQuery();

                // 3. Increase book stock back by 1
                string updateStockQuery = "UPDATE Books SET Stock = Stock + 1 WHERE BookId = @bookId";
                SqlCommand updateStockCmd = new SqlCommand(updateStockQuery, conn, transaction);
                updateStockCmd.Parameters.AddWithValue("@bookId", bookId);
                updateStockCmd.ExecuteNonQuery();

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}