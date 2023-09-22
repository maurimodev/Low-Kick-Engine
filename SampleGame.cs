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

public class SampleGame : Game
{
    private GraphicsDeviceManager _graphics;
    private ImGuiRenderer _imGuiRenderer;
    private DrawQueue _drawQueue;
    private PlayerController _player;

    private SpriteBatch batch;
    private Texture2D _xnaTexture;
    private IntPtr _imGuiTexture;

    private float f = 0.0f;
    private int goSelection = 0;

    private bool show_test_window = false;
    private bool show_another_window = false;
    private Num.Vector3 clear_color = new Num.Vector3(114f / 255f, 144f / 255f, 154f / 255f);
    private byte[] _textBuffer = new byte[100];
    private Keys pressedKey;
    private bool isCheckingForInput;
    private Keys leftKey;
    private Keys rightKey;

    private Camera2D camera;
    public SampleGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.SynchronizeWithVerticalRetrace = true;
        _graphics.PreferMultiSampling = true;

        IsMouseVisible = true;

        Content.RootDirectory = "Content";
    }

    protected override void Initialize()
    {
        _imGuiRenderer = new ImGuiRenderer(this);
        _imGuiRenderer.RebuildFontAtlas();
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

        InputManager.Initialize();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        batch = new SpriteBatch(GraphicsDevice);

        // Texture loading example
        _drawQueue = new DrawQueue();

        var hero = new Entity("Hero");
        hero.AddComponent<Sprite>(new Sprite(Content.Load<Texture2D>("chara_idle")));
        hero.AddComponent<Collider>(new Collider(hero.transform));
        hero.transform.position = new Vector2(40, 0);
        _player = hero.AddComponent<PlayerController>(new PlayerController());
        _player.Start();

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
        PhysicsSystem.Update(gameTime);
        TransformSystem.Update(gameTime);
        InputManager.Update(gameTime);
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
        GraphicsDevice.Clear(new Color(clear_color.X, clear_color.Y, clear_color.Z, 1));

        batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
            DepthStencilState.DepthRead, RasterizerState.CullNone, null, camera.GetTransformation(GraphicsDevice));
        _drawQueue.DrawAllInQueue(batch);

        batch.End();

        _imGuiRenderer.BeforeLayout(gameTime);
        ImGuiLayout();
        _imGuiRenderer.AfterLayout();

        base.Draw(gameTime);


    }

    protected virtual void ImGuiLayout()
    {
        // 1. Show a simple window
        // Tip: if we don't call ImGui.Begin()/ImGui.End() the widgets appears in a window automatically called "Debug"
        ImGui.Begin("Test1");
        if (ImGui.Button("Check key input"))
        {
            isCheckingForInput = true;
        }
        ImGui.Text("Test key: " + pressedKey.ToString());
        ImGui.SliderFloat("float", ref f, 0.0f, 1.0f, string.Empty);
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
        var numPos = transform.position.TranslateVector2();
        
        ImGui.DragFloat2("Position", ref numPos);
        transform.position = numPos.TranslateVector2();
        ImGui.DragFloat("Rotation", ref transform.rotation, 0.05f);

        if (transform.entity.ContainsComponent<Camera2D>())
        {
            ImGui.DragFloat("Zoom", ref camera.zoom, 0.05f);
        }
        else
        {
            var numScale = transform.scale.TranslateVector2();
            ImGui.DragFloat2("Scale", ref numScale);
            transform.scale = numScale.TranslateVector2();
            if (transform.entity.ContainsComponent<PlayerController>())
            {
                if(ImGui.Button("Attach Camera to this Entity"))
                    camera.AttachToEntity(_player.entity);

                if (ImGui.Button("Detach Camera from Entity"))
                    camera.AttachToEntity(null);

                ImGui.BeginGroup();
                ImGui.DragFloat("Player Speed", ref _player.speed);
                ImGui.DragFloat("Acceleration Scale", ref _player.accelerationScale);
                ImGui.LabelText("Current Acceleration:",  _player.currentAccel.ToString());
                ImGui.BeginTabBar("Tab");
                var jumpMenuOn = ImGui.BeginMenu("Hi");
                if (jumpMenuOn)
                {
                    ImGui.DragFloat("Jump Height", ref _player.jumpHeight);
                    ImGui.DragFloat("Jump Time until Apex", ref _player.timeUntilApex);
                    ImGui.DragFloat("Coyote Time Threshold", ref _player.coyoteTimeThreshold);
                    ImGui.LabelText("Coyote Timer", _player.coyoteTimer.ToString());
                    ImGui.LabelText("Used Up Coyote", _player.usedUpCoyote.ToString());
                    ImGui.LabelText("Is Grounded: ", _player.isGrounded.ToString());
                }
                ImGui.EndMenu();
                ImGui.EndTabBar();
            }
        }
        
        ImGui.End();
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
        hero.AddComponent<Sprite>(new Sprite(Content.Load<Texture2D>("chara_idle")));
        hero.transform.position = new Vector2(X, Y);
        hero.AddComponent<Collider>(new Collider(hero.transform));

    }

    private void SpawnLines()
    {
        for (int i = 1; i < 90 ; i++)
        {
            var hero = new Entity("Floor");
            hero.AddComponent<Sprite>(new Sprite(Content.Load<Texture2D>("chara_idle")));
            hero.AddComponent<Collider>(new Collider(hero.transform));
            var width = hero.GetComponent<Sprite>().rect.Width;
            var X = width * i;
            var Y = 100;
            hero.transform.position = new Vector2(X, Y);
        }
    }
}
