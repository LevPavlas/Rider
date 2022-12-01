namespace Rider.Contracts.Data
{
    public interface IRoutePoint
    {
        decimal Latitude { get; }
        decimal Longitude { get; }
        decimal Elevation { get; }
        decimal Distance { get; }
    }
}
