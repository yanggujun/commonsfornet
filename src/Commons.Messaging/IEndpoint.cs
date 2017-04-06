using System;

namespace Commons.Messaging
{
	public interface IEndpoint : IDisposable
    {
		string Send(object message);
		string Address { get; }
    }
}
