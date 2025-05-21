using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message_Processing_and_Anomaly_Detection.Services
{
    public class AnomalyDetectionService : IAnomalyDetectionService
    {
        private readonly AnomalyDetectionConfig _config;
        private readonly ISignalRService _signalRClient;
        private readonly Dictionary<string, ServerStatistics> _previousStats = new();
public AnomalyDetectionService(IOptions<AnomalyDetectionConfig> config, ISignalRService signalRClient)
        {
            _config = config.Value;
            _signalRClient = signalRClient;
        }

        public async Task AnalyzeAsync(ServerStatistics current)
        {
            if (!_previousStats.TryGetValue(current.ServerIdentifier, out var previous))
            {
                _previousStats[current.ServerIdentifier] = current;
                return;
            }
            if (current.MemoryUsage > previous.MemoryUsage * (1 + _config.MemoryUsageAnomalyThresholdPercentage))
            {
                await _signalRClient.SendAlertAsync("Memory Usage Anomaly Detected", current);
            }
            double totalMem = current.MemoryUsage + current.AvailableMemory;
            if (totalMem > 0 && (current.MemoryUsage / totalMem) > _config.MemoryUsageThresholdPercentage)
            {
                await _signalRClient.SendAlertAsync("High Memory Usage Detected", current);
            }
            _previousStats[current.ServerIdentifier] = current;

        }
    }
}
