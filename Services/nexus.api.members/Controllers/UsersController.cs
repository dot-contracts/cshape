
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
//using nexus.web.auth.Model;
//using nexus.web.auth.Service;
//using Web_API_Tutorials.Entities;
//using Web_API_Tutorials.Models;
//using Web_API_Tutorials.Services;

namespace nexus.web.auth
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private readonly IUsers _users;
        private readonly IJWTTokenService _jwttokenservice;

        // GET: api/<UsersController>

        public UsersController(IUsers users, IJWTTokenService jWTTokenServices)
        {
            _users = users;
            _jwttokenservice = jWTTokenServices;
        }

        [HttpGet]
        public IActionResult Get()
        {

            return Ok(_users.GetAll());
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Authenticate")]
        public IActionResult Authenticate(Users users)
        {
            var token = _jwttokenservice.Authenticate(users);

            if (token == null)
            {
                return Ok(new { Message = "Unauthorized" });
            }

            return Ok(token);
        }
    }
}
