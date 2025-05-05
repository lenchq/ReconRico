using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ReconRico;

public static class AssetsManager
{
    public static Texture2D Cursor { get; private set; }
    public static Texture2D Ball { get; private set; }
    public static Texture2D Bullet { get; private set; }

    public static SpriteFont DefaultFont { get; private set; }


    public static void Initialize(ContentManager content)
    {
        Ball = content.Load<Texture2D>("sprites/ball");
        Cursor = content.Load<Texture2D>("sprites/cursor");
        Bullet = content.Load<Texture2D>("sprites/bullet");
        
        DefaultFont = content.Load<SpriteFont>("Arial");
    }
}