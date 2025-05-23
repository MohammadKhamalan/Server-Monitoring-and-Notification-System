using SignalR_Event_Consumer_Service;
using SignalR_Event_Consumer_Service.Config;
using SignalR_Event_Consumer_Service.Interfaces;

namespace SignalRAlertConsumer;

class Program
{
    static void Main(string[] args)
    {
        var configHelper = new ConfigurationHelper();
        string signalRHubUrl = configHelper.GetSignalRHubUrl();

        ISignalRClient signalRClient = new SignalRClient(signalRHubUrl);
        signalRClient.Start();

        signalRClient.RegisterAlertHandler(alert =>
        {
            Console.WriteLine(alert);
        });

        Console.ReadLine();
    }
}