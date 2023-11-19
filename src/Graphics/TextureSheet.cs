using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KungFuPlatform.src.Graphics;

public class TextureSheet
{
    public Texture2D texture;
    public List<TextureSheetFrame> frames = new();
    
    public TextureSheet(Texture2D sheet, int frameWidth, int frameHeight)
    {
        this.texture = sheet;
        
        int x = 0;
        int y = 0;
        
       for(int i = 0; i < sheet.Height / frameHeight; i++)
       {
           for(int j = 0; j < sheet.Width / frameWidth; j++)
           {
               frames.Add(new TextureSheetFrame(x, y, frameWidth, frameHeight, j));
               x += frameWidth;
           }
           y += frameHeight;
           x = 0;
       }
    }
    public TextureSheet(Texture2D sheet, int frameWidth, int frameHeight, float pivotX, float pivotY)
    {
        this.texture = sheet;
        
        int x = 0;
        int y = 0;
        
        for(int i = 0; i < sheet.Height / frameHeight; i++)
        {
            for(int j = 0; j < sheet.Width / frameWidth; j++)
            {
                frames.Add(new TextureSheetFrame(x, y, frameWidth, frameHeight, j, pivotX, pivotY));
                x += frameWidth;
            }
            y += frameHeight;
            x = 0;
        }
    }
    
    public TextureSheetFrame GetFrame(int index)
    {
        return frames[index];
    }
}

public struct TextureSheetFrame
{
    public int X;
    public int Y;

    public int Width;
    public int Height;
    
    public Rectangle SourceRect => new Rectangle(X, Y, Width, Height);
    public Vector2 Pivot;

    public int Index;
    public TextureSheetFrame(int x, int y, int width, int height, int index, float pivotX = 0.5f, float pivotY = 0.5f)
    {
        X = x;
        Y = y;

        Width = width;
        Height = height;

        Index = index;
        Pivot = new Vector2(pivotX * Width, pivotY * Height);
    }
}