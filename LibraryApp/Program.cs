using System;
using System.Collections.Generic;

using LibraryData;

namespace LibraryApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BookRepository bookRepo = new BookRepository();
            MemberRepository memberRepo = new MemberRepository();
            LibraryTransactionRepository txRepo = new LibraryTransactionRepository();
            BackupHelper backupHelper = new BackupHelper();

            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== Library Management System ===");
                Console.WriteLine("1. Add Book");
                Console.WriteLine("2. View All Books");
                Console.WriteLine("3. Add Member");
                Console.WriteLine("4. View All Members");
                Console.WriteLine("5. Issue Book to Member (Transaction)");
                Console.WriteLine("6. Return Book (Transaction)");
                Console.WriteLine("7. Export Books to JSON");
                Console.WriteLine("8. Import Books from JSON");
                Console.WriteLine("9. Exit");
                Console.Write("Choose an option (1-9): ");

                string? choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        {
                            Book book = new Book();
                            Console.Write("Enter Title: ");
                            book.Title = Console.ReadLine() ?? string.Empty;
                            Console.Write("Enter Author: ");
                            book.Author = Console.ReadLine() ?? string.Empty;
                            Console.Write("Enter Stock: ");
                            if (int.TryParse(Console.ReadLine(), out int stock))
                            {
                                book.Stock = stock;
                                bookRepo.AddBook(book);
                                Console.WriteLine("\nBook added successfully!");
                            }
                            else
                            {
                                Console.WriteLine("\nInvalid stock number.");
                            }
                        }
                        break;

                    case "2":
                        {
                            List<Book> books = bookRepo.GetAllBooks();
                            Console.WriteLine("--- Book List ---");
                            foreach (var b in books)
                            {
                                Console.WriteLine($"ID: {b.BookId} | Title: {b.Title} | Author: {b.Author} | Stock: {b.Stock}");
                            }
                        }
                        break;

                    case "3":
                        {
                            Member member = new Member();
                            Console.Write("Enter Member Name: ");
                            member.Name = Console.ReadLine() ?? string.Empty;
                            memberRepo.AddMember(member);
                            Console.WriteLine("\nMember added successfully!");
                        }
                        break;

                    case "4":
                        {
                            List<Member> members = memberRepo.GetAllMembers();
                            Console.WriteLine("--- Member List ---");
                            foreach (var m in members)
                            {
                                Console.WriteLine($"ID: {m.MemberId} | Name: {m.Name}");
                            }
                        }
                        break;

                    case "5":
                        {
                            Console.Write("Enter Book ID to issue: ");
                            if (int.TryParse(Console.ReadLine(), out int bookId))
                            {
                                Console.Write("Enter Member ID: ");
                                if (int.TryParse(Console.ReadLine(), out int memberId))
                                {
                                    bool success = txRepo.IssueBook(bookId, memberId);
                                    if (success)
                                        Console.WriteLine("\nBook issued successfully!");
                                    else
                                        Console.WriteLine("\nFailed to issue book (Check if book exists and has available stock).");
                                }
                            }
                        }
                        break;

                    case "6":
                        {
                            Console.Write("Enter Issue ID to return: ");
                            if (int.TryParse(Console.ReadLine(), out int issueId))
                            {
                                bool success = txRepo.ReturnBook(issueId);
                                if (success)
                                    Console.WriteLine("\nBook returned successfully!");
                                else
                                    Console.WriteLine("\nFailed to return book (Invalid issue ID or already returned).");
                            }
                        }
                        break;

                    case "7":
                        {
                            List<Book> books = bookRepo.GetAllBooks();
                            string filePath = "books_backup.json";
                            backupHelper.ExportBooksToJson(books, filePath);
                            Console.WriteLine($"\nBooks successfully exported to {filePath}");
                        }
                        break;

                    case "8":
                        {
                            string filePath = "books_backup.json";
                            List<Book> importedBooks = backupHelper.ImportBooksFromJson(filePath);
                            Console.WriteLine("--- Imported Books from JSON ---");
                            foreach (var b in importedBooks)
                            {
                                Console.WriteLine($"Title: {b.Title} | Current Stock: {b.Stock}");
                            }
                        }
                        break;

                    case "9":
                        running = false;
                        Console.WriteLine("Exiting application. Goodbye!");
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please choose between 1 and 9.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }
    }
}