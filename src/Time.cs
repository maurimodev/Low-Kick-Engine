using System.Collections.Generic;
using Coroutine;
using Microsoft.Xna.Framework;

public static class Time
{
    public static float unscaledDeltaTime;
    public static float deltaTime;
    public static float time;
    public static float unscaledTime;
    public static float timeScale = 0.0f;
    public static void Update(GameTime gameTime)
    {
        CoroutineHandler.Tick(gameTime.ElapsedGameTime.TotalSeconds);
        
        deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds * timeScale;
        time = (float)gameTime.TotalGameTime.TotalSeconds * timeScale;
        unscaledDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        unscaledTime = (float)gameTime.TotalGameTime.TotalSeconds;
    }
}