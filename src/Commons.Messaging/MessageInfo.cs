
namespace Commons.Messaging
{
    public class InboundInfo
    {
        public long SequenceNo { get; set; }
        public object Content { get; set; }
    }

    public class OutboundInfo : InboundInfo
    {
        public bool Success { get; set; }
    }

}
