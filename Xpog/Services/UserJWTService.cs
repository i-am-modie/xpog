using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xpog.Models;

namespace Xpog.Services
{
    public class UserJWTService : JWTService
    {
        public UserJWTService(IOptions<UserJwtOptions> options) : base(options.Value.ValidityTimeInMin, options.Value.GetByteKey()) { }
    }
}
