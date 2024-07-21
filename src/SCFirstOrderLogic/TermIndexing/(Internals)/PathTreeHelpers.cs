// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.TermIndexing;

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
            VariableReference variable => new PathTreeVariableNodeKey((int)variable.Identifier),
            _ => throw new ArgumentException("Unrecognised term type", nameof(term))
        };
    }

    /// <summary>
    /// Gets the set of values that appear in all of the inner enumerables.
    /// </summary>
    public static IEnumerable<T> IntersectAll<T>(this IEnumerable<IEnumerable<T>> values)
    {
        using var valuesEnumerator = values.GetEnumerator();

        if (!valuesEnumerator.MoveNext())
        {
            return Enumerable.Empty<T>();
        }

        var commonValues = new HashSet<T>(valuesEnumerator.Current);
        while (commonValues.Count > 0 && valuesEnumerator.MoveNext())
        {
            commonValues.IntersectWith(valuesEnumerator.Current);
        }

        return commonValues;
    }

    /// <summary>
    /// Gets the set of values that appear in all of the inner async enumerables.
    /// </summary>
    public static async IAsyncEnumerable<T> IntersectAll<T>(this IAsyncEnumerable<IAsyncEnumerable<T>> values)
    {
        await using var valuesEnumerator = values.GetAsyncEnumerator();

        if (!await valuesEnumerator.MoveNextAsync())
        {
            yield break;
        }

        var commonValues = new HashSet<T>(await valuesEnumerator.Current.ToListAsync());
        while (commonValues.Count > 0 && await valuesEnumerator.MoveNextAsync())
        {
            commonValues.IntersectWith(await valuesEnumerator.Current.ToListAsync());
        }

        foreach (var value in commonValues)
        {
            yield return value;
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
            enumerator.Dispose();
        }
    }

    /// <summary>
    /// Tries to find a singular common value in all of the inner async enumerables.
    /// </summary>
    public static async Task<(bool isSucceeded, T? value)> TryGetCommonValueAsync<T>(this IAsyncEnumerable<IAsyncEnumerable<T>> values)
    {
        var enumerator = IntersectAll(values).GetAsyncEnumerator();
        try
        {
            if (!await enumerator.MoveNextAsync())
            {
                return (false, default);
            }

            var value = enumerator.Current;

            if (await enumerator.MoveNextAsync())
            {
                return (false, default);
            }

            return (true, value);
        }
        finally
        {
            await enumerator.DisposeAsync();
        }
    }
}
