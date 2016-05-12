using System;
using System.IO;
using System.Reflection;

namespace Commons.MemQueue
{
    internal class TypeLoader
    {
        public static Type Load(string typeName)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var dll = Directory.GetFiles(currentDir, "*.dll");
            foreach (var f in dll)
            {
                var ass = Assembly.LoadFile(f);
                var type = ass.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            var exe = Directory.GetFiles(currentDir, "*.exe");
            foreach (var f in exe)
            {
                var ass = Assembly.LoadFile(f);
                var type = ass.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }
    }
}
