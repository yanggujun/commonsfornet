using System;

namespace Commons.Messaging
{
    public interface IServiceBus
    {
		/// <summary>
		/// Host the service bus
		/// </summary>
		/// <param name="config">The service bus configuration</param>
        void Host(Action<IConfigurator> config = null);

		void Send(object msg);

		void Send(object msg, string endpoint);

		void Consume<T>(Action<T> handler);

		void Consume<T>(Action<T> handler, string endpoint);

    }
}
