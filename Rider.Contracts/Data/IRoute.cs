namespace Rider.Contracts.Data
{
    public interface IRoute
    {
        decimal LatitudeMin { get; }
        decimal LatitudeMax { get; }
        decimal LongitudeMin { get; }
        decimal LongitudeMax { get; }

        decimal Distance { get; }
        IReadOnlyList<IRoutePoint> Points { get; }

        bool IsEmpty { get; }
    }
}
