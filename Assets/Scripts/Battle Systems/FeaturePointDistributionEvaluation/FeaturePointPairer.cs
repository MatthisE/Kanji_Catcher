using System;
using System.Collections.Generic;
using UnityEngine;

public class FeaturePointPairer
{
    private readonly float _positionalWeight;
    private readonly float _directionalWeight;
    private readonly float _normalizedCharacterSize;

    public FeaturePointPairer(float positionalWeight = 0.5f, float directionalWeight = 0.5f, float normalizedCharacterSize = 1f)
    {
        _positionalWeight = positionalWeight;
        _directionalWeight = directionalWeight;
        _normalizedCharacterSize = normalizedCharacterSize;
    }

    // compute principal one-to-one correspondences between an input stroke and a reference stroke
    public FeaturePointPairing Pair(Vector2[] inputFeaturePoints, Vector2[] referenceFeaturePoints)
    {
        int inputCount = inputFeaturePoints.Length;
        int referenceCount = referenceFeaturePoints.Length;

        float[,] cost = new float[inputCount, referenceCount]; // cost matrix

        for (int i = 0; i < inputCount; i++)
        {
            for (int j = 0; j < referenceCount; j++)
            {
                cost[i, j] = CalculatePairCost(inputFeaturePoints[i], referenceFeaturePoints[j]);
            }
        }

        DpMatchingResult dp = RunDpMatching(cost);

        var rawCorrespondences = ExtractRawCorrespondences(dp, cost);
        var principalPairs = ReduceToOneToOnePairs(rawCorrespondences, inputCount, referenceCount);

        return new FeaturePointPairing(inputFeaturePoints, referenceFeaturePoints, principalPairs);
    }

    // equation: E(i,j) = Ep(i,j) * wp + Ed(i,j) * wd
    private float CalculatePairCost(Vector2 inputPoint, Vector2 referencePoint)
    {
        float positionalCost = CalculatePositionalCost(inputPoint, referencePoint);
        float directionalCost = CalculateDirectionalCost(inputPoint, referencePoint);

        return _positionalWeight * positionalCost + _directionalWeight * directionalCost;
    }

    // positional feature: Ep(i,j)
    private float CalculatePositionalCost(Vector2 inputPoint, Vector2 referencePoint)
    {
        float distance = Vector2.Distance(inputPoint, referencePoint);
        return distance / Mathf.Max(_normalizedCharacterSize, 0.0001f);
    }

    // directional feature: Ed(i,j)
    private float CalculateDirectionalCost(Vector2 inputPoint, Vector2 referencePoint)
    {
        // no direction vector computed
        return 0f;
    }

    // calculate directional cost from explicit local direction vectors
    public float CalculateDirectionalCostFromVectors(Vector2 inputDirection, Vector2 referenceDirection)
    {
        if (inputDirection.sqrMagnitude < 0.000001f || referenceDirection.sqrMagnitude < 0.000001f)
        {
            return 0f;
        }

        float angle = Vector2.Angle(inputDirection.normalized, referenceDirection.normalized);

        // normalize angle from [0, 180] to [0, 1]
        return angle / 180f;
    }

    // dynamic-programming alignment between the feature-point sequences
    private DpMatchingResult RunDpMatching(float[,] cost)
    {
        int rows = cost.GetLength(0);
        int columns = cost.GetLength(1);

        var dp = new float[rows + 1, columns + 1];
        var predecessor = new int[rows + 1, columns + 1];

        const float infinity = float.MaxValue / 4f;

        for (int i = 0; i <= rows; i++)
            for (int j = 0; j <= columns; j++)
                dp[i, j] = infinity;

        dp[0, 0] = 0f;

        for (int i = 1; i <= rows; i++)
        {
            dp[i, 0] = infinity;
        }

        for (int j = 1; j <= columns; j++)
        {
            dp[0, j] = infinity;
        }

        for (int i = 1; i <= rows; i++)
        {
            for (int j = 1; j <= columns; j++)
            {
                float diagonal = dp[i - 1, j - 1] + cost[i - 1, j - 1];
                float up = dp[i - 1, j] + cost[i - 1, j - 1];
                float left = dp[i, j - 1] + cost[i - 1, j - 1];

                float best = diagonal;
                int bestPredecessor = 0;

                if (up < best)
                {
                    best = up;
                    bestPredecessor = 1;
                }

                if (left < best)
                {
                    best = left;
                    bestPredecessor = 2;
                }

                dp[i, j] = best;
                predecessor[i, j] = bestPredecessor;
            }
        }

        return new DpMatchingResult(dp, predecessor, rows, columns);
    }

    // backtrack the DP table and produce raw correspondences
    private List<FeatureCorrespondence> ExtractRawCorrespondences(DpMatchingResult dp, float[,] cost)
    {
        var correspondences = new List<FeatureCorrespondence>();

        int i = dp.InputCount;
        int j = dp.ReferenceCount;

        while (i > 0 && j > 0)
        {
            int predecessor = dp.Predecessor[i, j];

            switch (predecessor)
            {
                // diagonal moves create one-to-one correspondences
                case 0:
                    correspondences.Add(new FeatureCorrespondence(i - 1, j - 1, FeatureCorrespondenceType.OneToOne));
                    i--;
                    j--;
                    break;

                // vertical and horizontal moves create one-to-many or many-to-one correspondences
                case 1:
                    correspondences.Add(new FeatureCorrespondence(i - 1, j - 1, FeatureCorrespondenceType.ManyInputToOneReference));
                    i--;
                    break;

                case 2:
                    correspondences.Add(new FeatureCorrespondence(i - 1, j - 1, FeatureCorrespondenceType.OneInputToManyReference));
                    j--;
                    break;

                default:
                    throw new InvalidOperationException("Invalid DP predecessor.");
            }
        }

        correspondences.Reverse();
        return correspondences;
    }

    // apply reduction rules
    private List<FeaturePointPair> ReduceToOneToOnePairs(
        List<FeatureCorrespondence> correspondences,
        int inputCount,
        int referenceCount)
    {
        var inputUsed = new bool[inputCount];
        var referenceUsed = new bool[referenceCount];

        var pairs = new List<FeaturePointPair>();

        // P1: start-to-start
        if (correspondences.Count > 0)
        {
            var first = correspondences[0];

            if (!inputUsed[first.InputIndex] && !referenceUsed[first.ReferenceIndex])
            {
                AddPair(pairs, inputUsed, referenceUsed, first);
            }
        }

        // P1: end-to-end
        if (correspondences.Count > 1)
        {
            var last = correspondences[^1];

            if (!inputUsed[last.InputIndex] && !referenceUsed[last.ReferenceIndex])
            {
                AddPair(pairs, inputUsed, referenceUsed, last);
            }
        }

        // P2 and P3: retain only one-to-one correspondences whose points are still unused
        foreach (var correspondence in correspondences)
        {
            if (correspondence.Type != FeatureCorrespondenceType.OneToOne)
                continue;

            if (inputUsed[correspondence.InputIndex] || referenceUsed[correspondence.ReferenceIndex])
            {
                continue;
            }

            AddPair(pairs, inputUsed, referenceUsed, correspondence);
        }

        // P4: scan from the end to the start and add remaining valid one-to-one candidates
        for (int index = correspondences.Count - 1; index >= 0; index--)
        {
            var correspondence = correspondences[index];

            if (inputUsed[correspondence.InputIndex] || referenceUsed[correspondence.ReferenceIndex])
            {
                continue;
            }

            AddPair(pairs, inputUsed, referenceUsed, correspondence);
        }

        pairs.Sort((a, b) => a.InputIndex.CompareTo(b.InputIndex));

        return pairs;
    }

    private void AddPair(List<FeaturePointPair> pairs, bool[] inputUsed, bool[] referenceUsed, FeatureCorrespondence correspondence)
    {
        inputUsed[correspondence.InputIndex] = true;
        referenceUsed[correspondence.ReferenceIndex] = true;

        pairs.Add(new FeaturePointPair(correspondence.InputIndex, correspondence.ReferenceIndex));
    }
}

public enum FeatureCorrespondenceType
{
    OneToOne,
    OneInputToManyReference,
    ManyInputToOneReference
}

public readonly struct FeatureCorrespondence
{
    public int InputIndex { get; }
    public int ReferenceIndex { get; }
    public FeatureCorrespondenceType Type { get; }

    public FeatureCorrespondence(int inputIndex, int referenceIndex, FeatureCorrespondenceType type)
    {
        InputIndex = inputIndex;
        ReferenceIndex = referenceIndex;
        Type = type;
    }
}

public readonly struct FeaturePointPair
{
    public int InputIndex { get; }
    public int ReferenceIndex { get; }

    public FeaturePointPair(int inputIndex, int referenceIndex)
    {
        InputIndex = inputIndex;
        ReferenceIndex = referenceIndex;
    }
}

public sealed class FeaturePointPairing
{
    public Vector2[] InputFeaturePoints { get; }
    public Vector2[] ReferenceFeaturePoints { get; }
    public List<FeaturePointPair> PrincipalPairs { get; }

    public FeaturePointPairing(Vector2[] inputFeaturePoints, Vector2[] referenceFeaturePoints, List<FeaturePointPair> principalPairs)
    {
        InputFeaturePoints = inputFeaturePoints;
        ReferenceFeaturePoints = referenceFeaturePoints;
        PrincipalPairs = principalPairs;
    }
}

internal readonly struct DpMatchingResult
{
    public float[,] Cost { get; }
    public int[,] Predecessor { get; }
    public int InputCount { get; }
    public int ReferenceCount { get; }

    public DpMatchingResult(float[,] cost, int[,] predecessor, int inputCount, int referenceCount)
    {
        Cost = cost;
        Predecessor = predecessor;
        InputCount = inputCount;
        ReferenceCount = referenceCount;
    }
}