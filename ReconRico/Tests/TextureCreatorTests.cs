// using System;
// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using Moq;
// using NUnit.Framework;
// using ReconRico.General;
//
// namespace ReconRico.Tests;
//
// [TestFixture]
// public class TextureCreatorTests
// {
//     private TextureCreator _textureCreator;
//     private GraphicsDevice _graphicsDevice;
//
//     [SetUp]
//     public void Setup()
//     {
//         // Create a real GraphicsDevice instance for testing
//         var presentationParameters = new PresentationParameters
//         {
//             BackBufferWidth = 800,
//             BackBufferHeight = 600,
//             DeviceWindowHandle = IntPtr.Zero
//         };
//         _graphicsDevice = new GraphicsDevice(
//             GraphicsAdapter.DefaultAdapter,
//             GraphicsProfile.Reach,
//             presentationParameters
//         );
//
//         var spriteBatch = new SpriteBatch(_graphicsDevice);
//         _textureCreator = new TextureCreator(spriteBatch);
//     }
//
//     [TearDown]
//     public void TearDown()
//     {
//         _graphicsDevice?.Dispose();
//     }
//
//     [Test]
//     public void CreateBorderedRectangle_WithNoBorder_CreatesSolidTexture()
//     {
//         // Arrange
//         var width = 10;
//         var height = 10;
//         var fillColor = Color.Red;
//
//         // Act
//         var texture = _textureCreator.CreateBorderedRectangle(width, height, fillColor);
//
//         // Assert
//         Assert.That(texture, Is.Not.Null);
//         Assert.That(texture.Width, Is.EqualTo(width));
//         Assert.That(texture.Height, Is.EqualTo(height));
//
//         var colors = new Color[width * height];
//         texture.GetData(colors);
//
//         foreach (var color in colors)
//         {
//             Assert.That(color, Is.EqualTo(fillColor));
//         }
//     }
//
//     [Test]
//     public void CreateBorderedRectangle_WithBorder_CreatesBorderedTexture()
//     {
//         // Arrange
//         var width = 10;
//         var height = 10;
//         var fillColor = Color.Red;
//         var borderColor = Color.Blue;
//         var border = 2;
//
//         // Act
//         var texture = _textureCreator.CreateBorderedRectangle(width, height, fillColor, border, borderColor);
//
//         // Assert
//         Assert.That(texture, Is.Not.Null);
//         Assert.That(texture.Width, Is.EqualTo(width));
//         Assert.That(texture.Height, Is.EqualTo(height));
//
//         var colors = new Color[width * height];
//         texture.GetData(colors);
//
//         // Check border pixels
//         for (var y = 0; y < height; y++)
//         {
//             for (var x = 0; x < width; x++)
//             {
//                 var index = y * width + x;
//                 var expectedColor = IsBorderPixel(x, y, width, height, border) ? borderColor : fillColor;
//                 Assert.That(colors[index], Is.EqualTo(expectedColor), 
//                     $"Pixel at ({x}, {y}) should be {expectedColor}");
//             }
//         }
//     }
//
//     [Test]
//     public void CreateBorderedRectangle_WithTransparentColors_CreatesTransparentTexture()
//     {
//         // Arrange
//         var width = 10;
//         var height = 10;
//
//         // Act
//         var texture = _textureCreator.CreateBorderedRectangle(width, height);
//
//         // Assert
//         Assert.That(texture, Is.Not.Null);
//         Assert.That(texture.Width, Is.EqualTo(width));
//         Assert.That(texture.Height, Is.EqualTo(height));
//
//         var colors = new Color[width * height];
//         texture.GetData(colors);
//
//         foreach (var color in colors)
//         {
//             Assert.That(color, Is.EqualTo(Color.Transparent));
//         }
//     }
//
//     [Test]
//     public void CreateBorderedRectangle_WithSameParameters_ReturnsCachedTexture()
//     {
//         // Arrange
//         var width = 10;
//         var height = 10;
//         var fillColor = Color.Red;
//         var borderColor = Color.Blue;
//         var border = 2;
//
//         // Act
//         var texture1 = _textureCreator.CreateBorderedRectangle(width, height, fillColor, border, borderColor);
//         var texture2 = _textureCreator.CreateBorderedRectangle(width, height, fillColor, border, borderColor);
//
//         // Assert
//         Assert.That(texture2, Is.SameAs(texture1), "Second call should return cached texture");
//     }
//
//     [Test]
//     public void CreateBorderedRectangle_WithDifferentParameters_CreatesNewTexture()
//     {
//         // Arrange
//         var width = 10;
//         var height = 10;
//         var fillColor1 = Color.Red;
//         var fillColor2 = Color.Blue;
//
//         // Act
//         var texture1 = _textureCreator.CreateBorderedRectangle(width, height, fillColor1);
//         var texture2 = _textureCreator.CreateBorderedRectangle(width, height, fillColor2);
//
//         // Assert
//         Assert.That(texture2, Is.Not.SameAs(texture1), "Different parameters should create new texture");
//     }
//
//     [Test]
//     public void CreateBorderedRectangle_WithZeroDimensions_ThrowsArgumentException()
//     {
//         // Act & Assert
//         Assert.Throws<ArgumentException>(() => _textureCreator.CreateBorderedRectangle(0, 10));
//         Assert.Throws<ArgumentException>(() => _textureCreator.CreateBorderedRectangle(10, 0));
//         Assert.Throws<ArgumentException>(() => _textureCreator.CreateBorderedRectangle(0, 0));
//     }
//
//     [Test]
//     public void CreateBorderedRectangle_WithNegativeBorder_ThrowsArgumentException()
//     {
//         // Act & Assert
//         Assert.Throws<ArgumentException>(() => _textureCreator.CreateBorderedRectangle(10, 10, border: -1));
//     }
//
//     [Test]
//     public void CreateBorderedRectangle_WithBorderLargerThanDimensions_ThrowsArgumentException()
//     {
//         // Act & Assert
//         Assert.Throws<ArgumentException>(() => _textureCreator.CreateBorderedRectangle(10, 10, border: 6));
//     }
//
//     private static bool IsBorderPixel(int x, int y, int width, int height, int border)
//     {
//         return x < border || x >= width - border || y < border || y >= height - border;
//     }
// } 