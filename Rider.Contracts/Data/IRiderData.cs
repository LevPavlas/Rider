namespace Rider.Contracts.Data
{
    public interface IRiderData
    {
        IRoute Route { get; }
        bool IsEmpty { get; }
    }
}
