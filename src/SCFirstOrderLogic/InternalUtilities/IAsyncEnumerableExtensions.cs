using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.InternalUtilities
{
    /// <summary>
    /// Extension methods for <see cref="IAsyncEnumerable{T}"/> instances.
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
            List<T> list = new();

            IAsyncEnumerator<T>? enumerator = null;
            try
            {
                enumerator = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
                while (await enumerator.MoveNextAsync())
                {
                    list.Add(enumerator.Current);
                }
            }
            finally
            {
                if (enumerator != null)
                {
                    await enumerator.DisposeAsync();
                }
            }

            return list;
        }
    }
}