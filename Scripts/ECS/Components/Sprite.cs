using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Sprite : Component
{
    public Texture2D texture;
    public Rectangle rect;
    public Vector2 pivot = Vector2.Zero;
    public Sprite(Texture2D texture, Transform transform)
    {
        this.texture = texture;
        rect = new Rectangle((int)transform.position.X, (int)transform.position.Y, texture.Width,
            texture.Height);
        pivot = Vector2.Zero;
        DrawQueue.RegisterForDraw(this);
    }
    public override void Start()
    {
        base.Start();
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void OnDestroy()
    {

    }
}