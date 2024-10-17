using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models;

namespace Message_Processing_and_Anomaly_Detection.Services.AnomalyCheck
{
    public class MemoryAnomalyCheck : IAnomalyCheck, IHighUsageCheck
    {
        private readonly double _anomalyThreshold;
        private readonly double _usageThreshold;

        public MemoryAnomalyCheck(double anomalyThreshold, double usageThreshold)
        {
            _anomalyThreshold = anomalyThreshold;
            _usageThreshold = usageThreshold;
        }

        public bool CheckAnomaly(ServerStatistics current, ServerStatistics previous)
        {
            if (current == null || previous == null)
            {
                return false;
            }

            return current.MemoryUsage > (previous.MemoryUsage * (1 + _anomalyThreshold));
        }

        public string GetAnomalyMessage()
        {
            return "Memory Anomaly Detected!";
        }

        public bool CheckHighUsage(ServerStatistics statistics)
        {
            return (statistics.MemoryUsage / (statistics.MemoryUsage + statistics.AvailableMemory)) > _usageThreshold;
        }

        public string GetHighUsageMessage()
        {
            return "High Memory Usage Detected!";
        }

    }
}
