using System;
using System.Collections.Generic;

namespace Commons.Messaging
{
	public class ServiceBus : IServiceBus
    {
		private static readonly object locker = new object();
		private readonly Dictionary<string, IBus> hosts = new Dictionary<string, IBus>();
		private readonly Dictionary<string, IEndpoint> endpoints = new Dictionary<string, IEndpoint>();
		private readonly Dictionary<string, IConsumer> consumers = new Dictionary<string, IConsumer>();

        public void Host(Action<IConfigurator> config = null)
        {
			var bus = new Bus();
			bus.Start(config);
			lock (locker)
			{
				hosts.Add(Constants.DefaultServerEndpoint, bus);
			}
        }

	    public void Send(object msg)
	    {
			Send(msg, Constants.DefaultServerEndpoint);
	    }

	    public void Send(object msg, string address)
	    {
			IEndpoint endpoint;
			if (!endpoints.TryGetValue(address, out endpoint))
			{
				endpoint = new TcpEndpoint(address);
			}
			endpoint.Send(msg);
	    }

	    public void Consume<T>(Action<T> handler)
	    {
			Consume(handler, Constants.DefaultPublishEndpoint);
	    }

	    public void Consume<T>(Action<T> handler, string endpoint)
	    {
			IConsumer consumer;
			if (!consumers.TryGetValue(endpoint, out consumer))
			{
				consumer = new TcpConsumer<T>(endpoint);
			}
			var con = (IConsumer<T>)consumer;
			con.Subscribe(handler);
	    }
    }
}
