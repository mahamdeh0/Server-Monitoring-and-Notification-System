using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models;

namespace Message_Processing_and_Anomaly_Detection.Services.AnomalyCheck
{
    public class HighUsageChecker
    {
        private readonly IEnumerable<IHighUsageCheck> _highUsageChecks;
        private readonly ISignalRService _signalRService;

        public HighUsageChecker(IEnumerable<IHighUsageCheck> highUsageChecks, ISignalRService signalRService)
        {
            _highUsageChecks = highUsageChecks;
            _signalRService = signalRService;
        }

        public async Task CheckForHighUsageAsync(ServerStatistics statistics)
        {
            var highUsageTasks = _highUsageChecks.Select(async check =>
            {
                if (check.CheckHighUsage(statistics))
                {
                    await _signalRService.SendAlertAsync(check.GetHighUsageMessage());
                }
            });

            await Task.WhenAll(highUsageTasks);
        }
    }
}
