using System;
using System.Collections.Generic;
using SimplexNoise;

/*
 * MapGenerator - Gerador de mapas utilizando Simplex Noise
 * 
 * PARÂMETROS EXPLICADOS:
 * 
 * 1. Frequência (frequency):
 *    - Controla o tamanho e a escala das características do terreno (por exemplo, montanhas, ilhas, continentes).
 *    - Valores mais baixos (e.g., 0.005f a 0.01f) resultam em características maiores, como grandes continentes.
 *    - Valores mais altos (e.g., 0.1f a 0.2f) produzem detalhes menores e mais ruído, resultando em paisagens com muitas pequenas ilhas ou montanhas.
 * 
 * 2. Amplitude (amplitude):
 *    - Define a intensidade das variações de elevação do terreno.
 *    - Uma amplitude maior cria diferenças de elevação mais pronunciadas, resultando em montanhas mais altas e vales mais profundos.
 *    - No entanto, a amplitude é reduzida a cada octave para criar uma variação mais realista e suave.
 * 
 * 3. Octaves:
 *    - Representa o número de camadas de ruído combinadas para criar a elevação final do terreno.
 *    - Cada octave adiciona mais detalhes em diferentes escalas:
 *      - A primeira octave é a camada base (mais baixa frequência e mais amplitude).
 *      - As octaves subsequentes dobram a frequência e reduzem a amplitude pela metade, adicionando detalhes mais refinados.
 *    - Geralmente, 3 a 6 octaves são suficientes para produzir um terreno natural e variado.
 * 
 * 4. warpAmount (usado em "warped noise" para geração de continentes distribuídos):
 *    - Define o quanto as coordenadas de ruído são deslocadas para criar um efeito de distorção ("warp") nas características do terreno.
 *    - Valores mais altos resultam em padrões mais distorcidos e variados, criando continentes menos uniformes e mais naturais.
 *    - Se o valor de warpAmount for muito alto, os continentes podem parecer muito caóticos ou incoerentes.
 *    - Um valor típico está na faixa de 10f a 100f, dependendo do nível desejado de variação.
 * 
 * 5. Limiares de Elevação (Elevation Thresholds):
 *    - Controla a classificação dos tipos de terreno com base nos valores de elevação normalizados:
 *      - Por exemplo: 
 *        - 0.0 a 0.4 → Ocean (áreas de baixa elevação)
 *        - 0.4 a 0.6 → Land (áreas de média elevação)
 *        - 0.6 a 1.0 → Mountain (áreas de alta elevação)
 *    - Ajustar esses valores permite personalizar a proporção de diferentes tipos de terreno (mais oceano, mais terra, mais montanhas, etc.).
 * 
 * 6. Seed:
 *    - Valor numérico que inicializa o gerador de números aleatórios e a geração do ruído simplex.
 *    - Usar a mesma seed resulta em mapas idênticos em cada execução, enquanto mudar a seed resulta em um mapa completamente diferente.
 * 
 * 7. riverCount:
 *    - Especifica a quantidade de rios a serem gerados no mapa.
 *    - O valor é aleatório (entre 100 e 300 neste exemplo) e define quantos rios devem iniciar em áreas montanhosas e fluir para o oceano.
 * 
 * 8. maxRiverAttempts:
 *    - Número máximo de tentativas para iniciar a criação de um rio.
 *    - Isso garante que mesmo que muitos rios não encontrem um caminho válido, o gerador continuará tentando até atingir o objetivo.
 * 
 * 9. maxLength (em CreateRiver):
 *    - Define o comprimento máximo que um rio pode ter antes de parar.
 *    - Limitar o comprimento impede que rios fiquem muito longos ou excessivamente complexos.
 * 
 * Resumo da Geração:
 * - Usando uma combinação de frequência, amplitude e octaves, o mapa é gerado com características de terreno realistas.
 * - O warpAmount é aplicado para adicionar variações naturais e impedir que os continentes pareçam padrões simples.
 * - O resultado final é normalizado e categorizado em tipos de terreno (oceanos, terra e montanhas), prontos para uso no jogo.
 */


public class MapGenerator
{
    public int width;          // Largura do mapa em tiles
    public int height;         // Altura do mapa em tiles
    private Tile[,] tiles;      // Matriz bidimensional que armazena os tiles do mapa
    private Random random;      
    private int riverCount;     // Número de rios a serem gerados

    public MapGenerator(int width, int height, int seed)
    {
        this.width = width;
        this.height = height;
        tiles = new Tile[width, height];
        random = new Random(seed);
        riverCount = random.Next(100, 300); // Define aleatoriamente a quantidade de rios
        Noise.Seed = seed;                  // Define a seed para o ruído simplex para gerar mapas consistentes
    }

    /// <summary>
    /// Gera o mapa utilizando o método padrão de geração de terreno.
    /// </summary>
    public Tile[,] GenerateMap()
    {
        GenerateTerrain();
        GenerateRivers();
        AddOceanBorder();
        return tiles;
    }

    /// <summary>
    /// Gera o mapa utilizando o tipo de terreno especificado.
    /// </summary>
    /// <param name="terrainType">O tipo de terreno a ser gerado.</param>
    public Tile[,] GenerateMap(TerrainGenerationType terrainType)
    {
        GenerateTerrain(terrainType);
        GenerateRivers();
        AddOceanBorder();
        return tiles;
    }

    /// <summary>
    /// Gera o terreno utilizando o método padrão.
    /// </summary>
    private void GenerateTerrain()
    {
        // Chama a geração de terreno padrão
        GenerateTerrain(TerrainGenerationType.Default);
    }

    /// <summary>
    /// Gera o terreno com base no tipo especificado.
    /// </summary>
    /// <param name="terrainType">O tipo de geração de terreno a ser utilizado.</param>
    private void GenerateTerrain(TerrainGenerationType terrainType)
    {
        switch (terrainType)
        {
            case TerrainGenerationType.CenteredContinents:
                GenerateTerrainWithCenteredContinents();
                break;
            case TerrainGenerationType.DistributedContinents:
                GenerateTerrainWithDistributedContinents();
                break;
            default:
                GenerateTerrainDefault();
                break;
        }
    }

    /// <summary>
    /// Gera o terreno utilizando o método padrão de ruído simplex.
    /// </summary>
    private void GenerateTerrainDefault()
    {
        // Parâmetros para a geração de terreno
        float frequency = 0.01f; // Controla o tamanho das características do terreno (frequência do ruído)
        float amplitude = 1.0f;  // Controla a amplitude das variações de elevação
        int octaves = 4;         // Número de camadas de ruído para detalhes em diferentes escalas

        // Variáveis para normalização da elevação
        float maxElevation = float.MinValue;
        float minElevation = float.MaxValue;

        // Matriz para armazenar as elevações antes da normalização
        float[,] elevationMap = new float[width, height];

        // Geração da elevação para cada ponto do mapa
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float elevation = 0;
                float tempFrequency = frequency;
                float tempAmplitude = amplitude;

                // Aplicação de múltiplas camadas de ruído (octaves)
                for (int octave = 0; octave < octaves; octave++)
                {
                    // Calcula o valor do ruído para as coordenadas atuais
                    float noiseValue = Noise.CalcPixel2D(x, y, tempFrequency) / 255f;
                    noiseValue = noiseValue * 2 - 1; // Converte para o intervalo -1 a 1
                    elevation += noiseValue * tempAmplitude;

                    // Ajusta a frequência e amplitude para a próxima camada
                    tempFrequency *= 2;
                    tempAmplitude /= 2;
                }

                elevationMap[x, y] = elevation;

                // Atualiza os valores máximo e mínimo de elevação para normalização posterior
                if (elevation > maxElevation) maxElevation = elevation;
                if (elevation < minElevation) minElevation = elevation;
            }
        }

        // Normalização da elevação e atribuição dos tipos de terreno
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float elevation = elevationMap[x, y];

                // Normaliza a elevação para o intervalo 0 - 1
                elevation = (elevation - minElevation) / (maxElevation - minElevation);

                // Cria um novo Tile e define a elevação
                tiles[x, y] = new Tile { Elevation = elevation };

                // Define o tipo de terreno com base nos limiares de elevação
                if (elevation < 0.4f)
                    tiles[x, y].Type = TileType.Ocean;      // Oceanos
                else if (elevation < 0.6f)
                    tiles[x, y].Type = TileType.Land;       // Terras baixas
                else
                    tiles[x, y].Type = TileType.Mountain;   // Montanhas
            }
        }
    }

    /// <summary>
    /// Gera o terreno com continentes centralizados, aplicando uma máscara de distância.
    /// </summary>
    private void GenerateTerrainWithCenteredContinents()
    {
        // Parâmetros para a geração de terreno
        float frequency = 0.0055f; // Frequência menor para características maiores
        float amplitude = 1.0f;
        int octaves = 5;

        // Coordenadas do centro do mapa
        float centerX = width / 2f;
        float centerY = height / 2f;

        // Variáveis para normalização da elevação
        float maxElevation = float.MinValue;
        float minElevation = float.MaxValue;

        // Matriz para armazenar as elevações antes da normalização
        float[,] elevationMap = new float[width, height];

        // Geração da elevação para cada ponto do mapa
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float elevation = 0;
                float tempFrequency = frequency;
                float tempAmplitude = amplitude;

                // Aplicação de múltiplas camadas de ruído (octaves)
                for (int octave = 0; octave < octaves; octave++)
                {
                    float noiseValue = Noise.CalcPixel2D(x, y, tempFrequency) / 255f;
                    noiseValue = noiseValue * 2 - 1; // Converte para o intervalo -1 a 1
                    elevation += noiseValue * tempAmplitude;

                    tempFrequency *= 2;
                    tempAmplitude /= 2;
                }

                // Armazena a elevação calculada
                elevationMap[x, y] = elevation;

                // Atualiza os valores máximo e mínimo de elevação
                if (elevation > maxElevation) maxElevation = elevation;
                if (elevation < minElevation) minElevation = elevation;
            }
        }

        // Normalização da elevação e aplicação da máscara de distância
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float elevation = elevationMap[x, y];

                // Normaliza a elevação para o intervalo 0 - 1
                elevation = (elevation - minElevation) / (maxElevation - minElevation);

                // Calcula a distância do ponto ao centro do mapa
                float dx = (x - centerX) / (width / 2f);
                float dy = (y - centerY) / (height / 2f);
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                distance = Math.Min(distance, 1f);

                // Aplica a máscara de distância para aumentar a elevação no centro
                elevation *= (1 - distance * distance);

                // Cria um novo Tile e define a elevação
                tiles[x, y] = new Tile { Elevation = elevation };

                // Define o tipo de terreno com base nos limiares de elevação
                if (elevation < 0.68f)
                    tiles[x, y].Type = TileType.Ocean;      // Oceanos
                else if (elevation < 0.65f)
                    tiles[x, y].Type = TileType.Land;       // Terras baixas
                else
                    tiles[x, y].Type = TileType.Mountain;   // Montanhas
            }
        }
    }

    /// <summary>
    /// Gera o terreno com continentes distribuídos, utilizando "warped noise".
    /// </summary>
    private void GenerateTerrainWithDistributedContinents()
    {
        // Parâmetros para a geração de terreno
        float frequency = 0.007f; // Frequência ajustada para características grandes
        float amplitude = 1.0f;
        int octaves = 6;
        float warpAmount = 70f; // Intensidade do "warp" nas coordenadas

        // Variáveis para normalização da elevação
        float maxElevation = float.MinValue;
        float minElevation = float.MaxValue;

        // Matriz para armazenar as elevações antes da normalização
        float[,] elevationMap = new float[width, height];

        // Geração da elevação para cada ponto do mapa
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float elevation = 0;
                float tempFrequency = frequency;
                float tempAmplitude = amplitude;

                // Aplicação de múltiplas camadas de ruído (octaves)
                for (int octave = 0; octave < octaves; octave++)
                {
                    // Calcula as coordenadas "warped" para maior variação
                    float warpX = x + (Noise.CalcPixel2D(x, y, tempFrequency) / 255f) * warpAmount;
                    float warpY = y + (Noise.CalcPixel2D(x + 1000, y + 1000, tempFrequency) / 255f) * warpAmount;

                    // Calcula o valor do ruído nas coordenadas "warped"
                    float noiseValue = Noise.CalcPixel2D((int)warpX, (int)warpY, tempFrequency) / 255f;
                    noiseValue = noiseValue * 2 - 1; // Converte para o intervalo -1 a 1
                    elevation += noiseValue * tempAmplitude;

                    tempFrequency *= 2;
                    tempAmplitude /= 2;
                }

                elevationMap[x, y] = elevation;

                // Atualiza os valores máximo e mínimo de elevação
                if (elevation > maxElevation) maxElevation = elevation;
                if (elevation < minElevation) minElevation = elevation;
            }
        }

        // Normalização da elevação e atribuição dos tipos de terreno
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float elevation = elevationMap[x, y];

                // Normaliza a elevação para o intervalo 0 - 1
                elevation = (elevation - minElevation) / (maxElevation - minElevation);

                // Cria um novo Tile e define a elevação
                tiles[x, y] = new Tile { Elevation = elevation };

                // Define o tipo de terreno com base nos limiares de elevação
                if (elevation < 0.58f)
                    tiles[x, y].Type = TileType.Ocean;      // Oceanos
                else if (elevation < 0.75f)
                    tiles[x, y].Type = TileType.Land;       // Terras baixas
                else
                    tiles[x, y].Type = TileType.Mountain;   // Montanhas
            }
        }
    }

    /// <summary>
    /// Gera os rios no mapa, iniciando em montanhas e fluindo para o oceano.
    /// </summary>
    private void GenerateRivers()
    {
        int maxRiverAttempts = 3000; // Número máximo de tentativas para criar rios
        int riversCreated = 0;

        for (int i = 0; i < maxRiverAttempts && riversCreated < riverCount; i++)
        {
            int startX = random.Next(width);
            int startY = random.Next(height);

            // Verifica se o tile inicial é uma montanha
            if (tiles[startX, startY].Type == TileType.Mountain)
            {
                if (CreateRiver(startX, startY))
                    riversCreated++;
            }
        }
    }

    /// <summary>
    /// Cria um rio a partir de uma posição específica, seguindo a inclinação do terreno.
    /// </summary>
    /// <param name="x">Posição X inicial.</param>
    /// <param name="y">Posição Y inicial.</param>
    /// <returns>Verdadeiro se o rio alcançou o oceano; caso contrário, falso.</returns>
    private bool CreateRiver(int x, int y)
    {
        int maxLength = 200; // Comprimento máximo do rio
        int length = 0;
        HashSet<(int, int)> visited = new HashSet<(int, int)>(); // Conjunto para evitar loops

        while (length < maxLength)
        {
            if (tiles[x, y].Type == TileType.Land || tiles[x, y].Type == TileType.Mountain)
                tiles[x, y].Type = TileType.River; // Marca o tile como parte do rio

            if (tiles[x, y].Type == TileType.Ocean)
                return true; // O rio alcançou o oceano

            visited.Add((x, y));

            // Obtém o vizinho com a menor elevação
            (int nextX, int nextY) = GetLowestNeighbor(x, y, visited);

            if (nextX == x && nextY == y)
                break; // Não há vizinho mais baixo, o rio termina aqui

            x = nextX;
            y = nextY;
            length++;
        }

        return false; // O rio não alcançou o oceano
    }

    /// <summary>
    /// Obtém o vizinho com a menor elevação que ainda não foi visitado.
    /// </summary>
    /// <param name="x">Posição X atual.</param>
    /// <param name="y">Posição Y atual.</param>
    /// <param name="visited">Conjunto de posições já visitadas.</param>
    /// <returns>Coordenadas do vizinho com a menor elevação.</returns>
    private (int, int) GetLowestNeighbor(int x, int y, HashSet<(int, int)> visited)
    {
        float minElevation = tiles[x, y].Elevation;
        int minX = x;
        int minY = y;

        // Direções para os vizinhos (incluindo diagonais)
        int[] dx = { -1, 1, 0, 0, -1, -1, 1, 1 };
        int[] dy = { 0, 0, -1, 1, -1, 1, -1, 1 };

        for (int i = 0; i < dx.Length; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            // Verifica se as coordenadas estão dentro dos limites do mapa
            if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                continue;

            // Verifica se já foi visitado
            if (visited.Contains((nx, ny)))
                continue;

            // Atualiza se encontrar uma elevação menor
            if (tiles[nx, ny].Elevation < minElevation)
            {
                minElevation = tiles[nx, ny].Elevation;
                minX = nx;
                minY = ny;
            }
        }

        return (minX, minY);
    }
    /// <summary>
    /// Garante que as bordas do mapa sejam cobertas por 5 tiles de oceano.
    /// </summary>
    private void AddOceanBorder()
    {
        int borderSize = 10; // Tamanho da borda do oceano

        // Aumentar gradualmente a probabilidade de transformar em oceano conforme se aproxima da borda
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calcula a distância do tile até a borda mais próxima
                int distanceToEdge = Math.Min(Math.Min(x, width - 1 - x), Math.Min(y, height - 1 - y));

                // Verifica se o tile está dentro da borda de oceano
                if (distanceToEdge < borderSize)
                {
                    // Aumenta a probabilidade de transformar o tile em oceano à medida que se aproxima da borda
                    float probability = (float)(borderSize - distanceToEdge) / borderSize;

                    // Gera um número aleatório entre 0 e 1
                    if (random.NextDouble() < probability)
                    {
                        tiles[x, y].Type = TileType.Ocean;
                    }
                }
            }
        }
    }

    public int GetWidth() { return this.width; }

    public int GetHeight() { return this.height; }

}

/// <summary>
/// Enumeração dos tipos de terreno que podem ser gerados.
/// </summary>
public enum TerrainGenerationType
{
    Default,               // Geração padrão
    CenteredContinents,    // Continentes centralizados
    DistributedContinents  // Continentes distribuídos
}

