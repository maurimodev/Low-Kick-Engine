using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace KungFuPlatform.Editor;

public class RenderWindow
{
    public string Title = "Render Window";
    public GraphicsDeviceManager Graphics;
    public Vector2 Size = new Vector2(800, 600);
    public Vector2 Position;
    public Vector3 ClearColor = new Vector3(114f / 255f, 144f / 255f, 154f / 255f);

    public bool Visible = true;

    private bool lockedSize = false;
    
    private RenderTarget2D RenderTarget;
    
    public RenderWindow(GraphicsDeviceManager graphics)
    {
        Graphics = graphics;
    }
    
    public void Render(float deltaTime, SpriteBatch batch, DrawQueue queue, Camera2D camera)
    {
        if (!Visible)
            return;
        
        RenderTarget = new RenderTarget2D(Graphics.GraphicsDevice, (int)Size.X, (int)Size.Y);

        Graphics.PreferredBackBufferWidth = (int)Size.X;
        Graphics.PreferredBackBufferHeight = (int)Size.Y;
        
        Graphics.GraphicsDevice.SetRenderTarget(RenderTarget);
        Graphics.GraphicsDevice.Clear(new Color(ClearColor.X, ClearColor.Y, ClearColor.Z, 1));
        
        batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
            DepthStencilState.DepthRead, RasterizerState.CullNone, null, camera.GetTransformation(Graphics.GraphicsDevice));

        queue.DrawAllInQueue(batch);
        
        batch.End();

        Graphics.GraphicsDevice.SetRenderTarget(null);
    }

    public void ImGuiLayout(ImGuiRenderer imGuiRenderer)
    {
        if (!Visible)
            return;
        
        ImGui.Begin(Title, ref Visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.DockNodeHost | ImGuiWindowFlags.NoMove);
        // Create menu top bar
        
        
        ImGui.BeginTabBar("View Options", ImGuiTabBarFlags.None);

        if (ImGui.TabItemButton("Lock Aspect"))
            lockedSize = true;

        if (ImGui.TabItemButton("Unlock Aspect"))
            lockedSize = false;
        
        ImGui.EndTabBar();

        if (lockedSize)
            Size = new Vector2(640, 360);
        else
            Size = ImGui.GetWindowSize();
        
        ImGui.SetNextWindowContentSize(Size);
        var renderPtr = imGuiRenderer.BindTexture(RenderTarget); 
        ImGui.Image(renderPtr, Size);
        ImGui.End();
    }
    
}