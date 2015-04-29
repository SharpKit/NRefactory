using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.Extensions
{
    public class FakeMethod : FakeMember, IMethod
    {
        public FakeMethod(SymbolKind et)
            : base(et)
        {
            Parameters = new List<IParameter>();
            ReturnTypeAttributes = new List<IAttribute>();
            TypeParameters = new List<ITypeParameter>();
            Parts = new List<IUnresolvedMethod>();
        }
        #region IMethod Members

        public IList<IUnresolvedMethod> Parts { get; set; }

        public IList<IAttribute> ReturnTypeAttributes { get; set; }

        public IList<ITypeParameter> TypeParameters { get; set; }

        public bool IsExtensionMethod { get; set; }

        public bool IsConstructor
        {
            get { return SymbolKind == ICSharpCode.NRefactory.TypeSystem.SymbolKind.Constructor; }
        }

        public bool IsDestructor
        {
            get { return SymbolKind == ICSharpCode.NRefactory.TypeSystem.SymbolKind.Destructor; }
        }

        public bool IsOperator
        {
            get { return SymbolKind == ICSharpCode.NRefactory.TypeSystem.SymbolKind.Operator; }
        }

        public bool IsAccessor
        {
            get { return SymbolKind == ICSharpCode.NRefactory.TypeSystem.SymbolKind.Accessor; }
        }

        public IMember AccessorOwner { get; set; }

        #endregion

        #region IParameterizedMember Members

        public IList<IParameter> Parameters { get; set; }

        #endregion

        #region IMethod Members


        public bool IsParameterized
        {
            get { throw new NotImplementedException(); }
        }

        public IList<IType> TypeArguments
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsPartial
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsAsync
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasBody
        {
            get { throw new NotImplementedException(); }
        }

        public IMethod ReducedFrom
        {
            get { throw new NotImplementedException(); }
        }

        public new IMethod Specialize(TypeParameterSubstitution substitution)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class FakeMember : IMember
    {
        public object Tag { get; set; }
        public FakeMember(SymbolKind et)
        {
            SymbolKind = et;
            Attributes = new List<IAttribute>();
            MemberDefinition = this;

        }
        #region IMember Members

        public IMember MemberDefinition { get; set; }

        public IUnresolvedMember UnresolvedMember { get; set; }

        public virtual IType ReturnType { get; set; }
        public IList<IMember> ImplementedInterfaceMembers { get; set; }

        public bool IsExplicitInterfaceImplementation { get; set; }
        public bool IsVirtual { get; set; }

        public bool IsOverride { get; set; }

        public bool IsOverridable { get; set; }

        public IMemberReference ToMemberReference()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEntity Members

        public EntityType EntityType { get; set; }

        public DomRegion BodyRegion { get; set; }
        public DomRegion Region { get; set; }

        public ITypeDefinition DeclaringTypeDefinition { get; set; }

        public IType DeclaringType { get; set; }

        public IAssembly ParentAssembly { get; set; }
        public IList<IAttribute> Attributes { get; set; }

        public ICSharpCode.NRefactory.Documentation.DocumentationComment Documentation { get; set; }

        public bool IsStatic { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsSealed { get; set; }

        public bool IsShadowing { get; set; }

        public bool IsSynthetic { get; set; }

        #endregion

        #region IResolved Members

        public ICompilation Compilation { get; set; }

        #endregion

        #region INamedElement Members

        public string FullName { get; set; }
        public string ReflectionName { get; set; }
        public string Namespace { get; set; }
        public string Name { get; set; }

        #endregion

        #region IHasAccessibility Members

        public Accessibility Accessibility { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsPublic { get; set; }
        public bool IsProtected { get; set; }

        public bool IsInternal { get; set; }

        public bool IsProtectedOrInternal { get; set; }

        public bool IsProtectedAndInternal { get; set; }

        #endregion

        #region IMember Members


        public TypeParameterSubstitution Substitution
        {
            get { throw new NotImplementedException(); }
        }

        public IMember Specialize(TypeParameterSubstitution substitution)
        {
            throw new NotImplementedException();
        }

		public IMemberReference ToReference()
		{
			throw new NotImplementedException();
		}

		ISymbolReference ISymbol.ToReference()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ISymbol Members


		public SymbolKind SymbolKind{get;private set;}

        #endregion
    }

    public class FakeField : FakeMember, IField
    {
        public FakeField()
            : base(SymbolKind.Field)
        {
        }


        public override IType ReturnType
        {
            get
            {
                return Type;
            }
            set
            {
                Type = value;
            }
        }
        #region IField Members



        public bool IsReadOnly { get; set; }

        public bool IsVolatile { get; set; }

        #endregion



        #region IVariable Members


        public IType Type { get; set; }

        public bool IsConst { get; set; }

        public object ConstantValue { get; set; }

        #endregion

        #region IField Members

        public bool IsFixed
        {
            get { return false; }
        }

        #endregion

    }
}
