using System;

namespace Commons.Messaging
{
	public interface IBus
    {
		void Start(Action<IConfigurator> config);
		void Shutdown();
    }
}
