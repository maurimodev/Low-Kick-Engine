﻿using System;
using System.Collections.Generic;
using KungFuPlatform.Editor.Windows;
using KungFuPlatform.Scripts.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class DrawQueue
{
    public static bool DrawDebug = false;
    private static List<Sprite> sprites = new();
    private static List<AnimatedSprite> animatedSprites = new();
    private static List<Collider> colliders = new();

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

    public static void DrawAllInQueue(SpriteBatch batch, Camera2D camera, GraphicsDevice graphicsDevice)
    {
        //for (int i = 0; i < sprites.Count; i++)
        //{
        //    batch.Draw(sprites[i].texture, sprites[i].entity.transform.position, Color.White);
        //}
        batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
            DepthStencilState.DepthRead, RasterizerState.CullNone, null, camera.GetTransformation(graphicsDevice));
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
            var pixel = new Texture2D(batch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            foreach (var collider in colliders)
            {
                // Left Side
                batch.Draw(pixel, new RectangleF((collider.entity.transform.position.X + collider.offset.X - 1), 
                        collider.entity.transform.position.Y + collider.offset.Y, 
                        1, 
                        collider.bounds.Height * collider.scale.Y * collider.entity.transform.scale.Y),
                    Color.Green);
                
                // Right Side
                var rectRight = new RectangleF(
                    (collider.bounds.Width * collider.scale.X * collider.entity.transform.scale.X) + collider.entity.transform.position.X + collider.offset.X - 1,
                    collider.entity.transform.position.Y + collider.offset.Y,
                    1,
                    collider.bounds.Height * collider.scale.Y * collider.entity.transform.scale.Y);
                
                batch.Draw(pixel, rectRight,
                    Color.Green);
                // Top Side
                batch.Draw(pixel, new RectangleF(collider.entity.transform.position.X + collider.offset.X, 
                        collider.entity.transform.position.Y - 1 + collider.offset.Y, 
                        collider.bounds.Width * collider.scale.X * collider.entity.transform.scale.X , 
                        1),
                    Color.Green);
                
                // Bottom Side
                batch.Draw(pixel, new RectangleF(collider.entity.transform.position.X + collider.offset.X, 
                        (collider.bounds.Height * collider.scale.Y * collider.entity.transform.scale.Y) + collider.entity.transform.position.Y + collider.offset.Y - 1, 
                        collider.bounds.Width * collider.scale.X * collider.entity.transform.scale.X ,
                        1),
                    Color.Green);
            }
        }
        batch.End();
    }

    public static void DisposeAll()
    {
        for (int i = 0; i < sprites.Count; i++)
        {
            sprites[i].texture.Dispose();
        }
    }
}