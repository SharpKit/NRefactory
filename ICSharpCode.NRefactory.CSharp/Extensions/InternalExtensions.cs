using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ICSharpCode.NRefactory.Extensions
{
    static class InternalExtensions
    {
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key)
        {
            TValue value;
            dic.TryGetValue(key, out value);
            return value;
        }
        public static bool Parallel = true;
        public static Action ParallelPreAction { get; set; }
        public static void ForEachParallel<T>(this IEnumerable<T> items, Action<T> action, bool parallel)
        {
            if (parallel)
            {
                Action<T> action2 = action;
                if (ParallelPreAction != null)
                    action2 = t => { ParallelPreAction(); action(t); };
                items.AsParallel().ForAll(action2);
            }
            else
                items.ToList().ForEach(action);
        }
        public static void ForEachParallel<T>(this IEnumerable<T> items, Action<T> action)
        {
            items.ForEachParallel(action, Parallel);
        }

    }


    class StopwatchHelper
    {
        [DebuggerStepThrough]
        public static long TimeInMs(Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }


    }

}
