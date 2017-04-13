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
					serviceBus.Send(new DetailedOrder { Id = i, Name = i.ToString() });
					++i;
				}
				else
				{
					Task[] tasks = new Task[2];
					for (var k = 0; k < tasks.Length; k++)
					{
						tasks[k] = Task.Factory.StartNew(() =>
						{
							for (var j = 0; j < 20; j++)
							{
								serviceBus.Send(new Bill { Count = j, Id = j });
							}
						});
					}

					Task.WaitAll(tasks);
					Console.WriteLine("messages are sent");
				}
            }
        }
    }
}
