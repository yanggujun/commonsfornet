
namespace Commons.Messaging
{
    public interface IWorker<in TI, out TO> : IWorker
    {
        TO Do(TI message);
    }

    public interface IWorker
    {
    }
}
