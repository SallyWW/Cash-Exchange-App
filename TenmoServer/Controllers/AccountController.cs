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
        public ActionResult<UserBalance> Balance()
        {
            UserBalance balance = new UserBalance();
            int userId = int.Parse(this.User.FindFirst("sub").Value);

            balance.Balance = userDAO.GetBalance(userId);

            return Ok(balance);
        }
        
    }
}
