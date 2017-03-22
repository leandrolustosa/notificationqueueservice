using AInBox.Astove.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AInBox.Messaging.Model
{
    public class AddModuleToDatabaseBinding
    {
        public int EnterpriseId { get; set; }

        public string Name { get; set; }

        public string FromName { get; set; }
        public string FromEmail { get; set; }

        public string RestrictIps { get; set; }

        public bool Active { get; set; }
    }

    public class AddModuleToDatabaseResult : BaseResultModel
    {
    }
}
