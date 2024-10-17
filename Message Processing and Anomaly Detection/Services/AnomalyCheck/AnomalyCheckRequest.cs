using MediatR;
using Message_Processing_and_Anomaly_Detection.Models;

namespace Message_Processing_and_Anomaly_Detection.Services.AnomalyCheck
{
    public class AnomalyCheckRequest : IRequest<bool>
    {
        public ServerStatistics Current { get; }
        public ServerStatistics Previous { get; }

        public AnomalyCheckRequest(ServerStatistics current, ServerStatistics previous)
        {
            Current = current;
            Previous = previous;
        }
    }
}
