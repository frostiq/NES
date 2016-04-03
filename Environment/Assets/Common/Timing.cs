using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Common.Utility
{
    public class Timing : IDisposable
    {
        public static Timing Log(string metricsName)
        {
            return new Timing(metricsName + ": ");
        }

        public static Timing Log()
        {
            return new Timing(string.Empty);
        }

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly string _prefix;

        private Timing(string prefix)
        {
            _stopwatch.Start();
            _prefix = prefix;
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            var message = _prefix + _stopwatch.Elapsed.TotalMilliseconds;
            Debug.Log(message);
        }
    }
}
