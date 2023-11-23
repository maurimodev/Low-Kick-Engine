using System;
using System.Collections.Generic;
using ImGuiNET;
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
    public static int GetComponentCount()
    {
        return components.Count;
    }

    public static T GetComponentByIndex(int i)
    {
        return components[i] as T;
    }
}