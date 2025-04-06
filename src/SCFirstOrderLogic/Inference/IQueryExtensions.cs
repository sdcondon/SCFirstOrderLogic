// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.Inference;

/// <summary>
/// Helpful extension methods for <see cref="IQuery"/> instances.
/// </summary>
public static class IQueryExtensions
{
    /// <summary>
    /// Executes a query to completion.
    /// </summary>
    /// <param name="query">The query to execute.</param>
    /// <returns>The result of the query.</returns>
    public static bool Execute(this IQuery query)
    {
        return query.ExecuteAsync().GetAwaiter().GetResult();
    }
}
