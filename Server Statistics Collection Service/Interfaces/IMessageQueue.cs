using Server_Statistics_Collection_Service.Models;

namespace Server_Statistics_Collection_Service.Interfaces
{
    public interface IMessageQueue
    {
        void Publish(string topic, ServerStatistics statistics);
    }
}
