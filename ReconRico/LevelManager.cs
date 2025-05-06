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
                    createdEntity = EntityDirector.CreateBullet(position, rotation, velocity);
                    break;

                case "solid_wall":
                    Vector2 size1 = new(entity.Size[0], entity.Size[1]);
                    createdEntity = EntityDirector.CreateSolidWallObstacle(position, size1, rotation);
                    break;

                case "breakable_wall":
                    Vector2 size2 = new(entity.Size[0], entity.Size[1]);
                    createdEntity = EntityDirector.CreateBreakableWallObstacle(position, size2, rotation);
                    break;

                case "reflector_wall":
                    Vector2 size3 = new(entity.Size[0], entity.Size[1]);
                    createdEntity = EntityDirector.CreateReflectorWallObstacle(position, size3, rotation);
                    break;

                case "reflector_breakable_wall":
                    Vector2 size4 = new(entity.Size[0], entity.Size[1]);
                    createdEntity =
                        EntityDirector.CreateReflectorBreakableWallObstacle(position, size4, rotation);
                    break;
                case "enemy":
                    var patrolPoints = entity.PatrolPoints?
                        .Select(patrolCoords
                            => new Vector2(patrolCoords[0], patrolCoords[1]))
                        .ToArray();
                    createdEntity = EntityDirector.CreateEnemy(position, patrolPoints);

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