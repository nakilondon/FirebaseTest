using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthSeries.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthSeries.Services
{
    public class TokenService : ITokenService
    {
        private const double EXP_DURATION_MINUTES = 30;
//        private readonly IAipRolesProcessor10 _aipRolesProcessor10;

//        public TokenService(IAipRolesProcessor10 aipRolesProcessor10)
//        {
//            _aipRolesProcessor10 = aipRolesProcessor10;
//        }

        public string BuildToken(string key, string issuer, UserModel userModel)
        {
            if (userModel.Email == null)
            {
                throw new Exception("Email Required");
            }
            // TODO: put real-world logic to evaluate sign-in credetials
            // ...
//            var roles = _aipRolesProcessor10.GetBitString(new List<int>{(int)AipRole.Viewer});
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userModel.Email),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
//                new Claim("rls", roles)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer: issuer, audience: issuer, claims, expires: DateTime.Now.AddMinutes(EXP_DURATION_MINUTES), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}