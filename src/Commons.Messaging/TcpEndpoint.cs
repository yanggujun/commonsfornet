using Commons.Json;
using NetMQ;
using NetMQ.Sockets;

namespace Commons.Messaging
{
	public class TcpEndpoint : IEndpoint
    {
		private readonly RequestSocket req;

		public TcpEndpoint(string address)
		{
			Address = address;
			req = new RequestSocket();
            req.Connect(address);
		}

	    public string Send(object message)
	    {
			var type = message.GetType().AssemblyQualifiedName;
			var json = JsonMapper.ToJson(message);
			req.SendFrame(type, true);
			req.SendFrame(json);
            var resp = req.ReceiveFrameString();
            return resp;
	    }

	    public string Address { get; }

	    public void Dispose()
	    {
			req.Close();
	    }
    }
}
