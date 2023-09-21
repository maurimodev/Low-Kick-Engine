using System;
using System.Collections.Generic;

public class PhysicsSystem : BaseSystem<Collider>
{
    public static Action<Collider, Collider> OnCollision = Action;
    public static float gravityScale = 1.0f;
    private static void Action(Collider arg1, Collider arg2)
    {
        Console.WriteLine("There has been a collision between {0} and {1}.", arg1.entity.name, arg2.entity.name);
    }

    public static List<Collider> GetColliders()
    {
        return components;
    }

}
