using System;
using System.Reflection;
using Commons.Collections.Queue;

namespace Commons.Messaging
{
    public class MessageHandler : IMessageHandler<InboundInfo>
    {
        private IWorker worker;
        public MessageHandler(IWorker worker)
        {
            this.worker = worker;
        }

        public void Handle(InboundInfo message)
        {
            var workerType = worker.GetType();
            var method = workerType.GetMethod("Do");
            var success = false;
            object result = null;

            try
            {
                result = method.Invoke(worker, new[] { message.Content });
                success = true;
            }
            catch (Exception)
            {

            }
            var outbound = new OutboundInfo
            {
                Content = result,
                RemoteIp = message.RemoteIp,
                SequenceNo = message.SequenceNo,
                Success = success
            };
            Completed?.Invoke(this, new WorkCompleteEventArgs { OutboundMessage = outbound });
        }

        public event EventHandler<WorkCompleteEventArgs> Completed;
    }

    public class WorkCompleteEventArgs : EventArgs
    {
        public OutboundInfo OutboundMessage { get; set; }
    }

}
