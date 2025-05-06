using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ReconRico.Components;
using ReconRico.General;
using System.IO;
using System.Security.Cryptography;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ReconRico.Systems;

public class LevelEditorSystem
{
    private enum EditorState
    {
        Idle,
        WaitingForWidth,
        WaitingForHeight
    }

    private EditorState _currentState = EditorState.Idle;
    private Vector2 _wallCenter;
    private float _wallWidth;
    private MouseState _previousMouseState;
    private readonly string _levelEditorPath = Path.GetFullPath("Levels/_leveleditor.yml");

    public void Update(Point windowPosition, GameTime gameTime)
    {
        if (!GameSettings.DEBUG)
            return;

        var currentMouseState = Mouse.GetState();
        var mousePosition = new Vector2(
            currentMouseState.X,
            currentMouseState.Y
        );
        var relativeMousePos = mousePosition - new Vector2(windowPosition.X, windowPosition.Y);
        if (GameSettings.DEBUG)
            Console.WriteLine(mousePosition);

        // Check for right mouse button press
        if (currentMouseState.RightButton == ButtonState.Pressed &&
            _previousMouseState.RightButton == ButtonState.Released)
        {
            switch (_currentState)
            {
                case EditorState.Idle:
                    _wallCenter = mousePosition;
                    _currentState = EditorState.WaitingForWidth;
                    break;

                case EditorState.WaitingForWidth:
                    _wallWidth = Math.Abs(mousePosition.X - _wallCenter.X);
                    _currentState = EditorState.WaitingForHeight;
                    break;

                case EditorState.WaitingForHeight:
                    var wallHeight = Math.Abs(mousePosition.Y - _wallCenter.Y);
                    CreateWall(_wallCenter, new Vector2(_wallWidth * 2, wallHeight * 2));
                    _currentState = EditorState.Idle;
                    break;
            }
        }

        _previousMouseState = currentMouseState;
    }

    private void CreateWall(Vector2 center, Vector2 size)
    {
        // Create the wall entity
        var wall = EntityDirector.CreateSolidWallObstacle(center, size, 0);
        EntityManager.AddEntity(wall);

        // Write to YAML file
        WriteToYamlFile(center, size);
    }

    private void WriteToYamlFile(Vector2 position, Vector2 size)
    {
        var entity = new LevelEditorEntity
        {
            Type = "solid_wall",
            Position = new[] { position.X, position.Y },
            Rotation = 0,
            Size = new[] { size.X, size.Y }
        };

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        LevelEditorData levelData;
        if (File.Exists(_levelEditorPath))
        {
            var yaml = File.ReadAllText(_levelEditorPath);
            levelData = deserializer.Deserialize<LevelEditorData>(yaml);
        }
        else
        {
            levelData = new LevelEditorData { Entities = new List<LevelEditorEntity>() };
        }

        // Add new entity
        levelData.Entities.Add(entity);

        // Write back to file
        var newYaml = serializer.Serialize(levelData);
        File.WriteAllText(_levelEditorPath, newYaml);
    }
}