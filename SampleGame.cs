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
using KungFuPlatform.ECS.Systems;
using KungFuPlatform.Editor;
using KungFuPlatform.Editor.Windows;
using KungFuPlatform.src.Graphics;
using SDL2;

public class SampleGame : Game
{
    private Editor _editor;
    
    private GraphicsDeviceManager _graphics;

    private SpriteBatch batch;
    private EntityManager entityManager;

    private Camera2D camera;
    public SampleGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 980;
        _graphics.SynchronizeWithVerticalRetrace = true;
        _graphics.PreferMultiSampling = true;
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += (sender, args) =>
        {
            _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            _graphics.ApplyChanges();
        };

        IsMouseVisible = true;
        IsFixedTimeStep = true;
        Content.RootDirectory = "Content";
    }

    protected override void Initialize()
    {
        InputManager.Initialize();
        // Create Entity Manager
        entityManager = new EntityManager();
        
        // Create Editor
        _editor = new Editor(this, _graphics);
        _editor.Initialize();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        batch = new SpriteBatch(GraphicsDevice);

        // Texture loading example

        var hero = new Entity("Hero");
        var playerSheet = new TextureSheet(Content.Load<Texture2D>("Textures/player_sheet"), 50,37);
        hero.AddComponent<AnimatedSprite>(new AnimatedSprite(playerSheet));
        hero.AddComponent<Collider>(new Collider(hero.transform, 21, 42));
        hero.transform.position = new Vector2(40, 0);
        hero.AddComponent<PlayerController>(new PlayerController());

        // Load texturesheet
        var sheet = Content.Load<Texture2D>("test_texturesheet");
        var textureSheet = new TextureSheet(sheet, 32, 32);
        var sheetEntity = new Entity("Sheet");
        sheetEntity.transform.position = new Vector2(80, 0);
        sheetEntity.AddComponent<AnimatedSprite>(new AnimatedSprite(textureSheet));
        SpawnLines();
        
        // Create camera

        var cameraObject = new Entity("Camera");
        camera = cameraObject.AddComponent<Camera2D>(new Camera2D());
        base.LoadContent();
    }

    protected override void UnloadContent()
    {
        batch.Dispose();
        DrawQueue.DisposeAll();
    }
    protected override void Update(GameTime gameTime)
    {
        // Update Systems
        Time.Update(gameTime);
        
        InputManager.Update();
        
        entityManager.Update();

    }

    protected override void Draw(GameTime gameTime)
    {
        // Render Views
        if (Editor.CurrentMode == EDITOR_MODE.GAME)
            _editor.RenderToGameView(batch, camera);
        
        _editor.Layout(gameTime);
        base.Draw(gameTime);
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
