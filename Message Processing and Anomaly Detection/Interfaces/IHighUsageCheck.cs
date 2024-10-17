using Message_Processing_and_Anomaly_Detection.Models;

namespace Message_Processing_and_Anomaly_Detection.Interfaces
{
    public interface IHighUsageCheck
    {
        bool CheckHighUsage(ServerStatistics statistics);
        string GetHighUsageMessage();
    }

}
