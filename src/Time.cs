using System.Collections.Generic;
using Microsoft.Xna.Framework;

public static class Time
{
    public static float deltaTime;
    public static float time;
    public static float timeScale = 0.0f;
    public static void Update(GameTime gameTime)
    {
        deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds * timeScale;
        time = (float)gameTime.TotalGameTime.TotalSeconds * timeScale;
    }
}