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

namespace WebApp.Hubs
{
    public class Doors : Hub
    {
        public class DoorLog{
            public int ID {get;set;}
            public string State {get;set;}
        }

         ServiceClient _serviceClient;
         ILogger<Doors> _logger;
        string activeIoTHubConnectionString;
        string consumerGroupName;
        string ehConnectionString;
        int eventHubPartitionsCount;

        public Doors(IConfiguration configuration, ILogger<Doors> logger){
            _logger=logger;
            activeIoTHubConnectionString = configuration["IoTHub:ConnectionString"];
            consumerGroupName = configuration["IoTHub:ConsumerGroup"];
            ehConnectionString=configuration["IoTHub:EventHubConnectionString"];

            try
            {
                _serviceClient= ServiceClient.CreateFromConnectionString(activeIoTHubConnectionString, Microsoft.Azure.Devices.TransportType.Amqp);
            }
            catch(Exception ex){
                _logger.LogError(ex.ToString());
            }
        }

        public Task Send(string message)
        {
            return Clients.All.InvokeAsync("Send", message + " to the world!");
        }
        public IObservable<DoorLog> StreamDoors(string doorId)
        {
            return Observable.Create(
            async (IObserver<DoorLog> observer) =>
            {
                PartitionReceiver eventHubReceiver = null;
                try
                {
                    EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString($"{ehConnectionString};EntityPath=jcmlabf7e9a");
                    eventHubPartitionsCount = (await eventHubClient.GetRuntimeInformationAsync()).PartitionCount;
                    string partition = EventHubPartitionKeyResolver.ResolveToPartition(doorId, eventHubPartitionsCount);
                    eventHubReceiver = eventHubClient.CreateReceiver(consumerGroupName, partition, DateTime.Now);
                    var events = await eventHubReceiver.ReceiveAsync(int.MaxValue, TimeSpan.FromSeconds(20));
                    List<DoorLog> initialLog = new List<DoorLog>();
                    if (events != null)
                    {
                        foreach (var eventData in events)
                        {
                            var data = getEvent(eventData, doorId);
                            if (data != null)
                            {
                                initialLog.Add(data);
                            }
                        }
                    }
                    observer.OnNext(new DoorLog { ID = 0, State = "value" });
                    foreach (var d in initialLog)
                    {
                        observer.OnNext(d);
                    }
                    while (true)
                    {
                        var receivedEvents = await eventHubReceiver.ReceiveAsync(100, TimeSpan.FromSeconds(1));
                        if (receivedEvents != null)
                        {
                            foreach (var eventData in receivedEvents)
                            {
                                var data = getEvent(eventData, doorId);
                                observer.OnNext(data);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            });
        }

        static int counter = 0;
        private static DoorLog getEvent(EventData eventData, string doorId)
        {
            
            var data = Encoding.UTF8.GetString(eventData.Body.ToArray());
            var connectionDeviceId = eventData.Properties["iothub-connection-device-id"].ToString();
            if (connectionDeviceId == doorId)
            {
                return new DoorLog { ID = counter++, State = data };
            }
            else
                return null;
        }
    }
}