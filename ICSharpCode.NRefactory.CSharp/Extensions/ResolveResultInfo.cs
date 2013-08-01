using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.Extensions
{
    public class ResolveResultInfo
    {
        public ResolveResultInfo()
        {
            Nodes = new List<AstNode>();
        }
        public CSharpResolver GetResolverStateAfter()
        {
            var node = Nodes.FirstOrDefault();
            if (node == null || Resolver == null)
                return null;
            var state = Resolver.GetResolverStateAfter(node);
            return state;
        }
        public CSharpResolver GetResolverStateBefore()
        {
            var node = Nodes.FirstOrDefault();
            if (node == null || Resolver == null)
                return null;
            var state = Resolver.GetResolverStateBefore(node);
            return state;
        }

        public ResolveResultInfo Clone()
        {
            return new ResolveResultInfo
            {
                Conversion = Conversion,
                ConversionTargetType = ConversionTargetType,
                Nodes = Nodes.ToList(),
                OriginalInfo = OriginalInfo,
                Resolver = Resolver,
                ResolveResult = ResolveResult,
            };
        }

        public ResolveResult ResolveResult { get; set; }
        public List<AstNode> Nodes { get; set; }
        public Conversion Conversion { get; set; }
        public IType ConversionTargetType { get; set; }
        public CSharpAstResolver Resolver { get; set; }
        public ResolveResultInfo OriginalInfo { get; set; }

    }
}
