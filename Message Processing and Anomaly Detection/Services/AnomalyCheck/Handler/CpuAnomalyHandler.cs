using MediatR;
using Microsoft.Extensions.Configuration;

namespace Message_Processing_and_Anomaly_Detection.Services.AnomalyCheck.Handler
{
    public class CpuAnomalyHandler : IRequestHandler<AnomalyCheckRequest, bool>
    {
        private readonly double _cpuUsageAnomalyThreshold;

        public CpuAnomalyHandler(IConfiguration configuration)
        {
            _cpuUsageAnomalyThreshold = configuration.GetValue<double>("AnomalyDetectionConfig:CpuUsageAnomalyThresholdPercentage");
        }

        public Task<bool> Handle(AnomalyCheckRequest request, CancellationToken cancellationToken)
        {
            var result = request.Current.CpuUsage > (request.Previous.CpuUsage * (1 + _cpuUsageAnomalyThreshold));
            return Task.FromResult(result);
        }
    }

}
