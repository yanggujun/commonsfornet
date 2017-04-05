using System;

namespace Commons.Messaging
{
    public interface IServiceBus
    {
        void Start(Action<IConfigurator> config = null);
    }
}
