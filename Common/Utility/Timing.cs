using System;
using System.Diagnostics;

namespace Common.Utility
{
    public class Timing : IDisposable
    {
        public static Timing Log(string metricsName)
        {
            return new Timing(metricsName);
        }

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly string _metricsName;

        private Timing(string metricsName)
        {
            _stopwatch.Start();
            _metricsName = metricsName;
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            var message = $"{_metricsName}: {_stopwatch.Elapsed.TotalMilliseconds}";
            Debug.Write(message);
        }
    }
}
