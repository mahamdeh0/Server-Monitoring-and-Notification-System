using Message_Processing_and_Anomaly_Detection.Models;

namespace Message_Processing_and_Anomaly_Detection.Interfaces
{
    public interface IAnomalyCheck
    {
        bool CheckAnomaly(ServerStatistics current, ServerStatistics previous);
        string GetAnomalyMessage();
    }

}
