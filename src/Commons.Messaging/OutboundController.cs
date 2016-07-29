using Commons.Collections.Queue;
using System.Net.Http;

namespace Commons.Messaging
{
    public class OutboundController : IMessageHandler<OutboundInfo>
    {
        public void Handle(OutboundInfo message)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage();
        }
    }
}
