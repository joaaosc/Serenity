using System;

public class MapGenerator
{
    private int width;
    private int height;
    private Tile[,] tiles;
    private int riverCount = 10; // Número de rios a serem gerados
    private Random random = new Random();

    public MapGenerator(int width, int height)
    {
        this.width = width;
        this.height = height;
        tiles = new Tile[width, height];
    }

    public Tile[,] GenerateMap()
    {
        GenerateTerrain();
        GenerateRivers();
        return tiles;
    }

    private void GenerateTerrain()
    {
        // Implementação do ruído de Perlin para gerar elevações
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float elevation = PerlinNoise(x, y);
                tiles[x, y] = new Tile { Elevation = elevation };

                if (elevation < 0.3f)
                    tiles[x, y].Type = TileType.Ocean;
                else if (elevation < 0.6f)
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
            // Seleciona um ponto de partida aleatório acima de uma certa elevação
            int startX, startY;
            do
            {
                startX = random.Next(width);
                startY = random.Next(height);
            } while (tiles[startX, startY].Elevation < 0.6f);

            // Cria o rio a partir do ponto de partida
            CreateRiver(startX, startY);
        }
    }

    private void CreateRiver(int x, int y)
    {
        int maxLength = 200; // Comprimento máximo do rio
        int length = 0;

        while (length < maxLength)
        {
            tiles[x, y].Type = TileType.River;

            // Verifica se o rio chegou ao oceano
            if (tiles[x, y].Type == TileType.Ocean)
                break;

            // Encontra o vizinho com a menor elevação
            (int nextX, int nextY) = GetLowestNeighbor(x, y);

            // Se não houver vizinho mais baixo, termina o rio
            if (nextX == x && nextY == y)
                break;

            x = nextX;
            y = nextY;
            length++;
        }
    }

    private (int, int) GetLowestNeighbor(int x, int y)
    {
        float minElevation = tiles[x, y].Elevation;
        int minX = x;
        int minY = y;

        // Verifica os vizinhos (8-direções)
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int nx = x + dx;
                int ny = y + dy;

                // Ignora se estiver fora dos limites ou for o próprio tile
                if ((dx == 0 && dy == 0) || nx < 0 || ny < 0 || nx >= width || ny >= height)
                    continue;

                if (tiles[nx, ny].Elevation < minElevation)
                {
                    minElevation = tiles[nx, ny].Elevation;
                    minX = nx;
                    minY = ny;
                }
            }
        }

        return (minX, minY);
    }

    // Implementação simples do Perlin Noise (substitua por uma real ou use uma biblioteca)
    private float PerlinNoise(int x, int y)
    {
        // Código para gerar ruído de Perlin
        return (float)random.NextDouble();
    }
}
