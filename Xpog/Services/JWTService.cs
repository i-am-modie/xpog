using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Xpog.Services
{
    public struct JWTToken
    {
        public JWTToken(string token, DateTime expiryDate, int userId)
        {
            Token = token;
            ExpiryDate = expiryDate;
            UserId = userId;
        }
        public string Token { get; }
        public DateTime ExpiryDate { get; }
        public int UserId { get; }
    }
    public interface IJWTService
    {
        JWTToken CreateToken(int userId);
    }
    public abstract class JWTService: IJWTService
    {
        int _tokenValidityTimeInMinutes;
        public JWTService(int tokenValidityTimeInMinutes)
        {
            _tokenValidityTimeInMinutes = tokenValidityTimeInMinutes;
        }

        public JWTToken CreateToken(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("33376e95-5a12-439a-8c2d-d18e83a14be4");
            var expiryDate = DateTime.UtcNow.AddMinutes(_tokenValidityTimeInMinutes);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId.ToString())
                }),
                Expires = expiryDate,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new JWTToken(tokenHandler.WriteToken(token), expiryDate, userId);
        }
    }
}
