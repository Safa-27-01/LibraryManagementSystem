using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;

namespace LibraryData
{
    public class MemberRepository
    {
        private readonly string connectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=MyDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30";

        // Add a new member using parameterized query
        public void AddMember(Member member)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            string query = "INSERT INTO Members (Name) VALUES (@name)";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", member.Name);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // Retrieve every member as a List<Member>
        public List<Member> GetAllMembers()
        {
            List<Member> members = new List<Member>();
            SqlConnection conn = new SqlConnection(connectionString);
            string query = "SELECT MemberId, Name FROM Members";

            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Member member = new Member
                {
                    MemberId = reader.GetInt32(0),
                    Name = reader.GetString(1)
                };
                members.Add(member);
            }

            return members;
        }
    }
}
