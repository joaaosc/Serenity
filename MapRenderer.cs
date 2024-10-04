using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;

public class MapRenderer
{
    private Tile[,] tiles;
    private int tileSize = 32; // Tamanho do tile em pixels
    private Texture2D oceanTexture;
    private Texture2D landTexture;
    private Texture2D mountainTexture;
    private Texture2D riverTexture;
    private Texture2D highlightTexture;
    // Higlight effects
    private float highlightAlpha = 0f;
    private bool increasingAlpha = true;

    public MapRenderer(Tile[,] tiles)
    {
        this.tiles = tiles;
    }

    public Texture2D LoadContent(GraphicsDevice graphicsDevice)
    {
        // Cria texturas de cores sólidas para cada tipo de tile
        oceanTexture = CreateSolidTexture(graphicsDevice, Color.Blue);
        landTexture = CreateSolidTexture(graphicsDevice, Color.Green);
        mountainTexture = CreateSolidTexture(graphicsDevice, Color.Gray);
        riverTexture = CreateSolidTexture(graphicsDevice, Color.Cyan);
        highlightTexture = CreateSolidTexture(graphicsDevice, Color.Transparent);

        // Adiciona uma borda violeta ao highlightTexture
        Texture2D texture = new Texture2D(graphicsDevice, tileSize, tileSize);
        Color[] highlightData = new Color[tileSize * tileSize];
        for (int i = 0; i < highlightData.Length; ++i) highlightData[i] = Color.Transparent;
        for (int i = 0; i < tileSize; i++) // Cria uma borda violeta
        {
            highlightData[i] = Color.Red;
            highlightData[i + tileSize * (tileSize - 1)] = Color.Red;
            highlightData[i * tileSize] = Color.Red;
            highlightData[i * tileSize + (tileSize - 1)] = Color.Red;
        }
        highlightTexture.SetData(highlightData);
        return texture;
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
        DrawTiles(spriteBatch, camera);
        DrawHighlightedTile(spriteBatch, camera);
    }

    private void DrawTiles(SpriteBatch spriteBatch, Camera2D camera)
    {
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

    private void DrawHighlightedTile(SpriteBatch spriteBatch, Camera2D camera)
    {
        MouseState mouseState = Mouse.GetState();
        Vector2 mouseWorldPosition = Vector2.Transform(new Vector2(mouseState.X, mouseState.Y), Matrix.Invert(camera.Transform));
        int mouseTileX = (int)(mouseWorldPosition.X / tileSize);
        int mouseTileY = (int)(mouseWorldPosition.Y / tileSize);

        if (mouseTileX >= 0 && mouseTileY >= 0 && mouseTileX < tiles.GetLength(0) && mouseTileY < tiles.GetLength(1))
        {
            Vector2 highlightPosition = new Vector2(mouseTileX * tileSize, mouseTileY * tileSize);
            Color highlightColor = new Color(1f, 1f, 1f, highlightAlpha); // Efeito de brilho

            spriteBatch.Begin(transformMatrix: camera.Transform);
            spriteBatch.Draw(highlightTexture, highlightPosition, highlightColor);
            spriteBatch.End();
        }
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

    public void Update(GameTime gameTime)
    {
        UpdateHighlightAlpha(gameTime);
    }

    public void SetTileType(int x, int y, TileType newType)
    {
        if (x >= 0 && x < tiles.GetLength(0) && y >= 0 && y < tiles.GetLength(1))
        {
            tiles[x, y].Type = newType;
        }
    }

    private void UpdateHighlightAlpha(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (increasingAlpha)
        {
            highlightAlpha += delta;
            if (highlightAlpha >= 1f)
            {
                highlightAlpha = 1f;
                increasingAlpha = false;
            }
        }
        else
        {
            highlightAlpha -= delta;
            if (highlightAlpha <= 0f)
            {
                highlightAlpha = 0f;
                increasingAlpha = true;
            }
        }
    }

}
