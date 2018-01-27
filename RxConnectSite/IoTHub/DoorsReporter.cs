using RxConnectSite.Models;
using RxConnectSite.Observers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RxConnectSite.IoTHub
{
    public class DoorsReporter : IObservable<DoorMessage>, IDisposable
    {
        string _doorId;
        public DoorsReporter(string doorId)
        {
            _doorId = doorId;
        }

        public void OnMessage(DoorMessage message)
        {
            if (String.Compare( message.DoorId , _doorId, true)==0 || message.DoorId==null)
            {
                foreach (var observer in _observers)
                {
                    try
                    {
                        observer.OnNext(message);
                    }
                    catch(Exception ex)
                    {
                        observer.OnError(ex);
                    }
                }
            }
        }
        
        List<IObserver<DoorMessage>> _observers = new List<IObserver<DoorMessage>>();
        public IDisposable Subscribe(IObserver<DoorMessage> observer)
        {
            _observers.Add(observer);
            return new Unsubscriber<DoorMessage>(_observers, observer);
        }

        public void Dispose()
        {
            foreach(var observer in _observers)
            {
                observer.OnCompleted();
            }
            _observers.Clear();
        }
    }
}
