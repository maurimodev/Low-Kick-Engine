using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Sprite : Component
{
    public Texture2D texture;
    public Rectangle rect;
    public Vector2 pivot = Vector2.Zero;
    public Sprite(Texture2D texture)
    {
        this.texture = texture;
        rect = texture.Bounds;
        pivot = new Vector2(rect.Width / 2, rect.Height / 2);
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