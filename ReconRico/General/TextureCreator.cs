using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReconRico.General;

public class TextureCreator(SpriteBatch spriteBatch)
{
    private readonly Dictionary<(Point size, Color? fillColor, int border, Color? borderColor), Texture2D>
        TextureCache = new();

    public Texture2D CreateBorderedRectangle(int width, int height, Color? fillColor = null, int border = 0,
        Color? borderColor = null)
    {
        var cacheKey = (new Point(width, height), fillColor, border, borderColor);
        if (TextureCache.TryGetValue(cacheKey, out var texture))
            return texture;

        fillColor ??= Color.Transparent;
        borderColor ??= Color.Transparent;

        var rect = new Texture2D(spriteBatch.GraphicsDevice, width, height);
        var colors = new Color[height][];
        for (var i = 0; i < height; i++)
            colors[i] = Enumerable.Repeat((Color)fillColor, width).ToArray();

        for (var i = 0; i < height; i++)
        for (var j = 0; j < width; j++)
            if (i < border || i >= height - border ||
                j < border || j >= width - border)
                colors[i][j] = (Color)borderColor;

        rect.SetData(colors
            .SelectMany(x => x)
            .ToArray());

        TextureCache[cacheKey] = rect;

        return rect;
    }
}