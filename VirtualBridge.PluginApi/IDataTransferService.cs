namespace VirtualBridge.PluginApi;

public interface IDataTransferService
{
    public event EventHandler<IDataTransferObject>? ReceivedData;
    public void SendDataToGame(object content, string type);
    public void RegisterReceiver(string type, Action<IDataTransferObject> action);
    public void RegisterReceiver<T>(string type, Action<IDataTransferObject<T>> action);
}