// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// A static class offering a couple of handy common feature vector selection methods to use -
/// taken from Stephan Schulz's 2013 paper.
/// </summary>
/// <seealso href="http://wwwlehre.dhbw-stuttgart.de/~sschulz/PAPERS/Schulz2013-FVI.pdf"/>
public static class FeatureVectorSelectors
{
    /// <summary>
    /// Feature vector selection logic that returns a feature vector consisting of: 
    /// positive literal count, negative literal count, occurence count of each occuring
    /// identifier among positive literals, and occurence count of each occuring identifier
    /// among negative literals.
    /// </summary>
    /// <param name="clause">The clause to retrieve a feature vector for.</param>
    /// <returns>A feature vector.</returns>
    public static IEnumerable<KeyValuePair<object, int>> OccurenceCounts(CNFClause clause)
    {
        return null;
    }

    /// <summary>
    /// Feature vector selection logic that returns a feature vector consisting of the max depth
    /// of each occuring identifier among positive literals, and the max depth of each occuring 
    /// identifier among negative literals.
    /// </summary>
    /// <param name="clause">The clause to retrieve a feature vector for.</param>
    /// <returns>A feature vector.</returns>
    public static IEnumerable<KeyValuePair<object, int>> MaxDepths(CNFClause clause)
    {
        return null;
    }

    private class IdentifierFeature
    {

    }

    private class OccurenceCountVisitor : RecursiveSentenceVisitor
    {
        private readonly IDictionary<object, int> featureVector;

        public OccurenceCountVisitor(IDictionary<object, int> featureVector)
        {
            this.featureVector = featureVector;
        }

        public override void Visit(Predicate predicate)
        {
            AddOccurence(predicate.Identifier);
            base.Visit(predicate);
        }

        public override void Visit(Function function)
        {
            AddOccurence(function.Identifier);
            base.Visit(function);
        }

        private void AddOccurence(object identifier)
        {
            featureVector.TryGetValue(identifier, out var value);
            featureVector[identifier] = value++;
        }
    }

    private class MaxDepthsVisitor : RecursiveSentenceVisitor<int>
    {
        private readonly IDictionary<object, int> featureVector;

        public MaxDepthsVisitor(IDictionary<object, int> featureVector)
        {
            this.featureVector = featureVector;
        }

        public override void Visit(Predicate predicate, int depth)
        {
            UpdateMaxDepth(predicate.Identifier, depth);
            base.Visit(predicate, depth + 1);
        }

        public override void Visit(Function function, int depth)
        {
            UpdateMaxDepth(function.Identifier, depth);
            base.Visit(function, depth + 1);
        }

        private void UpdateMaxDepth(object identifier, int depth)
        {
            featureVector.TryGetValue(identifier, out var maxDepth);

            if (depth > maxDepth)
            {
                featureVector[identifier] = depth;
            }
        }
    }
}
