using Enterprise.Shared.Database;

namespace Location.Shared.Models;

public class DailyRoomCountRecording : EntityBaseWithDeleted
{
    public DateTimeOffset Date { get; set; }
    public int Count { get; set; }

    public Location Location { get; set; }
}
