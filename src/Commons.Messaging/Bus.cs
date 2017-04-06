using System;
using System.Threading;
using Commons.Collections.Queue;
using NetMQ;
using NetMQ.Sockets;

namespace Commons.Messaging
{
	public class Bus : IBus
    {
		private string serverEndPoint = Constants.DefaultServerEndpoint;
		private string publishEndPiont = Constants.DefaultPublishEndpoint;

		private Thread recvThread;
		private MemQueue<Tuple<string, string>> innerq;
		private ResponseSocket server;
		private PublisherSocket publisher;

	    public void Start(Action<IConfigurator> config)
	    {
			innerq = new MemQueue<Tuple<string, string>>("inner", Publish);
			server = new ResponseSocket();
			publisher = new PublisherSocket();

			server.Bind(serverEndPoint);
			publisher.Bind(publishEndPiont);

			recvThread = new Thread(Recv);
			recvThread.Start();
	    }

	    public void Shutdown()
	    {
			innerq.Close();
			//server.Close();
			//publisher.Close();
	    }

	    private void Recv()
        {
            while(true)
			{
				bool more;
				var msgType = server.ReceiveFrameString(out more);
				if (more)
				{
					var msg = server.ReceiveFrameString(out more);
					innerq.Enqueue(new Tuple<string, string>(msgType, msg));
					server.SendFrame("ACK");
				}
				else
				{
					server.SendFrame("NACK");
				}
			}
        }

		private void Publish(Tuple<string, string> message)
		{
			publisher.SendMoreFrame(message.Item1).SendFrame(message.Item2);
		}
    }
}
