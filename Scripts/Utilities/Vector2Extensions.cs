using System.Numerics;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vec2 = System.Numerics.Vector2;
public static class Vector2Extensions
{
    public static void SetVector2(this Vector2 fnaVector, Vec2 numericVector)
    {
        fnaVector.X = numericVector.X;
        fnaVector.Y = numericVector.Y;
    }

    public static void SetVec2(this Vec2 numericVector, Vector2 fnaVector)
    {
        numericVector.X = fnaVector.X;
        numericVector.Y = fnaVector.Y;
    }

    public static Vec2 TranslateVector2(this Vector2 fnaVector)
    {
        return new Vec2(fnaVector.X, fnaVector.Y);
    }

    public static Vector2 TranslateVector2(this Vec2 numericVector)
    {
        return new Vector2(numericVector.X, numericVector.Y);
    }
}