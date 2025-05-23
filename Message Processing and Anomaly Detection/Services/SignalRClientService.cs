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
            StartConnection();
        }

        private async void StartConnection()
        {
            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to SignalR: {ex.Message}");
            }
        }

        public async Task SendAlertAsync(string message)
        {
            try
            {
                await _connection.InvokeAsync("SendAlert", message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending alert: {ex.Message}");
            }
        }
    }
}