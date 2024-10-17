using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models;

namespace Message_Processing_and_Anomaly_Detection.Services
{
    public class StatisticsProcessor
    {
        private readonly IMessageQueue _messageQueue;
        private readonly IMongoDbService _mongoDbService;
        private readonly ISignalRService _signalRService;
        private readonly List<IAnomalyObserver> _anomalyObservers;
        private ServerStatistics _previousStatistics;

        public StatisticsProcessor(IMessageQueue messageQueue, IMongoDbService mongoDbService, ISignalRService signalRService)
        {
            _messageQueue = messageQueue;
            _mongoDbService = mongoDbService;
            _signalRService = signalRService;
            _anomalyObservers = new List<IAnomalyObserver>();
        }

        public void RegisterObserver(IAnomalyObserver observer)
        {
            _anomalyObservers.Add(observer);
        }

        public void RemoveObserver(IAnomalyObserver observer)
        {
            _anomalyObservers.Remove(observer);
        }

        public async Task ProcessStatistics(ServerStatistics statistics)
        {
         
                await _mongoDbService.InsertStatisticsAsync(statistics);

                foreach (var observer in _anomalyObservers)
                {
                    observer.CheckAnomaly(statistics, _previousStatistics);
                }

                _previousStatistics = statistics;
         
        }
    }
}
