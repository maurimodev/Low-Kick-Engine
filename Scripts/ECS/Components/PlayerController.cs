using System;
using System.Threading;
using Microsoft.Xna.Framework;

public class PlayerController : Component
{
    public Transform transform;
    public Collider collider;
    private float xRemainder = 0;
    public float speed = 50;
    public float maxAccel = 3;
    public float currentAccel = 0;
    public float accelerationScale = 15;
    public float deccelerationScale = 20;

    private Vector2 lastAxis;
    
    public float slideSpeed = 5;
    public float bounceOffSpeed = 40;
    public bool isBouncingOff = false;
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
            lastAxis = horizontalAxis;
            if (currentAccel < maxAccel)
            {
                currentAccel += (float)gameTime.ElapsedGameTime.TotalSeconds * accelerationScale;
            }
            else
            {
                currentAccel = maxAccel;
            }
            MoveX(horizontalAxis.X * speed * (float)gameTime.ElapsedGameTime.TotalSeconds * currentAccel, gameTime, null);
        }
        else
        {
            if (currentAccel > 0)
            {
                currentAccel -= (float)gameTime.ElapsedGameTime.TotalSeconds * deccelerationScale;
            }
            else
            {
                currentAccel = 0;
            }
            MoveX(lastAxis.X * slideSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * currentAccel, gameTime, null);
        }
    }
    public void MoveX(float amt, GameTime gameTime, Action<int, GameTime> onCollide)
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
                        onCollide(sign, gameTime);
                    break;
                }
            }
        }
    }
}