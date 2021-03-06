using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.Security.Models;

namespace TenmoServer.DAO
{
    public class UserSqlDAO : IUserDAO
    {
        private readonly string connectionString;
        const decimal startingBalance = 1000;

        public object CreateTranser => throw new NotImplementedException();

        public UserSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public User GetUser(string username)
        {
            User returnUser = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT user_id, username, password_hash, salt FROM users WHERE username = @username", conn);
                cmd.Parameters.AddWithValue("@username", username);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows && reader.Read())
                {
                    returnUser = GetUserFromReader(reader);
                }
            }

            return returnUser;
        }

        public List<User> GetUsers()
        {
            List<User> returnUsers = new List<User>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT user_id, username, password_hash, salt FROM users", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        User u = GetUserFromReader(reader);
                        returnUsers.Add(u);
                    }

                }
            }

            return returnUsers;
        }

        public User AddUser(string username, string password)
        {
            IPasswordHasher passwordHasher = new PasswordHasher();
            PasswordHash hash = passwordHasher.ComputeHash(password);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO users (username, password_hash, salt) VALUES (@username, @password_hash, @salt)", conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password_hash", hash.Password);
                cmd.Parameters.AddWithValue("@salt", hash.Salt);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("SELECT @@IDENTITY", conn);
                int userId = Convert.ToInt32(cmd.ExecuteScalar());

                cmd = new SqlCommand("INSERT INTO accounts (user_id, balance) VALUES (@userid, @startBalance)", conn);
                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@startBalance", startingBalance);
                cmd.ExecuteNonQuery();
            }

            return GetUser(username);
        }

        private User GetUserFromReader(SqlDataReader reader)
        {
            return new User()
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                Username = Convert.ToString(reader["username"]),
                PasswordHash = Convert.ToString(reader["password_hash"]),
                Salt = Convert.ToString(reader["salt"]),
            };
        }

        public decimal GetBalance(int userId)
        {
            decimal balance = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT balance FROM accounts WHERE user_id = @userId", conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                SqlDataReader reader = cmd.ExecuteReader();

                reader.Read();
                balance = Convert.ToDecimal(reader["balance"]);
            }
            return balance;
        }

        public void CreateTransfer(int type, int status, int fromUserId, int toUserId, decimal amount)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                //Use userId to get accountId
                int fromAccountId = GetAccountId(fromUserId);
                int toAccountId = GetAccountId(toUserId);

                SqlCommand cmd = new SqlCommand("INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                    "VALUES(@typeId, @statusId, @fromId, @sendId, @transferAmount)", conn);
                cmd.Parameters.AddWithValue("@typeId", type);
                cmd.Parameters.AddWithValue("@statusId", status);
                cmd.Parameters.AddWithValue("@fromId", fromAccountId);
                cmd.Parameters.AddWithValue("@sendId", toAccountId);
                cmd.Parameters.AddWithValue("@transferAmount", amount);

                cmd.ExecuteNonQuery();
            }
        }

        private int GetAccountId(int userId)
        {
            int accountId;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT account_id FROM accounts WHERE user_id = @userId", conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                SqlDataReader reader = cmd.ExecuteReader();

                reader.Read();
                accountId = Convert.ToInt32(reader["account_id"]);
            }
            return accountId;
        }

        public void UpdateBalance(int userId, decimal delta)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("UPDATE accounts SET balance = balance + @amount WHERE user_id = @userId", conn);
                cmd.Parameters.AddWithValue("@amount", delta);
                cmd.Parameters.AddWithValue("@userId", userId);

                cmd.ExecuteNonQuery();
            }
        }

        public List<TransferDetails> ViewTransfers(int userId)
        {
            List<TransferDetails> transfers = new List<TransferDetails>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT t.transfer_id AS id, t.amount AS amount, usTo.user_id AS ToUserId, usTo.username AS ToUsername, " +
                    "usFrom.user_id AS FromUserId, usFrom.username AS FromUsername, ts.transfer_status_desc, tt.transfer_type_desc " +
                    "FROM transfers t " +
                    "INNER JOIN accounts acTo ON t.account_to = acTo.account_id " +
                    "INNER JOIN users usTo ON acTo.user_id = UsTo.user_id " +
                    "INNER JOIN accounts acFrom ON t.account_from = acFrom.account_id " +
                    "INNER JOIN users usFrom ON acFrom.user_id = UsFrom.user_id " +
                    "INNER JOIN transfer_statuses ts ON t.transfer_status_id = ts.transfer_status_id " +
                    "INNER JOIN transfer_types tt ON t.transfer_type_id = tt.transfer_type_id " +
                    "WHERE usTo.user_id = @userid " +
                    "OR usFrom.user_id = @userid", conn);

                cmd.Parameters.AddWithValue("@userid", userId);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        TransferDetails viewTransfer = new TransferDetails();
                        viewTransfer.TransferId = Convert.ToInt32(reader["id"]);
                        viewTransfer.FromUserId = Convert.ToInt32(reader["FromUserId"]);
                        viewTransfer.TransferAmount = Convert.ToDecimal(reader["amount"]);
                        viewTransfer.FromUsername = Convert.ToString(reader["FromUsername"]);
                        viewTransfer.ToUserId = Convert.ToInt32(reader["ToUserId"]);
                        viewTransfer.ToUserName = Convert.ToString(reader["ToUsername"]);
                        viewTransfer.Type = Convert.ToString(reader["transfer_type_desc"]);
                        viewTransfer.Status = Convert.ToString(reader["transfer_status_desc"]);

                        transfers.Add(viewTransfer);
                    }

                }
            }
            return transfers;
        }
    }
}
