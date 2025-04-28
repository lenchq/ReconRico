namespace ReconRico.Components;

public class PlayerComponent : IComponent
{
    public void Destroy()
    {
        Console.WriteLine("Player is dead!");
    }
}