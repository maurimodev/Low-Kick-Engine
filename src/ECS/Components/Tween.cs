using System;
using System.Collections;
using ImGuiNET;
using KungFuPlatform.Editor.Windows;
using Microsoft.Xna.Framework;

public class Tween : Component
{
    public EaseType easeType;
    
    public Action<Tween> OnStart;
    public Action<Tween> OnUpdate;
    public Action<Tween> OnComplete;

    public bool Active { get; private set; }
    public bool UseUnscaledTime { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsComplete { get; private set; }
    public bool Reverse { get; private set; }
    public bool Started { get; private set; }

    public float Duration { get; private set; }
    public float Speed { get; private set; }
    public float Percent { get; private set; }
    public float Eased { get; private set; }
    public float Delay { get; private set; }

    private float _timeLeft;
    
    public TweenMode Mode { get; private set; }
    
    public static Tween Create(TweenMode mode, EaseType ease = EaseType.Linear, float duration = 1f, float delay = 0f, bool useUnscaledTime = false, bool start = true)
    {
        var tween = new Tween();
        tween.Mode = mode;
        tween.easeType = ease;
        tween.Duration = duration;
        tween.Delay = delay;
        tween.UseUnscaledTime = useUnscaledTime;
        tween._timeLeft = duration;
        
        return tween;
    }
    
    public static Tween Position(Entity entity, Vector2 targetPosition, float duration = 1, EaseType easeType = EaseType.Linear,
        TweenMode mode = TweenMode.OneShot)
    {
        Vector2 startPosition = entity.transform.position;
        var tween = Create(mode, easeType, duration);
        tween.OnUpdate = t =>
            entity.transform.position = Vector2.Lerp(startPosition, targetPosition, t.Eased);
        entity.AddComponent<Tween>(tween);
        return tween;
    }
    
    public static Tween Scale(Entity entity, Vector2 targetScale, float duration = 1, EaseType easeType = EaseType.Linear,
        TweenMode mode = TweenMode.OneShot)
    {
        Vector2 startScale = entity.transform.scale;
        var tween = Create(mode, easeType, duration);
        tween.OnUpdate = (Action<Tween>)(t =>
            entity.transform.scale = Vector2.Lerp(startScale, targetScale, t.Eased));
        return tween;
    }

    public override void Start()
    {
        if (Started)
        {
            return;
        }
        
        Started = true;
        Active = true;
        IsPaused = false;
        IsComplete = false;
        Reverse = false;
        Speed = 1f;
        Percent = 0f;
        Eased = 0f;
        
        OnStart?.Invoke(this);
    }
    public override void Update()
    {
        if (Delay > 0)
        {
            Delay -= (UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * Speed;
            return;
        }
        
        _timeLeft -= UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        Percent = Math.Max(0.0f, _timeLeft / Duration);
        
        Percent = 1f - Percent;
        
        Eased = Ease.Get(easeType, Percent);
        
        LKConsole.Log(Percent.ToString());
        
        OnUpdate?.Invoke(this);
        
        if (_timeLeft > 0.0f)
            return;

        _timeLeft = 0.0f;
        OnComplete?.Invoke(this);

        if (Mode == TweenMode.OneShot)
        {
            Active = false;
            IsComplete = true;
            entity.RemoveComponent(this);
            
        }
        
        // TODO: Support different modes!
    }

    public void Stop() => this.Active = false;

    public void Reset()
    {
        _timeLeft = Duration;
        Eased = Percent = 0.0f;
    }

    public IEnumerator Wait()
    {
        Tween tween = this;
        while (tween.Active)
            yield return null;
    }

    public override void ImGuiLayout()
    {
        ImGui.LabelText("Tween", $"{this.Mode} - {this.Duration} - {this.Percent}");
    }
}

public enum TweenMode
{
    Loop,
    OneShot,
    YoyoOneShot,
    YoyoLoop,
    Hold
}