using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RxConnectSite.Observers
{
    public class Unsubscriber<T> : IDisposable
    {
        ICollection<IObserver<T>> _observers;
        IObserver<T> _observer;
        public Unsubscriber(ICollection<IObserver<T>> observers, IObserver<T> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
            {
                _observers.Remove(_observer);
            }
        }
    }
}
