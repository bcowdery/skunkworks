using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PortAuthority
{
    /// <summary>
    /// Provides access to runtime application assemblies.
    /// </summary>
    public static class AssemblyHook
    {
        /// <summary>
        /// Assembly reference for PortAuthority
        /// </summary>
        public static Assembly Assembly = typeof(AssemblyHook).Assembly;
    }
}
