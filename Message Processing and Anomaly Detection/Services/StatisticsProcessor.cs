using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models; 
namespace Message_Processing_and_Anomaly_Detection.Services
{
    public class StatisticsProcessor
    {
        private readonly IMessageQueue _messageQueue;
        private readonly IMongoDbService _mongoDbService;
        private readonly IAnomalyDetectionService _anomalyDetectionService;
        private readonly ISignalRService _signalRService;
        private ServerStatistics _previousStatistics;

        public StatisticsProcessor(IMessageQueue messageQueue, IMongoDbService mongoDbService, IAnomalyDetectionService anomalyDetectionService, ISignalRService signalRService)
        {
            _messageQueue = messageQueue;
            _mongoDbService = mongoDbService;
            _anomalyDetectionService = anomalyDetectionService;
            _signalRService = signalRService;
        }

        public void Start()
        {
            _messageQueue.Subscribe("ServerStatistics", ProcessStatistics);
        }

        private void ProcessStatistics(ServerStatistics statistics)
        {
            _mongoDbService.InsertStatisticsAsync(statistics);

            if (_previousStatistics != null)
            {
                if (_anomalyDetectionService.CheckForMemoryAnomaly(statistics, _previousStatistics))
                {
                    _signalRService.SendAlert("Memory Anomaly Detected!!!!!!!!");
                }

                if (_anomalyDetectionService.CheckForCpuAnomaly(statistics, _previousStatistics))
                {
                    _signalRService.SendAlert("CPU Anomaly Detected!!!!!!!!");
                }
            }

            if (_anomalyDetectionService.CheckForHighMemoryUsage(statistics))
            {
                _signalRService.SendAlert("High Memory Usage Detected!!!!!!!");
            }

            if (_anomalyDetectionService.CheckForHighCpuUsage(statistics))
            {
                _signalRService.SendAlert("High CPU Usage Detected!!!!!!!!!");
            }

            _previousStatistics = statistics;
        }
    }
}
