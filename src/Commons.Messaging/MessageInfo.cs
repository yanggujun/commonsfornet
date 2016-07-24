
namespace Commons.Messaging
{
    public class InboundInfo
    {
        public int SequenceNo { get; set; }
        public object Content { get; set; }
        public string RemoteIp { get; set; }
    }

    public class OutboundInfo : InboundInfo
    {
        public bool Success { get; set; }
    }

}
