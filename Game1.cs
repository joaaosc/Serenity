using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Serenity.Input;
using Serenity.UI;
using System;
using System.Windows.Forms;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;


namespace Serenity
{
    public class Game1 : Game
    {
        Random random = new Random();

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera2D camera;
        MapGenerator mapGenerator;
        MapRenderer mapRenderer;

        // Variáveis para controle de input
        Vector2 previousMousePosition;
        float previousScrollValue;

        // UI
        private UserInterface _userInterface;
        private InputManager _inputManager;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 800;  // Largura
            graphics.PreferredBackBufferHeight = 600; // Altura
            graphics.ApplyChanges();

            // Inicializa a câmera
            camera = new Camera2D();

            // Gera o mapa
            mapGenerator = new MapGenerator(1024,512,random.Next());
            Tile[,] tiles = mapGenerator.GenerateMap(TerrainGenerationType.DistributedContinents);



            // Inicializa o renderizador do mapa
            mapRenderer = new MapRenderer(tiles);

            // Assina o evento de redimensionamento da janela
            Window.ClientSizeChanged += OnClientSizeChanged;

            // Eventos para quando a janela é ativada ou desativada
            Activated += OnActivated;
            Deactivated += OnDeactivated;


            // Inicializa a interface de usuário
            _userInterface = new UserInterface();
            _userInterface.SetGame(this);



            // Inicializa a interface de usuário com a ação do botão 1
            _userInterface = new UserInterface();

            MyraEnvironment.Game = this;


            base.Initialize();
        }



        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Defina a instância do jogo no Myra
            MyraEnvironment.Game = this;

            // Inicializa a interface de usuário
            _userInterface = new UserInterface();
            _userInterface.SetGame(this); // Agora o MyraEnvironment.Game foi definido

            // Carrega o conteúdo do MapRenderer
            mapRenderer.LoadContent(GraphicsDevice);


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

            // Atualiza a interface de usuário
            _userInterface.Update();

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
                camera.Zoom = MathHelper.Clamp(camera.Zoom, 0.02f, 2f);
                previousScrollValue = scrollValue;
            }

            camera.Update();
            // Fim do código de controle da câmera

            // Atualiza a interface de usuário
            _userInterface.Update();

            // Atualiza o MapRenderer, se necessário
            mapRenderer.Update(gameTime);

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

            // Desenha a interface de usuário
            _userInterface.Render();


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
