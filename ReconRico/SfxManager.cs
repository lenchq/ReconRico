using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace ReconRico;

public static class SfxManager
{
    public static bool SoundsEnabled = true;

    private static SoundEffect Alarm { get; set; }
    private static SoundEffect Hit { get; set; }
    private static SoundEffect Pause { get; set; }
    private static SoundEffect Resume { get; set; }
    private static SoundEffect Ricochet { get; set; }
    private static SoundEffect Shoot { get; set; }
    private static SoundEffect Explosion { get; set; }
    private static SoundEffect Pickup { get; set; }

    public static void Initialize(ContentManager content)
    {
        Alarm = content.Load<SoundEffect>("sfx/aware");
        Hit = content.Load<SoundEffect>("sfx/hit");
        Pause = content.Load<SoundEffect>("sfx/pause");
        Resume = content.Load<SoundEffect>("sfx/resume");
        Shoot = content.Load<SoundEffect>("sfx/shoot");
        Ricochet = content.Load<SoundEffect>("sfx/ricochet");
        Explosion = content.Load<SoundEffect>("sfx/explosion");
        Pickup = content.Load<SoundEffect>("sfx/pickup");
    }

    public static void PlayShoot()
    {
        if (!SoundsEnabled)
            return;
        Shoot.Play(1f, 0, 0);
    }

    public static void PlayRicochet()
    {
        if (!SoundsEnabled)
            return;
        Ricochet.Play(0.3f, 0, 0);
    }

    public static void PlayExplosion()
    {
        if (!SoundsEnabled)
            return;
        Explosion.Play(0.3f, 0, 0);
    }

    public static void PlayHit()
    {
        if (!SoundsEnabled)
            return;
        Hit.Play(1f, 0, 0);
    }

    public static void PlayPause()
    {
        if (!SoundsEnabled)
            return;
        Pause.Play(0.3f, 0, 0);
    }

    public static void PlayResume()
    {
        if (!SoundsEnabled)
            return;
        Resume.Play(0.3f, 0, 0);
    }
    public static void PlayPickup()
    {
        if (!SoundsEnabled)
            return;
        Pickup.Play(1f, 0, 0);
    }
    public static void PlayAlarm()
    {
        if (!SoundsEnabled)
            return;
        Alarm.Play(1f, 0, 0);
    }
}