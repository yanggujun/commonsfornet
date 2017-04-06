using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Commons.Json;
using NetMQ;
using NetMQ.Sockets;

namespace Commons.Messaging
{
	public class TcpConsumer<T> : IConsumer<T>
    {
		private readonly SubscriberSocket sub;
		private readonly Task subThread;
        private readonly List<Action<T>> pipe;
		public TcpConsumer(string address)
		{
			sub = new SubscriberSocket(address);
			var type = typeof(T).FullName;
			sub.Subscribe(type);
			subThread = new Task(Consume);
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
				string msgType, msg;
				bool more;
				var received = sub.TryReceiveFrameString(out msgType, out more);
				if (!received)
				{
					break;
				}
				if (!more)
				{
					// error
					break;
				}

				received = sub.TryReceiveFrameString(out msg, out more);
				if (!received)
				{
					break;
				}
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
    }
}
