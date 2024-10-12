using Message_Processing_and_Anomaly_Detection.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace Message_Processing_and_Anomaly_Detection.Services
{
    public class SignalRService : ISignalRService
    {
        private readonly HubConnection _connection;

        public SignalRService(string signalRUrl)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(signalRUrl)
                .Build();
            _connection.StartAsync().Wait();
        }

        public void SendAlert(string message)
        {
            _connection.InvokeAsync("SendAlert", message);
        }
    }
}
