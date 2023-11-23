using System.Collections.Generic;
using ImGuiNET;
using KungFuPlatform;
using KungFuPlatform.ECS.Systems;
using KungFuPlatform.Editor;
using KungFuPlatform.Editor.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public enum EDITOR_MODE
{
    GAME,
    TEXTURE_EDITOR
}

public class Editor
{
    public static EDITOR_MODE CurrentMode = EDITOR_MODE.GAME;
    public static void RequestLayout(EDITOR_MODE mode) => CurrentMode = mode;
    
    private Game CurrentGame;
    private GraphicsDeviceManager Graphics;
    private ImGuiRenderer ImGuiRenderer;
    
    private GameEditorInfo gameEditorInfo;
    private TextureEditorInfo textureEditorInfo;
    
    // Constructor
    public Editor(Game game, GraphicsDeviceManager graphics)
    {
        CurrentGame = game;
        Graphics = graphics;
        ImGuiRenderer = new ImGuiRenderer(CurrentGame);
        ImGuiRenderer.RebuildFontAtlas();
        
        gameEditorInfo = new GameEditorInfo(graphics, ImGuiRenderer);
        textureEditorInfo = new TextureEditorInfo(graphics, ImGuiRenderer);
    }

    public void Initialize()
    {
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DpiEnableScaleViewports;
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
    }
    
    public void Layout(GameTime gameTime)
    {
        ImGuiRenderer.BeforeLayout(gameTime);
        // Main Menu Bar
        ImGui.Begin("LowKick");
        ImGui.BeginMainMenuBar();
        
        if(ImGui.Button("Game"))
        {
            CurrentMode = EDITOR_MODE.GAME;
        }

        if (ImGui.Button("Texture Editor"))
        {
            CurrentMode = EDITOR_MODE.TEXTURE_EDITOR;
        }
        
        ImGui.EndMainMenuBar();
        // Set up dock space
        ImGui.DockSpaceOverViewport(ImGui.GetMainViewport());
        
        switch (CurrentMode)
        {
            case EDITOR_MODE.GAME:
                GameViewLayout();
                break;
            case EDITOR_MODE.TEXTURE_EDITOR:
                break;
        }
        
        ImGui.End();
        ImGuiRenderer.AfterLayout();
    }
    
    private void GameViewLayout()
    {
        // Docked windows
        ImGui.Begin("Test1");
        
        ImGui.SliderFloat("Gravity Scale", ref PhysicsSystem.gravityScale, 0, 1);
        ImGui.SliderFloat("Time Scale", ref Time.timeScale, 0, 1);
        ImGui.Checkbox("Draw Debug", ref DrawQueue.DrawDebug);
        ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate,
            ImGui.GetIO().Framerate));
        ImGui.End();

        ImGui.Begin("Entity Manager");
        
        if(EntityManager.GetEntityCount() > 0)
            ImGui.ListBox("", ref gameEditorInfo.goSelection, EntityManager.GetEntityNames(), EntityManager.GetEntityCount());

        var entity = EntityManager.GetEntityByIndex(gameEditorInfo.goSelection);
        
        entity.ImGuiLayout();
        gameEditorInfo.ImGuiLayout();
    }

    public void RenderToGameView(SpriteBatch batch, Camera2D camera2D)
    {
        gameEditorInfo.RenderToGameView(batch, camera2D);
    }
}

public class EditorInfo
{
    public List<Window> Windows;
    protected ImGuiRenderer _renderer;
    
    public void ImGuiLayout()
    {
        foreach (var window in Windows)
        {
            window.ImGuiLayout(_renderer);
        }
    }
}

public class GameEditorInfo : EditorInfo
{
    public int goSelection = 0;
    
    public GameEditorInfo(GraphicsDeviceManager graphics, ImGuiRenderer renderer)
    {
        Windows = new List<Window>()
        {
            new RenderWindow(graphics),
            new ConsoleWindow(),
            new TextureViewerWindow(graphics),
        };
        _renderer = renderer;
    }
    
    public void RenderToGameView(SpriteBatch batch, Camera2D camera2D)
    {
        // Render the game view
        ((RenderWindow)Windows[0]).Render(batch, camera2D);
    }
}
public class TextureEditorInfo : EditorInfo
{
    public int selectedTexture = 0;

    public TextureEditorInfo(GraphicsDeviceManager graphics, ImGuiRenderer renderer)
    {
        _renderer = renderer;
    }
}