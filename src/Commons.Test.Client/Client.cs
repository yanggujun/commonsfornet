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
            while (Console.ReadLine() != "exit")
            {
                for (var i = 0; i < 100; i++)
                {
                    serviceBus.Send(new Order { Id = i, Name = i.ToString() });
                }
            }
        }
    }
}
