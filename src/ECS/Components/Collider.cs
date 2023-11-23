using System;
using ImGuiNET;
using KungFuPlatform.Scripts.Math;
using Microsoft.Xna.Framework;

public class Collider : Component
{
    public RectangleF bounds;
    public Vector2 origin;
    public Vector2 offset;
    public Vector2 scale;
    public bool enabled = true;
    public bool checkAlways = true;
    public bool isColliding = true;
    public Collider(Transform transform)
    {
        bounds = new RectangleF(transform.position.X, transform.position.Y, 21,21);
        origin = new Vector2(0, 0);
        scale = new Vector2(1,1);
        PhysicsSystem.Register(this);
        DrawQueue.RegisterForDebugDraw(this);
    }

    public Collider(Transform transform, int width, int height)
    {
        bounds = new RectangleF(transform.position.X, transform.position.Y, width, height);
        origin = new Vector2(0, 0);
        scale = new Vector2(1, 1);
        PhysicsSystem.Register(this);
        DrawQueue.RegisterForDebugDraw(this);
    }

    public override void Update()
    {
        bounds.X = entity.transform.position.X + offset.X;
        bounds.Y = entity.transform.position.Y + offset.Y;

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
        var checkAt = new RectangleF(position.X + offset.X, position.Y + offset.Y, bounds.Width * scale.X * entity.transform.scale.X, bounds.Height * scale.Y * entity.transform.scale.Y);
        //Console.WriteLine(checkAt);

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
        var checkAt = new RectangleF(position.X + offset.X, position.Y + offset.Y, bounds.Width * scale.X * entity.transform.scale.X, bounds.Height * scale.Y * entity.transform.scale.Y);
        //Console.WriteLine(checkAt);

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

    public override void ImGuiLayout()
    {
        var numOffset = offset.TranslateVector2();
        var numScale = scale.TranslateVector2();
        ImGuiNET.ImGui.DragFloat2("Collider Offset", ref numOffset, 0.1f);
        ImGuiNET.ImGui.DragFloat2("Collider Scale", ref numScale, 0.1f);
        scale = numScale.TranslateVector2();
        offset = numOffset.TranslateVector2();
    }
}