using Azure.Messaging;
using Azure.Messaging.EventGrid;

namespace MunsonPickles.Events
{
    public class EventGridPublisher : IEventGridPublisher
    {
        // Event Grid Client
        private readonly string topicEndpoint = "https://ct-webreview-mail-we-dev.westeurope-1.eventgrid.azure.net/api/events";
        private readonly string topicKey = Environment.GetEnvironmentVariable("EVENTGRID_TOPIC_KEY") ?? "CdGOHDWgYzC86qYgrDOKhT8P1cm1v0z1gJ16pk9xNgNdIb687SwzJQQJ99BGAC5RqLJXJ3w3AAABAZEG9dTV";

        public async Task PublishEventAsync<T>(T eventData) where T : class
        {

            var credentials = new Azure.AzureKeyCredential(topicKey);

            var client = new EventGridPublisherClient(new Uri(topicEndpoint), credentials);

            var cloudEvent = new CloudEvent(
                "/products/reviews",             // subject
                "NewReviewCreated",              // eventType
                eventData                       // event data
            );

            await client.SendEventAsync(cloudEvent);
        }
    }
}
