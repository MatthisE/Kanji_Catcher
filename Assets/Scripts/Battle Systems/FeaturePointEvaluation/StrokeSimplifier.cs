using System;
using System.Collections.Generic;
using UnityEngine;

public static class StrokeSimplifier
{
    public static Vector2[] Simplify(Vector2[] points, float epsilon)
    {
        if (epsilon < 0f || float.IsNaN(epsilon) || float.IsInfinity(epsilon))
        {
            throw new ArgumentOutOfRangeException(nameof(epsilon));
        }

        if (points.Length <= 2)
        {
            return (Vector2[])points.Clone();
        }

        bool[] keep = new bool[points.Length];

        // keep start and end points
        keep[0] = true;
        keep[points.Length - 1] = true;

        float epsilonSquared = epsilon * epsilon;

        // stack to push and pop new segments
        Stack<Vector2Int> segments = new Stack<Vector2Int>();
        segments.Push(new Vector2Int(0, points.Length - 1));

        while (segments.Count > 0)
        {
            Vector2Int segment = segments.Pop();

            int startIndex = segment.x;
            int endIndex = segment.y;

            float maxDistanceSquared = -1f;
            int farthestIndex = -1;

            // check for farthest away point in segment
            for (int i = startIndex + 1; i < endIndex; i++)
            {
                float distanceSquared = DistanceToSegmentSquared(
                    points[i],
                    points[startIndex],
                    points[endIndex]
                );

                if (distanceSquared > maxDistanceSquared)
                {
                    maxDistanceSquared = distanceSquared;
                    farthestIndex = i;
                }
            }

            // check if point is far enough away
            if (farthestIndex >= 0 && maxDistanceSquared > epsilonSquared)
            {
                keep[farthestIndex] = true;

                // push two new segments
                segments.Push(new Vector2Int(startIndex, farthestIndex));
                segments.Push(new Vector2Int(farthestIndex, endIndex));
            }
        }

        List<Vector2> result = new List<Vector2>();

        // kept points
        for (int i = 0; i < points.Length; i++)
        {
            if (keep[i])
            {
                result.Add(points[i]);
            }
        }

        return result.ToArray();
    }

    private static float DistanceToSegmentSquared(Vector2 point, Vector2 start, Vector2 end)
    {
        Vector2 segment = end - start;
        float lengthSquared = segment.sqrMagnitude;

        // if start and end are identical (circle stroke)
        if (lengthSquared == 0f)
        {
            return (point - start).sqrMagnitude;
        }

        float t = Vector2.Dot(point - start, segment) / lengthSquared;
        t = Mathf.Clamp01(t);
        Vector2 closestPoint = start + t * segment;

        return (point - closestPoint).sqrMagnitude;
    }
}