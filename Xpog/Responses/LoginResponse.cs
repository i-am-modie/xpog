using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Xpog.Responses
{
    public class LoginResponse
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public DateAndTime ExpiryDate { get; set; }
    }
}