using CleanAuthenticator.Application;
using CleanAuthenticator.Application.Interfaces.Account;
using CleanAuthenticator.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CleanAuthenticator.Infrastructure
{
    public class Jwtgenerator:IJwtgenerator
    {
        private readonly IConfiguration _config;
        public Jwtgenerator(IConfiguration config)
        {
           _config=config;
        }

        public async Task<string> CreateToken(Accounts user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            List<Claim> ClaimsList = new List<Claim>
            {
                new Claim(ClaimTypes.GivenName,user.Username),
                new Claim(ClaimTypes.Email,user.EmailAddress),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),

            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims: ClaimsList,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

    }
}
