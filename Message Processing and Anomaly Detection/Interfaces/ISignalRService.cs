namespace Message_Processing_and_Anomaly_Detection.Interfaces
{
    public interface ISignalRService
    {
        void SendAlert(string message);
    }
}
