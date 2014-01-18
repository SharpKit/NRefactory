using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Semantics;
using System.Runtime.CompilerServices;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using System.Collections.Concurrent;
using ICSharpCode.NRefactory.CSharp.TypeSystem;

namespace ICSharpCode.NRefactory.Extensions
{
    public static class Extensions
    {
        public static T ParentOrSelf<T>(this AstNode node) where T : AstNode
        {
            if (node is T)
                return (T)node;
            return node.GetParent<T>();
        }
        public static IEnumerable<Tuple<T1, T2>> SelectTwice<T1, T2>(this IEnumerable<T1> list1, IEnumerable<T2> list2)
        {
            using (var x1 = list1.GetEnumerator())
            {
                using (var x2 = list2.GetEnumerator())
                {
                    var has1 = true;
                    var has2 = true;
                    if (has1)
                        has1 = x1.MoveNext();
                    if (has2 = x2.MoveNext())
                        has2 = x2.MoveNext();
                    if (has1 && has2)
                        yield return new Tuple<T1, T2>(x1.Current, x2.Current);
                    else if (has1)
                        yield return new Tuple<T1, T2>(x1.Current, default(T2));
                    else if (has2)
                        yield return new Tuple<T1, T2>(default(T1), x2.Current);
                    else
                        yield break;
                }
            }
        }
        public static bool IsDescendantOf(this AstNode node, AstNode parent)
        {
            var node2 = node;
            while (node2 != null)
            {
                if (node2 == parent)
                    return true;
                node2 = node2.Parent;
            }
            return false;
        }


        public static void InsertOrAdd<T>(this IList<T> list, int index, T item)
        {
            if (index >= list.Count)
                list.Add(item);
            else
                list.Insert(index, item);
        }
        public static bool IsEnumMember(this IEntity me)
        {
            if (me.SymbolKind == SymbolKind.Field && me.DeclaringType != null && me.DeclaringType.Kind == TypeKind.Enum)
                return true;
            return false;
        }
        public static IEnumerable<IMethod> GetMethodsAndAccessorMethodsAndConstructors(this IType ce, Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            var list = ce.GetMethods(filter, options).Concat(ce.GetConstructors(filter, options));
            var list2 = ce.GetAccessorMethods(null, options);
            var list3 = list2.Where(t => filter((IUnresolvedMethod)t.UnresolvedMember));
            var list4 = list.Concat(list3);
            foreach (var item in list4)
                yield return item;
        }
        public static IEnumerable<IMethod> GetMethodsAndAccessorMethods(this IType ce, Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            var list = ce.GetMethods(filter, options);
            var list2 = ce.GetAccessorMethods(null, options);
            var list3 = list2.Where(t => filter((IUnresolvedMethod)t.UnresolvedMember));
            var list4 = list.Concat(list3);
            foreach (var item in list4)
                yield return item;
        }
        public static IEnumerable<IMethod> GetAccessorMethods(this IType ce, Predicate<IUnresolvedMember> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            foreach (var pe in ce.GetProperties(filter, options))
            {
                if (pe.Getter != null)
                    yield return pe.Getter;
                if (pe.Setter != null)
                    yield return pe.Setter;
            }
            foreach (var pe in ce.GetEvents(filter, options))
            {
                if (pe.AddAccessor != null)
                    yield return pe.AddAccessor;
                if (pe.RemoveAccessor != null)
                    yield return pe.RemoveAccessor;
            }
        }
        public static List<IMethod> GetMethods(this ITypeDefinition ce, string name)
        {
            return ce.GetMethodsAndAccessorMethodsAndConstructors(t => t.Name == name).ToList();
        }
        public static ITypeDefinition GetEntityType(this IType tr)
        {
            return tr.GetDefinition();// tr.Resolve(Project.Compilation.TypeResolveContext).GetDefinition();
        }
        public static IEntity GetParent(this IAttribute att)
        {
            return null; //TODO:
        }
        public static IList<ITypeParameter> GetGenericArguments(this ITypeDefinition ce)
        {
            return ce.TypeParameters;
        }
        public static IList<ITypeParameter> GetGenericArguments(this IMethod me)
        {
            return me.TypeParameters;
        }
        public static IEnumerable<ITypeDefinition> GetTypes(this IAssembly asm)
        {
            return asm.TopLevelTypeDefinitions;
        }
        public static bool IsGenericClass(this ITypeDefinition ce)
        {
            return ce.TypeParameterCount > 0;
        }
        public static IType GetEntityTypeRef(this ITypeDefinition ce)
        {
            return ce;
            //throw new NotImplementedException();
        }
        public static bool IsGetter(this IMethod me)
        {
            return me.IsAccessor() && me.Name.StartsWith("get_");
        }
        public static bool IsGetter(this IMember me)
        {
            throw new NotImplementedException();
        }
        public static IEnumerable<IProperty> GetDeclaredPropertiesAndIndexers(this ITypeDefinition ce)
        {
            throw new NotImplementedException();
        }
        public static ITypeDefinition GetDeclaringTypeDefinition(this IEntity me)
        {
            return me.DeclaringTypeDefinition;
        }
        public static IType GetBaseType(this IType ce)
        {
            if (ce.Kind == TypeKind.Interface)
                return null;
            return ce.DirectBaseTypes.FirstOrDefault(t => t.Kind != TypeKind.Interface);
        }

        public static ITypeDefinition GetBaseTypeDefinition(this ITypeDefinition ce)
        {
            if (ce.Kind == TypeKind.Interface)
                return null;
            var ce2 = ce.DirectBaseTypes.FirstOrDefault(t => t.Kind != TypeKind.Interface);
            if (ce2 == null)
                return null;
            return ce2.GetDefinition();
        }
        public static bool IsEnum(this IEntity me)
        {
            return me.SymbolKind == SymbolKind.TypeDefinition && ((ITypeDefinition)me).Kind == TypeKind.Enum;
        }
        public static bool IsEnum(this ITypeDefinition ce)
        {
            return ce.Kind == TypeKind.Enum;
        }
        public static bool IsInterface(this ITypeDefinition ce)
        {
            return ce.Kind == TypeKind.Interface;
        }
        public static bool IsGenerated(this IEntity me, NProject project)
        {
            return me.IsCompilerGenerated();
            //throw new NotImplementedException();
        }
        public static bool IsAutomatic(this IEvent ev, NProject project)
        {
            var decl = NProjectExtensions.GetDeclaration(ev.AddAccessor);
            return decl == null;
            //if (decl != null)
            //    return decl is EventDeclaration; //manual events return 'Accessor' type in declaration
            //throw new NotImplementedException();
        }

        public static IProperty GetProperty(this IType ce, string name)
        {
            return ce.GetProperties(t => t.Name == name).FirstOrDefault();
        }
        public static List<IMethod> GetConstructors(this ITypeDefinition ce, bool instanceCtors, bool staticCtors)
        {
            var list = ce.GetConstructors(null, GetMemberOptions.IgnoreInheritedMembers).ToList();
            if (!instanceCtors)
                list.RemoveAll(t => !t.IsStatic);
            if (staticCtors)
                list.AddRange(ce.Methods.Where(t => t.SymbolKind == SymbolKind.Constructor && t.IsStatic));
            return list;
        }
        public static IMember GetOwner(this IMethod me)
        {
            return me.AccessorOwner;
            //if (me.Name.StartsWith("get_") || me.Name.StartsWith("set_"))
            //{
            //    if (!me.IsAccessor())
            //        return null;
            //    var name = me.Name.Substring(me.Name.IndexOf("_") + 1);
            //    var list = me.DeclaringType.GetProperties(t => t.Name == name, GetMemberOptions.IgnoreInheritedMembers);
            //    var meDef = me.MemberDefinition;
            //    foreach (var pe in list)
            //    {
            //        //doesn't work with generic
            //        //if (pe.Getter == me || pe.Setter == me)
            //        //    return pe;
            //        if (pe.Getter != null && pe.Getter.MemberDefinition == meDef)
            //            return pe;
            //        if (pe.Setter != null && pe.Setter.MemberDefinition == meDef)
            //            return pe;
            //    }
            //}
            //else if (me.Name.StartsWith("add_") || me.Name.StartsWith("remove_"))
            //{
            //    if (!me.IsAccessor())
            //        return null;
            //    var name = me.Name.Substring(me.Name.IndexOf("_") + 1);
            //    var list = me.DeclaringType.GetEvents(t => t.Name == name, GetMemberOptions.IgnoreInheritedMembers);
            //    var meDef = me.MemberDefinition;
            //    foreach (var ev in list)
            //    {
            //        //doesn't work with generic
            //        //if (pe.Getter == me || pe.Setter == me)
            //        //    return pe;
            //        if (ev.AddAccessor != null && ev.AddAccessor.MemberDefinition == meDef)
            //            return ev;
            //        if (ev.RemoveAccessor != null && ev.RemoveAccessor.MemberDefinition == meDef)
            //            return ev;
            //    }

            //}
            //return null;
        }
        public static AstNode GetDefinition(this IMethod me)
        {
            var decl = me.GetDeclaration();
            if (decl == null)
                return null;
            return decl.GetDefinition();
        }
        public static BlockStatement GetDefinition(this EntityDeclaration decl)
        {
            return decl.GetChildByRole<BlockStatement>(Roles.Body);
        }
        public static IMember GetBaseMember(this IMember me)
        {
            return InheritanceHelper.GetBaseMember(me);
        }
        public static IMethod GetBaseMethod(this IMethod me)
        {
            return (IMethod)InheritanceHelper.GetBaseMember(me);
        }

        /// <summary>
        /// Array types are not reference equal for some reason
        /// </summary>
        /// <param name="ce1"></param>
        /// <param name="ce2"></param>
        /// <returns></returns>
        public static bool EqualsTo(this IType ce1, IType ce2)
        {
            if (ce1 == ce2)
                return true;
            return ce1.FullName == ce2.FullName;
        }

        public static bool IsAutomaticProperty(this IProperty pe)
        {
            if (!pe.CanGet || !pe.CanSet || pe.IsIndexer)
                return false;
            if (pe.ParentAssembly.IsMainAssembly)
            {
                if (pe.Getter == null || pe.Setter == null)
                    return false;
                if (pe.Getter.IsCompilerGenerated())
                    return true;
                var decl = (Accessor)pe.Getter.GetDeclaration();
                if (decl == null || decl.Body == null || decl.Body.IsNull)
                    return true;
                return false;
            }
            else
            {
                var gen = (pe.Getter.IsCompilerGenerated());
                return gen;
            }
        }
        public static bool IsCompilerGenerated(this IEntity me)
        {
            if (me.IsSynthetic)
                return true;
            var x = me.Attributes.FindByAttributeTypeFullName<CompilerGeneratedAttribute>().FirstOrDefault() != null;
            if (x)
                return true;
            var me2 = me as IMember;
            if (me2 != null)
            {
                var decl = me2.GetDeclaration();
                if (decl == null)
                {
                    if (me2.SymbolKind == SymbolKind.Accessor)
                    {
                        var me3 = me as IMethod;
                        var owner = me3.AccessorOwner;
                        if (owner!=null && owner.SymbolKind == SymbolKind.Event)
                        {
                            return true;
                        }
                    }
                }
                if (decl is EventDeclaration)
                    return true;

                else if (decl is PropertyDeclaration)
                {
                    var pe2 = (PropertyDeclaration)decl;
                    if (pe2.Getter != null && pe2.Getter.Body.IsNull && pe2.Setter != null && pe2.Setter.Body.IsNull)
                        return true;
                    return false;
                }
                else if (decl is Accessor)
                {
                    var acc = (Accessor)decl;
                    var pe2 = acc.GetParent<PropertyDeclaration>();
                    if (pe2 != null && pe2.Getter != null && pe2.Getter.Body.IsNull && pe2.Setter != null && pe2.Setter.Body.IsNull)
                        return true;
                    return false;

                }
            }
            return false;
        }
        public static bool IsDelegate(this ITypeDefinition prm)
        {
            if (prm == null)
                return false;
            if (prm.DirectBaseTypes.FirstOrDefault() == null)
                return false;
            var baseCe = prm.DirectBaseTypes.First();
            if (baseCe.IsKnownType(KnownTypeCode.MulticastDelegate))
                return true;
            return false;
        }
        public static bool IsAnonymousMethod(this IMethod prm)
        {
            throw new NotImplementedException();
        }

        public static bool IsGenericParam(this ITypeParameter prm)
        {
            return prm.IsUnbound();
        }
        public static bool IsIndexerAccessor(this IMethod me)
        {
            var owner = me.GetOwner();
            return owner != null && owner.SymbolKind == SymbolKind.Indexer;
        }
        public static bool IsGenericTypeParameter(this IType tr)
        {
            if (tr.Kind != TypeKind.TypeParameter)// || tr.Kind == TypeKind.UnboundTypeArgument;
                return false;
            var prm = (ITypeParameter)tr;
            return prm.OwnerType == SymbolKind.TypeDefinition;
        }
        public static bool IsGenericMethodArgument(this IType ce)
        {
            if (ce.Kind != TypeKind.TypeParameter)
                return false;
            var prm = (ITypeParameter)ce;
            return prm.OwnerType == SymbolKind.Method;
        }

        public static string GetFileOrigin(this ITypeDefinition ce)
        {
            var part = ce.Parts.Where(t => t.UnresolvedFile != null).FirstOrDefault();
            if (part == null)
                return null;
            return part.UnresolvedFile.FileName;
            //var n = ce.GetDeclaration();
            //if (n == null)
            //    return null;
            //return n.getFilePath();
        }
        public static string GetFullyQualifiedCLRName(this IType tr)
        {
            return tr.ReflectionName;
        }

        public static IType GetPropertyTypeRef(this IProperty pe)
        {
            return pe.ReturnType;//.ToTypeReference();
        }
        public static bool IsIndexerSetter(this IMethod me)
        {
            var owner = me.GetOwner() as IProperty;
            return owner != null && owner.SymbolKind == SymbolKind.Indexer && owner.Setter == me;
        }

        public static bool IsVoid(this IType type)
        {
            return type.Kind == TypeKind.Void;
        }

        public static bool IsAutomaticEventAccessor(this IMethod me)
        {
            return me.IsEventAccessor() && me.IsAutomaticAccessor();
        }

        public static bool IsAutomaticPropertyAccessor(this IMethod me)
        {
            return me.IsPropertyAccessor() && me.IsAutomaticAccessor();
        }

        public static bool IsEventAccessor(this IMethod me)
        {
            var owner = me.GetOwner();
            return owner != null && owner.SymbolKind == SymbolKind.Event;
        }
        public static bool IsEventAddAccessor(this IMethod me)
        {
            return me.IsEventAccessor() && me.Name.StartsWith("add_");
        }
        public static bool IsEventRemoveAccessor(this IMethod me)
        {
            return me.IsEventAccessor() && me.Name.StartsWith("remove_");
        }

        public static bool IsPropertyAccessor(this IMethod me)
        {
            var owner = me.GetOwner();
            return owner != null && owner.SymbolKind == SymbolKind.Property;
        }


        public static bool IsAccessor(this IMethod me)
        {
            return me.SymbolKind == SymbolKind.Accessor;
            //if (me.EntityType != EntityType.Method)
            //    return false;
            //return me.AccessorOwner!=null;//.DeclaringType.GetMethods(t => t.Name == me.Name, GetMemberOptions.IgnoreInheritedMembers).FirstOrDefault() == null;
        }
        public static IMethod GetConstructor(this ITypeDefinition ce)
        {
            return ce.GetConstructors(t => !t.IsStatic && t.Parameters.Count == 0).FirstOrDefault();
        }
        public static bool IsAutomaticAccessor(this IMethod me)
        {
            return me.IsAccessor() && me.IsCompilerGenerated();
        }
        public static void SetConstantValue(this IField me, object value)
        {
            throw new NotImplementedException();
        }
        public static List<IField> GetConstants(this ITypeDefinition ce)
        {
            return ce.GetFields().ToList();
            //throw new NotImplementedException();
        }
        public static List<IMethod> GetAllMethods(this ITypeDefinition ce, string name)
        {
            return ce.GetMethods(t => t.Name == name).ToList();
        }
        public static string GetFileName(this AstNode node)
        {
            var region = node.GetRegion();
            if (region == null || region.IsEmpty)
                return null;
            return region.FileName;
        }
        public static T SetEntity<T>(this T node, IEntity me) where T : AstNode
        {
            if (me is IMember)
                node.SetResolveResult(new MemberResolveResult(null, (IMember)me));
            else if (me is IType)
                node.SetResolveResult(new TypeResolveResult((IType)me));
            else
                throw new NotImplementedException();
            return node;
        }
        public static bool IsConstructor(this IMember me)
        {
            return me.SymbolKind == SymbolKind.Constructor;
        }

        public static T PrevSibling<T>(this AstNode node) where T : AstNode
        {
            var prev = node.PrevSibling;
            while (prev != null)
            {
                if (prev is T)
                    return (T)prev;
                prev = prev.PrevSibling;
            }
            return null;
        }

        public static ITypeDefinition GetParentType(this AstNode node)
        {
            return node.FindThisEntity();
        }
        public static ITypeDefinition GetParentType(this ResolveResult res)
        {
            return res.GetInfo().GetResolverStateAfter().CurrentTypeDefinition;
        }
        public static T AssociateWithOriginal<T>(this T fake, ResolveResult original) where T : ResolveResult
        {
            var info = original.GetInfo();
            if (info == null)
                return fake;
            var info2 = info.Clone();
            info2.OriginalInfo = info;
            fake.SetInfo(info2);
            return fake;
        }
        public static ResolveResult GetParent(this ResolveResult res, NProject project)
        {
            var node = res.GetFirstNode();
            if (node == null)
                return null;
            var parent = node.Parent;
            if (parent == null)
                return null;
            return parent.Resolve();
        }

        public static ITypeDefinition FindThisEntity(this AstNode n)
        {
            var state = GetResolverStateAfter(n);
            return state.CurrentTypeDefinition;
        }

        public static CSharpResolver GetResolverStateAfter(this AstNode node)
        {
            var info = node.GetInfo();
            var state = info.Resolver.GetResolverStateAfter(node);
            return state;
        }

        public static CSharpResolver GetResolverStateBefore(this AstNode node)
        {
            var info = node.GetInfo();
            var state = info.Resolver.GetResolverStateBefore(node);
            return state;
        }

        public static IMethod GetCurrentMethod(this ResolveResult res)
        {
            return (IMethod)res.GetCurrentMember();
        }

        public static IMethod GetCurrentMethod(this AstNode node)
        {
            return (IMethod)node.GetCurrentMember();
        }
        public static IMember GetCurrentMember(this ResolveResult res)
        {
            var info = res.GetInfo();
            var node = info.Nodes.FirstOrDefault();
            if (node == null)
                return null;
            return node.GetCurrentMember();
        }
        public static IMember GetCurrentMember(this AstNode node)
        {
            var state = node.GetResolverStateAfter();
            return state.CurrentMember;
        }

        public static BlockStatement GetDeclarationBody(this IMethod me)
        {
            var decl = me.GetDeclaration();
            if (decl == null)
                return null;
            if (decl is MethodDeclaration)
                return ((MethodDeclaration)decl).Body;
            if (decl is Accessor)
                return ((Accessor)decl).Body;
            if (decl is OperatorDeclaration)
                return ((OperatorDeclaration)decl).Body;
            if (decl is ConstructorDeclaration)
                return ((ConstructorDeclaration)decl).Body;
            if (decl is IndexerDeclaration)
            {
                if (me.Name.StartsWith("get_"))
                    return ((IndexerDeclaration)decl).Getter.Body;
                else if (me.Name.StartsWith("set_"))
                    return ((IndexerDeclaration)decl).Setter.Body;
                else
                    throw new NotImplementedException();
            }
            else
                throw new NotImplementedException();
        }
        public static IEnumerable<IAssembly> GetReferencedAssemblies(this ICompilation comp)
        {
            return comp.Assemblies.Where(t => t != comp.MainAssembly);
        }

        public static bool IsStatic(this IEntity me)
        {
            if (me.IsStatic)
                return me.IsStatic;
            //HACK: IField const returns static=false;
            if (me.SymbolKind == SymbolKind.Field)
                return ((IField)me).IsConst;
            return false;
        }
        public static bool IsStatic(this IField me)
        {
            return me.IsStatic || me.IsConst;
        }

        public static List<ParameterBinding> GetArgumentsForCall2(this CSharpInvocationResolveResult res)
        {

            var args = res.Arguments;
            var callArgs = res.GetArgumentsForCall();

            var map = res.GetArgumentToParameterMap();
            var prms = res.Member.Parameters;
            var list = callArgs.Select(t => new ParameterBinding { CallResult = t }).ToList();
            var index = -1;
            foreach (var arg in args)
            {
                index++;
                var index2 = map == null ? index : map[index];
                var prm = prms[index2];
                var binding = list[index2];
                binding.Parameter = prm;
                binding.ArgResult = arg;
            }
            index = 0;
            foreach (var binding in list)
            {
                var prm = prms[index];
                if (binding.Parameter == null)
                    binding.Parameter = prm;
                else if (binding.Parameter != prm)
                    throw new NotImplementedException();
                if (!prm.IsParams)
                    index++;
            }
            return list;
        }

        public static IEnumerable<ITypeDefinition> GetOwnAndGenericParametersTypeDefinitions(this IType type)
        {
            var def = type.GetDefinition();
            if (def != null)
                yield return def;
            foreach (var arg in type.TypeArguments)
            {
                foreach (var def2 in GetOwnAndGenericParametersTypeDefinitions(arg))
                    yield return def2;
            }
        }

        public static IEnumerable<IAttribute> FindByAttributeTypeFullName<T>(this IEnumerable<IAttribute> list)
        {
            return list.Where(t => t.AttributeType.FullName == typeof(T).FullName);
        }

        /// <summary>
        /// Converts this syntax tree into a parsed file that can be stored in the type system.
        /// </summary>
        public static CSharpUnresolvedFile ToMappedTypeSystem(this SyntaxTree tree)
        {
            if (string.IsNullOrEmpty(tree.FileName))
            {
                throw new InvalidOperationException("Cannot use ToTypeSystem() on a syntax tree without file name.");
            }
            MappedTypeSystemConvertVisitor typeSystemConvertVisitor = new MappedTypeSystemConvertVisitor(tree.FileName);
            typeSystemConvertVisitor.VisitSyntaxTree(tree);
            return typeSystemConvertVisitor.UnresolvedFile;
        }

        public static VariableInitializer GetVariable(this IField field)
        {
            var decl = (FieldDeclaration)field.GetDeclaration();
            if (decl != null)
            {
                if (decl.Variables.Count <= 1) return decl.Variables.FirstOrDefault();

                foreach (var init in decl.Variables)
                {
                    if (init.Name == field.Name) return init;
                }
            }
            return null;
        }

        public static Expression GetInitializer(this IField field)
        {
            var variable = field.GetVariable();
            if (variable == null) return null;
            return variable.Initializer;
        }

    }


    public class ParameterBinding
    {
        public ResolveResult CallResult { get; set; }
        public IParameter Parameter { get; set; }
        public ResolveResult ArgResult { get; set; }
        public ParameterBinding Clone()
        {
            return new ParameterBinding
            {
                CallResult = CallResult,
                Parameter = Parameter,
                ArgResult = ArgResult
            };
        }



    }
}
