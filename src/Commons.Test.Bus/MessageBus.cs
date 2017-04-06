using System;
using Commons.Messaging;

namespace Commons.Test.Bus
{
    public class MessageBus
    {
        public static void Main(string[] args)
        {
            var sb = new ServiceBus();
            sb.Host();
            Console.WriteLine("MessageBus Started");
            Console.ReadLine();
        }
    }
}
