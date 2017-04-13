using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Commons.Messaging
{
    public interface IBusFactory
    {
		IBus CreateHost(Action<IConfigurator> configure);
    }
}
