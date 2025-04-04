using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReconRico.Components;

public class SpriteComponent : IComponent
{
    public Texture2D Texture { get; set; }
    public Color ColorMask { get; set; } = Color.White;
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
}