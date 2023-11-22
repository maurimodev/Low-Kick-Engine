using System.Collections;
using System.Collections.Generic;
using Coroutine;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace KungFuPlatform.Editor.Windows;

public enum LogType
{
    Info,
    Warning,
    Error,
}

public class ConsoleWindow : Window
{
    public LogType selectedType;
    public bool Update = true;
    public ConsoleWindow()
    {
        Title = "Console";
        Size = new Vector2(500, 250);
    }

    public override void Layout(ImGuiRenderer renderer)
    {
        if (!Update)
        {
            return;
        }
        
        ImGui.BeginTabBar("Message Types:", ImGuiTabBarFlags.None);
        if (ImGui.BeginTabItem("Info"))
        {
            selectedType = LogType.Info;
            ImGui.EndTabItem();
        }

        if (ImGui.BeginTabItem("Warning"))
        {
            selectedType = LogType.Warning;
            ImGui.EndTabItem();
        }

        if (ImGui.BeginTabItem("Error"))
        {
            selectedType = LogType.Error;
            ImGui.EndTabItem();
        }

        ImGui.EndTabBar();

        ImGui.BeginChild("Console", new System.Numerics.Vector2(0,0), true, ImGuiWindowFlags.AlwaysAutoResize);

        foreach (var message in LKConsole.GetLog())
        {
            if (message.Type == selectedType)
                ImGui.Text(message);
        }
        
        ImGui.SetScrollY(ImGui.GetScrollMaxY());
        ImGui.EndChild();
    }
}

public static class LKConsole
{
    private static readonly List<LogMessage> log = new();
    
    public static List<LogMessage> GetLog()
    {
        return log;
    }
    public static void Log(string message, LogType type = LogType.Info)
    {
        log.Add(new LogMessage()
        {
            Message = message,
            Type = type,
        });

        if (log.Count > 100)
        {
            log.RemoveAt(0);
        }
    }

    public static void Clear()
    {
        log.Clear();
    }
}

public struct LogMessage
{
    public string Message;
    public LogType Type;
    
    public override string ToString()
    {
        return $"[{Type.ToString()}] {Message}";
    }
    
    public static implicit operator string(LogMessage message)
    {
        return message.ToString();
    }
}