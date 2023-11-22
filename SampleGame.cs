using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Num = System.Numerics;
using ImGuiNET;
using KungFuPlatform;
using Microsoft.Xna.Framework.Input;
using Coroutine;
using KungFuPlatform.Editor;
using KungFuPlatform.Editor.Windows;
using KungFuPlatform.src.Graphics;
using SDL2;

public class SampleGame : Game
{
    private GraphicsDeviceManager _graphics;
    private ImGuiRenderer _imGuiRenderer;
    private DrawQueue _drawQueue;
    private PlayerController _player;

    private SpriteBatch batch;
    private Texture2D _xnaTexture;
    private IntPtr _imGuiTexture;

    private RenderWindow gameView;
    private TextureViewerWindow textureViewerWindow;
    private ConsoleWindow consoleWindow;
    
    private float f = 0.0f;
    private int goSelection = 0;

    private bool show_test_window = false;
    private bool show_another_window = false;
    private Num.Vector3 clear_color = new Num.Vector3(114f / 255f, 144f / 255f, 154f / 255f);
    private Keys pressedKey;
    private bool isCheckingForInput;
    private Keys leftKey;
    private Keys rightKey;

    private Camera2D camera;
    public Num.Vector2 gameWindowSize = new Num.Vector2(1280, 720);
    public SampleGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.SynchronizeWithVerticalRetrace = true;
        _graphics.PreferMultiSampling = true;

        IsMouseVisible = true;
        IsFixedTimeStep = true;
        Content.RootDirectory = "Content";
    }

    protected override void Initialize()
    {
        _imGuiRenderer = new ImGuiRenderer(this);
        _imGuiRenderer.RebuildFontAtlas();
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

        InputManager.Initialize();
        
        // Create windows
        textureViewerWindow = new TextureViewerWindow(_graphics);
        consoleWindow = new ConsoleWindow();
        gameView = new RenderWindow(_graphics);
        gameView.Title = "Game View";
        gameView.Size = new Num.Vector2(1280, 720);
        gameView.Position = new Num.Vector2(0, 0);
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        batch = new SpriteBatch(GraphicsDevice);

        // Texture loading example
        _drawQueue = new DrawQueue();

        var hero = new Entity("Hero");
        hero.AddComponent<Sprite>(new Sprite(Content.Load<Texture2D>("chara_idle"), hero.transform));
        hero.AddComponent<Collider>(new Collider(hero.transform, 21, 42));
        hero.transform.position = new Vector2(40, 0);
        _player = hero.AddComponent<PlayerController>(new PlayerController());
        _player.Start();

        // Load texturesheet
        var sheet = Content.Load<Texture2D>("test_texturesheet");
        var textureSheet = new TextureSheet(sheet, 32, 32);
        var sheetEntity = new Entity("Sheet");
        sheetEntity.transform.position = new Vector2(80, 0);
        sheetEntity.AddComponent<AnimatedSprite>(new AnimatedSprite(textureSheet));
        SpawnLines();
        
        // Create camera

        var cameraObject = new Entity("Camera");
        camera = cameraObject.AddComponent<Camera2D>(new Camera2D()) as Camera2D;
        // First, load the texture as a Texture2D (can also be done using the XNA/FNA content pipeline)
        _xnaTexture = CreateTexture(GraphicsDevice, 300, 150, pixel =>
        {
            var red = (pixel % 300) / 2;
            return new Color(red, 1, 1);
        });

        // Then, bind it to an ImGui-friendly pointer, that we can use during regular ImGui.** calls (see below)
        _imGuiTexture = _imGuiRenderer.BindTexture(_xnaTexture);

        base.LoadContent();
    }

    protected override void UnloadContent()
    {
        batch.Dispose();
        _drawQueue.DisposeAll();
    }
    protected override void Update(GameTime gameTime)
    {
        // Update Systems
        Time.Update(gameTime);
        CoroutineHandler.Tick(gameTime.ElapsedGameTime.TotalSeconds);
        
        TransformSystem.Update(gameTime);
        PhysicsSystem.Update(gameTime);
        InputManager.Update(gameTime);
        AnimationSystem.Update(gameTime);
        
        _player.Update(gameTime);
        camera.Update(gameTime);

        KeyboardState keyboardCur = Keyboard.GetState();

        if(isCheckingForInput)
        {
            var pressed = keyboardCur.GetPressedKeys();

            if (pressed.Length > 0)
            {
                pressedKey = pressed[0];
                isCheckingForInput = false;
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        // Render Views

        gameView.Render(Time.deltaTime, batch, _drawQueue, camera);
        GraphicsDevice.Clear(new Color(clear_color.X, clear_color.Y, clear_color.Z, 1));
        _imGuiRenderer.BeforeLayout(gameTime);
        ImGuiLayout();
        _imGuiRenderer.AfterLayout();
        base.Draw(gameTime);
    }

    protected virtual void ImGuiLayout()
    {
        // 1. Show a simple window
        // Tip: if we don't call ImGui.Begin()/ImGui.End() the widgets appears in a window automatically called "Debug"
        ImGui.BeginMainMenuBar();
        
        if(ImGui.Button("Reset Game View"))
        {
            gameWindowSize = new Num.Vector2(1280, 720);
        }
        
        ImGui.EndMainMenuBar();

        ImGui.DockSpaceOverViewport(ImGui.GetMainViewport());
        ImGui.Begin("Test1");
        if (ImGui.Button("Check key input"))
        {
            isCheckingForInput = true;
        }
        ImGui.Text("Test key: " + pressedKey.ToString());
        ImGui.SliderFloat("Gravity Scale", ref PhysicsSystem.gravityScale, 0, 1);
        ImGui.SliderFloat("Time Scale", ref Time.timeScale, 0, 1);
        ImGui.Checkbox("Draw Debug", ref DrawQueue.DrawDebug);
        ImGui.ColorEdit3("clear color", ref clear_color);
        ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate,
            ImGui.GetIO().Framerate));
        ImGui.Image(_imGuiTexture, new Num.Vector2(300, 150), Num.Vector2.Zero, Num.Vector2.One, Num.Vector4.One, Num.Vector4.One); // Here, the previously loaded texture is used
        ImGui.End();

        ImGui.Begin("Entity Manager");
        
        if (ImGui.Button("Spawn Sprite For Batch"))
        {
            CreateNewSpriteForBatch();
        }

        if(TransformSystem.GetGameObjectNames().Length > 0)
            ImGui.ListBox("", ref goSelection, TransformSystem.GetGameObjectNames(), TransformSystem.GetComponentCount());

        var transform = TransformSystem.GetComponentByIndex(goSelection);
        transform.ImGuiLayout();

        if (ImGui.Button("Attach Camera to this Entity"))
            camera.AttachToEntity(transform.entity);

        if (ImGui.Button("Detach Camera from Entity"))
            camera.AttachToEntity(null);
        if (transform.entity.TryGetComponent(out Camera2D cam))
            cam.ImGuiLayout();
        if(transform.entity.TryGetComponent(out Collider col))
            col.ImGuiLayout();
        if (transform.entity.TryGetComponent(out PlayerController player))
            player.ImGuiLayout();
        
        ImGui.End();

        gameView.ImGuiLayout(_imGuiRenderer);
        textureViewerWindow.ImGuiLayout(_imGuiRenderer);
        consoleWindow.ImGuiLayout(_imGuiRenderer);
    }
    public static Texture2D CreateTexture(GraphicsDevice device, int width, int height, Func<int, Color> paint)
    {
        //initialize a texture
        var texture = new Texture2D(device, width, height);

        //the array holds the color for each pixel in the texture
        Color[] data = new Color[width * height];
        for (var pixel = 0; pixel < data.Length; pixel++)
        {
            //the function applies the color according to the specified pixel
            data[pixel] = paint(pixel);
        }

        //set the color
        texture.SetData(data);

        return texture;
    }

    private void CreateNewSpriteForBatch()
    {
        var random = new Random();
        var X = random.Next(0,400);
        var Y = random.Next(0,400);

        var hero = new Entity("Hero");
        hero.AddComponent<Sprite>(new Sprite(Content.Load<Texture2D>("chara_idle"), hero.transform));
        hero.transform.position = new Vector2(X, Y);
        hero.AddComponent<Collider>(new Collider(hero.transform));

    }

    private void SpawnLines()
    {
        for (int i = 1; i < 90 ; i++)
        {
            var hero = new Entity("Floor");
            var sprite = hero.AddComponent<Sprite>(new Sprite(Content.Load<Texture2D>("collider_square"), hero.transform));
            hero.AddComponent<Collider>(new Collider(hero.transform, sprite.rect.Width, sprite.rect.Height));
            var X = sprite.rect.Width * i;
            var Y = 100;
            hero.transform.position = new Vector2(X, Y);
        }
    }
}
