using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Services;
using Message_Processing_and_Anomaly_Detection.Services.AnomalyCheck;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Message_Processing_and_Anomaly_Detection
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;

                    services.AddTransient<IAnomalyCheck>(provider =>
                    {
                        var anomalyThreshold = configuration.GetValue<double>("AnomalyDetectionConfig:MemoryUsageAnomalyThresholdPercentage");
                        var usageThreshold = configuration.GetValue<double>("AnomalyDetectionConfig:MemoryUsageThresholdPercentage");
                        return new MemoryAnomalyCheck(anomalyThreshold, usageThreshold);
                    });

                    services.AddTransient<IAnomalyCheck>(provider =>
                    {
                        var anomalyThreshold = configuration.GetValue<double>("AnomalyDetectionConfig:CpuUsageAnomalyThresholdPercentage");
                        var usageThreshold = configuration.GetValue<double>("AnomalyDetectionConfig:CpuUsageThresholdPercentage");
                        return new CpuAnomalyCheck(anomalyThreshold, usageThreshold);
                    });

                    services.AddSingleton<ISignalRService, SignalRService>(provider =>
                    {
                        var signalRUrl = configuration.GetValue<string>("SignalRConfig:SignalRUrl");
                        return new SignalRService(signalRUrl);
                    });

                    services.AddSingleton<IMessageQueue, RabbitMqMessageQueue>(provider =>
                    {
                        var hostName = configuration.GetValue<string>("RabbitMQConfig:HostName");
                        var port = configuration.GetValue<int>("RabbitMQConfig:Port");
                        var userName = configuration.GetValue<string>("RabbitMQConfig:UserName");
                        var password = configuration.GetValue<string>("RabbitMQConfig:Password");
                        return new RabbitMqMessageQueue(hostName, port, userName, password);
                    });

                    services.AddSingleton<IMongoDbService, MongoDbService>(provider =>
                    {
                        var connectionString = configuration.GetValue<string>("MongoDBConfig:ConnectionString");
                        var databaseName = configuration.GetValue<string>("MongoDBConfig:DatabaseName");
                        var collectionName = configuration.GetValue<string>("MongoDBConfig:CollectionName");
                        return new MongoDbService(connectionString, databaseName, collectionName);
                    });

                    services.AddSingleton<AnomalyChecker>();
                    services.AddSingleton<HighUsageChecker>();
                    services.AddSingleton<StatisticsProcessor>();
                })
                .Build();

            var messageQueue = host.Services.GetRequiredService<IMessageQueue>();
            var statisticsProcessor = host.Services.GetRequiredService<StatisticsProcessor>();

            messageQueue.Subscribe(async (statistics) =>
            {
                await statisticsProcessor.ProcessStatistics(statistics);
            });

            await host.RunAsync();
        }
    }
}
