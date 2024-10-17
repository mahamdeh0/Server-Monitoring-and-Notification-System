using MediatR;
using Microsoft.Extensions.Configuration;

namespace Message_Processing_and_Anomaly_Detection.Services.AnomalyCheck.Handler
{
    public class HighCpuUsageHandler : IRequestHandler<HighUsageCheckRequest, bool>
    {
        private readonly double _cpuUsageThreshold;

        public HighCpuUsageHandler(IConfiguration configuration)
        {
            _cpuUsageThreshold = configuration.GetValue<double>("AnomalyDetectionConfig:CpuUsageThresholdPercentage");
        }

        public Task<bool> Handle(HighUsageCheckRequest request, CancellationToken cancellationToken)
        {
            var result = request.Statistics.CpuUsage > _cpuUsageThreshold;
            return Task.FromResult(result);
        }
    }

}
