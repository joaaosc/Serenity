public enum TileType
{
    Ocean,
    Land,
    Mountain,
    River
}

public class Tile
{
    public TileType Type { get; set; }
    public float Elevation { get; set; }
}