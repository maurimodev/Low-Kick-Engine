using System;
using System.Collections.Generic;
using System.Threading;
using Coroutine;
using ImGuiNET;
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
    public float fallMaxSpeed = 60;
    private Vector2 lastAxis;
    
    public float slideSpeed = 5;
    public bool isGrounded = false;
    public bool isJumping = false;

    // Jumping
    public float jumpHeight = 5;
    public float jumpSpeed = 10;
    public float timeUntilApex = 0.150f;
    public float jumpMaxAcceleration = 4;
    public float jumpAcceleration = 0;
    public float jumpAccelerationScale = 60;
    public float jumpDeccelerationScale = 20;
    public float timeUntilFall = 0.06f;
    public float coyoteTimeThreshold = 0.2f;

    public int cornerThreshold = 4;

    public float coyoteTimer = 0;
    public bool usedUpCoyote = false;
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
        var axis = InputManager.GetAxis(Axis.LEFT_STICK);

        // HORIZONTAL MOVEMENT
        if (axis.X != 0)
        {
            lastAxis = axis;
            if (currentAccel < maxAccel)
            {
                currentAccel += Time.deltaTime * accelerationScale;
            }
            else
            {
                currentAccel = maxAccel;
            }
            MoveX(axis.X * speed * Time.deltaTime * currentAccel, null);
        }
        else
        {
            if (currentAccel > 0)
            {
                currentAccel -= Time.deltaTime * deccelerationScale;
            }
            else
            {
                currentAccel = 0;
            }
            MoveX(lastAxis.X * slideSpeed * Time.deltaTime * currentAccel, null);
        }

        // JUMP INPUT
        if(InputManager.GetInputDown(ControlScheme.A))
        {
            if(isGrounded && !isJumping)
                CoroutineHandler.Start(Jump(gameTime));
            else
            {
                if (coyoteTimer > 0 & !usedUpCoyote)
                {
                    CoroutineHandler.Start(Jump(gameTime));
                    usedUpCoyote = true;
                }
            }

        }

        // GRAVITY / GROUNDED CHECK
        if(!collider.CheckCollideAt(transform.position + new Vector2(0, 1)) && !isJumping)
        {
            isGrounded = false;
            if(currentFallAccel < maxFallAccel)
            {
                currentFallAccel += Time.deltaTime * fallAccelerationScale;
            }
            MoveY(1 * fallMaxSpeed * Time.deltaTime * currentFallAccel * PhysicsSystem.gravityScale, OnFallCollide);
        }
        else
        {
            isGrounded = true;
            
            if (!isJumping)
                usedUpCoyote = false;

            coyoteTimer = coyoteTimeThreshold;
            currentFallAccel = 0;
        }

        if (!isGrounded && coyoteTimer > 0)
        {
            coyoteTimer -= Time.deltaTime;
        }
    }
    public void MoveX(float amt, Action<int, float> onCollide)
    {
        xRemainder += amt;
        var move = (int)Math.Round(xRemainder);
        if (move != 0)
        {
            xRemainder -= move;
            int sign = Math.Sign(move);
            while (move != 0)
            {
                if(collider.CheckCollideAt(new Vector2(transform.position.X + sign, transform.position.Y)) == false)
                {
                    transform.position.X += sign;
                    move -= sign;
                }
                else
                {
                    //Console.WriteLine("Would collide!");
                    if (onCollide != null)
                        onCollide(sign, Time.deltaTime);
                    break;
                }
            }
        }
    }
    public void MoveY(float amt, Action<Collider> OnCollide)
    {
        yRemainder += amt;
        var move = (int)yRemainder;
        if (move != 0)
        {
            yRemainder -= move;
            int sign = Math.Sign(move);
            while (move != 0)
            {
                if (collider.CheckCollideAt(new Vector2(transform.position.X, transform.position.Y + sign), out var collide))
                {
                    //Console.WriteLine("Would collide!");

                    if (OnCollide != null)
                        OnCollide(collide);

                    if (isJumping)
                    {
                        if (!CheckCornerCorrection(collide))
                        {
                            isJumping = false;
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    transform.position.Y += sign;
                    move -= sign;
                }
            }
        }
    }

    public IEnumerator<Wait> Jump(GameTime gameTime)
    {
        usedUpCoyote = true;
        isJumping = true;
        var t0 = 0.0f;
        jumpAcceleration = jumpMaxAcceleration / 1.5f;
        while(t0 < timeUntilApex)
        {
            t0 += Time.deltaTime;
            if (InputManager.GetInputUp(ControlScheme.A) || !isJumping)
            {
                t0 = timeUntilApex;
            }
            if (jumpAcceleration >= jumpMaxAcceleration)
            {
                jumpAcceleration -= Time.deltaTime * jumpDeccelerationScale;
            }
            else
            {
                jumpAcceleration += Time.deltaTime * jumpAccelerationScale;
            }
            MoveY(-jumpSpeed * jumpHeight * jumpAcceleration * Time.deltaTime, null);
            yield return new Wait(Time.deltaTime);
        }
        jumpAcceleration = 0;
        var t1 = 0.0f;

        while(t1 < timeUntilFall)
        {
            if (InputManager.GetInputUp(ControlScheme.A))
                t1 = timeUntilFall;
            t1 += Time.deltaTime;
            MoveY(jumpSpeed * -jumpHeight * Time.deltaTime * 0.4f, null);
            yield return new Wait(Time.deltaTime);
        }
        isJumping = false;
    }

    public bool CheckCornerCorrection(Collider other)
    {
        // Check corners, slide player to left/right if needbe

        var playerRect = collider.bounds;
        var otherRect = other.bounds;

        // Player is to the left of the other rect
        if (playerRect.Location.X < otherRect.Location.X)
        {
            if (Math.Abs(playerRect.Right - otherRect.Left) <= cornerThreshold)
            {
                if (collider.CheckCollideAt(transform.position + new Vector2(-Math.Abs(playerRect.Right - otherRect.Left), 0)))
                    return false;

                MoveX(-(Math.Abs(playerRect.Right - otherRect.Left)), null);
                return true;
            }
        }
        else
        {
            if (Math.Abs(playerRect.Left - otherRect.Right) <= cornerThreshold)
            {
                if (collider.CheckCollideAt(transform.position + new Vector2(Math.Abs(playerRect.Left - otherRect.Right), 0)))
                    return false;
                MoveX(Math.Abs(playerRect.Left - otherRect.Right), null);
                return true;
            }
        }
        return false;
    }

    public void ImGuiLayout()
    {
        ImGui.BeginGroup();
        ImGui.DragFloat("Player Speed", ref speed);
        ImGui.DragFloat("Acceleration Scale", ref accelerationScale);
        ImGui.LabelText("Current Acceleration:", currentAccel.ToString());
        ImGui.BeginTabBar("Tab");
        var jumpMenuOn = ImGui.BeginMenu("Jump Properties");
        if (jumpMenuOn)
        {
            ImGui.DragFloat("Jump Speed", ref jumpSpeed);
            ImGui.DragFloat("Jump Height", ref jumpHeight);
            ImGui.DragFloat("Jump Acceleration Scale", ref jumpAccelerationScale);
            ImGui.DragFloat("Jump Time Until Apex", ref timeUntilApex);
            ImGui.DragFloat("Time Until Fall", ref timeUntilFall);
            ImGui.DragFloat("Coyote Time Threshold", ref coyoteTimeThreshold);
            ImGui.LabelText("Coyote Timer", coyoteTimer.ToString());
            ImGui.LabelText("Used Up Coyote", usedUpCoyote.ToString());
            ImGui.LabelText("Is Grounded: ", isGrounded.ToString());
            ImGui.LabelText("Is Jumping: ", isJumping.ToString());
        }
        ImGui.EndMenu();
        ImGui.EndTabBar();
    }
    // ON COLLIDE
    public void OnFallCollide(Collider collideWith)
    {
        MoveY(-Math.Abs(collideWith.bounds.Top - collider.bounds.Bottom), null);
    }
}