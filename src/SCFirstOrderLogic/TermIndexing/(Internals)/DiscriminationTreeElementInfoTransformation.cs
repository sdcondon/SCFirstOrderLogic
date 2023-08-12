// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;

namespace SCFirstOrderLogic.TermIndexing
{
    /// <summary>
    /// Transformation logic that converts <see cref="Term"/>s into the equivalent path of <see cref="IDiscriminationTreeElementInfo"/>s,
    /// for storage in or querying of a discrimination tree. That is, converts terms into an enumerable that represents a depth-first
    /// traversal of their constituent elements.
    /// </summary>
    internal class DiscriminationTreeElementInfoTransformation
    {
        // TODO-PERFORMANCE: a dictionary is almost certainly overkill given the low number of vars likely to
        // appear in any given term. Plain old list likely to perform better. Test me.
        private readonly Dictionary<object, int> variableIdMap = new();

        public IEnumerable<IDiscriminationTreeElementInfo> ApplyTo(Term term)
        {
            return term switch
            {
                Constant constant => ApplyTo(constant),
                VariableReference variable => ApplyTo(variable),
                Function function => ApplyTo(function),
                _ => throw new ArgumentException($"Unrecognised Term type '{term.GetType()}'", nameof(term))
            };
        }

        public IEnumerable<IDiscriminationTreeElementInfo> ApplyTo(Constant constant)
        {
            yield return new DiscriminationTreeConstantInfo(constant.Identifier);
        }

        public IEnumerable<IDiscriminationTreeElementInfo> ApplyTo(Function function)
        {
            yield return new DiscriminationTreeFunctionInfo(function.Identifier, function.Arguments.Count);

            foreach (var argument in function.Arguments)
            {
                foreach (var node in ApplyTo(argument))
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<IDiscriminationTreeElementInfo> ApplyTo(VariableReference variable)
        {
            // Variable declarations are "ordinalised" (probably not the "right" terminology - need to look this up).
            // That is, converted into the ordinal of where they first appear in a depth-first traversal of the term.
            // This is useful because it means e.g. F(X, X) is transformed to a path that is identical to the transformation of F(Y, Y),
            // but different to the transformation of F(X, Y).
            if (!variableIdMap.TryGetValue(variable.Identifier, out var ordinal))
            {
                ordinal = variableIdMap[variable.Identifier] = variableIdMap.Count;
            }

            yield return new DiscriminationTreeVariableInfo(ordinal);
        }
    }
}
