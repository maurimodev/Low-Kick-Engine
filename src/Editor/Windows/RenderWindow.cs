using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace KungFuPlatform.Editor;

public class RenderWindow : Window
{
    private GraphicsDeviceManager Graphics;

    private bool lockedSize = true;
    
    private RenderTarget2D RenderTarget;

    private new Vector2 Size = new Vector2(800, 600);
    private new Vector2 Position;
    private new Vector3 ClearColor = new Vector3(114f / 255f, 144f / 255f, 154f / 255f);
    
    public RenderWindow(GraphicsDeviceManager graphics)
    {
        Title = "Game View";
        Graphics = graphics;
        RenderTarget = new RenderTarget2D(Graphics.GraphicsDevice, 1920, 1080);
    }
    
    public void Render(SpriteBatch batch, Camera2D camera)
    {
        if (!Visible)
            return;

        Graphics.GraphicsDevice.SetRenderTarget(RenderTarget);
        Graphics.GraphicsDevice.Clear(new Color(ClearColor.X, ClearColor.Y, ClearColor.Z, 1));
        DrawQueue.DrawAllInQueue(batch, camera, Graphics.GraphicsDevice);
        Graphics.GraphicsDevice.SetRenderTarget(null);
        
        Graphics.GraphicsDevice.Clear(new Color(ClearColor.X, ClearColor.Y, ClearColor.Z, 1));

        batch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
        batch.Draw(RenderTarget, new Rectangle(0, 0, (int)Size.X, (int)Size.Y), null, Color.White);
        batch.End();

    }

    public override void Layout(ImGuiRenderer imGuiRenderer)
    {
        if (!Visible)
            return;
        
        ImGui.Begin(Title, ref Visible, ImGuiWindowFlags.DockNodeHost | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMove);
        // Create menu top bar
        
        ImGui.BeginTabBar("View Options", ImGuiTabBarFlags.None);

        if (ImGui.TabItemButton("1920x1080"))
            lockedSize = true;

        if (ImGui.TabItemButton("Free Aspect"))
            lockedSize = false;
        
        ImGui.EndTabBar();

        Size = ImGui.GetContentRegionAvail();
        
        // Lock size to aspect ratio 16:9
        if (lockedSize)
        {
            var aspect = 16f / 9f;
            var aspectSize = new Vector2(Size.X, Size.X / aspect);
            if (aspectSize.Y > Size.Y)
            {
                aspectSize = new Vector2(Size.Y * aspect, Size.Y);
            }

            Size = aspectSize;
        }
        
        ImGui.SetCursorPos(ImGui.GetCursorPos() + (ImGui.GetContentRegionAvail() - Size) * 0.5f);
        var renderPtr = imGuiRenderer.BindTexture(RenderTarget);
        ImGui.Image(renderPtr, Size, Vector2.Zero, Vector2.One);
        ImGui.End();
        
    }
    
}