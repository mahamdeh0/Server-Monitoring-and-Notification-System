using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models;
using Message_Processing_and_Anomaly_Detection.Services.AnomalyCheck;

namespace Message_Processing_and_Anomaly_Detection.Services
{
    public class StatisticsProcessor
    {
        private readonly IMessageQueue _messageQueue;
        private readonly IMongoDbService _mongoDbService;
        private readonly AnomalyChecker _anomalyChecker;
        private readonly HighUsageChecker _highUsageChecker;
        private ServerStatistics _previousStatistics;

        public StatisticsProcessor(IMessageQueue messageQueue, IMongoDbService mongoDbService,
                                   AnomalyChecker anomalyChecker, HighUsageChecker highUsageChecker)
        {
            _messageQueue = messageQueue;
            _mongoDbService = mongoDbService;
            _anomalyChecker = anomalyChecker;
            _highUsageChecker = highUsageChecker;
        }

        private async Task SaveStatisticsAsync(ServerStatistics statistics)
        {
            await _mongoDbService.InsertStatisticsAsync(statistics);
        }

        public async Task ProcessStatistics(ServerStatistics statistics)
        {
     
            await SaveStatisticsAsync(statistics);

            await CheckHighUsage(statistics);
            if (_previousStatistics != null)
            {
                await CheckAnomalies(statistics);
            }

            _previousStatistics = statistics;
        }

        private async Task CheckAnomalies(ServerStatistics statistics)
        {
            await _anomalyChecker.CheckForAnomaliesAsync(statistics, _previousStatistics);
        }

        private async Task CheckHighUsage(ServerStatistics statistics)
        {
            await _highUsageChecker.CheckForHighUsageAsync(statistics);
        }
    }
}
