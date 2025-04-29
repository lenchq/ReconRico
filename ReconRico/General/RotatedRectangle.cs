using Microsoft.Xna.Framework;
using System;

public struct RotatedRectangle
{
    public Vector2 Position;
    public Vector2 Size;

    /// <summary>
    /// Rotation in radians
    /// </summary>
    public float Rotation;

    private readonly Vector2[] _corners;
    private readonly Vector2[] _axes;

    // These properties now account for rotation by using the actual corners
    public float Left => Math.Min(Math.Min(TopLeft.X, TopRight.X), Math.Min(BottomLeft.X, BottomRight.X));
    public float Right => Math.Max(Math.Max(TopLeft.X, TopRight.X), Math.Max(BottomLeft.X, BottomRight.X));
    public float Top => Math.Min(Math.Min(TopLeft.Y, TopRight.Y), Math.Min(BottomLeft.Y, BottomRight.Y));
    public float Bottom => Math.Max(Math.Max(TopLeft.Y, TopRight.Y), Math.Max(BottomLeft.Y, BottomRight.Y));

    public Vector2 TopLeft => _corners[0];
    public Vector2 TopRight => _corners[1];
    public Vector2 BottomRight => _corners[2];
    public Vector2 BottomLeft => _corners[3];

    public RotatedRectangle(Vector2 position, Vector2 size, float rotation = 0f)
    {
        Position = position;
        Size = size;
        Rotation = rotation;

        _corners = new Vector2[4];
        _axes = new Vector2[4];

        UpdateCornersAndAxes();
    }

    public void Rotate(float radians)
    {
        Rotation += radians;
        UpdateCornersAndAxes();
    }

    public bool Intersects(RotatedRectangle other)
    {
        var refThis = this;
        // Check all axes of both rectangles
        return _axes.All(axis => IsOverlappingOnAxis(refThis, other, axis)) &&
               other._axes.All(axis => IsOverlappingOnAxis(refThis, other, axis));
    }

    private static bool IsOverlappingOnAxis(RotatedRectangle rect1, RotatedRectangle rect2, Vector2 axis)
    {
        ProjectOntoAxis(rect1._corners, axis, out var min1, out var max1);
        ProjectOntoAxis(rect2._corners, axis, out var min2, out var max2);

        return !(max1 < min2 || max2 < min1);
    }

    private static void ProjectOntoAxis(Vector2[] corners, Vector2 axis, out float min, out float max)
    {
        var dot = Vector2.Dot(corners[0], axis);
        min = max = dot;

        for (var i = 1; i < 4; i++)
        {
            dot = Vector2.Dot(corners[i], axis);
            if (dot < min) min = dot;
            else if (dot > max) max = dot;
        }
    }

    private void UpdateCornersAndAxes()
    {
        var halfSize = Size / 2f;

        // Local corner positions (centered at origin)
        var topLeft = new Vector2(-halfSize.X, -halfSize.Y);
        var topRight = new Vector2(halfSize.X, -halfSize.Y);
        var bottomRight = new Vector2(halfSize.X, halfSize.Y);
        var bottomLeft = new Vector2(-halfSize.X, halfSize.Y);

        // Rotate corners around origin and translate to position
        _corners[0] = RotatePoint(topLeft) + Position;
        _corners[1] = RotatePoint(topRight) + Position;
        _corners[2] = RotatePoint(bottomRight) + Position;
        _corners[3] = RotatePoint(bottomLeft) + Position;

        // Compute edge axes (normalized)
        _axes[0] = Vector2.Normalize(_corners[1] - _corners[0]);
        _axes[1] = Vector2.Normalize(_corners[2] - _corners[1]);
        _axes[2] = Vector2.Normalize(_corners[3] - _corners[2]);
        _axes[3] = Vector2.Normalize(_corners[0] - _corners[3]);
    }

    private Vector2 RotatePoint(Vector2 point)
    {
        var cos = (float)Math.Cos(Rotation);
        var sin = (float)Math.Sin(Rotation);
        return new Vector2(
            point.X * cos - point.Y * sin,
            point.X * sin + point.Y * cos
        );
    }

    // Optional: for debug drawing
    public Vector2[] GetCorners()
    {
        return (Vector2[])_corners.Clone();
    }

    public Vector2? GetIntersectPoint(RotatedRectangle other) => GetIntersectPoint(this, other);

    public static Vector2? GetIntersectPoint(RotatedRectangle r1, RotatedRectangle r2)
    {
        var corners1 = r1.GetCorners();
        var corners2 = r2.GetCorners();

        for (var i = 0; i < 4; i++)
        {
            var a1 = corners1[i];
            var a2 = corners1[(i + 1) % 4];

            for (var j = 0; j < 4; j++)
            {
                var b1 = corners2[j];
                var b2 = corners2[(j + 1) % 4];

                if (LineSegmentsIntersect(a1, a2, b1, b2, out var intersection))
                    return intersection;
            }
        }

        return null;
    }

    public static bool LineSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2, out Vector2 intersection)
    {
        intersection = Vector2.Zero;

        var r = p2 - p1;
        var s = q2 - q1;

        var denominator = Cross(r, s);
        if (denominator == 0)
            return false; // parallel

        var t = Cross(q1 - p1, s) / denominator;
        var u = Cross(q1 - p1, r) / denominator;

        if (!(t >= 0) || !(t <= 1) || !(u >= 0) || !(u <= 1)) return false;

        intersection = p1 + t * r;
        return true;
    }

    private static float Cross(Vector2 a, Vector2 b)
    {
        return a.X * b.Y - a.Y * b.X;
    }
}