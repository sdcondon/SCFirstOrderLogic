// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.InternalUtilities;

internal class EmptyAsyncEnumerable<T> : IAsyncEnumerable<T>
{
    private EmptyAsyncEnumerable() { }

    public static EmptyAsyncEnumerable<T> Instance { get; } = new();

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => EmptyAsyncEnumerator.Instance;

    private class EmptyAsyncEnumerator : IAsyncEnumerator<T>
    {
        private EmptyAsyncEnumerator() { }

        public static EmptyAsyncEnumerator Instance { get; } = new();

        public T Current => throw new InvalidOperationException("Enumerable is empty");

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(false);
    }
}
