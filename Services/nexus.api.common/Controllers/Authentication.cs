
using System.Data;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using nexus.common;
using nexus.common.dal;
using nexus.common.helper;
using nexus.shared.common;


namespace nexus.api.common
{
    [Route("cmn")]
    [ApiController]
    public class getToken : ControllerBase
    {
        [HttpPost("getToken")]
        public IActionResult Post([FromBody] LoginRequest user)
        {
            bool Validated = false;

            if (user is null)                                                                return BadRequest("Invalid user request!!!");
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))  return BadRequest("Invalid username/password");

            using (ValidateLogin login = new ValidateLogin()) Validated = login.Validate(user.Username, user.Password);

            nexus.common.helpers.WriteToLog("Validate: " + user.Username + ":" + user.Password + " " + ((Validated) ? "Validated OK" : "Validate Failed")) ;

            if (Validated)
            {
                string diag = "";

                var secretKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(nexus.api.common.ConfigurationManager.AppSetting["JWT:Secret"]));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);


                var claims = new[] {
                   new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                   new Claim(JwtRegisteredClaimNames.Sub,   user.Username) //,
                   //new Claim(JwtRegisteredClaimNames.Email, user.EmailAddress),
                   //new Claim("DateOfJoing", userInfo.DateOfJoing.ToString("yyyy-MM-dd"))
                 };

                var tokeOptions = new JwtSecurityToken(   issuer:   nexus.api.common.ConfigurationManager.AppSetting["JWT:ValidIssuer"],
                                                          audience: nexus.api.common.ConfigurationManager.AppSetting["JWT:ValidAudience"],
                                                          claims:   claims,
                                                          expires:  DateTime.Now.AddDays(7),
                                                          signingCredentials: signinCredentials  );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new JWTTokenResponse { Token = tokenString });
            }

            return Unauthorized("Invalid username/password");

        }

        [HttpPost("Login")]
        public LoginResponse Login([FromBody] LoginRequest request)
        {
            bool Validated = false;

            LoginResponse response = new LoginResponse();
            response.Status = "Bad";

            if (request is null) response.Response = "Bad Request";
            else if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password)) response.Response = "Invalid username/password";
            else
            {
                using (ValidateLogin login = new ValidateLogin()) response = login.Login(request.Username, request.Password);
            }

            return response;
        }


        [HttpGet("getSystemDate")]
        public IActionResult getSystemDate()
        {
            return Ok(DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss:ff"));
        }


        [HttpGet("logintest")]
        public IActionResult LoginTest()
        {
            return Ok("running");
        }

    }


    public class JWTTokenResponse
    {
        public string? Token { get; set; }
    }


}
