using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models;
using Microsoft.Extensions.Configuration;

namespace Message_Processing_and_Anomaly_Detection.Services.AnomalyCheck
{
    public class CpuAnomalyObserver : IAnomalyObserver
    {
        private readonly double _cpuUsageAnomalyThreshold;
        private readonly ISignalRService _signalRService;

        public CpuAnomalyObserver(IConfiguration configuration, ISignalRService signalRService)
        {
            _cpuUsageAnomalyThreshold = configuration.GetValue<double>("AnomalyDetectionConfig:CpuUsageAnomalyThresholdPercentage");
            _signalRService = signalRService;
        }

        public async Task CheckAnomaly(ServerStatistics current, ServerStatistics previous)
        {
            if (previous != null && current.CpuUsage > (previous.CpuUsage * (1 + _cpuUsageAnomalyThreshold)))
            {
                await _signalRService.SendAlertAsync("CPU Anomaly Detected!");
            }
        }
    }
}
