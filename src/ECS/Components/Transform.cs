using Microsoft.Xna.Framework;

public class Transform : Component
{
    public Vector2 position = Vector2.Zero;
    public Vector2 scale = Vector2.One;
    public int layerDepth = 0;
    public float rotation = 0;

    public Transform(Entity entity)
    {
        this.entity = entity;
        TransformSystem.Register(this);
    }

    public void ImGuiLayout()
    {
        var numPos = position.TranslateVector2();
        var numScale = scale.TranslateVector2();
        ImGuiNET.ImGui.DragFloat2("Position", ref numPos);
        position = numPos.TranslateVector2();
        ImGuiNET.ImGui.DragFloat("Rotation", ref rotation, 0.05f);
        ImGuiNET.ImGui.DragFloat2("Scale", ref numScale);
        scale = numScale.TranslateVector2();
    }
}