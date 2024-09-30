using Microsoft.Xna.Framework;

public class Camera2D
{
    public Matrix Transform { get; private set; }
    public Vector2 Position { get; set; }
    public float Zoom { get; set; } = 1f;

    public void Update()
    {
        Transform = Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
                    Matrix.CreateScale(Zoom, Zoom, 1);
    }
}