using System;
using Microsoft.Xna.Framework;

public class Collider : Component
{
    public Rectangle bounds;
    public Vector2 origin;
    public Vector2 offset;
    public Vector2 scale;
    public bool enabled = true;
    public bool checkAlways = true;
    public bool isColliding = true;
    public Collider(Transform transform)
    {
        bounds = new Rectangle((int)transform.position.X, (int)transform.position.Y, 21,21);
        origin = new Vector2(0, 0);
        scale = new Vector2(1,1);
        PhysicsSystem.Register(this);
        DrawQueue.RegisterForDebugDraw(this);
    }

    public Collider(Transform transform, int width, int height)
    {
        bounds = new Rectangle((int)transform.position.X, (int)transform.position.Y, width, height);
        origin = new Vector2(0, 0);
        scale = new Vector2(1, 1);
        PhysicsSystem.Register(this);
        DrawQueue.RegisterForDebugDraw(this);
    }

    public override void Update(GameTime gameTime)
    {
        bounds.X = (int)entity.transform.position.X + (int)offset.X * (int)scale.X;
        bounds.Y = (int)entity.transform.position.Y + (int)offset.Y * (int)scale.Y;

        if (checkAlways && enabled)
        {
            foreach (var component in PhysicsSystem.GetColliders())
            {
                if (component == this)
                {
                    continue;
                }

                if (bounds.Intersects(component.bounds))
                {
                    PhysicsSystem.OnCollision.Invoke(this, component);
                    break;
                }
            }
        }
    }

    public bool CheckCollideAt(Vector2 position, out Collider collidesWith)
    {
        var checkAt = new Rectangle((int)position.X + (int)offset.X, (int)position.Y + (int)offset.Y, bounds.Width * (int)scale.X, bounds.Height * (int)scale.Y);
        Console.WriteLine(checkAt);

        foreach (var component in PhysicsSystem.GetColliders())
        {
            if (component == this)
            {
                continue;
            }

            if (checkAt.Intersects(component.bounds))
            {
                collidesWith = component;
                return true;
            }
        }
        collidesWith = null;
        return false;
    }
    public bool CheckCollideAt(Vector2 position)
    {
        var checkAt = new Rectangle((int)position.X + (int)offset.X, (int)position.Y + (int)offset.Y, bounds.Width * (int)scale.X, bounds.Height * (int)scale.Y);
        Console.WriteLine(checkAt);

        foreach (var component in PhysicsSystem.GetColliders())
        {
            if (component == this)
            {
                continue;
            }

            if (checkAt.Intersects(component.bounds))
            {
                return true;
            }
        } 
        return false;
    }

    public void ImGuiLayout()
    {
        var numOffset = offset.TranslateVector2();
        var numScale = scale.TranslateVector2();
        ImGuiNET.ImGui.InputFloat2("Offset", ref numOffset);
        ImGuiNET.ImGui.DragFloat2("Scale", ref numScale, 1);
        scale = numScale.TranslateVector2();
        offset = numOffset.TranslateVector2();
    }
}