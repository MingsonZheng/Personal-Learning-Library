using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LighterApi.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        [HttpPost]
        [Route("signin")]
        public IActionResult SignIn()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secret88secret666"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:6001",
                audience: "https://localhost:6001",
                new List<Claim>
                {
                    new Claim(ClaimTypes.Role, "Administrators"),
                    new Claim("name", "mingson")
                },
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
