
using System;

namespace Commons.MemQueue
{
    internal class QueueDescriptor<T> : IQueueDescriptor<T>
    {
        public IQueueDescriptor<T> AddHandler(IMessageHandler<T> handler)
        {
            throw new NotImplementedException();
        }

        public IQueueDescriptor<T> AddHandler(Action<T> handle)
        {
            throw new NotImplementedException();
        }

        public IQueueDescriptor<T> CreateQueue(string name)
        {
            throw new NotImplementedException();
        }
    }
}
