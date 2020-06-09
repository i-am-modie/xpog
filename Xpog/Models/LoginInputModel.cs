using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Xpog.Models
{
    public class LoginInputModel
    {
        [Required]
        [MinLength(4)]
        public string Username { get; set; }

        [Required]
        [MinLength(4)]
        public string Password { get; set; }
    }
}
