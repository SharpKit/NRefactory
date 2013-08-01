using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.Extensions
{
    public static class NProjectExtensions
    {
        public static NFile GetNFile(this IUnresolvedFile file)
        {
            return file.Tag as NFile;
        }
        public static NFile GetNFile(this SyntaxTree me)
        {
            return me.Annotation<NFile>();
        }
        public static NFile GetNFile(this AstNode node)
        {
            var unit = node.ParentOrSelf<SyntaxTree>();
            var file = unit.GetNFile();
            return file;
        }
        public static NProject GetNProject(this IEntity me)
        {
            var p = me.ParentAssembly.UnresolvedAssembly as IProjectContent;
            return p.GetNProject();
        }
        public static NProject GetNProject(this IProjectContent project)
        {
            var p = project as CSharpProjectContent;
            if (p == null)
                return null;
            return p.Tag as NProject;
        }
        public static void SetResolveResult(this AstNode node, ResolveResult res)
        {
            node.AddAnnotation(res);
        }
        public static ResolveResultInfo GetInfo(this AstNode node)
        {
            return node.Annotation<ResolveResultInfo>();
        }
        public static void SetInfo(this ResolveResult res, ResolveResultInfo info)
        {
            res.Tag = info;
        }
        public static Conversion GetConversion(this Expression node)
        {
            var unit = node.GetParent<SyntaxTree>();
            var file = unit.GetNFile();
            var res = file.CSharpAstResolver.GetConversion(node);
            return res;
        }
        public static ResolveResultInfo GetInfo(this ResolveResult res)
        {
            var info = (ResolveResultInfo)res.Tag;
            return info;
        }
        public static List<AstNode> GetNodes(this ResolveResult res)
        {
            var info = (ResolveResultInfo)res.Tag;
            if (info == null)
                return null;
            return info.Nodes;
        }
        public static ResolveResult Resolve(this AstNode node)
        {
            var res = node.Annotation<ResolveResult>();
            if (res == null)
            {
                var unit = node.GetNFile();
                res = Resolve(node, unit);
            }
            return res;
        }
        static ResolveResult Resolve(AstNode node, NFile file)
        {
            if (file.CSharpAstResolver == null)
                file.Project.ApplyNavigator(file);
            var res = node.Annotation<ResolveResult>();// NodesToResults.TryGetValue(node);
            if (res == null)
            {
                var resolver = file.CSharpAstResolver;
                res = resolver.Resolve(node);
            }
            return res;
        }
        public static AstNode GetFirstNode(this ResolveResult res)
        {
            var nodes = res.GetNodes();
            if (nodes != null)
                return nodes.FirstOrDefault();
            return null;
        }
        public static MethodDeclaration GetMethodDeclaration(this IMethod me)
        {
            if (me.UnresolvedMember == null)
                return null;
            return me.UnresolvedMember.Declaration as MethodDeclaration;
        }
        public static AstNode GetDeclaration(this IAttribute me)
        {
            throw new NotImplementedException();
        }
        public static TypeDeclaration GetDeclaration(this ITypeDefinition ce)
        {
            if (ce.Parts.Count == 0)
                return null;
            return ce.Parts[0].Declaration as TypeDeclaration;
        }
        public static EntityDeclaration GetDeclaration(this IMethod me)
        {
            if (me.UnresolvedMember == null)
                return null;
            var x = me.UnresolvedMember.Declaration as EntityDeclaration;
            if (x == null && me.IsAccessor())
            {
                var decl = me.AccessorOwner.GetDeclaration();
                if (decl is PropertyDeclaration)
                {
                    var pe = (PropertyDeclaration)decl;
                    if (me.IsGetter())
                        return pe.Getter;
                    return pe.Setter;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            return x;
        }
        public static EntityDeclaration GetDeclaration(this IMember me)
        {
            if (me is IMethod)
                return ((IMethod)me).GetDeclaration();
            if (me.UnresolvedMember == null)
                return null;
            return me.UnresolvedMember.Declaration as EntityDeclaration;
        }

    }
}
