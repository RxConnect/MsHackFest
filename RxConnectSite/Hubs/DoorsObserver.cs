using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RxConnectSite.Hubs
{
    public class DoorsObservable : IObservable<DoorMessage>
    {
        string _doorId;
        public DoorsObservable(string doorId)
        {
            _doorId = doorId;
        }

        public void OnMessage(DoorMessage message)
        {
            if (String.Compare( message.DoorId , _doorId, true)==0)
            {
                foreach (var observer in _observers)
                {
                    observer.OnNext(message);
                }
            }
        }

        class autoRemove : IDisposable
        {
            DoorsObservable _main;
            IObserver<DoorMessage> _observer;
            public autoRemove(DoorsObservable main, IObserver<DoorMessage> observer)
            {
                _main = main;
                _observer = observer;
            }

            public void Dispose()
            {
                _main._observers.Remove(_observer);
            }
        }
        List<IObserver<DoorMessage>> _observers = new List<IObserver<DoorMessage>>();
        public IDisposable Subscribe(IObserver<DoorMessage> observer)
        {
            _observers.Add(observer);
            return new autoRemove(this, observer);
        }


    }
}
