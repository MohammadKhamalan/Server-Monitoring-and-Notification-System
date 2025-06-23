using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models;
using Microsoft.Extensions.Configuration;


namespace Message_Processing_and_Anomaly_Detection.Services
{
    public class AnomalyDetectionService : IAnomalyDetectionService
    {
        private readonly double _memoryUsageAnomalyThreshold;
        private readonly double _cpuUsageAnomalyThreshold;
        private readonly double _memoryUsageThreshold;
        private readonly double _cpuUsageThreshold;

        public AnomalyDetectionService(IConfiguration configuration)
        {
            _memoryUsageAnomalyThreshold = configuration.GetValue<double>("AnomalyDetectionConfig:MemoryUsageAnomalyThresholdPercentage");
            _cpuUsageAnomalyThreshold = configuration.GetValue<double>("AnomalyDetectionConfig:CpuUsageAnomalyThresholdPercentage");
            _memoryUsageThreshold = configuration.GetValue<double>("AnomalyDetectionConfig:MemoryUsageThresholdPercentage");
            _cpuUsageThreshold = configuration.GetValue<double>("AnomalyDetectionConfig:CpuUsageThresholdPercentage");
        }

        public bool CheckForMemoryAnomaly(ServerStatistics current, ServerStatistics previous)
        {
            return current.MemoryUsage > (previous.MemoryUsage * (1 + _memoryUsageAnomalyThreshold));
        }

        public bool CheckForCpuAnomaly(ServerStatistics current, ServerStatistics previous)
        {
            return current.CpuUsage > (previous.CpuUsage * (1 + _cpuUsageAnomalyThreshold));
        }

        public bool CheckForHighMemoryUsage(ServerStatistics statistics)
        {
            return (statistics.MemoryUsage / (statistics.MemoryUsage + statistics.AvailableMemory)) > _memoryUsageThreshold;
        }

        public bool CheckForHighCpuUsage(ServerStatistics statistics)
        {
            return statistics.CpuUsage > _cpuUsageThreshold;
        }
    }
}