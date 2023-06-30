namespace VirtualBridge.PluginApi;

public interface IDataTransferService
{
    public event EventHandler<IDataTransferObject>? ReceivedData;
}