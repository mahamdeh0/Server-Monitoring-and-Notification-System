using Message_Processing_and_Anomaly_Detection.Models;

namespace Message_Processing_and_Anomaly_Detection.Interfaces
{
    public interface IAnomalyDetectionService
    {
        public bool CheckForMemoryAnomaly(ServerStatistics current, ServerStatistics previous);
        public bool CheckForCpuAnomaly(ServerStatistics current, ServerStatistics previous);
        public bool CheckForHighMemoryUsage(ServerStatistics statistics);
        public bool CheckForHighCpuUsage(ServerStatistics statistics);
    }
}
