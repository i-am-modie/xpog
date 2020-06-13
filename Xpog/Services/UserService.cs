using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Ef6CoreForPosgreSQL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Xpog.Entities;
using Xpog.Models;
using Xpog.Responses;

namespace Xpog.Services
{
    public interface IUserService
    {
        LoginResponse Authenticate(string username, string password);
        Task<int> Register(string username, string password);
        Task<ActionResult<IEnumerable<User>>> GetAll();
    }

    public class UserService : IUserService
    {
        private IHashingService _hashingService;
        private ExpenseAppContext _context;
        IJWTService _userJwtService;

        public UserService(IHashingService hashingService, ExpenseAppContext expenseAppContext, IJWTService jwtService)
        {
            _hashingService = hashingService;
            _context = expenseAppContext;
            _userJwtService = jwtService;
        }


        public LoginResponse Authenticate(string username, string password)
        {
            var user = _context.Users.Where(x => x.Username == username).FirstOrDefault();

            if (user == null)
            {
                throw new Exception("No user with that username");
            }

            if(!_hashingService.Verify(password, user.PasswordHash)) {
                throw new Exception("Invalid credentials");
            }

            var token = _userJwtService.CreateToken(user.Id);
            var response = new LoginResponse();

            response.Username = username;
            response.Token = token.Token;
            response.ExpiryDate = token.ExpiryDate;
            return response;
        }

        public async Task<int> Register(string username, string password)
        {
            var existingCheck = _context.Users.Where(x => x.Username == username).FirstOrDefault();
            if (existingCheck != null)
            {
                throw new Exception("Username already taken");
            }

            var user = new User { Username = username, PasswordHash = _hashingService.Hash(password) };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user.Id;
        }

        private int CreatedAtAction(string v, object p, User user)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }
    }
}