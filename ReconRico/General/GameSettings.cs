using Microsoft.Xna.Framework.Input;

namespace ReconRico.General;

static class GameSettings
{
    #region PLAYER CONTROLS

    public const Keys PLAYER_MOVE_UP_KEY = Keys.W;
    public const Keys PLAYER_MOVE_DOWN_KEY = Keys.S;
    public const Keys PLAYER_MOVE_LEFT_KEY = Keys.A;
    public const Keys PLAYER_MOVE_RIGHT_KEY = Keys.D;

    public const float PLAYER_MOVE_VERTICAL_SPEED = 0.35f;
    public const float PLAYER_MOVE_HORIZONTAL_SPEED = 0.35f;

    public const Keys CAMERA_CONTROL_FLY = Keys.LeftShift;

    #endregion

    #region DEBUG CONSTS

#if DEBUG
    public const bool HITBOX_GIZMO = true;
    public const bool TRANSFORM_GIZMO = true;
#else
    public const bool HITBOX_GIZMO = false;
    public const bool TRANSFORM_GIZMO = false;
#endif

    #endregion
}