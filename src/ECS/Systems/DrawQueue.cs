using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class DrawQueue
{
    public static bool DrawDebug = true;
    protected static List<Sprite> sprites = new List<Sprite>();
    protected static List<AnimatedSprite> animatedSprites = new();
    protected static List<Collider> colliders = new List<Collider>();

    public static void RegisterForDraw(Sprite sprite)
    {
        sprites.Add(sprite);
    }

    public static void RegisterForDraw(AnimatedSprite sprite)
    {
        animatedSprites.Add(sprite);
    }
    public static void RegisterForDebugDraw(Collider collider)
    {
        colliders.Add(collider);
    }

    public static void UnregisterForDraw(Sprite sprite)
    {
        sprites.Remove(sprite);
    }

    public static void UnregisterForDebugDraw(Collider collider)
    {
        colliders.Remove(collider);
    }

    public void DrawAllInQueue(SpriteBatch batch)
    {
        //for (int i = 0; i < sprites.Count; i++)
        //{
        //    batch.Draw(sprites[i].texture, sprites[i].entity.transform.position, Color.White);
        //}

        foreach (var sprite in sprites)
        {
            batch.Draw(sprite.texture, sprite.entity.transform.position, 
                null, 
                Color.White, 
                sprite.entity.transform.rotation, 
                sprite.pivot,
                sprite.entity.transform.scale,
                SpriteEffects.None,
                sprite.entity.transform.layerDepth);
        }

        foreach (var sprite in animatedSprites)
        {
            var currentFrame = sprite.GetCurrentFrame();
            batch.Draw(sprite.activeAnimation.Sheet.texture, sprite.entity.transform.position, currentFrame.SourceRect,
                Color.White, sprite.entity.transform.rotation, currentFrame.Pivot, sprite.entity.transform.scale,
                SpriteEffects.None, sprite.entity.transform.layerDepth);
        }
        if (DrawDebug)
        {
            foreach (var collider in colliders)
            {
                var tex = new Texture2D(batch.GraphicsDevice, (int)collider.bounds.Width, (int)collider.bounds.Height);

                Color[] data = new Color[tex.Width * tex.Height];
                for (var pixel = 0; pixel < data.Length; pixel++)
                {
                    //the function applies the color according to the specified pixel
                    data[pixel] = new Color(255, 255, 255, 0.6f);
                }

                tex.SetData(data);
                batch.Draw(tex, new Vector2(collider.entity.transform.position.X + collider.offset.X, collider.entity.transform.position.Y + collider.offset.Y), null, Color.Green,
                    collider.entity.transform.rotation, collider.origin, collider.scale * collider.entity.transform.scale, SpriteEffects.None,
                    collider.entity.transform.layerDepth);
            }
        }
    }

    public void DisposeAll()
    {
        for (int i = 0; i < sprites.Count; i++)
        {
            sprites[i].texture.Dispose();
        }
    }
}