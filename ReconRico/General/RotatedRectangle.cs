using Microsoft.Xna.Framework;
using System;

public struct RotatedRectangle
{
    public Vector2 Position;
    public Vector2 Size;
    public float Rotation; // In radians

    private Vector2[] _corners;
    private Vector2[] _axes;

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
        // Check all axes of both rectangles
        foreach (var axis in _axes)
        {
            if (!IsOverlappingOnAxis(this, other, axis))
                return false;
        }

        foreach (var axis in other._axes)
        {
            if (!IsOverlappingOnAxis(this, other, axis))
                return false;
        }

        return true; // No separating axis found, must be intersecting
    }

    private static bool IsOverlappingOnAxis(RotatedRectangle rect1, RotatedRectangle rect2, Vector2 axis)
    {
        ProjectOntoAxis(rect1._corners, axis, out float min1, out float max1);
        ProjectOntoAxis(rect2._corners, axis, out float min2, out float max2);

        return !(max1 < min2 || max2 < min1);
    }

    private static void ProjectOntoAxis(Vector2[] corners, Vector2 axis, out float min, out float max)
    {
        float dot = Vector2.Dot(corners[0], axis);
        min = max = dot;

        for (int i = 1; i < 4; i++)
        {
            dot = Vector2.Dot(corners[i], axis);
            if (dot < min) min = dot;
            else if (dot > max) max = dot;
        }
    }

    private void UpdateCornersAndAxes()
    {
        Vector2 halfSize = Size / 2f;

        // Local corner positions (centered at origin)
        Vector2 topLeft = new Vector2(-halfSize.X, -halfSize.Y);
        Vector2 topRight = new Vector2(halfSize.X, -halfSize.Y);
        Vector2 bottomRight = new Vector2(halfSize.X, halfSize.Y);
        Vector2 bottomLeft = new Vector2(-halfSize.X, halfSize.Y);

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
        float cos = (float)Math.Cos(Rotation);
        float sin = (float)Math.Sin(Rotation);
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
        Vector2[] corners1 = r1.GetCorners();
        Vector2[] corners2 = r2.GetCorners();

        for (int i = 0; i < 4; i++)
        {
            Vector2 a1 = corners1[i];
            Vector2 a2 = corners1[(i + 1) % 4];

            for (int j = 0; j < 4; j++)
            {
                Vector2 b1 = corners2[j];
                Vector2 b2 = corners2[(j + 1) % 4];

                if (LineSegmentsIntersect(a1, a2, b1, b2, out Vector2 intersection))
                    return intersection;
            }
        }

        return null;
    }

    private static bool LineSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2, out Vector2 intersection)
    {
        intersection = Vector2.Zero;

        Vector2 r = p2 - p1;
        Vector2 s = q2 - q1;

        float denominator = Cross(r, s);
        if (denominator == 0)
            return false; // parallel

        float t = Cross(q1 - p1, s) / denominator;
        float u = Cross(q1 - p1, r) / denominator;

        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            intersection = p1 + t * r;
            return true;
        }

        return false;
    }

    private static float Cross(Vector2 a, Vector2 b)
    {
        return a.X * b.Y - a.Y * b.X;
    }
}
