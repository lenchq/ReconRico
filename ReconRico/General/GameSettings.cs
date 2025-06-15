using Microsoft.Xna.Framework.Input;

namespace ReconRico.General;

static class GameSettings
{
    public const bool DEBUG = false;

    #region GRAPHICS

    public const int WINDOW_WIDTH = 1280;
    public const int WINDOW_HEIGHT = 720;

    #endregion

    public const int LEVEL_COUNT = 5;

    #region PLAYER CONTROLS

    public const Keys PLAYER_MOVE_UP_KEY = Keys.W;
    public const Keys PLAYER_MOVE_DOWN_KEY = Keys.S;
    public const Keys PLAYER_MOVE_LEFT_KEY = Keys.A;
    public const Keys PLAYER_MOVE_RIGHT_KEY = Keys.D;

    public const float PLAYER_MOVE_VERTICAL_SPEED = 0.25f;
    public const float PLAYER_MOVE_HORIZONTAL_SPEED = 0.25f;

    public const int GUN_SHOOT_DELAY_MS = 500;

    public const Keys CAMERA_CONTROL_FLY = Keys.LeftShift;

    public const bool IS_MOUSE_VISIBLE = false;
    public const bool GRAB_MOUSE = true;

    #endregion

    #region DEBUG CONSTS

#if DEBUG
    public const bool COLLIDER_GIZMO = true;
    public const bool TRANSFORM_GIZMO = true;
#else
    public const bool COLLIDER_GIZMO = false;
    public const bool TRANSFORM_GIZMO = false;
#endif

    #endregion
}