// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.InternalUtilities;

/// <summary>
/// <para>
/// Extension methods for <see cref="IAsyncEnumerable{T}"/> instances.
/// </para>
/// <para>
/// NB: Yes, we could take a dependency on System.Linq.Async instead, but I quite
/// like the fact that this package has (almost..) no third-party dependencies.
/// </para>
/// </summary>
internal static class IAsyncEnumerableExtensions
{
    /// <summary>
    /// Asynchronously converts an <see cref="IAsyncEnumerable{T}"/> instance into a <see cref="List{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable's elements.</typeparam>
    /// <param name="asyncEnumerable">The anumerable to convert.</param>
    /// <param name="cancellationToken">A cancellation token for the operation.</param>
    /// <returns>A task representing the completion of the operation.</returns>
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> asyncEnumerable, CancellationToken cancellationToken = default)
    {
        var list = new List<T>();

        await foreach (var element in asyncEnumerable.WithCancellation(cancellationToken))
        {
            list.Add(element);
        }

        return list;
    }

    /// <summary>
    /// Asynchronously determines if a given <see cref="IAsyncEnumerable{T}"/> contains any elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the enumerable in question.</typeparam>
    /// <param name="asyncEnumerable">The enumerable to examine.</param>
    /// <param name="cancellationToken">A cancellation token for the operation.</param>
    /// <returns>A task that returns true if and only if the given enumerable contains any elements.</returns>
    public static async Task<bool> AnyAsync<T>(this IAsyncEnumerable<T> asyncEnumerable, CancellationToken cancellationToken = default)
    {
        await using var enumerator = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
        return await enumerator.MoveNextAsync();
    }

    /// <summary>
    /// Asynchronously determines if a given <see cref="IAsyncEnumerable{T}"/> contains a particular element.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the enumerable in question.</typeparam>
    /// <param name="asyncEnumerable">The enumerable to examine.</param>
    /// <param name="target">The element to look for.</param>
    /// <param name="cancellationToken">A cancellation token for the operation.</param>
    /// <returns>A task that returns true if and only if the given enumerable contains the given element.</returns>
    public static async Task<bool> ContainsAsync<T>(this IAsyncEnumerable<T> asyncEnumerable, T target, CancellationToken cancellationToken = default)
    {
        await foreach(var element in asyncEnumerable.WithCancellation(cancellationToken))
        {
            if (Equals(target, element))
            {
                return true;
            }
        }

        return false;
    }

    public static async IAsyncEnumerable<T> Where<T>(
        this IAsyncEnumerable<T> asyncEnumerable,
        Func<T, bool> predicate,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var element in asyncEnumerable.WithCancellation(cancellationToken))
        {
            if (predicate(element))
            {
                yield return element;
            }
        }
    }

    public static async IAsyncEnumerable<TOut> Select<TIn, TOut>(
        this IAsyncEnumerable<TIn> asyncEnumerable,
        Func<TIn, int, TOut> map,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var enumerator = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
        for (int i = 0; await enumerator.MoveNextAsync(); i++)
        {
            yield return map(enumerator.Current, i);
        }
    }
}