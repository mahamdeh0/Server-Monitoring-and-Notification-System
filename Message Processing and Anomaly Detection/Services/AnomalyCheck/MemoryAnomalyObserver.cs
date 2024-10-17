using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models;
using Microsoft.Extensions.Configuration;

namespace Message_Processing_and_Anomaly_Detection.Services.AnomalyCheck
{
    public class MemoryAnomalyObserver : IAnomalyObserver
    {
        private readonly double _memoryUsageAnomalyThreshold;
        private readonly ISignalRService _signalRService;

        public MemoryAnomalyObserver(IConfiguration configuration, ISignalRService signalRService)
        {
            _memoryUsageAnomalyThreshold = configuration.GetValue<double>("AnomalyDetectionConfig:MemoryUsageAnomalyThresholdPercentage");
            _signalRService = signalRService;
        }

        public async Task CheckAnomaly(ServerStatistics current, ServerStatistics previous)
        {
            if (previous != null && current.MemoryUsage > (previous.MemoryUsage * (1 + _memoryUsageAnomalyThreshold)))
            {
                await _signalRService.SendAlertAsync("Memory Anomaly Detected!");
            }
        }

    }
}
