using System;
using Microsoft.Xna.Framework;

public class Collider : Component
{
    public Rectangle bounds;
    public Sprite sprite;
    public bool enabled = true;
    public bool checkAlways = true;
    public bool isColliding = true;
    public Collider(Transform transform)
    {
        sprite = transform.entity.GetComponent<Sprite>();
        bounds = new Rectangle((int)transform.position.X, (int)transform.position.Y, sprite.texture.Width, sprite.texture.Height);
        PhysicsSystem.Register(this);
    }

    public override void Update(GameTime gameTime)
    {
        bounds.X = (int)entity.transform.position.X;
        bounds.Y = (int)entity.transform.position.Y;

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

    public bool CheckCollideAt(Vector2 position)
    {
        var checkAt = new Rectangle((int)position.X, (int)position.Y, bounds.Width, bounds.Height);
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
}