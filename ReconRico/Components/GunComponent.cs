namespace ReconRico.Components;

public class GunComponent(int maxAmmo, int shootDelay, int ricochetTimes = 0) : IComponent
{
    public bool ShootRequested { get; set; } = false;
    public double? LastShoot { get; set; } = null;
    public int ShootDelay { get; set; } = shootDelay;
    public int MaxAmmo { get; set; } = maxAmmo;
    public int Ammo { get; set; } = maxAmmo;
    public int RicochetTimes { get; set; } = ricochetTimes;
}