using System;
using System.IO;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using ReconRico;
using ReconRico.Components;

namespace ReconRico.Tests;

public class LevelManagerTests : IDisposable
{
    private const string TestLevelDirectory = "Levels";
    private const string TestLevelName = "test_level";

    public LevelManagerTests()
    {
        if (!Directory.Exists(TestLevelDirectory))
        {
            Directory.CreateDirectory(TestLevelDirectory);
        }
    }

    [SetUp]
    public void LevelMangerTestsSetup()
    {
        EntityManager.Entities.Clear();
    }

    public void Dispose()
    {
        var testLevelPath = Path.Combine(TestLevelDirectory, TestLevelName + ".yml");
        if (File.Exists(testLevelPath))
        {
            File.Delete(testLevelPath);
        }
    }

    [Test]
    public void LoadLevel_ValidLevelFile_LoadsSuccessfully()
    {
        var validLevelYaml = @"
entities:
  - type: player
    position: [100, 100]
    rotation: 0
  - type: solid_wall
    position: [200, 200]
    rotation: 0
    size: [50, 50]";
        File.WriteAllText(Path.Combine(TestLevelDirectory, TestLevelName + ".yml"), validLevelYaml);

        try
        {
            LevelManager.LoadLevel(TestLevelName);
        }
        catch
        {
            Assert.Fail();
        }
    }

    [Test]
    public void LoadLevel_MissingLevelFile_ThrowsFileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(() => LevelManager.LoadLevel("nonexistent_level"));
    }

    [Test]
    public void LoadLevel_NoPlayer_ThrowsInvalidDataException()
    {
        var levelWithoutPlayer = @"
entities:
  - type: solid_wall
    position: [200, 200]
    rotation: 0
    size: [50, 50]";
        File.WriteAllText(Path.Combine(TestLevelDirectory, TestLevelName + ".yml"), levelWithoutPlayer);

        Assert.Throws<InvalidDataException>(() => LevelManager.LoadLevel(TestLevelName));
    }

    [Test]
    public void LoadLevel_EntityOutsideBounds_LogsWarning()
    {
        var levelWithOutOfBoundsEntity = @"
entities:
  - type: player
    position: [100, 100]
    rotation: 0
  - type: solid_wall
    position: [9999, 9999]
    rotation: 0
    size: [50, 50]";
        File.WriteAllText(Path.Combine(TestLevelDirectory, TestLevelName + ".yml"), levelWithOutOfBoundsEntity);

        LevelManager.LoadLevel(TestLevelName);

        Assert.That(EntityManager.Entities.Count, Is.EqualTo(2));
    }

    [Test]
    public void LoadLevel_UnknownEntityType_DoNotAdd()
    {
        var levelWithUnknownEntity = @"
entities:
  - type: player
    position: [100, 100]
    rotation: 0
  - type: unknown_entity
    position: [200, 200]
    rotation: 0";
        File.WriteAllText(Path.Combine(TestLevelDirectory, TestLevelName + ".yml"), levelWithUnknownEntity);

        LevelManager.LoadLevel(TestLevelName);

        var entities = EntityManager.GetEntitiesWithComponent<PlayerComponent>();
        Assert.That(entities.Count(), Is.EqualTo(1));
        Assert.That(EntityManager.Entities.Count, Is.EqualTo(2));
    }

    [Test]
    public void LoadLevel_AllEntityTypes_CreatesCorrectEntities()
    {
        var levelWithAllEntities = @"
entities:
  - type: player
    position: [100, 100]
    rotation: 0
  - type: bullet
    position: [150, 150]
    rotation: 0
    velocity: [1, 1]
  - type: solid_wall
    position: [200, 200]
    rotation: 0
    size: [50, 50]
  - type: pass_breakable_wall
    position: [250, 250]
    rotation: 0
    size: [50, 50]
  - type: absorb_breakable_wall
    position: [300, 300]
    rotation: 0
    size: [50, 50]
  - type: reflector_wall
    position: [350, 350]
    rotation: 0
    size: [50, 50]
  - type: reflector_breakable_wall
    position: [400, 400]
    rotation: 0
    size: [50, 50]
  - type: enemy
    position: [450, 450]
    rotation: 0
    patrolPoints: [[460, 460], [470, 470]]
  - type: door
    position: [500, 500]
    rotation: 0
    size: [50, 50]";
        File.WriteAllText(Path.Combine(TestLevelDirectory, TestLevelName + ".yml"), levelWithAllEntities);

        LevelManager.LoadLevel(TestLevelName);

        Assert.That(EntityManager.GetEntitiesWithComponent<PlayerComponent>().Count(), Is.EqualTo(1));
        Assert.That(EntityManager.GetEntitiesWithComponent<ProjectileComponent>().Count(), Is.EqualTo(1));
        Assert.That(EntityManager.GetEntitiesWithComponent<ObstacleComponent>().Count(), Is.EqualTo(5));
        Assert.That(EntityManager.GetEntitiesWithComponent<EnemyComponent>().Count(), Is.EqualTo(1));
        Assert.That(EntityManager.GetEntitiesWithComponent<DoorComponent>().Count(), Is.EqualTo(1));
    }
}