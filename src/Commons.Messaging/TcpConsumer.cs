using System;
using System.Collections.Generic;
using System.Threading;
using Commons.Json;
using NetMQ;
using NetMQ.Sockets;

namespace Commons.Messaging
{
    public class TcpConsumer<T> : IConsumer<T>
    {
        private readonly SubscriberSocket sub;
        private readonly Thread subThread;
        private readonly List<Action<T>> pipe;

        public TcpConsumer(string address)
        {
            Address = address;
            sub = new SubscriberSocket();
            sub.Connect(Address);
            var type = typeof(T).AssemblyQualifiedName;
            sub.Subscribe(type);
            subThread = new Thread(Consume);
            subThread.Start();
            pipe = new List<Action<T>>();
        }

        public void Register(Action<T> handler)
        {
            pipe.Add(handler);
        }

        private void Consume()
        {
            while (true)
            {
                bool more;
                var msgType = sub.ReceiveFrameString(out more);
                if (!more)
                {
                    // error
                    break;
                }

                var msg =sub.ReceiveFrameString(out more);
                if (more)
                {
                    // error
                    break;
                }

                var type = Type.GetType(msgType);
                if (type == null)
                {
                    //error
                    break;
                }

                var message = (T)JsonMapper.To(type, msg);
                for (var i = 0; i < pipe.Count; i++)
                {
                    var h = pipe[i];
                    h(message);
                }
            }
        }

        public void Dispose()
        {
            sub.Close();
        }

        public string Address { get; }
    }
}
