using Myra;
using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.Brushes;

namespace Serenity.UI
{
    public class UserInterface
    {
        private Desktop _desktop;

        public UserInterface()
        {
            // Nada é inicializado no construtor agora
        }

        public void SetGame(Game game)
        {
            // Define a instância do jogo no MyraEnvironment
            MyraEnvironment.Game = game;

            // Inicializa o Desktop agora que o Game foi definido
            _desktop = new Desktop();

            // Cria a UI
            BuildUI();
        }

        private void BuildUI()
        {
            var panel = new Panel
            {
                Background = new SolidBrush(new Color(50, 100, 150, 200)), // Cor customizada RGBA (com transparência)
                Width = 200
            };


            // Cria os botões
            var button1 = new TextButton
            {
                Text = "Botão 1",
                Width = 180,
                Left = 10,
                Top = 10
            };
            button1.Click += (s, a) =>
            {
                // Ação do botão 1
            };

            var button2 = new TextButton
            {
                Text = "Botão 2",
                Width = 180,
                Left = 10,
                Top = 50
            };
            button2.Click += (s, a) =>
            {
                // Ação do botão 2
            };

            // Adiciona os botões ao painel
            panel.Widgets.Add(button1);
            panel.Widgets.Add(button2);

            // Adiciona o painel ao Desktop
            _desktop.Widgets.Add(panel);
        }

        // Método para atualizar a UI
        public void Update()
        {
            _desktop?.UpdateInput();
        }

        // Método para desenhar a UI
        public void Render()
        {
            if (_desktop != null)
                _desktop.Render();
        }
    }
}
