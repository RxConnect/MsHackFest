using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;
using Microsoft.Azure.Devices.Common;
using System.Text;
using System.Collections.Generic;
using Microsoft.Azure.EventHubs;
using System.Threading;
using System.Collections;

namespace RxConnectSite.Hubs
{
    public class Doors : Hub
    {
        ILogger<Doors> _logger;
        string activeIoTHubConnectionString;
        string consumerGroupName;
        string ehConnectionString;
        string storageConnectionString;
        string hubName;
          

        public Doors(IConfiguration configuration, ILogger<Doors> logger){
            _logger=logger;
            hubName= configuration["IoTHub:Name"];
            activeIoTHubConnectionString = configuration["IoTHub:ConnectionString"];
            consumerGroupName = configuration["IoTHub:ConsumerGroup"];
            ehConnectionString=configuration["IoTHub:EventHubConnectionString"];
            storageConnectionString= configuration["IoTHub:StorageConnectionString"];
        }

        public Task Send(string message)
        {
            return Clients.All.InvokeAsync("Send", message + " to the world!");
        }
        public IObservable<DoorMessage> StreamDoors(string doorId)
        {
            createListener().Wait();
            if (!observables.ContainsKey(doorId))
            {
                factorySemaphore.Wait();
                try
                {
                    if (!observables.ContainsKey(doorId))
                    {
                        observables.Add(doorId, new DoorsObservable(doorId));
                    }
                }
                finally
                {
                    factorySemaphore.Release();
                }
            }
            return observables[doorId];
        }

        static Dictionary<string, DoorsObservable> observables = new Dictionary<string, DoorsObservable>();
        static DoorsEventProcessorFactory factory;
        static SemaphoreSlim factorySemaphore = new SemaphoreSlim(1);
        private async Task createListener()
        {
            try
            {
                if (factory == null)
                {
                    factorySemaphore.Wait();
                    try
                    {
                        if (factory == null)
                        {
                            factory = new DoorsEventProcessorFactory((message) =>
                            {
                                foreach (var door in observables.Keys)
                                {
                                    observables[door].OnMessage(message);
                                }
                            });
                            await DoorsEventProcessor.AttachProcessorForHub("webapp", ehConnectionString, storageConnectionString, hubName,
                                consumerGroupName, factory);
                        }
                    }
                    finally
                    {
                        factorySemaphore.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        //static async Task<IObservable<EventData>> getPartitionObserver(string doorId)
        //{
        //    var eventHubReceiver = await getReceiver(doorId);
        //    if(!partitionObservers.ContainsKey(eventHubReceiver.PartitionId))
        //    {
        //        partitionObservers[eventHubReceiver.PartitionId]= Observable.Create(async (IObserver<EventData> observer)=>
        //        {
        //            while (true)
        //            {
        //                var receivedEvents = await eventHubReceiver.ReceiveAsync(100, TimeSpan.FromSeconds(1));
        //                if (receivedEvents != null)
        //                {
        //                    foreach (var eventData in receivedEvents)
        //                    {
        //                        observer.OnNext(eventData);
        //                    }
        //                }
        //            }
        //        });
        //    }
        //    return partitionObservers[eventHubReceiver.PartitionId];
        //}

        //private static async Task<IObservable<DoorMessage>> getObserver(string doorId)
        //{
        //    if (!observers.ContainsKey(doorId))
        //    {
        //        await semaphore.WaitAsync();
        //        try
        //        {
        //            if (!observers.ContainsKey(doorId))
        //            {
        //                var partobs = await getPartitionObserver(doorId);
        //                using(var a = partobs.Subscribe(data =>
        //                {

        //                }))

        //                var doorObserver = Observable.Create(
        //                     (IObserver<DoorMessage> observer) =>
        //                    {
        //                        partobs.
        //                        partobs.Subscribe(eventData => {
        //                                var data = getEvent(eventData, doorId);
        //                                if (data != null)
        //                                {
        //                                    observer.OnNext(data);
        //                                }

        //                        });
        //                    });
        //                observers.Add(doorId, doorObserver);
        //            }
        //            return observers[doorId];
        //        }
        //        finally
        //        {
        //            semaphore.Release();
        //        }
        //    }
        //    return observers[doorId];
        //}

        //static SemaphoreSlim semaphore = new SemaphoreSlim(1);
        //static EventHubClient eventHubClient;
        //private static async Task<PartitionReceiver> getReceiver(string doorId)
        //{
        //    if (eventHubClient == null)
        //    {
        //        eventHubClient = EventHubClient.CreateFromConnectionString($"{ehConnectionString};EntityPath=jcmlabf7e9a");
        //    }
        //    eventHubPartitionsCount = (await eventHubClient.GetRuntimeInformationAsync()).PartitionCount;
        //    string partition = EventHubPartitionKeyResolver.ResolveToPartition(doorId, eventHubPartitionsCount);
        //    if (!eventHubReceivers.ContainsKey(partition))
        //    {
        //        eventHubReceivers.Add(partition, eventHubClient.CreateReceiver(consumerGroupName, partition, DateTime.Now));
        //    }
        //    return eventHubReceivers[partition];
        //}

    }
}