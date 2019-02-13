using System.Collections.Generic;
using System.Diagnostics;

namespace UiPath.FlowProfiler.Activities
{
    internal static class ValueFormatterFactory
    {
        private static readonly List<string> BytesPerformanceCounters = new List<string>
        {
            "Working Set", "Working Set - Private", "Working Set Peak", "Virtual Bytes", "Virtual Bytes Peak",
            "Private Bytes", "Pool Paged Bytes", "Pool Nonpaged Bytes", "Page File Bytes", "Page File Bytes Peak",
            "I/O Data Bytes/sec", "I/O Read Bytes/sec", "I/O Write Bytes/sec"
        };

        public static ICounterFormatter Create(PerformanceCounter performanceCounter)
        {
            return Create(performanceCounter.CounterName);
        }

        public static ICounterFormatter Create(string performanceCounterName)
        {
            if (BytesPerformanceCounters.Contains(performanceCounterName))
            {
                return new BytesCounterFormatter();
            }

            return new DefaultCounterFormatter();
        }
    }
}
