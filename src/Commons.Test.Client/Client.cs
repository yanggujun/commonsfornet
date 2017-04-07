using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commons.Messaging;

namespace Commons.Test.Client
{
    public class Client
    {
        public static void Main(string[] args)
        {
            var serviceBus = new ServiceBus();
            Console.WriteLine("Messaging client: waiting for user input");
			string line;
			var i = 0;
            while ((line = Console.ReadLine()) != "exit")
            {
				if (line == "order")
				{
					serviceBus.Send(new Order { Id = i, Name = i.ToString() });
					++i;
				}
				else
				{
					serviceBus.Send(new Bill { Count = 100, Id = i });
					++i;
				}
            }
        }
    }
}
