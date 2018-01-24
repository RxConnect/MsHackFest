using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;

namespace WebApp.Hubs
{
    public class Doors : Hub
    {
        public class DoorLog{
            public string ID {get;set;}
            public string State {get;set;}
        }

         ServiceClient serviceClient;
         ILogger<Doors> _logger;
        public Doors(IConfiguration configuration, ILogger<Doors> logger){
            _logger=logger;
            string connectionString=configuration["ConnectionStrings:IoTHubConnectionString"];
            try{
                serviceClient= ServiceClient.CreateFromConnectionString(connectionString, TransportType.Amqp);
            }
            catch(Exception ex){
                _logger.LogError(ex.ToString());
            }
        }

        public Task Send(string message)
        {
            return Clients.All.InvokeAsync("Send", message + " to the world!");
        }

        public IObservable<DoorLog> StreamDoors(string doorId){
            return Observable.Create(
                async (IObserver<DoorLog> observer) =>
                {
                    int counter=0;
                    while (true)
                    {
                       var doorLog= new DoorLog(){ID=(counter++).ToString(), State=doorId};
                       observer.OnNext(doorLog);
                        await Task.Delay(1000);
                    }
                });
        }
    }
}