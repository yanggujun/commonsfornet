using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Commons.Messaging;

namespace Commons.Test.MessageServer
{
    public class Server
    {
        public static void Main(string[] args)
        {
            var serviceBus = new ServiceBus();
            serviceBus.Consume<Order>(x =>
            {
                Console.WriteLine("order: " + x.Id);
                Thread.Sleep(5);
            });
			serviceBus.Consume<Bill>(x =>
			{
				Console.WriteLine("bill: " + x.Id);
				Thread.Sleep(5);
			});
            Console.WriteLine("Messaging Server started");
            Console.ReadLine();
        }
    }
}
