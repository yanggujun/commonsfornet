using System;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;

namespace Commons.Messaging
{
    public class ServiceBus : IServiceBus
    {
        private string serverEndPoint = "tcp://*:5011";
        private string publishEndPiont = "tcp://*:5012";
        private string msgType;
        private string msg;
        private AutoResetEvent received = new AutoResetEvent(false);

        public void Start(Action<IConfigurator> config = null)
        {
        }

        private void Recv()
        {
            using (var server = new ResponseSocket())
            {
                server.Bind(serverEndPoint);
                while(true)
                {
                    bool more;
                    msgType = server.ReceiveFrameString(out more);
                    if (more)
                    {
                        msg = server.ReceiveFrameString(out more);
                        received.Set();
                        server.SendFrame("ACK");
                    }
                    else
                    {
                        server.SendFrame("NACK");
                    }
                }
            }
        }

        private void Pub()
        {
            using (var pub = new PublisherSocket())
            {
                pub.Bind(publishEndPiont);
                while (received.WaitOne())
                {
                    pub.SendMoreFrame(msgType).SendFrame(msg);
                }
            }
        }
    }
}
