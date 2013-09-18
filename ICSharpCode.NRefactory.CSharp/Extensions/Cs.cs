using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.CSharp.Resolver;

namespace ICSharpCode.NRefactory.Extensions
{
    public static partial class Cs
    {
        public static OperatorResolveResult Trinary(this ResolveResult first, System.Linq.Expressions.ExpressionType opType, ResolveResult second, ResolveResult third, IType resultType)
        {
            return new OperatorResolveResult(resultType, opType, first, second, third);
        }
        public static OperatorResolveResult Conditional(this ResolveResult first, ResolveResult second, ResolveResult third, IType resultType)
        {
            return new OperatorResolveResult(resultType, System.Linq.Expressions.ExpressionType.Conditional, first, second, third);
        }
        public static OperatorResolveResult Binary(this ResolveResult left, System.Linq.Expressions.ExpressionType opType, ResolveResult right, IType resultType)
        {
            return new OperatorResolveResult(resultType, opType, left, right);
        }
        public static OperatorResolveResult Assign(this ResolveResult left, ResolveResult right)
        {
            return left.Binary(System.Linq.Expressions.ExpressionType.Assign, right, left.Type);
        }
        public static OperatorResolveResult Equal(this ResolveResult left, ResolveResult right, NProject compiler)
        {
            return left.Binary(System.Linq.Expressions.ExpressionType.Equal, right, BooleanType(compiler));
        }
        public static OperatorResolveResult NotEqual(this ResolveResult left, ResolveResult right, NProject compiler)
        {
            return left.Binary(System.Linq.Expressions.ExpressionType.NotEqual, right, BooleanType(compiler));
        }
        public static MemberResolveResult Member(this ResolveResult target, IMember me)
        {
            return new MemberResolveResult(target, me);
        }
        public static MemberResolveResult Member(this ResolveResult target, IMember me, bool forceNonConstant)
        {
            if (forceNonConstant)
                return new MemberResolveResultForceNonConst(target, me);
            return Member(target, me);
        }
        public static CSharpInvocationResolveResult Invoke(this MemberResolveResult target, params ResolveResult[] args)
        {
            return new CSharpInvocationResolveResult(target.TargetResult, (IParameterizedMember)target.Member, args);
        }
        public static CSharpInvocationResolveResult InvokeMethod(this ResolveResult target, IParameterizedMember me, params ResolveResult[] args)
        {
            return new CSharpInvocationResolveResult(target, me, args);
        }
        public static CSharpInvocationResolveResult InvokeMethod(this IParameterizedMember me, ResolveResult target, params ResolveResult[] args)
        {
            return target.Member(me).Invoke(args);
        }
        public static ResolveResult Null()
        {
            return new ResolveResult(SpecialType.NullType);
        }
        public static IType CharType(NProject compiler)
        {
            return compiler.Compilation.FindType(KnownTypeCode.Char);
        }
        public static IType BooleanType(NProject compiler)
        {
            return compiler.Compilation.FindType(KnownTypeCode.Boolean);
        }
        public static ITypeDefinition ArrayType(ICompilation compiler)
        {
            return (ITypeDefinition)compiler.FindType(KnownTypeCode.Array);
        }
        public static ResolveResult Value(object value, NProject compiler)
        {
            if (value == null)
                return Null();
            return new ConstantResolveResult(compiler.Compilation.FindType(value.GetType()), value);
        }
        /// <summary>
        /// Access a static member, or a this.member
        /// </summary>
        /// <returns></returns>
        public static MemberResolveResult AccessSelf(this IMember me, bool forceNonConst)
        {
            if (me.IsStatic() || me.SymbolKind == SymbolKind.Constructor)
                return Member(new TypeResolveResult(me.DeclaringType), me, forceNonConst);
            return me.DeclaringType.This().Member(me, forceNonConst);
        }
        public static MemberResolveResult AccessSelf(this IMember me)
        {
            return AccessSelf(me, false);
        }
        public static MemberResolveResult AccessSelfForceNonConst(this IMember me)
        {
            return AccessSelf(me, true);
        }
        public static ThisResolveResult This(this IType type)
        {
            return new ThisResolveResult(type);
        }


        public static ResolveResult New(IType ce)
        {
            var ctor = ce.GetConstructors(t => !t.IsStatic && t.Parameters.Count == 0, GetMemberOptions.IgnoreInheritedMembers).FirstOrDefault();
            if (ctor == null)
                throw new Exception("Cannot find parameterless contrsuctor for type: " + ce.FullName);
            return Cs.InvokeMethod(ctor, null);
        }



    }

    class MemberResolveResultForceNonConst : MemberResolveResult
    {

        public MemberResolveResultForceNonConst(ResolveResult target, IMember me)
            : base(target, me)
        {
        }

        public override bool IsCompileTimeConstant
        {
            get
            {
                return false;
            }
        }

    }

}
