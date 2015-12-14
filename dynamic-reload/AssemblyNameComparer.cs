using System;
using System.Collections.Generic;
using System.Reflection;

namespace DynamicReloader
{   
    public class AssemblyNameComparer : IEqualityComparer<AssemblyName>
    {
        public static IEqualityComparer<AssemblyName> OrdinalIgnoreCase = new AssemblyNameComparer();

        public bool Equals(AssemblyName x, AssemblyName y)
        {
            return
                string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.CultureName ?? "", y.CultureName ?? "", StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(AssemblyName obj)
        {
            var hashCode = 0;
            if (obj.Name != null)
            {
                hashCode ^= obj.Name.ToUpperInvariant().GetHashCode();
            }

            hashCode ^= (obj.CultureName?.ToUpperInvariant() ?? "").GetHashCode();
            return hashCode;
        }
    }   
}