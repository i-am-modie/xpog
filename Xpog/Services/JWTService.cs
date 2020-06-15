using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xpog.Models;

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
        private int _tokenValidityTimeInMinutes;
        private readonly Byte[] _key;
        public JWTService(int tokenValidityTimeInMinutes, Byte[] key)
        {
            _tokenValidityTimeInMinutes = tokenValidityTimeInMinutes;
            _key = key;
        }

        public JWTToken CreateToken(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
           
            var expiryDate = DateTime.UtcNow.AddMinutes(_tokenValidityTimeInMinutes);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId.ToString())
                }),
                Expires = expiryDate,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new JWTToken(tokenHandler.WriteToken(token), expiryDate, userId);
        }
    }
}
