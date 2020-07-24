using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Auther.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public UsersController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("SlackAuthFailed")]
        [AllowAnonymous]
        public IActionResult SlackAuthFailed() => Redirect(_config.GetValue<string>("Slack:AuthFailRedirectUrl"));

        [HttpGet("LoginWithSlack")]
        [Authorize(AuthenticationSchemes = "Slack")]
        public IActionResult LoginWithSlack(string redirectUrl)
        {
            // Here is where you'd ensure that a user account
            // exists for this particular Slack user.
            // Keep in mind this doesn't have to be their first
            // time loging in. So only create an account if needed.

            // Make sure to use all the information gathered from Slack.
            // That is:

            // Their unique Slack ID:
            // User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value

            // Their Slack username:
            // User.Claims.First(c => c.Type == ClaimTypes.Name).Value

            // For example let's say we create a new user
            // and have his ID now. We'd like to store that
            // user ID in our JWT.
            var userId = Guid.NewGuid().ToString();

            string key = _config.GetValue<string>("Jwt:EncryptionKey");
            var issuer = _config.GetValue<string>("Jwt:Issuer");
            var audience = _config.GetValue<string>("Jwt:Audience");
        
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            permClaims.Add(new Claim("userid", userId));
        
            var token = new JwtSecurityToken(
                issuer,
                audience,
                permClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);

            return Redirect($"{redirectUrl}?token={jwt_token}");
        }
    }
}
