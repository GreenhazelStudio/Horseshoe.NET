using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET
{
    public static class Lib
    {
        public static Assembly Assembly => typeof(Lib).Assembly;

        public static AssemblyName AssemblyName => Assembly.GetName();

        public static string Name => AssemblyName.Name;

        public static string FullName => AssemblyName.FullName;

        public static string Version => AssemblyName.GetDisplayVersion(minDepth: 3);

        public static string DisplayName => AssemblyName.GetDisplayName(minDepth: 3);

        public static Assembly[] Assemblies => Horseshoe.NET.Assemblies.List()
            .Where(a => a.GetName().Name.StartsWith(Name))
            .OrderBy(a => a.GetName().Name)
            .ToArray();

        public static AssemblyName[] AssemblyNames => Assemblies
            .Select(a => a.GetName())
            .ToArray();

        public static string[] Names => AssemblyNames
            .Select(a => a.Name)
            .ToArray();

        public static string[] FullNames => AssemblyNames
            .Select(an => an.FullName)
            .ToArray();

        public static string[] DisplayNames => AssemblyNames
            .Select(an => an.GetDisplayName(minDepth: 3))
            .ToArray();
    }
}
