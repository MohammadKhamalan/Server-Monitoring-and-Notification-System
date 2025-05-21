using Server_Statistics_Collection_Service.Models;
namespace Server_Statistics_Collection_Service.Interfaces
{
    public interface IStatisticsPublisher
    {
       public Task PublishAsync(ServerStatistics statistics, string topic);
    }
}
