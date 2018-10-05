using System;

namespace Commons.Messaging
{
    public class BusFactory : IBusFactory
    {
        public IBus CreateHost(Action<IConfigurator> configure)
        {
            throw new NotImplementedException();
        }
    }
}
