using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Serenity
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera2D camera;
        MapGenerator mapGenerator;
        MapRenderer mapRenderer;

        // Variáveis para controle de input
        Vector2 previousMousePosition;
        float previousScrollValue;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            // Inicializa a câmera
            camera = new Camera2D();

            // Gera o mapa
            mapGenerator = new MapGenerator(126,126);
            Tile[,] tiles = mapGenerator.GenerateMap();

            // Inicializa o renderizador do mapa
            mapRenderer = new MapRenderer(tiles);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Carrega o conteúdo do MapRenderer
            mapRenderer.LoadContent(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            /*
             * The code below is used to control the camera with the mouse and zoom with the mouse wheel.
             */
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
                Vector2 delta = mousePosition - previousMousePosition;
                camera.Position -= delta / camera.Zoom;
            }
            previousMousePosition = new Vector2(mouseState.X, mouseState.Y);
            float scrollValue = mouseState.ScrollWheelValue;
            if (scrollValue != previousScrollValue)
            {
                camera.Zoom += (scrollValue - previousScrollValue) * 0.001f;
                camera.Zoom = MathHelper.Clamp(camera.Zoom, 0.5f, 2f); // Limita o zoom
                previousScrollValue = scrollValue;
            }
            camera.Update();
            // Fim do código de controle da câmera

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Desenha o mapa
            mapRenderer.Draw(spriteBatch, camera);

            base.Draw(gameTime);
        }
    }
}
