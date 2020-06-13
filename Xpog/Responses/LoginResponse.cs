
using System;

namespace Xpog.Responses
{
    public class LoginResponse
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}