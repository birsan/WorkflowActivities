namespace UiPath.FlowProfiler.Activities
{
    internal interface ICounterFormatter
    {
        string FormatValue(float value);

        string FormatHeader(string counterName);
    }
}
