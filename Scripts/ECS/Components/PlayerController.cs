using System;
using System.Collections.Generic;
using System.Threading;
using Coroutine;
using Microsoft.Xna.Framework;

public class PlayerController : Component
{
    public Transform transform;
    public Collider collider;
    private float xRemainder = 0;
    private float yRemainder = 0;
    public float speed = 50;
    public float maxAccel = 3;
    public float currentAccel = 0;
    public float accelerationScale = 15;
    public float deccelerationScale = 20;

    public float fallAccelerationScale = 15;
    public float currentFallAccel = 0;
    public float maxFallAccel = 3;
    public float fallMaxSpeed = 90;
    private Vector2 lastAxis;
    
    public float slideSpeed = 5;
    public float bounceOffSpeed = 40;
    public bool isBouncingOff = false;
    public bool isOnAir = false;
    public bool isJumping = false;

    // Jumping
    public float jumpHeight = 18;
    public float jumpSpeed = 10;
    public float timeUntilApex = 0.225f;
    public float jumpMaxAcceleration = 4;
    public float jumpAcceleration = 0;
    public float jumpAccelerationScale = 50;
    public float timeUntilFall = 0.1f;
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

        if(InputManager.GetInputDown(ControlScheme.A))
        {
            CoroutineHandler.Start(Jump(gameTime));
        }
        if(!collider.CheckCollideAt(transform.position + new Vector2(0, -1)) && !isJumping)
        {
            if(currentFallAccel < maxFallAccel)
            {
                currentFallAccel += Time.deltaTime * fallAccelerationScale;
            }
            MoveY(1 * fallMaxSpeed * Time.deltaTime * currentFallAccel, gameTime, null);
        }
        else
        {
            currentFallAccel = 0;
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
    public void MoveY(float amt, GameTime gameTime, Action OnCollide)
    {
        yRemainder += amt;
        var move = (int)Math.Round(yRemainder);
        if (move != 0)
        {
            yRemainder -= move;
            int sign = Math.Sign(move);
            while (move != 0)
            {
                if(collider.CheckCollideAt(transform.position + new Vector2(0, sign)) == false)
                {
                    transform.position.Y += sign;
                    move -= sign;
                }
                else
                { 
                    Console.WriteLine("Would collide!");
                    if (OnCollide != null)
                        OnCollide();
                    break;
                }
            }
        }
    }

    public IEnumerator<Wait> Jump(GameTime gameTime)
    {
        isJumping = true;
        var t0 = 0.0f;
        while(t0 < timeUntilApex)
        {
            t0 += Time.deltaTime;
            if (InputManager.GetInputUp(ControlScheme.A))
            {
                t0 = timeUntilApex;
            }
            if (jumpAcceleration < jumpAccelerationScale)
            {
                jumpAcceleration += Time.deltaTime * jumpAccelerationScale;
            }
            MoveY(-jumpSpeed * jumpHeight * Time.deltaTime, gameTime, null);
           
            yield return new Wait(Time.deltaTime);
        }
        jumpAcceleration = 0;
        var t1 = 0.0f;
        while(t1 < timeUntilFall)
        {
            t1 += Time.deltaTime;
            MoveY(-jumpHeight * Time.deltaTime * 0.4f, gameTime, null);
            yield return new Wait(Time.deltaTime);
        }
        isJumping = false;

    }
}