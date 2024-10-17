namespace Message_Processing_and_Anomaly_Detection.Interfaces
{
    public interface ISignalRService
    {
        Task SendAlertAsync(string message);
    }
}
