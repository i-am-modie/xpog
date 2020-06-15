using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xpog.Models
{
    public class UserJwtOptions
    {
        public const string Position = "UsersJwt";

        public string Key { get; set; }
        public int ValidityTimeInMin { get; set; }

        public Byte[] GetByteKey()
        {
            return Encoding.ASCII.GetBytes(Key);
        }
        
    }
}
