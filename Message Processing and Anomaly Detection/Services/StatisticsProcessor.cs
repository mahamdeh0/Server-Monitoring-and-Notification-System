using MediatR;
using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models;
using Message_Processing_and_Anomaly_Detection.Services.AnomalyCheck;

namespace Message_Processing_and_Anomaly_Detection.Services
{
    public class StatisticsProcessor
    {
        private readonly IMediator _mediator;
        private readonly ISignalRService _signalRService;
        private ServerStatistics _previousStatistics;
        private readonly IMongoDbService _mongoDbService;

        public StatisticsProcessor(IMediator mediator, ISignalRService signalRService, IMongoDbService mongoDbService)
        {
            _mediator = mediator;
            _signalRService = signalRService;
            _mongoDbService = mongoDbService;

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
            var anomalyRequests = new List<AnomalyCheckRequest>
            {
                new AnomalyCheckRequest(statistics, _previousStatistics)
            };

            foreach (var request in anomalyRequests)
            {
                if (await _mediator.Send(request))
                {
                    await _signalRService.SendAlertAsync("Anomaly Detected!");
                }
            }
        }

        private async Task CheckHighUsage(ServerStatistics statistics)
        {
            if (await _mediator.Send(new HighUsageCheckRequest(statistics)))
            {
                await _signalRService.SendAlertAsync("High Memory Usage Detected!");
            }

            if (await _mediator.Send(new HighUsageCheckRequest(statistics)))
            {
                await _signalRService.SendAlertAsync("High CPU Usage Detected!");
            }
        }
    }
}
