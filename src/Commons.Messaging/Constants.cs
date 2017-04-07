using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Commons.Messaging
{
    internal static class Constants
    {
		public const string DefaultServerEndpoint = "tcp://localhost:5011";
		public const string DefaultPublishEndpoint = "tcp://localhost:5012";
		public const string DefaultServerBindAddress = "tcp://*:5011";
		public const string DefaultPublishBindAddress = "tcp://*:5012";
    }
}
