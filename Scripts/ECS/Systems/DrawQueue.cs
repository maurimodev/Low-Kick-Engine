using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class DrawQueue
{
    protected static List<Sprite> sprites = new List<Sprite>();

    public static void RegisterForDraw(Sprite sprite)
    {
        sprites.Add(sprite);
    }

    public static void UnregisterForDraw(Sprite sprite)
    {
        sprites.Remove(sprite);
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
            
            //initialize a texture
            var texture = new Texture2D(batch.GraphicsDevice, sprite.texture.Width, sprite.texture.Height);

            //the array holds the color for each pixel in the texture
            Color[] data = new Color[texture.Width * texture.Height];
            for (var pixel = 0; pixel < data.Length; pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = new Color(255, 0, 0, 0.2f);
            }

            //set the color
            texture.SetData(data);

            batch.Draw(texture, sprite.entity.transform.position, null, Color.White, sprite.entity.transform.rotation, sprite.pivot, sprite.entity.transform.scale, SpriteEffects.None, sprite.entity.transform.layerDepth);
            
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