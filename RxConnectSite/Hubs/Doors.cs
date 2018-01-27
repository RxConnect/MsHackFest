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
using RxConnectSite.Models;
using RxConnectSite.IoTHub;

namespace RxConnectSite.Hubs
{
    public class Doors : Hub
    {
        static Dictionary<string, DoorsReporter> _reporters = new Dictionary<string, DoorsReporter>();
        ILogger<Doors> _logger;
        string activeIoTHubConnectionString;
        string consumerGroupName;
        string ehConnectionString;
        string storageConnectionString;
        string hubName;
          

        public Doors(IConfiguration configuration, ILogger<Doors> logger){
            _logger=logger;
            hubName= configuration["IoTHub:Name"];
            if (string.IsNullOrEmpty(hubName))
            {
                throw new ArgumentNullException("IoTHub:Name");
            }
            activeIoTHubConnectionString = configuration["IoTHub:ConnectionString"];
            if (string.IsNullOrEmpty(activeIoTHubConnectionString))
            {
                throw new ArgumentNullException("IoTHub:ConnectionString");
            }
            consumerGroupName = configuration["IoTHub:ConsumerGroup"];
            if (string.IsNullOrEmpty(consumerGroupName))
            {
                throw new ArgumentNullException("IoTHub:ConsumerGroup");
            }
            ehConnectionString =configuration["IoTHub:EventHubConnectionString"];
            if (string.IsNullOrEmpty(ehConnectionString))
            {
                throw new ArgumentNullException("IoTHub:EventHubConnectionString");
            }
            storageConnectionString = configuration["IoTHub:StorageConnectionString"];
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new ArgumentNullException("IoTHub:StorageConnectionString");
            }
        }

        public Task Send(string message)
        {
            return Clients.All.InvokeAsync("Send", message + " to the world!");
        }
        public IObservable<DoorMessage> StreamDoors(string doorId)
        {
            createListener().Wait();
            if (!_reporters.ContainsKey(doorId))
            {
                factorySemaphore.Wait();
                try
                {
                    if (!_reporters.ContainsKey(doorId))
                    {
                        _reporters.Add(doorId, new DoorsReporter(doorId));
                    }
                }
                finally
                {
                    factorySemaphore.Release();
                }
            }
            return _reporters[doorId];
        }

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
                            var webId = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");
                            if (string.IsNullOrEmpty(webId))
                            {
                                webId = Guid.NewGuid().ToString("N");
                            }

                            //max leaseId length is 63
                            if(webId.Length>60)
                                webId = webId.Substring(0, 60);

                            factory = new DoorsEventProcessorFactory((message) =>
                            {
                                foreach (var door in _reporters.Keys)
                                {
                                    _reporters[door].OnMessage(message);
                                }
                            });
                            await DoorsEventProcessor.AttachProcessorForHub(webId, ehConnectionString, storageConnectionString, hubName,
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
    }
}