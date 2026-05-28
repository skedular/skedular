using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class DailyDeskCountRecording : ModelBaseWithDeleted
{
    public DateTimeOffset Date { get; set; }
    public int Count { get; set; }

    public Location Location { get; set; } = new();
}
