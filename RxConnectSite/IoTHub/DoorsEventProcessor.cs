using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.DependencyInjection;
using RxConnectSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxConnectSite.IoTHub
{
    public class DoorsEventProcessor : IEventProcessor
    {
        private Action<DoorMessage> _onMessage;

        public DoorsEventProcessor(Action<DoorMessage> onMessage)
        {
            _onMessage = onMessage;
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
            //TODO: use logger and send error to observers
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
                    var deviceId = message.Properties["iothub-connection-device-id"].ToString();
                    this._onMessage(new DoorMessage { Id = message.SystemProperties.SequenceNumber, State = data, DoorId = deviceId, Enqueued = message.SystemProperties.EnqueuedTimeUtc });
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
