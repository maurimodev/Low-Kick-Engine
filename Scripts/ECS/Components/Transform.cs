using Microsoft.Xna.Framework;

public class Transform : Component
{
    public Vector2 position = Vector2.Zero;
    public Vector2 scale = Vector2.One;
    public float layerDepth = 0;
    public float rotation = 0;

    public Transform(Entity entity)
    {
        this.entity = entity;
        TransformSystem.Register(this);
    }
}