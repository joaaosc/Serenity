using Microsoft.Xna.Framework;

public class Camera2D
{
    public Matrix Transform { get; private set; } // Agora a propriedade Transform é pública
    public Vector2 Position { get; set; }
    public float Zoom { get; set; } = 1f;

    public void Update()
    {
        // Atualiza a matriz de transformação da câmera
        Transform = Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
                    Matrix.CreateScale(Zoom, Zoom, 1);
    }
}