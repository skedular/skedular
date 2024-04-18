namespace Enterprise.Shared.Time;

public interface ITimeHelper
{
    Task RandomSleepWhileStartingUpAsync(CancellationToken cancellationToken);
}

public class TimeHelper(System.Random random) : ITimeHelper
{
    public async Task RandomSleepWhileStartingUpAsync(CancellationToken cancellationToken) =>
        await Task.Delay(TimeSpan.FromSeconds(random.Next(30, 60)), cancellationToken);
}
