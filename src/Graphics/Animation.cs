using System;
using System.Collections.Generic;
using KungFuPlatform.src.Graphics;

public class Animation
{
    public TextureSheet Sheet;
    public string Name;
    public List<AnimationFrame> Frames = new();
    public bool Looping = true;
    public bool Playing = false;
    public float Speed = 1.0f;
    
    public float CurrentFrameTime = 0.0f;
    public int CurrentFrame = 0;
    
    public Action OnAnimationEnd;
    public Action OnAnimationStart;
    public Action OnAnimationLoop;
    public Animation(string name)
    {
        Name = name;
        Frames = new List<AnimationFrame>();
    }

    public Animation(string name, TextureSheet sheet)
    {
        Name = name;
        Sheet = sheet;
        Frames = new List<AnimationFrame>();
    }
    
    public static Animation Create(string name)
    {
        return new Animation(name);
    }
    
    public static Animation CreateFromSheet(string name, TextureSheet sheet, float duration = 0.1f)
    {
        Animation animation = new Animation(name, sheet);
        for(int i = 0; i < sheet.frames.Count; i++)
        {
            animation.AddFrame(sheet.frames[i], duration);
        }
        
        return animation;
    }

    public static Animation CreateFromSheet(TextureSheet sheet, float duration = 0.1f)
    {
        Animation animation = new Animation(sheet.texture.Name, sheet);
        for(int i = 0; i < sheet.frames.Count; i++)
        {
            animation.AddFrame(sheet.frames[i], duration);
        }
        return animation;
    }
    public void AddFrame(TextureSheetFrame frame, float duration = 0.1f)
    {
        Frames.Add(new AnimationFrame
        {
            Frame = frame,
            Duration = duration
        });
    }
    
    public AnimationFrame GetCurrentFrame()
    {
        return Frames[CurrentFrame];
    }

    public void SetSpeed(float speed)
    {
        Speed = speed;
    }
    
    public void SetFrameDuration(int index, float duration)
    {
        var animationFrame = Frames[index];
        animationFrame.Duration = duration;
        Frames[index] = animationFrame;
    }
    public void Update(float deltaTime)
    {
        if(Playing)
        {
            CurrentFrameTime += deltaTime * Speed;
            if(CurrentFrameTime >= Frames[CurrentFrame].Duration)
            {
                CurrentFrameTime = 0.0f;
                CurrentFrame++;
                if(CurrentFrame >= Frames.Count)
                {
                    if(Looping)
                    {
                        CurrentFrame = 0;
                        OnAnimationLoop?.Invoke();
                    }
                    else
                    {
                        CurrentFrame = Frames.Count - 1;
                        Playing = false;
                        OnAnimationEnd?.Invoke();
                    }
                }
            }
        }
    }
    
    public void Play()
    {
        Playing = true;
    }

    public void PlayFromBeginning()
    {
        CurrentFrame = 0;
        CurrentFrameTime = 0;
        
        Playing = true;
        
        OnAnimationStart?.Invoke();
    }
}

public struct AnimationFrame
{
    public TextureSheetFrame Frame;
    public float Duration;
    public void SetDuration(float dur) { Duration = dur; }
    public float GetDuration() { return Duration; }
}