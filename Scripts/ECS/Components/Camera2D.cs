using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Camera2D : Component
{
    public float zoom;
    public Matrix matrixTransform;
    public Camera2D()
    {
        zoom = 3;
    }

    public float Zoom
    {
        get => zoom;
        set 
        { 
            zoom = value;
            if (zoom < 0.1) zoom = 0.1f;
        }
    }

    public void Move(Vector2 dir)
    {
        entity.transform.position += dir;
    }

    public Matrix GetTransformation(GraphicsDevice device)
    {
        matrixTransform = Matrix.CreateTranslation(new Vector3(-entity.transform.position.X, -entity.transform.position.Y, 0)) *
                          Matrix.CreateRotationZ(entity.transform.rotation) *
                          Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                          Matrix.CreateTranslation(new Vector3(device.Viewport.Width * 0.5f,
                              device.Viewport.Height * 0.5f, 0));
        return matrixTransform;
    }
}