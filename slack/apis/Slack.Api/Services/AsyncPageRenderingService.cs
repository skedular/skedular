using System.Reactive.Subjects;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;

namespace Slack.Api.Services;

public interface IAsyncPageRenderingCallbacks
{
    Task HandleAsync(AppHomeOpened appHomeOpenedEvent, CancellationToken cancellationToken) => Task.CompletedTask;

    Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    Task HandleAsync(DatePickerAction action, BlockActionRequest request, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    Task HandleAsync(StaticSelectAction action, BlockActionRequest request, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    Task HandleAsync(CheckboxGroupAction action, BlockActionRequest request, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    Task HandleAsync(ChannelSelectAction action, BlockActionRequest request, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

public class AsyncPageRenderingService(
    ILogger<AsyncPageRenderingService> logger,
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime)
    : BackgroundService
{
    public Subject<(Type, AppHomeOpened)> EventHandlerStream { get; } = new();
    public Subject<(Type, ButtonAction, BlockActionRequest)> ButtonActionHandlerStream { get; } = new();
    public Subject<(Type, DatePickerAction, BlockActionRequest)> DatePickerActionHandlerStream { get; } = new();
    public Subject<(Type, StaticSelectAction, BlockActionRequest)> StaticSelectActionHandlerStream { get; } = new();
    public Subject<(Type, CheckboxGroupAction, BlockActionRequest)> CheckboxGroupActionHandlerStream { get; } = new();
    public Subject<(Type, ChannelSelectAction, BlockActionRequest)> ChannelSelectActionHandlerStream { get; } = new();

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        EventHandlerStream.Dispose();
        ButtonActionHandlerStream.Dispose();
        DatePickerActionHandlerStream.Dispose();
        StaticSelectActionHandlerStream.Dispose();
        CheckboxGroupActionHandlerStream.Dispose();
        ChannelSelectActionHandlerStream.Dispose();

        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var eventHandlerSubscription = EventHandlerStream.Subscribe(value => Task.Run(async () =>
                {
                    await using var scope = serviceProvider.CreateAsyncScope();
                    var service =
                        scope.ServiceProvider.GetRequiredService(value.Item1) as IAsyncPageRenderingCallbacks;
                    ArgumentNullException.ThrowIfNull(service);
                    await service.HandleAsync(value.Item2, cancellationToken);
                },
                cancellationToken));

            using var buttonActionHandlerSubscription = ButtonActionHandlerStream.Subscribe(value => Task.Run(async () =>
                {
                    await using var scope = serviceProvider.CreateAsyncScope();
                    var service =
                        scope.ServiceProvider.GetRequiredService(value.Item1) as IAsyncPageRenderingCallbacks;
                    ArgumentNullException.ThrowIfNull(service);
                    await service.HandleAsync(value.Item2, value.Item3, cancellationToken);
                },
                cancellationToken));

            using var datePickerActionHandlerSubscription = DatePickerActionHandlerStream.Subscribe(value => Task.Run(async () =>
                {
                    await using var scope = serviceProvider.CreateAsyncScope();
                    var service =
                        scope.ServiceProvider.GetRequiredService(value.Item1) as IAsyncPageRenderingCallbacks;
                    ArgumentNullException.ThrowIfNull(service);
                    await service.HandleAsync(value.Item2, value.Item3, cancellationToken);
                },
                cancellationToken));

            using var staticSelectActionHandlerSubscription = StaticSelectActionHandlerStream.Subscribe(value => Task.Run(async () =>
                {
                    await using var scope = serviceProvider.CreateAsyncScope();
                    var service =
                        scope.ServiceProvider.GetRequiredService(value.Item1) as IAsyncPageRenderingCallbacks;
                    ArgumentNullException.ThrowIfNull(service);
                    await service.HandleAsync(value.Item2, value.Item3, cancellationToken);
                },
                cancellationToken));

            using var checkboxGroupActionHandlerSubscription = CheckboxGroupActionHandlerStream.Subscribe(value => Task.Run(async () =>
                {
                    await using var scope = serviceProvider.CreateAsyncScope();
                    var service =
                        scope.ServiceProvider.GetRequiredService(value.Item1) as IAsyncPageRenderingCallbacks;
                    ArgumentNullException.ThrowIfNull(service);
                    await service.HandleAsync(value.Item2, value.Item3, cancellationToken);
                },
                cancellationToken));

            using var channelSelectActionHandlerSubscription = ChannelSelectActionHandlerStream.Subscribe(value => Task.Run(async () =>
                {
                    await using var scope = serviceProvider.CreateAsyncScope();
                    var service =
                        scope.ServiceProvider.GetRequiredService(value.Item1) as IAsyncPageRenderingCallbacks;
                    ArgumentNullException.ThrowIfNull(service);
                    await service.HandleAsync(value.Item2, value.Item3, cancellationToken);
                },
                cancellationToken));

            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process async Slack request");
            hostApplicationLifetime.StopApplication();
        }
    }
}
