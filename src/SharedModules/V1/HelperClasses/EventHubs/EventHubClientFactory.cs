using Azure.Messaging.EventHubs.Producer;
using shared.V1.HelperClasses.Contracts;

namespace shared.V1.HelperClasses.EventHubs;

// EventHubClientFactory is singleton and ISecretClient is scoped, so we use a delegate function
// to create the ISecretClient instance.
public class EventHubClientFactory(ISecretProvider _secretProvider) : IEventHubClientFactory
{
    public EventHubProducerClient Create(string eventName)
    {
        var connection = _secretProvider.GetSecret("EventHubConnection");
        return new EventHubProducerClient(connection, eventName);
    }
}
