using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;

namespace ICSharpCode.NRefactory.CSharp
{
    public interface ICSharpResolveResultVisitor<out R> : IResolveResultVisitor<R>
    {
        R VisitCSharpInvocationResolveResult(CSharpInvocationResolveResult res);
        R VisitLambdaResolveResult(LambdaResolveResult res);
        R VisitMethodGroupResolveResult(MethodGroupResolveResult res);
        R VisitDynamicInvocationResolveResult(DynamicInvocationResolveResult res);
		R VisitDynamicMemberResolveResult(DynamicMemberResolveResult res);
		R VisitAwaitResolveResult(AwaitResolveResult res);

	}
}

namespace ICSharpCode.NRefactory.CSharp.Resolver
{

    partial class CSharpInvocationResolveResult
    {
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return ((ICSharpResolveResultVisitor<R>)visitor).VisitCSharpInvocationResolveResult(this); }
    }
    partial class LambdaResolveResult
    {
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return ((ICSharpResolveResultVisitor<R>)visitor).VisitLambdaResolveResult(this); }
    }
    partial class MethodGroupResolveResult
    {
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return ((ICSharpResolveResultVisitor<R>)visitor).VisitMethodGroupResolveResult(this); }
    }
    partial class DynamicInvocationResolveResult
    {
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return ((ICSharpResolveResultVisitor<R>)visitor).VisitDynamicInvocationResolveResult(this); }
    }
    partial class DynamicMemberResolveResult
    {
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return ((ICSharpResolveResultVisitor<R>)visitor).VisitDynamicMemberResolveResult(this); }
    }
	partial class AwaitResolveResult
	{
		public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return ((ICSharpResolveResultVisitor<R>)visitor).VisitAwaitResolveResult(this); }
	}

}

