using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.TypeSystem;
using System.Diagnostics;

namespace ICSharpCode.NRefactory.Semantics
{
    partial class ResolveResult
    {
        public object Tag { get; set; }
        [DebuggerStepThrough]
        public virtual R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitResolveResult(this); }
    }
    partial class InvocationResolveResult
    {
        [DebuggerStepThrough]
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitInvocationResolveResult(this); }
    }
    partial class MemberResolveResult
    {
        [DebuggerStepThrough]
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitMemberResolveResult(this); }
    }
    partial class ThisResolveResult
    {
        [DebuggerStepThrough]
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitThisResolveResult(this); }
    }

    partial class ConstantResolveResult
    {
        [DebuggerStepThrough]
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitConstantResolveResult(this); }
    }
    partial class ConversionResolveResult
    {
        [DebuggerStepThrough]
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitConversionResolveResult(this); }
    }
    partial class LocalResolveResult
    {
        [DebuggerStepThrough]
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitLocalResolveResult(this); }
    }
    partial class TypeResolveResult
    {
        [DebuggerStepThrough]
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitTypeResolveResult(this); }
    }
    partial class TypeOfResolveResult
    {
        [DebuggerStepThrough]
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitTypeOfResolveResult(this); }
    }
    partial class OperatorResolveResult
    {
        [DebuggerStepThrough]
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitOperatorResolveResult(this); }
    }
    partial class TypeIsResolveResult
    {
        [DebuggerStepThrough]
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitTypeIsResolveResult(this); }
    }
    partial class ArrayCreateResolveResult
    {
        [DebuggerStepThrough]
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitArrayCreateResolveResult(this); }
    }
    partial class ArrayAccessResolveResult
    {
        [DebuggerStepThrough]
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitArrayAccessResolveResult(this); }
    }
    partial class InitializedObjectResolveResult
    {
        [DebuggerStepThrough]
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitInitializedObjectResolveResult(this); }
    }
    partial class ByReferenceResolveResult
    {
        [DebuggerStepThrough]
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitByReferenceResolveResult(this); }
    }

    partial class NamedArgumentResolveResult
    {
        [DebuggerStepThrough]
        public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitNamedArgumentResolveResult(this); }
    }

	partial class UnknownMethodResolveResult
	{
		[DebuggerStepThrough]
		public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitUnknownMethodResolveResult(this); }
	}
	partial class AmbiguousTypeResolveResult
	{
		[DebuggerStepThrough]
		public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitAmbiguousTypeResolveResult(this); }
	}

	partial class SizeOfResolveResult
	{
		[DebuggerStepThrough]
		public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitSizeOfResolveResult(this); }
	}
	partial class ErrorResolveResult
	{
		[DebuggerStepThrough]
		public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitErrorResolveResult(this); }
	}
	partial class ForEachResolveResult
	{
		[DebuggerStepThrough]
		public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitForEachResolveResult(this); }
	}
	partial class NamespaceResolveResult
	{
		[DebuggerStepThrough]
		public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitNamespaceResolveResult(this); }
	}
	partial class UnknownMemberResolveResult
	{
		[DebuggerStepThrough]
		public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitUnknownMemberResolveResult(this); }
	}
	partial class UnknownIdentifierResolveResult
	{
		[DebuggerStepThrough]
		public override R AcceptVisitor<R>(IResolveResultVisitor<R> visitor) { return visitor.VisitUnknownIdentifierResolveResult(this); }
	}


	public interface IResolveResultVisitor<out R>
    {
		R VisitResolveResult(ResolveResult res);
		R VisitMemberResolveResult(MemberResolveResult res);
		R VisitAmbiguousTypeResolveResult(AmbiguousTypeResolveResult res);
		R VisitUnknownMethodResolveResult(UnknownMethodResolveResult res);
		R VisitInvocationResolveResult(InvocationResolveResult res);
		R VisitThisResolveResult(ThisResolveResult res);
		R VisitConstantResolveResult(ConstantResolveResult res);
		R VisitConversionResolveResult(ConversionResolveResult res);
		R VisitLocalResolveResult(LocalResolveResult res);
		R VisitTypeResolveResult(TypeResolveResult res);
		R VisitTypeOfResolveResult(TypeOfResolveResult res);
		R VisitOperatorResolveResult(OperatorResolveResult res);
		R VisitTypeIsResolveResult(TypeIsResolveResult res);
		R VisitArrayCreateResolveResult(ArrayCreateResolveResult res);
		R VisitArrayAccessResolveResult(ArrayAccessResolveResult res);
		R VisitInitializedObjectResolveResult(InitializedObjectResolveResult res);
		R VisitByReferenceResolveResult(ByReferenceResolveResult res);
		R VisitNamedArgumentResolveResult(NamedArgumentResolveResult res);
		R VisitSizeOfResolveResult(SizeOfResolveResult res);
		R VisitErrorResolveResult(ErrorResolveResult res);
		R VisitForEachResolveResult(ForEachResolveResult res);
		R VisitNamespaceResolveResult(NamespaceResolveResult res);
		R VisitUnknownMemberResolveResult(UnknownMemberResolveResult res);
		R VisitUnknownIdentifierResolveResult(UnknownIdentifierResolveResult res);
	}
}


namespace ICSharpCode.NRefactory.TypeSystem
{

    partial interface IEntity
    {
        object Tag { get; set; }
    }
    partial interface IAssembly
    {
        object Tag { get; set; }
    }
}

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
    partial class SpecializedMember
    {
        public object Tag { get; set; }
    }
    partial class DefaultResolvedTypeDefinition
    {
        public object Tag { get; set; }
    }
    partial class AbstractResolvedEntity
    {
        public object Tag { get; set; }
    }
    partial class DefaultUnresolvedAssembly
    {
        partial class DefaultResolvedAssembly
        {
            public object Tag { get; set; }
        }
    }
}

namespace ICSharpCode.NRefactory.TypeSystem
{
    partial interface IUnresolvedEntity
    {
        object Declaration { get; set; }
    }
    /// <summary>
    /// Represents a single file that was parsed.
    /// </summary>
    partial interface IUnresolvedFile
    {
        object Tag { get; set; }
    }

}

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
    partial class AbstractUnresolvedEntity
    {
        public object Declaration { get; set; }
    }
}
 
 
 
 