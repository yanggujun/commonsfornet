using System;
using System.Threading;
using Commons.Json;
using NetMQ;
using NetMQ.Sockets;

namespace Commons.Messaging
{
	public class TcpConsumer<T> : IConsumer<T>
    {
		private readonly SubscriberSocket sub;
		private Thread subThread;
		private Action<T> handler;
		public TcpConsumer(string address)
		{
			sub = new SubscriberSocket(address);
		}
	    public void Subscribe(Action<T> handler)
	    {
			var type = typeof(T).FullName;
			sub.Subscribe(type);
			this.handler = handler;
			subThread = new Thread(Consume);
			subThread.Start();
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
				handler(message);
			}
		}

	    public void Dispose()
	    {
			sub.Close();
	    }
    }
}
