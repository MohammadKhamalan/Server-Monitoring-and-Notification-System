using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Server_Statistics_Collection_Service.Interfaces;
using Server_Statistics_Collection_Service.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server_Statistics_Collection_Service.Services
{
    public class StatisticsCollectorService : IHostedService, IDisposable
    {
        public int SamplingIntervalSeconds { get; set; }
        public string ServerIdentifier { get; set; }
        private readonly IStatisticsPublisher MessageQueue;
        private readonly IConfiguration Configuration;
        private readonly ILogger<StatisticsCollectorService> logger;
        private Timer timer;
        private PerformanceCounter CpuCounter;
        private PerformanceCounter MemoryUsageCounter;
        private PerformanceCounter AvailableMemoryCounter;


        public StatisticsCollectorService(IStatisticsPublisher messageQueue, IConfiguration configuration, ILogger<StatisticsCollectorService> logger)
        {
            MessageQueue = messageQueue;
            Configuration = configuration;
            this.logger = logger;
            SamplingIntervalSeconds = Configuration.GetValue<int>("ServerStatisticsConfig:SamplingIntervalSeconds");
            ServerIdentifier = Configuration.GetValue<string>("ServerStatisticsConfig:ServerIdentifier");
            CpuCounter= new PerformanceCounter("Processor", "% Processor Time", "_Total");
            MemoryUsageCounter= new PerformanceCounter("Process", "Working Set", Process.GetCurrentProcess().ProcessName);
            AvailableMemoryCounter = new PerformanceCounter("Memory", "Available MBytes");

        }
        private double GetCpuUsage()
        {
            return CpuCounter.NextValue();
        }

        private double GetMemoryUsage()
        {
            return MemoryUsageCounter.NextValue() / (1024 * 1024);
        }

        private double GetAvailableMemory()
        {
            return AvailableMemoryCounter.NextValue();
        }
       

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Server statistics collection service starting");
            timer = new Timer(async _ => await CollectAndPublishStatistics(), null, TimeSpan.Zero, TimeSpan.FromSeconds(SamplingIntervalSeconds));
            return Task.CompletedTask;
        }
        private async Task CollectAndPublishStatistics()
        {
            try
            {
                var statistics = new ServerStatistics
                {
                    MemoryUsage = GetMemoryUsage(),
                    AvailableMemory = GetAvailableMemory(),
                    CpuUsage = GetCpuUsage(),
                    Timestamp = DateTime.UtcNow
                };

                string topic = $"ServerStatistics.{ServerIdentifier}";
                await MessageQueue.PublishAsync(statistics, topic);

                logger.LogInformation("Published statistics: {0}", JsonSerializer.Serialize(statistics));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while collecting or publishing server statistics");
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Server statistics collection service stopping");
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
            CpuCounter.Dispose();
            MemoryUsageCounter.Dispose();
            AvailableMemoryCounter.Dispose();
        }
    }

}
