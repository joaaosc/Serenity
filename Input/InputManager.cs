using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Serenity.Input
{
    public class InputManager
    {
        private Camera2D _camera;
        private Texture2D _borderTexture;
        private int _tileWidth;
        private int _tileHeight;
        public int mapWidth;
        public int mapHeight;
        private int _tileX;
        private int _tileY;

        public int SelectedTileX => _tileX;
        public int SelectedTileY => _tileY;

        public InputManager(Camera2D camera, int mapWidth, int mapHeight, int tileWidth, int tileHeight, GraphicsDevice graphicsDevice)
        {
            _camera = camera;
            mapWidth = mapWidth;
            mapHeight = mapHeight;
            _tileWidth = tileWidth;
            _tileHeight = tileHeight;

            // Criar a textura de 1x1 usada para desenhar a borda dos tiles
            _borderTexture = new Texture2D(graphicsDevice, 1, 1);
            _borderTexture.SetData(new[] { Color.White });
        }

        public void Update()
        {
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);

            // Inverter a matriz de transformação da câmera para converter as coordenadas de tela para o mundo
            Matrix inverseTransform = Matrix.Invert(_camera.Transform);

            // Converter a posição do mouse para coordenadas do mundo
            Vector2 worldPosition = Vector2.Transform(mousePosition, inverseTransform);

            // Converter as coordenadas do mundo para coordenadas de tile
            _tileX = (int)(worldPosition.X / _tileWidth);
            _tileY = (int)(worldPosition.Y / _tileHeight);

            // Verificar se o tile está dentro dos limites do mapa
            if (_tileX < 0 || _tileX >= mapWidth || _tileY < 0 || _tileY >= mapHeight)
            {
                _tileX = -1;
                _tileY = -1;
            }
        }

        public void DrawTileHighlight(SpriteBatch spriteBatch)
        {
            if (_tileX >= 0 && _tileY >= 0)
            {
                // Coordenadas do tile no mundo
                Vector2 tilePosition = new Vector2(_tileX * _tileWidth, _tileY * _tileHeight);

                // Desenhar a borda violeta ao redor do tile
                int borderWidth = 2;
                Color borderColor = Color.Purple;

                spriteBatch.Begin();

                // Topo
                spriteBatch.Draw(_borderTexture, new Rectangle((int)tilePosition.X, (int)tilePosition.Y, _tileWidth, borderWidth), borderColor);
                // Baixo
                spriteBatch.Draw(_borderTexture, new Rectangle((int)tilePosition.X, (int)tilePosition.Y + _tileHeight - borderWidth, _tileWidth, borderWidth), borderColor);
                // Esquerda
                spriteBatch.Draw(_borderTexture, new Rectangle((int)tilePosition.X, (int)tilePosition.Y, borderWidth, _tileHeight), borderColor);
                // Direita
                spriteBatch.Draw(_borderTexture, new Rectangle((int)tilePosition.X + _tileWidth - borderWidth, (int)tilePosition.Y, borderWidth, _tileHeight), borderColor);

                spriteBatch.End();
            }
        }
    }
}
