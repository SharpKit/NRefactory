using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.Extensions
{
    public class NAssemblyCache : IDisposable
    {
        public NAssemblyCache()
        {
            Dictionary = new ConcurrentDictionary<string, NAssemblyCacheItem>();
            IdleTimer = new Timer(t => IdleTimer_Expired());
            IdleTimeToClear = TimeSpan.FromMinutes(10);
        }

        private void IdleTimer_Expired()
        {
            Dictionary.Clear();
            GC.Collect();
        }
        Timer IdleTimer;

        ConcurrentDictionary<string, NAssemblyCacheItem> Dictionary;

        private IUnresolvedAssembly LoadAssemblyWithCache(string filename)
        {
            var key = new FileInfo(filename).FullName;
            var lastWriteTime = File.GetLastWriteTime(filename);
            var asm = Dictionary.TryGetValue(key);
            if (asm == null || asm.LastWriteTime != lastWriteTime)
            {
                asm = new NAssemblyCacheItem
                {
                    UnresolvedAssembly = LoadAssembly(filename),
                    LastWriteTime = lastWriteTime,
                    Filename = filename,
                    Key = key,
                    LastHit = DateTime.Now,
                    Hits = 1,
                };
                Put(asm);
            }
            asm.LastHit = DateTime.Now;
            asm.Hits++;

            if (IdleTimeToClear.TotalMilliseconds > 0)
                IdleTimer.Change(IdleTimeToClear, TimeSpan.FromMilliseconds(-1));

            return asm.UnresolvedAssembly;
        }

        int MaxItems = 30;
        int LeaveAfterCleanup = 20;
        public TimeSpan IdleTimeToClear { get; set; }

        private void Put(NAssemblyCacheItem asm)
        {
            Dictionary[asm.Key] = asm;
            CleanupIfOverMaxItems();
        }

        private void CleanupIfOverMaxItems()
        {
            //Cleanup
            if (Dictionary.Count <= MaxItems)
                return;
            var removeLast = Dictionary.Count - LeaveAfterCleanup;
            var remove = Dictionary.Values.OrderBy(t => t.LastHit).Take(removeLast).ToList();
            foreach (var item in remove)
            {
                NAssemblyCacheItem item2;
                Dictionary.TryRemove(item.Key, out item2);
            }
        }
        private IUnresolvedAssembly LoadAssembly(string filename)
        {
            if (filename.Contains("System.EnterpriseServices.dll"))
                return null;//HACK: exception is thrown by cecil in this assembly

            var loader = new CecilLoader();
            try
            {
                var x = loader.LoadAssemblyFile(filename);
                return x;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading assembly: " + filename + ", " + e.Message);
                return null;
            }
        }
        public void LoadAssembly(NAssembly asm)
        {
            var filename = asm.Filename;
            asm.UnresolvedAssembly = LoadAssemblyWithCache(filename);
        }


        public void Dispose()
        {
            IdleTimer.Dispose();
        }
    }


    public class NAssemblyCacheItem
    {
        public DateTime LastHit { get; set; }
        public int Hits { get; set; }
        public string Filename { get; set; }
        public string Key { get; set; }
        public IUnresolvedAssembly UnresolvedAssembly { get; set; }
        public DateTime LastWriteTime { get; set; }
    }

}
