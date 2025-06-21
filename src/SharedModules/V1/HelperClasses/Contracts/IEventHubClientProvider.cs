using Azure.Messaging.EventHubs.Producer;

namespace shared.V1.HelperClasses.Contracts;

public interface IEventHubClientProvider
{
    EventHubProducerClient GetClient(string eventName);
}
