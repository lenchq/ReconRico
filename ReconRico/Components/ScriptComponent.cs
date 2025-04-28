using Microsoft.Xna.Framework;

namespace ReconRico.Components;

public class ScriptComponent : IComponent
{
    public Action<Entity, GameTime> OnUpdate { get; set; }
}