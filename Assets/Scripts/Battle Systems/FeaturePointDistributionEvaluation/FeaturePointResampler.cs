using System;
using System.Collections.Generic;
using UnityEngine;

public static class FeaturePointResampler
{
    // resample both strokes based on their principal feature-point pairs
    public static ResampledStrokePair Resample(Vector2[] inputFeaturePoints, Vector2[] referenceFeaturePoints, int targetPointCount = 20)
    {
        int segmentCount = inputFeaturePoints.Length - 1;

        if (segmentCount < 1)
        {
            return new ResampledStrokePair(inputFeaturePoints, referenceFeaturePoints);
        }

        float[] segmentWeights = CalculateSegmentWeights(inputFeaturePoints, referenceFeaturePoints);
        int[] pointsPerSegment = CalculatePointsPerSegment(segmentWeights, targetPointCount);
        Vector2[] resampledInput = CreateResampledPoints(inputFeaturePoints, pointsPerSegment, targetPointCount);
        Vector2[] resampledReference = CreateResampledPoints(referenceFeaturePoints, pointsPerSegment, targetPointCount);

        return new ResampledStrokePair(resampledInput, resampledReference);
    }

    // calculate the average weight of every segment between two successive principal feature-point pairs
    private static float[] CalculateSegmentWeights(Vector2[] inputFeaturePoints, Vector2[] referenceFeaturePoints)
    {
        int segmentCount = inputFeaturePoints.Length - 1;
        var weights = new float[segmentCount];
        float totalWeight = 0f;

        for (int i = 0; i < segmentCount; i++)
        {
            float inputSegmentLength = Vector2.Distance(inputFeaturePoints[i], inputFeaturePoints[i+1]);
            float referenceSegmentLength = Vector2.Distance(referenceFeaturePoints[i], referenceFeaturePoints[i+1]);
            float averageSegmentLength = (inputSegmentLength + referenceSegmentLength) / 2f;

            weights[i] = averageSegmentLength;
            totalWeight += weights[i];
        }

        for (int i = 0; i < segmentCount; i++)
        {
            weights[i] /= totalWeight;
        }

        return weights;
    }

    // distribute the target point count over the segments according to their relative lengths
    private static int[] CalculatePointsPerSegment(float[] segmentWeights, int targetPointCount)
    {
        int segmentCount = segmentWeights.Length;
        var pointsPerSegment = new int[segmentCount];
        int assignedPoints = 0;

        var fractionalParts = new float[segmentCount];
        var segmentIndices = new List<int>(segmentCount);

        for (int i = 0; i < segmentCount; i++)
        {
            int count = Mathf.FloorToInt(targetPointCount * segmentWeights[i]);
            pointsPerSegment[i] = count;
            assignedPoints += count;

            fractionalParts[i] = targetPointCount * segmentWeights[i] - count;
            segmentIndices.Add(i);
        }

        int remainingPoints = targetPointCount - assignedPoints;

        // assign remaining points to the segments with the largest fractional parts
        if (remainingPoints > 0)
        {
            segmentIndices.Sort((a, b) => fractionalParts[b].CompareTo(fractionalParts[a]));

            for (int i = 0; i < remainingPoints; i++)
            {
                pointsPerSegment[segmentIndices[i]]++;
            }
        }

        return pointsPerSegment;
    }

    // add interpolated auxiliary feature points to the list of feature points
    private static Vector2[] CreateResampledPoints(Vector2[] featurePoints, int[] pointsPerSegment, int targetPointCount)
    {
        var result = new List<Vector2>(targetPointCount);

        for (int segmentIndex = 0; segmentIndex < pointsPerSegment.Length; segmentIndex++)
        {
            Vector2 startPoint = featurePoints[segmentIndex];
            Vector2 endPoint = featurePoints[segmentIndex+1];

            int pointCountInSegment = pointsPerSegment[segmentIndex];

            result.Add(startPoint);

            for (int i = 1; i < pointCountInSegment + 1; i++)
            {
                float t = (float)i / (pointCountInSegment + 1);
                result.Add(Vector2.Lerp(startPoint, endPoint, t));
            }
        }

        result.Add(featurePoints[^1]);

        return result.ToArray();
    }
}

public sealed class ResampledStrokePair
{
    public Vector2[] InputPoints { get; }
    public Vector2[] ReferencePoints { get; }

    public ResampledStrokePair(Vector2[] inputPoints, Vector2[] referencePoints)
    {
        InputPoints = inputPoints;
        ReferencePoints = referencePoints;
    }
}