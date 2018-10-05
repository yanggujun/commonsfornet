using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Commons.Messaging
{
    public class BusConfigurator : IConfigurator
    {
        public IMessageConfig For(Type type)
        {
            throw new NotImplementedException();
        }

        public IMessageConfig For<T>()
        {
            throw new NotImplementedException();
        }

        public IMessageConfig For(string topic)
        {
            throw new NotImplementedException();
        }

        public IMessageConfig For(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
