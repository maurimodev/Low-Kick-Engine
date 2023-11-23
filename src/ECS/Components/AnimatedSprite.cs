using System;
using KungFuPlatform.src.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class AnimatedSprite : Component
{
    public Texture2D defaultSprite = null;
    public Animation activeAnimation;
    public AnimatedSprite(TextureSheet sheet)
    {
        activeAnimation = Animation.CreateFromSheet(sheet);
        DrawQueue.RegisterForDraw(this);
        AnimationSystem.Register(this);

        activeAnimation.SetFrameDuration(6,0.5f);
        activeAnimation.SetFrameDuration(2, 0.5f);
        
        activeAnimation.OnAnimationStart += () => { Console.WriteLine("Animation Started!"); };
        activeAnimation.OnAnimationEnd += () => { Console.WriteLine("Animation Ended!"); };
        activeAnimation.OnAnimationLoop += () => { 
            Console.WriteLine("Animation Loop!");
            activeAnimation.Looping = false;
        };
        
        activeAnimation.PlayFromBeginning();
    }
    
    public TextureSheetFrame GetCurrentFrame()
    {
        return activeAnimation.GetCurrentFrame().Frame;
    }
    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        activeAnimation.Update(Time.deltaTime);
    }
    
    public override void OnDestroy()
    {

    }
}