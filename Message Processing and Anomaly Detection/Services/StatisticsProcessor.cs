using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models;
namespace Message_Processing_and_Anomaly_Detection.Services
{
    public class StatisticsProcessor
    {
        private readonly IMessageQueue _messageQueue;
        private readonly IMongoDbService _mongoDbService;
        private readonly IAnomalyDetectionService _anomalyDetectionService;
        private readonly ISignalRService _signalRService;
        private ServerStatistics _previousStatistics;

        public StatisticsProcessor(IMessageQueue messageQueue, IMongoDbService mongoDbService, IAnomalyDetectionService anomalyDetectionService, ISignalRService signalRService)
        {
            _messageQueue = messageQueue;
            _mongoDbService = mongoDbService;
            _anomalyDetectionService = anomalyDetectionService;
            _signalRService = signalRService;
        }

        public async Task ProcessStatistics(ServerStatistics statistics)
        {
            try
            {
                await _mongoDbService.InsertAsync(statistics);

                if (_previousStatistics != null)
                {
                    if (_anomalyDetectionService.CheckForMemoryAnomaly(statistics, _previousStatistics))
                    {
                        await _signalRService.SendAlertAsync("Memory Anomaly Detected!");
                    }

                    if (_anomalyDetectionService.CheckForCpuAnomaly(statistics, _previousStatistics))
                    {
                        Console.WriteLine("Sending alert: High CPU Usage Detected!");

                        await _signalRService.SendAlertAsync("CPU Anomaly Detected!");
                    }
                }

                if (_anomalyDetectionService.CheckForHighMemoryUsage(statistics))
                {
                    await _signalRService.SendAlertAsync("High Memory Usage Detected!");
                }

                if (_anomalyDetectionService.CheckForHighCpuUsage(statistics))
                {
                    await _signalRService.SendAlertAsync("High CPU Usage Detected!");
                }

                _previousStatistics = statistics;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing statistics: {ex.Message}");
            }
        }
    }
}
