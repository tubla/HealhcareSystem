using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Hosting;
using shared.V1.Events;
using shared.V1.HelperClasses.Contracts;

namespace shared.V1.HelperClasses.EventHubs;

/// <summary>
/// Using IHostedService is a great way to go here, especially if we're aiming for async initialization at app startup.
/// </summary>

public class EventHubWarmupService(IEventHubClientFactory _factory) : IHostedService, IEventHubClientProvider
{
    private readonly Dictionary<string, EventHubProducerClient> _clients = [];

    public EventHubProducerClient GetClient(string eventName)
    {
        return _clients[eventName];
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Support multiple event hubs Producer Clients.
        var requiredEvents = new[] { EventNames.MediaDeleted, EventNames.AppointmentScheduled };

        foreach (var eventName in requiredEvents)
        {
            var client = await _factory.CreateAsync(eventName);
            _clients[eventName] = client;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

