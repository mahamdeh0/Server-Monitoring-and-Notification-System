using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models;
using Microsoft.Extensions.Configuration;

namespace Message_Processing_and_Anomaly_Detection.Services.AnomalyCheck
{
    public class HighCpuUsageObserver : IAnomalyObserver
    {
        private readonly double _cpuUsageThreshold;
        private readonly ISignalRService _signalRService;

        public HighCpuUsageObserver(IConfiguration configuration, ISignalRService signalRService)
        {
            _cpuUsageThreshold = configuration.GetValue<double>("AnomalyDetectionConfig:CpuUsageThresholdPercentage");
            _signalRService = signalRService;
        }

        public async Task CheckAnomaly(ServerStatistics current, ServerStatistics previous)
        {
            if (current.CpuUsage > _cpuUsageThreshold)
            {
                await _signalRService.SendAlertAsync("High CPU Usage Detected!");
            }
        }
    }
}
