using System;

namespace Commons.Messaging
{
	public interface IBus
    {
		void Start();
		void Shutdown();
    }
}
