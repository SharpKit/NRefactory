using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

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
        public override IUnresolvedEntity VisitPropertyDeclaration(PropertyDeclaration node)
        {
            var pe = (IUnresolvedProperty)base.VisitPropertyDeclaration(node);
            if (pe.Getter != null)
                pe.Getter.Declaration = node.Getter;
            if (pe.Setter != null)
                pe.Setter.Declaration = node.Setter;

            return SetDecl(pe, node);
        }

        public override IUnresolvedEntity VisitMethodDeclaration(MethodDeclaration methodDeclaration)
        {
            return SetDecl(base.VisitMethodDeclaration(methodDeclaration), methodDeclaration);
        }
        public override IUnresolvedEntity VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
        {
            return SetDecl(base.VisitOperatorDeclaration(operatorDeclaration), operatorDeclaration);
        }

        public override IUnresolvedEntity VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
        {
            return SetDecl(base.VisitConstructorDeclaration(constructorDeclaration), constructorDeclaration);
        }

        public override IUnresolvedEntity VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
        {
            var pe = (IUnresolvedProperty)base.VisitIndexerDeclaration(indexerDeclaration);
            if (pe.Getter != null)
                pe.Getter.Declaration = indexerDeclaration.Getter;
            if (pe.Setter != null)
                pe.Setter.Declaration = indexerDeclaration.Setter;

            return SetDecl(pe, indexerDeclaration);
        }

        public override IUnresolvedEntity VisitAccessor(Accessor accessor)
        {
            return SetDecl(base.VisitAccessor(accessor), accessor);
        }


        public override IUnresolvedEntity VisitEnumMemberDeclaration(EnumMemberDeclaration enumMemberDeclaration)
        {
            return SetDecl(base.VisitEnumMemberDeclaration(enumMemberDeclaration), enumMemberDeclaration);
        }

        public override IUnresolvedEntity VisitCustomEventDeclaration(CustomEventDeclaration node)
        {
            var ev = (IUnresolvedEvent)base.VisitCustomEventDeclaration(node);
            if (ev.AddAccessor != null)
                ev.AddAccessor.Declaration = ev.AddAccessor;
            if (ev.RemoveAccessor != null)
                ev.RemoveAccessor.Declaration = ev.RemoveAccessor;

            return SetDecl(ev, node);
        }
        public override IUnresolvedEntity VisitEventDeclaration(EventDeclaration node)
        {
            var ev = (IUnresolvedEvent)base.VisitEventDeclaration(node);
            return SetDecl(ev, node);
        }

        public override IUnresolvedEntity VisitTypeDeclaration(TypeDeclaration typeDeclaration)
        {
            return SetDecl(base.VisitTypeDeclaration(typeDeclaration), typeDeclaration);
        }

        public override IUnresolvedEntity VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
        {
            //if field declaration contains two fields, both IUnresolvedField are added to Members, but null will be returned.
            //return SetDecl(base.VisitFieldDeclaration(fieldDeclaration), fieldDeclaration);

            var f = (IUnresolvedField)base.VisitFieldDeclaration(fieldDeclaration);
            if (f == null)
            {
                //the last added  fields do not have a Declaration set. So this way works.
                //Another possible way: get the last N fields. N=fieldDeclaration.Variables.Count.

                var fields = new List<IUnresolvedField>();
                foreach (var mem in CurrentTypeDefinition.Members)
                {
                    if (mem.Declaration == null && mem is IUnresolvedField)
                    {
                        fields.Add((IUnresolvedField)mem);
                        SetDecl(mem, fieldDeclaration);
                    }
                }
                return null; //like VisitFieldDeclaration works, when Variables.Count>1.
            }
            return SetDecl(f, fieldDeclaration);
        }

        private IUnresolvedEntity SetDecl(IUnresolvedEntity unresolvedEntity, object decl)
        {
            unresolvedEntity.Declaration = decl;
            return unresolvedEntity;
        }
    }

}

namespace ICSharpCode.NRefactory.CSharp.TypeSystem
{

    public partial class TypeSystemConvertVisitor
    {
        public IUnresolvedTypeDefinition CurrentTypeDefinition
        {
            get
            {
                return this.currentTypeDefinition;
            }
        }
    }


}
