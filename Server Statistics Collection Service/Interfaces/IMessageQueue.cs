using Server_Statistics_Collection_Service.Models;

namespace Server_Statistics_Collection_Service.Interfaces
{
    public interface IMessageQueue
    {
        public Task PublishAsync(string topic, ServerStatistics statistics);
    }
}
