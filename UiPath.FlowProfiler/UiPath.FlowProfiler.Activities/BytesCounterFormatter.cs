namespace UiPath.FlowProfiler.Activities
{
    internal class BytesCounterFormatter : ICounterFormatter
    {
        public string FormatValue(float value)
        {
            return $"{value / 1024 / 1024}";
        }

        public string FormatHeader(string counterName)
        {
            return $"{counterName} in MB (delta)";
        }
    }
}
