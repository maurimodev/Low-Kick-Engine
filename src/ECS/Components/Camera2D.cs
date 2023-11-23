using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using Coroutine;
using System.Collections.Generic;
using System;
using KungFuPlatform.Editor.Windows;

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

    public void AttachToEntity(Entity entity)
    {
        if (entity == null)
        {
            isAttached = false;
            attachedEntity = null;
            return;
        }
        CoroutineHandler.Start(LerpBetweenPositions(this.entity.transform, entity.transform, 1));
        isAttached = true;
        attachedEntity = entity;
    }
    public override void Update()
    {
        if (isAttached && !isLerping)
        {
            entity.transform.position = attachedEntity.transform.position;
        }
    }

    public IEnumerator<Wait> LerpBetweenPositions(Transform current, Transform goal, float duration, EaseType ease = EaseType.OutCirc)
    {
        isLerping = true;
        var tween = Tween.Position(entity, goal.position, duration, ease);
        Event endEvent = new Event();
        tween.OnComplete = _ => CoroutineHandler.RaiseEvent(endEvent);
        LKConsole.Log("Started lerping");
        yield return new Wait(endEvent);
        isLerping = false;
        LKConsole.Log("Done lerping");

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

    public override void ImGuiLayout()
    {
        ImGuiNET.ImGui.DragFloat("Zoom", ref zoom);
    }
}