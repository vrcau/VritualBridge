namespace VirtualBridge.PluginApi;

public interface IDataTransferService
{
    public IReadOnlyList<IDataTransferObject> DataTransferObjects { get; }
    public event EventHandler<IDataTransferObject>? ReceivedData;
}