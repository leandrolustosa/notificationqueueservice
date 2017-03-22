using AInBox.Astove.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AInBox.Messaging.Model
{
    public class AddEnterpriseToDatabaseBinding
    {
        public string Name { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }
        public string ServerUsername { get; set; }
        public string ServerPassword { get; set; }
    }

    public class AddEnterpriseToDatabaseResult : BaseResultModel
    {
    }
}
