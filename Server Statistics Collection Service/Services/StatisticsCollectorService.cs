using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Server_Statistics_Collection_Service.Interfaces;
using Server_Statistics_Collection_Service.Models;
using System.Diagnostics;
using System.Text.Json;

namespace Server_Statistics_Collection_Service.Services
{
    public class StatisticsCollectorService : IHostedService, IDisposable
    {
        private readonly IMessageQueue _messageQueue;
        private readonly IConfiguration _configuration;
        private readonly ILogger<StatisticsCollectorService> _logger;
        private Timer _timer;
        private int _samplingInterval;
        private string _serverIdentifier;

        private PerformanceCounter _cpuCounter;

        public StatisticsCollectorService(IMessageQueue messageQueue, IConfiguration configuration, ILogger<StatisticsCollectorService> logger)
        {
            _messageQueue = messageQueue;
            _configuration = configuration;
            _logger = logger;

            _samplingInterval = _configuration.GetValue<int>("ServerStatisticsConfig:SamplingIntervalSeconds");
            _serverIdentifier = _configuration.GetValue<string>("ServerStatisticsConfig:ServerIdentifier");

            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        private double GetCpuUsage()
        {
            return _cpuCounter.NextValue();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Server statistics collection service starting");
            _timer = new Timer(async _ => await CollectAndPublishStatistics(), null, TimeSpan.Zero, TimeSpan.FromSeconds(_samplingInterval));
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

                string topic = $"ServerStatistics.{_serverIdentifier}";
                await _messageQueue.PublishAsync(topic, statistics);

                _logger.LogInformation("Published statistics: {0}", JsonSerializer.Serialize(statistics));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while collecting or publishing server statistics");
            }
        }



        private double GetMemoryUsage()
        {
            var currentProcess = Process.GetCurrentProcess();
            return currentProcess.WorkingSet64 / (1024 * 1024);
        }

        private double GetAvailableMemory()
        {
            var totalMemoryInBytes = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
            return totalMemoryInBytes / (1024 * 1024);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Server statistics collection service stopping");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

