using Microsoft.Xna.Framework;

public class Component
{
    public Entity entity;
    public virtual void Start()
    {

    }
    public virtual void Update()
    {

    }
    public virtual void OnDestroy()
    {
        entity = null;
    }

    public virtual void ImGuiLayout()
    {
        
    }
}
