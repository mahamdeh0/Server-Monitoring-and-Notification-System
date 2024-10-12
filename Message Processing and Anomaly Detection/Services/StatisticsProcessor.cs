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

        public async Task ProcessStatistics(ServerStatistics statistics)
        {
            try
            {
                await _mongoDbService.InsertStatisticsAsync(statistics);

                if (_previousStatistics != null)
                {
                    if (_anomalyDetectionService.CheckForMemoryAnomaly(statistics, _previousStatistics))
                    {
                        await _signalRService.SendAlertAsync("Memory Anomaly Detected!");
                    }

                    if (_anomalyDetectionService.CheckForCpuAnomaly(statistics, _previousStatistics))
                    {
                        await _signalRService.SendAlertAsync("CPU Anomaly Detected!");
                    }
                }

                if (_anomalyDetectionService.CheckForHighMemoryUsage(statistics))
                {
                    await _signalRService.SendAlertAsync("High Memory Usage Detected!");
                }

                if (_anomalyDetectionService.CheckForHighCpuUsage(statistics))
                {
                    await _signalRService.SendAlertAsync("High CPU Usage Detected!");
                }

                _previousStatistics = statistics;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing statistics: {ex.Message}");
            }
        }
    }

}