using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RxConnectSite.Hubs
{
    public class DoorsEventProcessorFactory : IEventProcessorFactory
    {
        private Action<DoorMessage> onMessage;

        public DoorsEventProcessorFactory(Action<DoorMessage> onMessage)
        {
            this.onMessage = onMessage;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new DoorsEventProcessor(onMessage);
        }

    }
}
