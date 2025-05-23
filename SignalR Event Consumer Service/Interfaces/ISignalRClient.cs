namespace SignalR_Event_Consumer_Service.Interfaces
{
    public interface ISignalRClient
    {
        void Start();
        void RegisterAlertHandler(Action<string> handler);
    }
}