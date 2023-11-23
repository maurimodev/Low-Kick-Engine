using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using KungFuPlatform.ECS.Systems;

public class Entity : IEnumerable<Component>, IEnumerable
{
    public int ID { get; set; }
    public string name;
    public Transform transform;
    public List<Component> components = new List<Component>();
    private List<Component> deleteQueue = new List<Component>();
    
    public bool active = true;
    public Entity(string name)
    {
        this.name = name;
        transform = AddComponent<Transform>(new Transform());
        EntityManager.Add(this);
    }

    #region Lifecycle Methods
    public virtual void Start()
    {
        foreach (Component component in components)
        {
            component.Start();
        }
    }
    
    public virtual void Update()
    {
        foreach (Component component in components)
        {
            component.Update();
        }
        
        foreach (Component component in deleteQueue)
        {
            component.OnDestroy();
            components.Remove(component);
        }
        
        deleteQueue.Clear();
    }
    
    #endregion

    #region  Component Methods
    public T AddComponent<T>(Component component) where T : Component
    {
        components.Add(component);
        component.entity = this;
        component.Start();
        return component as T;
    }

    public void AddComponents<T>(params T[] cmp) where T : Component
    {
        foreach (var c in cmp)
            AddComponent<T>(c);
    }

    public void RemoveComponent(Component component)
    {
        deleteQueue.Add(component);
    }
    
    public void RemoveComponents(params Component[] cmp)
    {
        foreach (var c in cmp)
            RemoveComponent(c);
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
    #endregion

    public IEnumerator<Component> GetEnumerator() => components.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    // ImGui
    
    public void ImGuiLayout()
    {
        ImGui.Separator();
        ImGui.Dummy(new Vector2(0, 10));
        foreach (var component in components)
        {
            ImGui.Text(component.GetType().Name);
            ImGui.Separator();
            component.ImGuiLayout();   
            ImGui.Separator();
            ImGui.Dummy(new Vector2(0, 5));
        }
    }
}