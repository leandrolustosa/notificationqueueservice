using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AInBox.Astove.Core.Resources
{
    public static class ResourceManager
    {
        public static string ReadText(string resourceName)
        {
            return ReadText(Assembly.GetCallingAssembly(), resourceName);
        }
        public static string ReadText(Assembly assembly, string resourceName)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                StreamReader reader = new StreamReader(assembly.GetManifestResourceStream(resourceName), Encoding.Default);
                string result = reader.ReadToEnd();
                reader.Close();
                return result;
            }
            catch
            {
                throw new ResourceNotFoundException(assembly.FullName + "-" + resourceName);
            }
        }
    }
}
