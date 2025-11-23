using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Replay;

public class Camera2D
{
    public Vector2 Position;

    public float Zoom = 5f;

    private Viewport viewport;

    public Camera2D(Viewport viewport)
    {
        this.viewport = viewport;
        Position = Vector2.Zero;
    }

    public void Follow(Vector2 target, Vector2 worldSize)
    {
        Position = target - new Vector2(viewport.Width / 2f / Zoom, viewport.Height / 2f / Zoom);

        Position.X = MathHelper.Clamp(Position.X, 0, worldSize.X - viewport.Width / Zoom);
        Position.Y = MathHelper.Clamp(Position.Y, 0, worldSize.Y - viewport.Height / Zoom);
    }

    public Vector2 WorldToScreen(Vector2 worldPosition)
    {
        return (worldPosition - Position) * Zoom;
    }
}
