using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.Extensions
{

    public class MappedTypeSystemConvertVisitor : TypeSystemConvertVisitor
    {
        public MappedTypeSystemConvertVisitor(string filename)
            : base(filename)
        {
        }

        public override IUnresolvedEntity VisitSyntaxTree(SyntaxTree unit)
        {
            var x = base.VisitSyntaxTree(unit);
            return x;
        }
        public override IUnresolvedEntity VisitAttribute(Attribute attribute)
        {
            return SetDecl(base.VisitAttribute(attribute), attribute);
        }
        public override IUnresolvedEntity VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
        {
            return SetDecl(base.VisitPropertyDeclaration(propertyDeclaration), propertyDeclaration);
        }

        public override IUnresolvedEntity VisitMethodDeclaration(MethodDeclaration methodDeclaration)
        {
            return SetDecl(base.VisitMethodDeclaration(methodDeclaration), methodDeclaration);
        }

        public override IUnresolvedEntity VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
        {
            return SetDecl(base.VisitConstructorDeclaration(constructorDeclaration), constructorDeclaration);
        }

        public override IUnresolvedEntity VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
        {
            return SetDecl(base.VisitIndexerDeclaration(indexerDeclaration), indexerDeclaration);
        }

        public override IUnresolvedEntity VisitAccessor(Accessor accessor)
        {
            return SetDecl(base.VisitAccessor(accessor), accessor);
        }

        public override IUnresolvedEntity VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration)
        {
            return SetDecl(base.VisitCustomEventDeclaration(eventDeclaration), eventDeclaration);
        }

        public override IUnresolvedEntity VisitEnumMemberDeclaration(EnumMemberDeclaration enumMemberDeclaration)
        {
            return SetDecl(base.VisitEnumMemberDeclaration(enumMemberDeclaration), enumMemberDeclaration);
        }

        public override IUnresolvedEntity VisitEventDeclaration(EventDeclaration eventDeclaration)
        {
            return SetDecl(base.VisitEventDeclaration(eventDeclaration), eventDeclaration);
        }

        public override IUnresolvedEntity VisitTypeDeclaration(TypeDeclaration typeDeclaration)
        {
            return SetDecl(base.VisitTypeDeclaration(typeDeclaration), typeDeclaration);
        }

        public override IUnresolvedEntity VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
        {
            return SetDecl(base.VisitFieldDeclaration(fieldDeclaration), fieldDeclaration);
        }

        private IUnresolvedEntity SetDecl(IUnresolvedEntity unresolvedEntity, object decl)
        {
            unresolvedEntity.Declaration = decl;
            return unresolvedEntity;
        }
    }
}
