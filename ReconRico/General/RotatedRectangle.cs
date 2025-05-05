// using Microsoft.Xna.Framework;
// using System;
//
// public struct RotatedRectangle
// {
//     public Vector2 Position;
//     public Vector2 Size;
//
//     /// <summary>
//     /// Rotation in radians
//     /// </summary>
//     public float Rotation;
//
//     private readonly Vector2[] _corners;
//     private readonly Vector2[] _axes;
//
//     // These properties now account for rotation by using the actual corners
//     public Vector2 Left => _axes[3];
//     public Vector2 Right => _axes[1];
//     public Vector2 Top => _axes[0];
//     public Vector2 Bottom => _axes[2];
//
//     public Vector2 TopLeft => _corners[0];
//     public Vector2 TopRight => _corners[1];
//     public Vector2 BottomRight => _corners[2];
//     public Vector2 BottomLeft => _corners[3];
//
//     public RotatedRectangle(Vector2 position, Vector2 size, float rotation = 0f)
//     {
//         Position = position;
//         Size = size;
//         Rotation = rotation;
//
//         _corners = new Vector2[4];
//         _axes = new Vector2[4];
//
//         UpdateCornersAndAxes();
//     }
//
//     public void Rotate(float radians)
//     {
//         Rotation += radians;
//         UpdateCornersAndAxes();
//     }
//
//     public bool Intersects(RotatedRectangle other)
//     {
//         var refThis = this;
//         // Check all axes of both rectangles
//         return _axes.All(axis => IsOverlappingOnAxis(refThis, other, axis)) &&
//                other._axes.All(axis => IsOverlappingOnAxis(refThis, other, axis));
//     }
//
//     private static bool IsOverlappingOnAxis(RotatedRectangle rect1, RotatedRectangle rect2, Vector2 axis)
//     {
//         ProjectOntoAxis(rect1._corners, axis, out var min1, out var max1);
//         ProjectOntoAxis(rect2._corners, axis, out var min2, out var max2);
//
//         return !(max1 < min2 || max2 < min1);
//     }
//
//     private static void ProjectOntoAxis(Vector2[] corners, Vector2 axis, out float min, out float max)
//     {
//         var dot = Vector2.Dot(corners[0], axis);
//         min = max = dot;
//
//         for (var i = 1; i < 4; i++)
//         {
//             dot = Vector2.Dot(corners[i], axis);
//             if (dot < min) min = dot;
//             else if (dot > max) max = dot;
//         }
//     }
//
//     private void UpdateCornersAndAxes()
//     {
//         var halfSize = Size / 2f;
//
//         // Local corner positions (centered at origin)
//         var topLeft = new Vector2(-halfSize.X, -halfSize.Y);
//         var topRight = new Vector2(halfSize.X, -halfSize.Y);
//         var bottomRight = new Vector2(halfSize.X, halfSize.Y);
//         var bottomLeft = new Vector2(-halfSize.X, halfSize.Y);
//
//         // Rotate corners around origin and translate to position
//         _corners[0] = RotatePoint(topLeft) + Position;
//         _corners[1] = RotatePoint(topRight) + Position;
//         _corners[2] = RotatePoint(bottomRight) + Position;
//         _corners[3] = RotatePoint(bottomLeft) + Position;
//
//         // Compute edge axes (normalized)
//         _axes[0] = Vector2.Normalize(_corners[1] - _corners[0]);
//         _axes[1] = Vector2.Normalize(_corners[2] - _corners[1]);
//         _axes[2] = Vector2.Normalize(_corners[3] - _corners[2]);
//         _axes[3] = Vector2.Normalize(_corners[0] - _corners[3]);
//     }
//
//     private Vector2 RotatePoint(Vector2 point)
//     {
//         var cos = (float)Math.Cos(Rotation);
//         var sin = (float)Math.Sin(Rotation);
//         return new Vector2(
//             point.X * cos - point.Y * sin,
//             point.X * sin + point.Y * cos
//         );
//     }
//
//     public Vector2[] GetCorners()
//     {
//         // return (Vector2[])_corners.Clone();
//         return _corners;
//     }
//
//     public Vector2? GetIntersectPoint(RotatedRectangle other) => GetIntersectPoint(this, other);
//
//     public static Vector2? GetIntersectPoint(RotatedRectangle r1, RotatedRectangle r2)
//     {
//         var corners1 = r1.GetCorners();
//         var corners2 = r2.GetCorners();
//
//         for (var i = 0; i < 4; i++)
//         {
//             var a1 = corners1[i];
//             var a2 = corners1[(i + 1) % 4];
//
//             for (var j = 0; j < 4; j++)
//             {
//                 var b1 = corners2[j];
//                 var b2 = corners2[(j + 1) % 4];
//
//                 if (LineSegmentsIntersect(a1, a2, b1, b2, out var intersection))
//                     return intersection;
//             }
//         }
//
//         return null;
//     }
//
//     public static bool LineSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2, out Vector2 intersection)
//     {
//         intersection = Vector2.Zero;
//
//         var r = p2 - p1;
//         var s = q2 - q1;
//
//         var denominator = Cross(r, s);
//         if (denominator == 0)
//             return false; // parallel
//
//         var t = Cross(q1 - p1, s) / denominator;
//         var u = Cross(q1 - p1, r) / denominator;
//
//         if (!(t >= 0) || !(t <= 1) || !(u >= 0) || !(u <= 1)) return false;
//
//         intersection = p1 + t * r;
//         return true;
//     }
//     
//     // public static Vector2 GetNearestPointOnLine(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
//     // {
//     //     // Vector representing the line direction
//     //     Vector2 lineDirection = lineEnd - lineStart;
//     //
//     //     // Vector from lineStart to the point
//     //     Vector2 pointToLineStart = point - lineStart;
//     //
//     //     // Project the point onto the line by finding the scalar (t) of the projection
//     //     float t = Vector2.Dot(pointToLineStart, lineDirection) / lineDirection.LengthSquared();
//     //
//     //     // Clamp t to ensure the projected point lies on the line segment
//     //     t = MathHelper.Clamp(t, 0f, 1f);
//     //
//     //     // Calculate the nearest point by scaling the direction vector
//     //     Vector2 nearestPoint = lineStart + t * lineDirection;
//     //
//     //     return nearestPoint;
//     // }
//     public Vector2 GetClosestEdgeNormal(Vector2 point)
//     {
//         float minDistanceSquared = float.MaxValue;
//         Vector2 closestNormal = Vector2.Zero;
//
//         for (int i = 0; i < 4; i++)
//         {
//             Vector2 p1 = _corners[i];
//             Vector2 p2 = _corners[(i + 1) % 4];
//             Vector2 axis = _axes[i];
//         
//             // Находим ближайшую точку на стороне к заданной точке
//             Vector2 closestPoint = GetClosestPointOnLine(point, p1, axis);
//
//             float distSq = Vector2.DistanceSquared(point, closestPoint);
//             if (distSq < minDistanceSquared)
//             {
//                 minDistanceSquared = distSq;
//
//                 // Вычисляем нормаль к ребру (перпендикуляр к вектору p2 - p1)
//                 Vector2 edge = p2 - p1;
//                 Vector2 normal = new Vector2(-edge.Y, edge.X); // Перпендикуляр влево
//                 normal.Normalize();
//                 closestNormal = normal;
//             }
//         }
//
//         return closestNormal;
//     }
//
//     public static Vector2 GetClosestPointOnLine(Vector2 sourcePoint, Vector2 rectPoint, Vector2 axis)
//     {
//         Vector2 AB = sourcePoint - rectPoint;
//         float projectionLength = Vector2.Dot(AB, axis);
//         Vector2 C = rectPoint + projectionLength * axis;
//         return C;
//     }
//
//     private static float Cross(Vector2 a, Vector2 b)
//     {
//         return a.X * b.Y - a.Y * b.X;
//     }
// }