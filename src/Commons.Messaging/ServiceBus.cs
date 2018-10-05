using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Commons.Messaging
{
    public class ServiceBus : IServiceBus
    {
        private readonly ConcurrentDictionary<string, IEndpoint> endpoints = new ConcurrentDictionary<string, IEndpoint>();
        private static readonly object locker = new object();
        public void Host(Action<IConfigurator> config = null)
        {
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
                endpoints[address] = endpoint;
            }
            lock (locker)
            {
                endpoint.Send(msg);
            }

        }

        public void Consume<T>(Action<T> handler)
        {
            Consume(handler, Constants.DefaultPublishEndpoint);
        }

        public void Consume<T>(Action<T> handler, string endpoint)
        {
            IConsumer<T> consumer = new TcpConsumer<T>(endpoint);
            consumer.Register(handler);
        }
    }
}
