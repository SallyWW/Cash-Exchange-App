using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IUserDAO
    {
        object CreateTranser { get; }

        User GetUser(string username);
        User AddUser(string username, string password);
        List<User> GetUsers();

        decimal GetBalance(int userId);
        void CreateTransfer(int type, int status, int fromUserId, int toUserId, decimal amount);
        void UpdateBalance(int userId, decimal delta);
    }
}
