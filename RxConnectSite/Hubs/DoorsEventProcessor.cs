using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxConnectSite.Hubs
{
    public class DoorsEventProcessor : IEventProcessor
    {
        private Action<DoorMessage> onItem;

        public DoorsEventProcessor(Action<DoorMessage> onItem)
        {
            this.onItem = onItem;
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            return Task.FromResult(false);
        }

        public Task OpenAsync(PartitionContext context)
        {
            return Task.FromResult(false);
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            //TODO: use logger
            Console.WriteLine(error.ToString());
            return Task.FromResult(true);
        }
        int counter;
        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var message in messages)
            {
                //ignore old messages
                if (DateTime.UtcNow - message.SystemProperties.EnqueuedTimeUtc < TimeSpan.FromSeconds(20))
                {
                    var data = Encoding.UTF8.GetString(message.Body.ToArray());
                    var connectionDeviceId = message.Properties["iothub-connection-device-id"].ToString();
                    this.onItem(new DoorMessage { Id = message.SystemProperties.SequenceNumber, State = data, DoorId = connectionDeviceId, Enqueued = message.SystemProperties.EnqueuedTimeUtc });
                }
            }
            if (counter++ % 50 == 0)
            {
                await context.CheckpointAsync();
            }
        }

        public static async Task<EventProcessorHost> AttachProcessorForHub(
    string processorName,
    string serviceBusConnectionString,
    string offsetStorageConnectionString,
    string eventHubName,
    string consumerGroupName,
    IEventProcessorFactory processorFactory)
        {
            var eventProcessorHost = new EventProcessorHost( eventHubName, consumerGroupName, serviceBusConnectionString, offsetStorageConnectionString,processorName);
            await eventProcessorHost.RegisterEventProcessorFactoryAsync(processorFactory);

            return eventProcessorHost;
        }
    }
}
