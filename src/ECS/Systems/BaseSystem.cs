using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class BaseSystem<T> where T : Component
{
    protected static List<T> components = new List<T>();

    public static void Register(T component)
    {
        components.Add(component);
    }

    public static void Unregister(T component)
    {
        components.Remove(component);
    }

    public static void Update(GameTime gameTime)
    {
        foreach (T component in components)
        {
            component.Update(gameTime);
        }
    }
    public static int GetComponentCount()
    {
        return components.Count;
    }

    public static T GetComponentByIndex(int i)
    {
        return components[i] as T;
    }

    public static string[] GetGameObjectNames()
    {
        string[] names = new string[components.Count];

        for (int i = 0; i < names.Length; i++)
        {
            names[i] = components[i].entity.name;
        }
        return names;
    }
}