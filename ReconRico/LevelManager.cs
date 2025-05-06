using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using ReconRico.Components;
using ReconRico.General;

namespace ReconRico;

public class LevelData
{
    public List<LevelEntity> Entities { get; set; }
}

public class LevelEntity
{
    public string Type { get; set; }
    public float[] Position { get; set; }
    public float Rotation { get; set; }
    public float[]? Velocity { get; set; }
    public float[]? Size { get; set; }
    public float[][]? PatrolPoints { get; set; }
}

public static class LevelManager
{
    private const string LevelDirectory = "Levels";
    private const int MaxWidth = GameSettings.WINDOW_WIDTH;
    private const int MaxHeight = GameSettings.WINDOW_HEIGHT;

    public static void LoadLevel(string levelName)
    {
        var path = Path.Combine(LevelDirectory, levelName + ".yml");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Level file not found: {path}");
        }

        var yaml = File.ReadAllText(path);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var levelData = deserializer.Deserialize<LevelData>(yaml);

        var hasPlayer = false;

        foreach (var entity in levelData.Entities)
        {
            Vector2 position = new(entity.Position[0], entity.Position[1]);
            var rotation = 0;

            if (position.X < 0 || position.X > MaxWidth || position.Y < 0 || position.Y > MaxHeight)
            {
                Console.WriteLine($"Warning: Entity {entity.Type} position {position} is outside level bounds.");
                continue;
            }

            Entity? createdEntity = null;
            switch (entity.Type)
            {
                case "player":
                    createdEntity = EntityDirector.CreatePlayer(position, rotation);
                    hasPlayer = true;
                    break;

                case "bullet":
                    Vector2 velocity = new(entity.Velocity[0], entity.Velocity[1]);
                    createdEntity = EntityDirector.CreateBullet(position, rotation, velocity, 0);
                    break;

                case "solid_wall":
                    Vector2 solidWallSize = new(entity.Size[0], entity.Size[1]);
                    createdEntity = EntityDirector.CreateSolidWallObstacle(position, solidWallSize, rotation);
                    break;

                case "pass_breakable_wall":
                    Vector2 passBreakableWall = new(entity.Size[0], entity.Size[1]);
                    createdEntity =
                        EntityDirector.CreatePassBreakableWallObstacle(position, passBreakableWall, rotation);
                    break;
                case "absorb_breakable_wall":
                    Vector2 absorbBreakableWallSize = new(entity.Size[0], entity.Size[1]);
                    createdEntity =
                        EntityDirector.CreateAbsorbBreakableWallObstacle(position, absorbBreakableWallSize, rotation);
                    break;

                case "reflector_wall":
                    Vector2 reflectorWallSize = new(entity.Size[0], entity.Size[1]);
                    createdEntity = EntityDirector.CreateReflectorWallObstacle(position, reflectorWallSize, rotation);
                    break;

                case "reflector_breakable_wall":
                    Vector2 reflectorBreakableWallSize = new(entity.Size[0], entity.Size[1]);
                    createdEntity =
                        EntityDirector.CreateReflectorBreakableWallObstacle(position, reflectorBreakableWallSize,
                            rotation);
                    break;
                case "enemy":
                    var patrolPoints = entity.PatrolPoints?
                        .Select(patrolCoords
                            => new Vector2(patrolCoords[0], patrolCoords[1]))
                        .ToArray();
                    createdEntity = EntityDirector.CreateEnemy(position, patrolPoints);
                    break;
                case "door":
                    Vector2 doorSize = new(entity.Size[0], entity.Size[1]);
                    createdEntity = EntityDirector.CreateDoor(position, doorSize);
                    break;

                default:
                    Console.WriteLine($"Unknown entity type: {entity.Type}");
                    break;
            }

            if (createdEntity is not null)
                EntityManager.AddEntity(createdEntity);
        }

        EntityManager.AddEntity(EntityDirector.CreateCursor());
        if (!hasPlayer)
            throw new InvalidDataException("Level must contain a player.");
    }
}