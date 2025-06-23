using Microsoft.AspNetCore.SignalR.Client;
using SignalR_Event_Consumer_Service.Interfaces;

namespace SignalR_Event_Consumer_Service
{
    public class SignalRClient : ISignalRClient
    {
        private readonly HubConnection _connection;

        public SignalRClient(string hubUrl)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .Build();
        }

        public void Start()
        {
            try
            {
                _connection.StartAsync().Wait();
                Console.WriteLine("SignalR connected.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR connection error: {ex.Message}");
            }
        }

        public void RegisterAlertHandler(Action<string> handler)
        {
            _connection.On("ReceiveAlert", handler);
        }
    }
}