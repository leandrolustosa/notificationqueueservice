using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using AInBox.Astove.Core.Exceptions;

namespace AInBox.Astove.Core.Util
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
