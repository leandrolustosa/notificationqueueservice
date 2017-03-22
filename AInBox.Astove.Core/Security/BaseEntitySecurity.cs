using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AInBox.Astove.Core.Security
{
    public class BaseEntitySecurity
    {
        private const string INTERNAL_KEY = "Astove.Security.37f6e36c-6b18-41b8-9d58-c267e8edda0c";
        public string SecurityKey { get { return INTERNAL_KEY; } }
    }
}
