using System.Globalization;

namespace UiPath.FlowProfiler.Activities
{
    internal class DefaultCounterFormatter : ICounterFormatter
    {
        public string FormatValue(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public string FormatHeader(string counterName)
        {
            return $"{counterName} (delta)";
        }
    }
}
