namespace Enterprise.Shared.Application.WebHostService;

public class TraceSettings
{
    public const string Key = "Trace";
    public bool EnableTraceParentOnResponseHeader { get; set; }
}
