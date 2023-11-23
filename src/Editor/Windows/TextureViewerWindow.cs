using System.Collections.Generic;
using System.IO;
using System.Numerics;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace KungFuPlatform.Editor.Windows;

public class TextureViewerWindow : Window
{
    private List<Texture2D> textureFiles = new List<Texture2D>();
    private int selectedTexture = 0;
    private GraphicsDeviceManager Graphics;
    
    public TextureViewerWindow(GraphicsDeviceManager graphics)
    {
        Title = "Texture Viewer";
        Graphics = graphics;
    }
    public void LoadTextures()
    {
        textureFiles = new List<Texture2D>();

        var files = Directory.GetFiles("Content/Textures");
        
        // Create a file stream and read each file in files

        foreach (var file in files)
        {
            using (var fileStream = new FileStream(file, FileMode.Open))
            {
                var texture = Texture2D.FromStream(Graphics.GraphicsDevice, fileStream);
                texture.Name = Path.GetFileNameWithoutExtension(file);
                textureFiles.Add(texture);
                fileStream.Dispose();
            }
        }
    }
    
    public List<string> GetTextureNames()
    {
        List<string> names = new List<string>();
        foreach (var tex in textureFiles)
        {
            names.Add(tex.Name);
        }
        return names;
    }
    
    public void DisposeTextures()
    {
    }
    public override void Layout(ImGuiRenderer renderer)
    {
        if(ImGui.Button("Load Textures"))
            LoadTextures();

        if (textureFiles.Count > 0)
            ImGui.ListBox("Textures", ref selectedTexture, GetTextureNames().ToArray(), textureFiles.Count);
        else
            return;

        var tex = renderer.BindTexture(textureFiles[selectedTexture]);
        ImGui.Image(tex, new Vector2(textureFiles[selectedTexture].Width, textureFiles[selectedTexture].Height), Vector2.Zero, Vector2.One);

    }
}