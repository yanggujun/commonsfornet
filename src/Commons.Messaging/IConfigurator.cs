using System;

namespace Commons.Messaging
{
    public interface IConfigurator
    {
		IMessageConfig For(Type type);
		IMessageConfig For<T>();
		IMessageConfig For(string topic);
		IMessageConfig For(byte[] bytes);
    }
}
