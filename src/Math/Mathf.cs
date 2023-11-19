using System;

public static class Mathf
{
    public static float Lerp(float a, float b, float t)
    {
        return (1.0f - t) * a + b * t;
    }

    public static float InverseLerp(float a, float b, float v)
    {
        return (v - a) / (b - a);
    }
    
    public static float Clamp(float v, float min, float max)
    {
        return Math.Max(min, Math.Min(max, v));
    }
    
    public static float Clamp01(float v)
    {
        return Clamp(v, 0.0f, 1.0f);
    }

    public static float Remap(float iMin, float iMax, float oMin, float oMax, float v)
    {
        float t = InverseLerp(iMin, iMax, v);
        return Lerp(oMin, oMax, t);
    }
}