using System;

namespace Commons.Messaging
{
	public interface IEndpoint : IDisposable
    {
		void Send(object message);
		string Address { get; }
    }
}
