using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using Coroutine;
using System.Collections.Generic;
using System;

public class Camera2D : Component
{
    public float zoom;
    public Matrix matrixTransform;
    public bool isAttached;
    public bool isLerping = false;
    private Entity attachedEntity;
    public Camera2D()
    {
        zoom = 3;
    }

    public float Zoom
    {
        get => zoom;
        set 
        { 
            zoom = value;
            if (zoom < 0.1) zoom = 0.1f;
        }
    }

    public void Move(Vector2 dir)
    {
        entity.transform.position += dir;
    }

    public void AttachToEntity(Entity entity)
    {
        if (entity == null)
        {
            isAttached = false;
            attachedEntity = null;
            return;
        }
        CoroutineHandler.Start(LerpBetweenPositions(this.entity.transform, entity.transform, 4));
        isAttached = true;
        attachedEntity = entity;
    }
    public override void Update(GameTime gameTime)
    {
        if (isAttached && !isLerping)
        {
            entity.transform.position = attachedEntity.transform.position;
        }
    }

    public IEnumerator<Wait> LerpBetweenPositions(Transform current, Transform goal, float speed)
    {
        isLerping = true;
        float t = 0;
        while(t < 1)
        {
            System.Numerics.Vector2 lerpPos = System.Numerics.Vector2.Lerp(current.position.TranslateVector2(), goal.position.TranslateVector2(), t);
            entity.transform.position = lerpPos.TranslateVector2();
            t += speed * Time.deltaTime;
            yield return new Wait(Time.deltaTime);
        }
        isLerping = false;
        Console.WriteLine("Finished routine!");

    }
    public Matrix GetTransformation(GraphicsDevice device)
    {
        matrixTransform = Matrix.CreateTranslation(new Vector3(-entity.transform.position.X, -entity.transform.position.Y, 0)) *
                          Matrix.CreateRotationZ(entity.transform.rotation) *
                          Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                          Matrix.CreateTranslation(new Vector3(device.Viewport.Width * 0.5f,
                              device.Viewport.Height * 0.5f, 0));
        return matrixTransform;
    }

    public void ImGuiLayout()
    {
        ImGuiNET.ImGui.DragFloat("Zoom", ref zoom);
    }
}