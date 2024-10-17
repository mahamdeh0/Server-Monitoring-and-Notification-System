using Message_Processing_and_Anomaly_Detection.Models;

namespace Message_Processing_and_Anomaly_Detection.Interfaces
{
    public interface IMessageQueue
    {
        public void Subscribe(Action<ServerStatistics> messageHandler);
    }
}
