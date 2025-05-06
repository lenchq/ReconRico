using Microsoft.Xna.Framework;
using ReconRico.Components;

namespace ReconRico.Systems;

public class GunSystem
{
    public const float BULLET_SPEED = 1100f;
    
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
            var bulletOffset = Vector2.Rotate(Vector2.UnitY * -35f, entPos.Rotation);
            var bullet = EntityDirector.CreateBullet(entPos.Position + bulletOffset,
                entPos.Rotation, Vector2.Rotate(-Vector2.UnitY * BULLET_SPEED, entPos.Rotation), gun.RicochetTimes);
            EntityManager.AddEntity(bullet);
            gun.Ammo -= 1;
            gun.LastShoot = gameTime.TotalGameTime.TotalMilliseconds;
            gun.ShootRequested = false;

            SfxManager.PlayShoot();
        }
    }

    public static Vector2 GetWallReflectedVelocity(Vector2 velocity, ColliderComponent bulletCollider,
        TransformComponent bulletPos, ColliderComponent targetCollider, TransformComponent targetPos,
        VelocityComponent bulletVel)
    {
        var bulletRect = bulletCollider.GetColliderRectangle(bulletPos);
        var targetRect = targetCollider.GetColliderRectangle(targetPos);

        float penetrationX = Math.Min(
            bulletRect.Right - targetRect.Left,
            targetRect.Right - bulletRect.Left
        );
        float penetrationY = Math.Min(
            bulletRect.Bottom - targetRect.Top,
            targetRect.Bottom - bulletRect.Top
        );

        Vector2 wallNormal;
        if (penetrationX < penetrationY)
            // X collision
            wallNormal = new Vector2(
                bulletPos.Position.X < targetPos.Position.X ? -1 : 1,
                0
            );
        else
            // Y collision
            wallNormal = new Vector2(
                0,
                bulletPos.Position.Y < targetPos.Position.Y ? -1 : 1
            );
        var reflectedVelocity = Vector2.Reflect(bulletVel.Velocity, wallNormal);
        reflectedVelocity = Vector2.Normalize(reflectedVelocity) * velocity.Length();
        return reflectedVelocity;
    }
}