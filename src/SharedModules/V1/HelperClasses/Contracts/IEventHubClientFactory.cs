using Azure.Messaging.EventHubs.Producer;

namespace shared.V1.HelperClasses.Contracts
{
    public interface IEventHubClientFactory
    {
        Task<EventHubProducerClient> CreateAsync(string eventName);
    }
}