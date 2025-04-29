using Microsoft.Xna.Framework;
using ReconRico.Components;

namespace ReconRico.Systems;

public class GunSystem
{
    public void Update(GameTime gameTime)
    {
        foreach (var (entity, gun) in EntityManager.GetEntitiesWithAll(
                         typeof(TransformComponent), typeof(GunComponent))
                     .Select(ent => (ent, gun: ent.GetComponent<GunComponent>()))
                     .ToArray())
        {
            var entPos = entity.GetComponent<TransformComponent>();
            if (!gun.ShootRequested
                || gun.Ammo == 0) continue;

            if (gun.LastShoot is not null
                && gun.LastShoot + gun.ShootDelay > gameTime.TotalGameTime.TotalMilliseconds)
            {
                gun.ShootRequested = false;
                continue;
            }

            Console.WriteLine("shoot");
            var bullet = EntityDirector.CreateBullet(entPos.Position,
                entPos.Rotation, Vector2.Rotate(-Vector2.UnitY * 4.5f, entPos.Rotation));
            EntityManager.AddEntity(bullet);
            gun.Ammo -= 1;
            gun.LastShoot = gameTime.TotalGameTime.TotalMilliseconds;
            gun.ShootRequested = false;
        }
    }
}