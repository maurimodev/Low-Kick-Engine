using System.Collections;
using System.Collections.Generic;

namespace KungFuPlatform.ECS.Systems;

public class EntityManager : IEnumerable<Entity>, IEnumerable
{
    private static List<Entity> entities = new();
    private static string[] entityNames;
    public static void Add(Entity entity)
    {
        if (entities.Contains(entity))
            return;
        
        entities.Add(entity);
        entity.ID = entities.Count - 1;
        UpdateEntityNames();
    }
    
    public static void Remove(Entity entity)
    {
        if (!entities.Contains(entity))
            return;
        
        entities.Remove(entity);
        UpdateEntityNames();
    }

    public void Start()
    {
        foreach (Entity entity in entities)
        {
            if(entity.active)
                entity.Start();
        }
    }
    
    public void Update()
    {
        foreach (Entity entity in entities)
        {
            if(entity.active)
                entity.Update();
        }
    }
    
    public IEnumerator<Entity> GetEnumerator()
    {
        return entities.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public static int GetEntityCount()
    {
        return entities.Count;
    }
    
    public static Entity GetEntityByIndex(int i)
    {
        return entities[i];
    }
    
    private static void UpdateEntityNames()
    {
        entityNames = new string[entities.Count];
        for (int i = 0; i < entities.Count; i++)
        {
            entityNames[i] = entities[i].name;
        }
    }
    
    public static string[] GetEntityNames() => entityNames;
}