using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models;
using Microsoft.Extensions.Configuration;

namespace Message_Processing_and_Anomaly_Detection.Services.AnomalyCheck
{
    public class HighMemoryUsageObserver : IAnomalyObserver
    {
        private readonly double _memoryUsageThreshold;
        private readonly ISignalRService _signalRService;

        public HighMemoryUsageObserver(IConfiguration configuration, ISignalRService signalRService)
        {
            _memoryUsageThreshold = configuration.GetValue<double>("AnomalyDetectionConfig:MemoryUsageThresholdPercentage");
            _signalRService = signalRService;
        }

        public async Task CheckAnomaly(ServerStatistics current, ServerStatistics previous)
        {
            if ((current.MemoryUsage / (current.MemoryUsage + current.AvailableMemory)) > _memoryUsageThreshold)
            {
                await _signalRService.SendAlertAsync("High Memory Usage Detected!");
            }
        }
    }
}
