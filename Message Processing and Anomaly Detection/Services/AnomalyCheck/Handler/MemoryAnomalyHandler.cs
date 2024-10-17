using MediatR;
using Microsoft.Extensions.Configuration;

namespace Message_Processing_and_Anomaly_Detection.Services.AnomalyCheck.Handler
{
    public class MemoryAnomalyHandler : IRequestHandler<AnomalyCheckRequest, bool>
    {
        private readonly double _memoryUsageAnomalyThreshold;

        public MemoryAnomalyHandler(IConfiguration configuration)
        {
            _memoryUsageAnomalyThreshold = configuration.GetValue<double>("AnomalyDetectionConfig:MemoryUsageAnomalyThresholdPercentage");
        }

        public Task<bool> Handle(AnomalyCheckRequest request, CancellationToken cancellationToken)
        {
            var result = request.Current.MemoryUsage > request.Previous.MemoryUsage * (1 + _memoryUsageAnomalyThreshold);
            return Task.FromResult(result);
        }
    }
}
