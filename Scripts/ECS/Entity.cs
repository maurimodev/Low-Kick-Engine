using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class Entity
{
    public int ID { get; set; }
    public string name;
    public Transform transform;
    public List<Component> components = new List<Component>();

    public Entity()
    {
        transform = new Transform(this);
    }

    public Entity(string name)
    {
        this.name = name;
        transform = new Transform(this);
    }

    public Entity(string name, Vector2 position)
    {
        this.name = name;
        transform = new Transform(this)
        {
            position = position
        };
    }
    public T AddComponent<T>(Component component) where T : Component
    {
        components.Add(component);
        component.entity = this;
        return component as T;
    }

    public T GetComponent<T>() where T : Component
    {
        foreach (Component component in components)
        {
            if(component.GetType() == typeof(T))
                return (T)component;
        }
        Console.WriteLine("Couldn't retrieve component {0} in entity {1}", typeof(T), ID);
        return null;
    }

    public bool TryGetComponent<T>(out T component) where T : Component
    {
        foreach (Component c in components)
        {
            if (c.GetType() == typeof(T))
            {
                component = (T)c;
                return true;
            }
        }
        component = null;
        return false;
    }
    public bool ContainsComponent<T>() where T : Component
    {
        foreach (Component component in components)
        {
            if (component.GetType() == typeof(T))
                return true;
        } 
        return false;
    }
}