using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;

public class MapRenderer
{
    private Tile[,] tiles;
    private int tileSize = 32; // Tamanho do tile em pixels
    private Texture2D oceanTexture;
    private Texture2D landTexture;
    private Texture2D mountainTexture;
    private Texture2D riverTexture;

    public MapRenderer(Tile[,] tiles)
    {
        this.tiles = tiles;
    }

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        // Cria texturas de cores sólidas para cada tipo de tile
        oceanTexture = CreateSolidTexture(graphicsDevice, Color.Blue);
        landTexture = CreateSolidTexture(graphicsDevice, Color.Green);
        mountainTexture = CreateSolidTexture(graphicsDevice, Color.Gray);
        riverTexture = CreateSolidTexture(graphicsDevice, Color.Cyan);
    }

    private Texture2D CreateSolidTexture(GraphicsDevice graphicsDevice, Color color)
    {
        Texture2D texture = new Texture2D(graphicsDevice, tileSize, tileSize);
        Color[] colorData = new Color[tileSize * tileSize];
        for (int i = 0; i < colorData.Length; i++)
            colorData[i] = color;
        texture.SetData(colorData);
        return texture;
    }

    public void Draw(SpriteBatch spriteBatch, Camera2D camera)
    {
        // Calcula os limites dos tiles visíveis na tela
        int startX = (int)(camera.Position.X / tileSize);
        int startY = (int)(camera.Position.Y / tileSize);
        int endX = startX + (int)(spriteBatch.GraphicsDevice.Viewport.Width / (tileSize * camera.Zoom)) + 2;
        int endY = startY + (int)(spriteBatch.GraphicsDevice.Viewport.Height / (tileSize * camera.Zoom)) + 2;

        startX = Math.Max(0, startX);
        startY = Math.Max(0, startY);
        endX = Math.Min(tiles.GetLength(0), endX);
        endY = Math.Min(tiles.GetLength(1), endY);

        spriteBatch.Begin(transformMatrix: camera.Transform);

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                Texture2D texture = GetTextureForTile(tiles[x, y]);
                Vector2 position = new Vector2(x * tileSize, y * tileSize);
                spriteBatch.Draw(texture, position, Color.White);
            }
        }

        spriteBatch.End();
    }

    private Texture2D GetTextureForTile(Tile tile)
    {
        switch (tile.Type)
        {
            case TileType.Ocean:
                return oceanTexture;
            case TileType.Land:
                return landTexture;
            case TileType.Mountain:
                return mountainTexture;
            case TileType.River:
                return riverTexture;
            default:
                return landTexture;
        }
    }
}
