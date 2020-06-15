using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xpog.Models.Configuration
{
    public class DbOptions
    {
        public const string Position = "Db";

        public string User { get; set; }

        public string Password { get; set; }

        public string Server { get; set; }

        public int Port { get; set; }

        public string DbName { get; set; }

        public string GetConnectionString()
        {
            return $"User ID ={User};Password={Password};Server={Server};Port={Port.ToString()};Database={DbName}; Integrated Security=true;Pooling=true;";
        }
    }
}
