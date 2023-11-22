using ImGuiNET;
using Microsoft.Xna.Framework;
namespace KungFuPlatform.Editor;

// A Window that doesn't require any FNA Graphic dependencies
public class Window
{
    public string Title = "Window";
    public Vector2 Size = new Vector2(800, 600);
    public Vector2 Position;
    public Vector3 ClearColor = new Vector3(114f / 255f, 144f / 255f, 154f / 255f);
    
    public bool Visible = true;

    public Window()
    {
        
    }

    public void ImGuiLayout(ImGuiRenderer renderer)
    {
        if (!Visible)
            return;

        ImGui.Begin(Title, ref Visible, ImGuiWindowFlags.DockNodeHost);
        
        Layout(renderer);
        
        ImGui.End();
    }

    //<summary>In this method, write the layout code.</summary>
    public virtual void Layout(ImGuiRenderer renderer)
    {
        
    }
}