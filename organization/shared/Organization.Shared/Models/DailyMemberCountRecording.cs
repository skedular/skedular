using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class DailyMemberCountRecording : ModelBaseWithDeleted
{
    public DateTimeOffset Date { get; set; }
    public int Count { get; set; }

    public Organization Organization { get; set; } = new();
}
