using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace UiPath.FlowProfiler.Activities
{
    public class ProfileActivity : NativeActivity
    {
        private const string ProcessCategoryName = "Process";
        internal const string ContextItemName = "ContextTarget";

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private readonly PerformanceCounterCategory _processPerformanceCounterCategory = new PerformanceCounterCategory(ProcessCategoryName);
        private readonly Dictionary<string, float> _initialValues = new Dictionary<string, float>();
        private readonly Dictionary<string, PerformanceCounter> _performanceCounters = new Dictionary<string, PerformanceCounter>();

        private List<string> _counters;
        private string _instanceName;
        private string _id;
        private string _fileName;
        private string _outputFolder;

        [Browsable(false)]
        public ActivityAction<object> Body { get; set; }

        [Category("Advanced")]
        [Description("Specify the process instance name for which the performance counters will be retrieved.e.g. UiPath.Vision.Host32. By default the current process is used for profiling.")]
        public InArgument<string> ProcessInstanceName { get; set; }

        [Category("Input")]
        [RequiredArgument]
        [Description("A custom user value that will be associated to the results. e.g. a file name or a document id.")]
        public InArgument<string> TargetId { get; set; }

        [Category("Input")]
        [RequiredArgument]
        [DefaultValue("Working Set,Working Set Peak,Working Set - Private")]
        [Description("A list of counter names from the Process category only, separated by comma. Make sure the counters exists.")]
        public InArgument<string> Counters { get; set; } = "Working Set,Working Set Peak,Working Set - Private";

        [Category("Results Location")]
        [RequiredArgument]
        [Description("The file name without extension. The file will be saved in a csv format. Consider having dynamic names if you want to compare the results later. e.q. use different versions of the packages")]
        public InArgument<string> FileName { get; set; }

        [Category("Results Location")]
        [RequiredArgument]
        [Description("The output folder where the output file will be saved.")]
        public InArgument<string> OutputFolder { get; set; }

        [Category("Output")]
        [Description("The time of execution of the target activity/flow.")]
        public OutArgument<string> Elapsed { get; set; }

        [Category("Output")]
        [Description("The performance counters delta values for the performance counter names given as input. The order is preserved. The values are comma separated.")]
        public OutArgument<string> CountersDelta { get; set; }

        public ProfileActivity()
        {
            Body = new ActivityAction<object>
            {
                Argument = new DelegateInArgument<object>(ContextItemName),
                Handler = new Sequence
                {
                    DisplayName = "Do"
                }
            };
        }

        protected override void Execute(NativeActivityContext context)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            _counters = Counters.Get(context).Split(',').Select(c => c.Trim()).ToList();
            _id = TargetId.Get(context);
            _fileName = FileName.Get(context);
            _outputFolder = OutputFolder.Get(context);
            _instanceName = ProcessInstanceName.Get(context);
            if (string.IsNullOrWhiteSpace(_instanceName))
            {
                _instanceName = PerformanceCounterInstanceName(Process.GetCurrentProcess());
            }

            foreach (var counter in _counters.Where(c =>
                !_performanceCounters.ContainsKey(c) && _processPerformanceCounterCategory.CounterExists(c)))
            {
                try
                {
                    _performanceCounters.Add(counter,
                        new PerformanceCounter(ProcessCategoryName, counter, _instanceName));
                    _initialValues.Add(counter, _performanceCounters[counter].RawValue);
                }
                catch
                {
                    _initialValues.Add(counter, 0);
                }
            }

            _stopwatch.Restart();
            context.ScheduleAction(Body, null, OnChildActivityCompleted);
        }

        private void OnChildActivityCompleted(NativeActivityContext context, ActivityInstance instance)
        {
            _stopwatch.Stop();
            string elapsed = _stopwatch.Elapsed.ToString();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            WriteOutput(context, elapsed);
        }

        private void WriteOutput(NativeActivityContext context, string elapsed)
        {
            List<string> counterDelta = new List<string>();
            foreach (var counter in _counters.Where(c => _initialValues.ContainsKey(c)))
            {
                var delta = GetPerformanceCounter(counter, _instanceName).RawValue - _initialValues[counter];
                counterDelta.Add(ValueFormatterFactory.Create(counter).FormatValue(delta));
            }

            string countersDeltaOutput = string.Join(",", counterDelta);

            Elapsed.Set(context, elapsed);
            CountersDelta.Set(context, countersDeltaOutput);

            string filePath = Path.Combine(_outputFolder, $"{_fileName}.csv");
            if (!Directory.Exists(_outputFolder))
            {
                Directory.CreateDirectory(_outputFolder);
            }

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
                File.AppendAllLines(filePath,
                    new List<string> {$"{nameof(TargetId)},{nameof(Elapsed)},{string.Join(",", _initialValues.Keys.Select(c => ValueFormatterFactory.Create(c).FormatHeader(c)))}"});
            }

            File.AppendAllLines(filePath, new List<string> {$"{_id},{elapsed},{countersDeltaOutput}"});
        }

        public static string PerformanceCounterInstanceName(Process process)
        {
            var matchesProcessId = new Func<string, bool>(instanceName =>
            {
                using (var pc = new PerformanceCounter(ProcessCategoryName, "ID Process", instanceName, true))
                {
                    if ((int) pc.RawValue == process.Id)
                    {
                        return true;
                    }
                }

                return false;
            });

            string processName = Path.GetFileNameWithoutExtension(process.ProcessName);
            return new PerformanceCounterCategory(ProcessCategoryName)
                .GetInstanceNames()
                .AsParallel()
                .FirstOrDefault(instanceName => instanceName.StartsWith(processName)
                                                && matchesProcessId(instanceName));
        }

        private PerformanceCounter GetPerformanceCounter(string counterName, string instanceName)
        {
            if (_performanceCounters.ContainsKey(counterName) && _performanceCounters[counterName] != null)
            {
                return _performanceCounters[counterName];
            }

            if (!_processPerformanceCounterCategory.CounterExists(counterName))
            {
                return null;
            }

            return new PerformanceCounter(ProcessCategoryName, counterName, instanceName);
        }
    }
}
