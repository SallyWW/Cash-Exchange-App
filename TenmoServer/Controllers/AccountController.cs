using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserDAO userDAO;
        public AccountController(IUserDAO _userDAO)
        {
            userDAO = _userDAO;
        }

        [HttpGet("balance")]
        public ActionResult<UserBalance> ViewBalance()
        {
            UserBalance balance = new UserBalance();
            int userId = int.Parse(this.User.FindFirst("sub").Value);

            balance.Balance = userDAO.GetBalance(userId);

            return Ok(balance);
        }

        [HttpGet("users")]
        public ActionResult<List<UserResponse>> GetUsers()
        {
            List<User> dBUsers = userDAO.GetUsers();
            List<UserResponse> users = new List<UserResponse>();

            foreach (User dBUser in dBUsers)
            {
                users.Add(new UserResponse { UserId = dBUser.UserId, Username = dBUser.Username });
            }

            return Ok(users);
        }

        [HttpPost("transfer")]
        public ActionResult SendTransfer(SendTransfer transfer)
        {
            int typeSendId = 1001;
            int statusApprovedId = 2001;

            decimal usersBalance = userDAO.GetBalance(transfer.SendId);

            userDAO.CreateTransfer(typeSendId, statusApprovedId, transfer.SendId, transfer.RecieveId, transfer.Amount);

            userDAO.UpdateBalance(transfer.RecieveId, transfer.Amount);

            userDAO.UpdateBalance(transfer.SendId, (transfer.Amount * -1));

            return Ok();
        }
    }
}
