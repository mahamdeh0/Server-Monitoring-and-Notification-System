using MediatR;
using Message_Processing_and_Anomaly_Detection.Models;

namespace Message_Processing_and_Anomaly_Detection.Services.AnomalyCheck
{
    public class HighUsageCheckRequest : IRequest<bool>
    {
        public ServerStatistics Statistics { get; }

        public HighUsageCheckRequest(ServerStatistics statistics)
        {
            Statistics = statistics;
        }
    }
}
