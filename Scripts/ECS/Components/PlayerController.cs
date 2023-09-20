using System;
using Microsoft.Xna.Framework;

public class PlayerController : Component
{
    public Transform transform;
    public Collider collider;
    private float xRemainder = 0;
    public float speed = 50;
    public PlayerController()
    {
    }

    public override void Start()
    {
        transform = entity.transform;
        collider = entity.GetComponent<Collider>();
    }
    public override void Update(GameTime gameTime)
    {
        var horizontalAxis = InputManager.GetAxis(Axis.LEFT_STICK);

        if (horizontalAxis.X != 0)
        {
            MoveX(horizontalAxis.X * speed * (float)gameTime.ElapsedGameTime.TotalSeconds, null);
        }
    }

    public void MoveX(float amt, Action onCollide)
    {
        xRemainder += amt;
        var move = (int)Math.Round(xRemainder);
        if (move != 0)
        {
            xRemainder -= move;
            int sign = Math.Sign(move);
            while (move != 0)
            {
                if(collider.CheckCollideAt(transform.position + new Vector2(sign, 0)) == false)
                {
                    transform.position.X += sign;
                    move -= sign;
                }
                else
                {
                    Console.WriteLine("Would collide!");
                    if (onCollide != null)
                        onCollide();
                    break;
                }
            }
        }
    }
}