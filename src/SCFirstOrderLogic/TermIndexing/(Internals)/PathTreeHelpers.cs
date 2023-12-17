using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.TermIndexing
{
    /// <summary>
    /// Helper methods for path tree implementations.
    /// </summary>
    internal static class PathTreeHelpers
    {
        /// <summary>
        /// Transforms a term to an appropriate <see cref="IPathTreeArgumentNodeKey"/> instance.
        /// </summary>
        /// <param name="term">The term to transform.</param>
        /// <returns>The <see cref="IPathTreeArgumentNodeKey"/> instance that is appropriate for the passed term.</returns>
        public static IPathTreeArgumentNodeKey ToNodeKey(this Term term)
        {
            return term switch
            {
                Function function => new PathTreeFunctionNodeKey(function.Identifier, function.Arguments.Count),
                Constant constant => new PathTreeConstantNodeKey(constant.Identifier),
                VariableReference variable => new PathTreeVariableNodeKey((int)variable.Identifier),
                _ => throw new ArgumentException("Unrecognised term type", nameof(term))
            };
        }

        /// <summary>
        /// <para>
        /// Ordinalises a term; replacing all variable identifiers with the ordinal of where they
        /// first appear in a depth-first traversal of the term (zero for the first, one for the second, and so on).
        /// </para>
        /// <para>
        /// This is useful because it makes the original identifiers irrelevant but preserves distinctness, so that 
        /// e.g. F(X, X) is transformed to a term that is identical to the transformation of F(Y, Y), but different
        /// to the transformation of F(X, Y).
        /// </para>
        /// </summary>
        /// <param name="term">The term to ordinalise.</param>
        /// <returns>The ordinalised term.</returns>
        public static Term Ordinalise(this Term term)
        {
            return term.Accept(new VariableOrdinalisation());
        }

        /// <summary>
        /// Gets the set of values that appear in all of the inner enumerables.
        /// </summary>
        public static IEnumerable<T> IntersectAll<T>(this IEnumerable<IEnumerable<T>> values)
        {
            HashSet<T> commonValues;
            var valuesEnumerator = values.GetEnumerator();
            try
            {
                if (!valuesEnumerator.MoveNext())
                {
                    return Enumerable.Empty<T>();
                }

                commonValues = new(valuesEnumerator.Current);
                while (commonValues.Count > 0 && valuesEnumerator.MoveNext())
                {
                    commonValues.IntersectWith(valuesEnumerator.Current);
                }

                return commonValues;
            }
            finally
            {
                valuesEnumerator.Dispose();
            }
        }

        /// <summary>
        /// Tries to find a singular common value in all of the inner enumerables.
        /// </summary>
        public static bool TryGetCommonValue<T>(this IEnumerable<IEnumerable<T>> values, [MaybeNullWhen(false)] out T value)
        {
            var enumerator = IntersectAll(values).GetEnumerator();
            try
            {
                if (!enumerator.MoveNext())
                {
                    value = default;
                    return false;
                }

                value = enumerator.Current;

                if (enumerator.MoveNext())
                {
                    value = default;
                    return false;
                }

                return true;
            }
            finally
            {
                enumerator?.Dispose();
            }
        }

        /// <summary>
        /// Checks whether "this" term is an instance of another.
        /// </summary>
        /// <param name="term">The potential instance.</param>
        /// <param name="generalisation">The generalisation.</param>
        /// <returns>A value indicating whether "this" term is an instance of the generalisation.</returns>
        public static bool IsInstanceOf(this Term term, Term generalisation)
        {
            return InstanceUnifier.TryCreate(generalisation, term, out _);
        }

        private class VariableOrdinalisation : RecursiveSentenceTransformation
        {
            private readonly Dictionary<object, VariableDeclaration> variableIdMap = new();

            public override VariableDeclaration ApplyTo(VariableDeclaration variable)
            {
                if (!variableIdMap.TryGetValue(variable.Identifier, out var declaration))
                {
                    declaration = variableIdMap[variable.Identifier] = new VariableDeclaration(variableIdMap.Count);
                }

                return declaration;
            }
        }

        private static class InstanceUnifier
        {
            public static bool TryCreate(Term generalisation, Term instance, [MaybeNullWhen(false)] out VariableSubstitution unifier)
            {
                var unifierAttempt = new VariableSubstitution();

                if (TryUpdateInPlace(generalisation, instance, unifierAttempt))
                {
                    unifier = unifierAttempt;
                    return true;
                }

                unifier = null;
                return false;
            }

            private static bool TryUpdateInPlace(Term generalisation, Term instance, VariableSubstitution unifier)
            {
                return (generalisation, instance) switch
                {
                    (VariableReference variable, _) => TryUpdateInPlace(variable, instance, unifier),
                    (Function functionX, Function functionY) => TryUpdateInPlace(functionX, functionY, unifier),
                    // Below, the only potential for equality is if they're both constants. Perhaps worth testing this
                    // versus that explicitly and a default that just returns false. Similar from a performance
                    // perspective.
                    _ => generalisation.Equals(instance),
                };
            }

            private static bool TryUpdateInPlace(VariableReference variable, Term instanceTerm, VariableSubstitution unifier)
            {
                if (variable.Equals(instanceTerm))
                {
                    return true;
                }
                else if (unifier.Bindings.TryGetValue(variable, out var variableValue))
                {
                    // The variable is already mapped to something - we need to make sure that the
                    // mapping is consistent with the "other" value.
                    return TryUpdateInPlace(variableValue, instanceTerm, unifier);
                }
                else
                {
                    unifier.AddBinding(variable, instanceTerm);
                    return true;
                }
            }

            private static bool TryUpdateInPlace(Function x, Function y, VariableSubstitution unifier)
            {
                if (!x.Identifier.Equals(y.Identifier) || x.Arguments.Count != y.Arguments.Count)
                {
                    return false;
                }

                for (int i = 0; i < x.Arguments.Count; i++)
                {
                    if (!TryUpdateInPlace(x.Arguments[i], y.Arguments[i], unifier))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
