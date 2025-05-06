using System.Collections.Generic;

namespace ReconRico.General;

public class LevelEditorData
{
    public List<LevelEditorEntity> Entities { get; set; } = new();
}

public class LevelEditorEntity
{
    public string Type { get; set; }
    public float[] Position { get; set; }
    public float Rotation { get; set; }
    public float[] Size { get; set; }
} 