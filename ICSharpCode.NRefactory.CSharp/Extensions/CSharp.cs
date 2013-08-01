using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Resolver;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
    partial class CSharpResolver
    {
        static ResolveResult DynamicResult { get { return new ResolveResult(SpecialType.Dynamic); } }

    }
    partial class CSharpOperators
    {
        partial class OperatorMethod
        {
            public object Tag { get; set; }
        }
    }
}
namespace ICSharpCode.NRefactory.CSharp
{

    partial class ReducedExtensionMethod
    {
        public object Tag { get; set; }
    }
    partial class CSharpProjectContent
    {
        public object Tag { get; set; }
    }
}

namespace ICSharpCode.NRefactory.CSharp.TypeSystem
{
    partial class CSharpAssembly : IAssembly
    {
        public object Tag { get; set; }
    }
    /// <summary>
    /// Represents a file that was parsed and converted for the type system.
    /// </summary>
    partial class CSharpUnresolvedFile
    {
        public object Tag { get; set; }

    }
}