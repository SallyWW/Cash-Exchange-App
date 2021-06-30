using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;

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
        public ActionResult<decimal> Balance()
        {
            int userId = int.Parse(this.User.FindFirst("sub").Value);
            decimal balance = userDAO.GetBalance(userId);

            return Ok(balance);
        }
        
    }
}
