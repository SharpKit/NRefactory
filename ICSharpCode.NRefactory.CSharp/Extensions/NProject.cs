﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using System.Collections.Concurrent;

namespace ICSharpCode.NRefactory.Extensions
{
    public class NProject
    {
        public bool Parallel { get; set; }

        public CSharpProjectContent ProjectContent { get; set; }
        public ICompilation Compilation { get; set; }
        public List<string> References { get; set; }
        public List<string> SourceFiles { get; set; }
        public List<NFile> NFiles { get; set; }
        public List<string> Defines { get; set; }
        public string AssemblyName { get; set; }


        public virtual void Parse()
        {
            if (References.Where(t => t.IndexOf("mscorlib.dll", StringComparison.InvariantCultureIgnoreCase) >= 0).FirstOrDefault() == null) //Don't add mscorlib twice
            {
                string mscorlib = null;
                if (!String.IsNullOrEmpty(TargetFrameworkVersion))
                {
                    if (TargetFrameworkVersion == "v2.0" ||
                        TargetFrameworkVersion == "v3.0" ||
                        TargetFrameworkVersion == "v3.5")
                        mscorlib = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"assembly\GAC_32\mscorlib\2.0.0.0__b77a5c561934e089\mscorlib.dll");
                }
                if (mscorlib == null)
                    mscorlib = typeof(string).Assembly.Location;
                References.Add(mscorlib);
            }

            NFiles = new List<NFile>();
            CSharpParser = new CSharpParser();
            if (Defines != null)
                Defines.ForEach(t => CSharpParser.CompilerSettings.ConditionalSymbols.Add(t));
            NFiles = SourceFiles.Select(t => new NFile { Filename = t, Project = this }).ToList();
            AssemblyReferences = References.Select(t => new NAssembly { Filename = t }).ToList();
            //danel: parser.ErrorPrinter

            ParseCsFiles();
            LoadAssemblies();
            var ms = StopwatchHelper.TimeInMs(LoadAssemblies);
            FormatLine("{0} References End: {1}ms", AssemblyReferences.Count, ms);
            ProjectContent = new CSharpProjectContent();
            ProjectContent = (CSharpProjectContent)ProjectContent.SetAssemblyName(AssemblyName);
            ProjectContent = (CSharpProjectContent)ProjectContent.AddAssemblyReferences(AssemblyReferences.Select(t => t.UnresolvedAssembly).Where(t => t != null));
            ProjectContent = (CSharpProjectContent)ProjectContent.AddOrUpdateFiles(NFiles.Select(t => t.UnresolvedFile));
            ProjectContent.Tag = this;
            ms = StopwatchHelper.TimeInMs(() => Compilation = ProjectContent.CreateCompilation());
            Compilation.CacheManager.SetShared(typeof(NProject), this);
            FormatLine("CreateCompilation {0}ms", ms);
            //Navigator = CreateNavigator(null);
            //ApplyNavigator will happen BeforeExport only on needed files, in parallel

        }

        private void LoadAssemblies()
        {
            AssemblyReferences.ForEachParallel(LoadAssembly, Parallel);
        }

        protected virtual void ParseCsFiles()
        {
            var ms = StopwatchHelper.TimeInMs(() => NFiles.ForEach(ParseSkFile));
            FormatLine("{0} Sources files {1}ms", NFiles.Count, ms);
        }

        List<NAssembly> AssemblyReferences;
        protected CSharpParser CSharpParser;



        void Store(CSharpAstResolver resolver, AstNode node, ResolveResult res)
        {
            var info = (ResolveResultInfo)res.Tag;
            if (info == null)
            {
                info = new ResolveResultInfo();
                res.Tag = info;
            }
            info.Resolver = resolver;
            if (node != null)
            {
                node.AddAnnotation(res);
                node.AddAnnotation(info);
                info.Nodes.Add(node);
            }
            info.ResolveResult = res;

            //HACK: Reported NRefactory issue: https://github.com/icsharpcode/NRefactory/issues/367
            //When AnonymousTypeCreateExpression Navigator methods not called on initializer statements
            if (node is AnonymousTypeCreateExpression)
            {
                var node2 = (AnonymousTypeCreateExpression)node;
                var res2 = res as InvocationResolveResult;
                if (res2 != null)
                {
                    foreach (var res3 in res2.InitializerStatements)
                    {
                        if (res3.Tag != null)
                            continue;
                        AstNode node3 = null;
                        if (res3 is OperatorResolveResult)
                        {
                            var opRes = (OperatorResolveResult)res3;
                            node3 = opRes.Operands[0].GetFirstNode();
                        }
                        Store(resolver, node3, res3);
                    }
                }
            }

        }

        DelegatedResolveVisitorNavigator CreateNavigator(CSharpAstResolver resolver)
        {
            var navigator = new DelegatedResolveVisitorNavigator();
            navigator.Resolved += (node, res) =>
            {
                Store(resolver, node, res);
            };
            navigator.ProcessConversion += (expression, res, conversion, targetType) =>
            {
                var info = (ResolveResultInfo)res.Tag;
                if (info == null)
                {
                    info = new ResolveResultInfo();
                    res.Tag = info;
                }
                info.Resolver = resolver;
                info.ResolveResult = res;
                info.Conversion = conversion;
                info.ConversionTargetType = targetType;
                if (info.Conversion != null && info.ConversionTargetType != null && info.ResolveResult != null && (!(info.ResolveResult is ConversionResolveResult)))
                {
                    info.ResolveResult = new ConversionResolveResult(info.ConversionTargetType, info.ResolveResult, info.Conversion);
                    expression.RemoveAnnotations<ResolveResult>();
                    expression.AddAnnotation(info.ResolveResult);
                }
            };
            return navigator;
        }
        public List<NFile> GetNFiles(List<ITypeDefinition> list)
        {
            var list2 = new List<NFile>();
            var set = new HashSet<IUnresolvedFile>();
            foreach (var ce in list)
            {
                foreach (var part in ce.Parts)
                {
                    var pf = part.UnresolvedFile;
                    if (pf == null) //external class in referenced assembly
                        continue;
                    if (set.Add(pf))
                    {
                        var skFile = pf.GetNFile();
                        list2.Add(skFile);
                    }
                }
            }
            return list2;
        }
        public void ApplyNavigator(List<NFile> files)
        {
            var watch = new Stopwatch();
            WriteLine("ApplyNavigator");
            watch.Start();
            files.ForEachParallel(ApplyNavigator, Parallel);
            watch.Stop();
            WriteLine("ApplyNavigator End:" + watch.ElapsedMilliseconds);
        }
        public void ApplyNavigator(NFile file)
        {
            if (file.CSharpAstResolver != null)
                return;
            try
            {
                //WriteLine("ApplyNavigator(" + file.ParsedFile.FileName + ")");
                file.CSharpAstResolver = new CSharpAstResolver(file.Project.Compilation, file.SyntaxTree, file.UnresolvedFile);
                file.CSharpAstResolver.ApplyNavigator(CreateNavigator(file.CSharpAstResolver));
            }
            catch (Exception e)
            {
                //file.Project.Compiler.Log.Debug(e.ToString());
                throw new CompilerException(file.Filename, 1, 1, "Error while applying navigator on file: " + file.Filename + " " + e);
            }
        }

        private void ParseSkFile(NFile file)
        {
            using (var fs = File.OpenRead(file.Filename))
            {
                file.SyntaxTree = CSharpParser.Parse(fs, file.Filename);
                fs.Close();
            }

            var q = new QueryExpressionExpander().ExpandQueryExpressions(file.SyntaxTree);
            if (q != null)
                file.SyntaxTree = (SyntaxTree)q.AstNode;

            file.UnresolvedFile = file.SyntaxTree.ToMappedTypeSystem();
            file.UnresolvedFile.Tag = file;
            file.SyntaxTree.AddAnnotation(file);
        }



        public NAssemblyCache AssemblyCache { get; set; }
        void LoadAssembly(NAssembly asm)
        {
            AssemblyCache.LoadAssembly(asm);
        }
        void LoadAssemblies(List<NAssembly> files)
        {
            files.ForEach(LoadAssembly);
        }


        public ITypeDefinition FindType(string name)
        {
            var ce = ReflectionHelper.ParseReflectionName(name).Resolve(Compilation.TypeResolveContext);
            //var ce = Compilation.FindType(name);
            if (ce == null)
                return null;
            return ce.GetDefinition();

        }
        public IAssembly MainAssembly
        {
            get
            {
                return Compilation.MainAssembly;
            }
        }




        protected virtual void WriteLine(object obj)
        {
            //Compiler.Log.WriteLine("{0:HH:mm:ss.fff}: {1}", DateTime.Now, obj);
        }
        protected virtual void FormatLine(string format, params object[] args)
        {
            //WriteLine(String.Format(format, args));
        }


        public string TargetFrameworkVersion { get; set; }
    }

    public class NFile
    {
        public NProject Project { get; set; }
        public string Filename { get; set; }
        public SyntaxTree SyntaxTree { get; set; }
        public CSharpUnresolvedFile UnresolvedFile { get; set; }
        public CSharpAstResolver CSharpAstResolver { get; set; }
    }

    public class NAssembly
    {
        public string Filename { get; set; }
        public IUnresolvedAssembly UnresolvedAssembly { get; set; }
    }



}
