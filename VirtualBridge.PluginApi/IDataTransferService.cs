namespace VirtualBridge.PluginApi;

public interface IDataTransferService
{
    public event EventHandler<IDataTransferObject>? ReceivedData;
    public void SendDataToGame(object content, string type);
}