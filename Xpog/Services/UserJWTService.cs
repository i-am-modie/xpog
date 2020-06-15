using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xpog.Services
{
    public class UserJWTService : JWTService
    {
        static int VALIDITY_TIME_IN_MIN = 150;
        public UserJWTService() : base(VALIDITY_TIME_IN_MIN) { }
    }
}
