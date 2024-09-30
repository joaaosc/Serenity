using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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

            // Assina o evento de redimensionamento da janela
            Window.ClientSizeChanged += OnClientSizeChanged;

            // Eventos para quando a janela é ativada ou desativada
            Activated += OnActivated;
            Deactivated += OnDeactivated;

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

            if (!IsActive)
            {
                // Se o jogo não está ativo, não atualiza o estado
                base.Update(gameTime);
                return;
            }


            /*
             * The code below is used to control the camera with the mouse and zoom with the mouse wheel.
             */

            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);

                if (previousMousePosition != Vector2.Zero)
                {
                    Vector2 delta = mousePosition - previousMousePosition;
                    camera.Position -= delta / camera.Zoom;
                }

                previousMousePosition = mousePosition;
            }
            else
            {
                previousMousePosition = Vector2.Zero;
            }
            float scrollValue = mouseState.ScrollWheelValue;
            if (scrollValue != previousScrollValue)
            {
                camera.Zoom += (scrollValue - previousScrollValue) * 0.001f;
                camera.Zoom = MathHelper.Clamp(camera.Zoom, 0.5f, 2f);
                previousScrollValue = scrollValue;
            }

            camera.Update();
            // Fim do código de controle da câmera

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Atualiza a viewport caso tenha sido alterada
            var viewport = GraphicsDevice.Viewport;
            spriteBatch.GraphicsDevice.Viewport = viewport;

            // Desenha o mapa
            mapRenderer.Draw(spriteBatch, camera);

            base.Draw(gameTime);
        }

        /// <summary>
        /// The codes on below section is not part of the original Monogame template.
        /// </summary>


        private void OnClientSizeChanged(object sender, EventArgs e)
        {
            // Atualiza o tamanho da viewport
            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            graphics.ApplyChanges();

            // Se necessário, atualize outros componentes (como a câmera)
        }

        private void OnActivated(object sender, EventArgs e)
        {
            // O jogo ganhou foco
            // Você pode reconfigurar estados se necessário
        }

        private void OnDeactivated(object sender, EventArgs e)
        {
            // O jogo perdeu foco
            // Redefine as posições anteriores do mouse
            previousMousePosition = Vector2.Zero;
            previousScrollValue = 0f;
        }
    }


}
