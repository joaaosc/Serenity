using System;
using System.Collections.Generic;
using SimplexNoise;

public class MapGenerator
{
    private int width;
    private int height;
    private Tile[,] tiles;
    private Random random;
    private int riverCount = new Random().Next(100,300);

    public MapGenerator(int width, int height, int seed)
    {
        this.width = width;
        this.height = height;
        tiles = new Tile[width, height];
        random = new Random(seed);
        Noise.Seed = seed;
    }

    public Tile[,] GenerateMap()
    {
        GenerateTerrain();
        GenerateRivers();
        return tiles;
    }

    private void GenerateTerrain()
    {
        float scale = 0.02f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float elevation = Noise.CalcPixel2D(x, y, scale) / 255f;
                tiles[x, y] = new Tile { Elevation = elevation };

                if (elevation < 0.4f)
                    tiles[x, y].Type = TileType.Ocean;
                else if (elevation < 0.7f)
                    tiles[x, y].Type = TileType.Land;
                else
                    tiles[x, y].Type = TileType.Mountain;
            }
        }
    }

    private void GenerateRivers()
    {
        for (int i = 0; i < riverCount; i++)
        {
            int startX, startY;
            do
            {
                startX = random.Next(width);
                startY = random.Next(height);
            } while (tiles[startX, startY].Type != TileType.Mountain);

            CreateRiver(startX, startY);
        }
    }

    private void CreateRiver(int x, int y)
    {
        int maxLength = 200;
        int length = 0;
        HashSet<(int, int)> visited = new HashSet<(int, int)>();

        while (length < maxLength)
        {
            if (tiles[x, y].Type == TileType.Land || tiles[x, y].Type == TileType.Mountain)
                tiles[x, y].Type = TileType.River;

            if (tiles[x, y].Type == TileType.Ocean)
                break;

            visited.Add((x, y));

            (int nextX, int nextY) = GetLowestNeighbor(x, y, visited);

            if (nextX == x && nextY == y)
                break;

            x = nextX;
            y = nextY;
            length++;
        }
    }

    private (int, int) GetLowestNeighbor(int x, int y, HashSet<(int, int)> visited)
    {
        float minElevation = tiles[x, y].Elevation;
        int minX = x;
        int minY = y;

        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                continue;

            if (visited.Contains((nx, ny)))
                continue;

            if (tiles[nx, ny].Elevation < minElevation)
            {
                minElevation = tiles[nx, ny].Elevation;
                minX = nx;
                minY = ny;
            }
        }

        return (minX, minY);
    }
}
