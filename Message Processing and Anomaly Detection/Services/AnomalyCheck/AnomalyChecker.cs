using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models;

namespace Message_Processing_and_Anomaly_Detection.Services.AnomalyCheck
{
    public class AnomalyChecker
    {
        private readonly IEnumerable<IAnomalyCheck> _anomalyChecks;
        private readonly ISignalRService _signalRService;

        public AnomalyChecker(IEnumerable<IAnomalyCheck> anomalyChecks, ISignalRService signalRService)
        {
            _anomalyChecks = anomalyChecks;
            _signalRService = signalRService;
        }

        public async Task CheckForAnomaliesAsync(ServerStatistics currentStatistics, ServerStatistics previousStatistics)
        {
            var anomalyTasks = _anomalyChecks.Select(async check =>
            {
                if (check.CheckAnomaly(currentStatistics, previousStatistics))
                {
                    await _signalRService.SendAlertAsync(check.GetAnomalyMessage());
                }
            });

            await Task.WhenAll(anomalyTasks);
        }
    }
}
