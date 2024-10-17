using MediatR;
using Microsoft.Extensions.Configuration;

namespace Message_Processing_and_Anomaly_Detection.Services.AnomalyCheck.Handler
{
    public class HighMemoryUsageHandler : IRequestHandler<HighUsageCheckRequest, bool>
    {
        private readonly double _memoryUsageThreshold;

        public HighMemoryUsageHandler(IConfiguration configuration)
        {
            _memoryUsageThreshold = configuration.GetValue<double>("AnomalyDetectionConfig:MemoryUsageThresholdPercentage");
        }

        public Task<bool> Handle(HighUsageCheckRequest request, CancellationToken cancellationToken)
        {
            var memoryUsagePercentage = request.Statistics.MemoryUsage / (request.Statistics.MemoryUsage + request.Statistics.AvailableMemory);
            var result = memoryUsagePercentage > _memoryUsageThreshold;

            return Task.FromResult(result);
        }
    }

}
