using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Commons.Messaging
{
    public static class Bus
    {
        public static IBusFactory Factory => new BusFactory();

        public static IBusClient Client => new BusClient();

        public static ISubscriber Subscriber => new BusSubscriber();
    }
}
