// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;

namespace SCFirstOrderLogic.InternalUtilities;

/// <summary>
/// A max priority queue implementation that uses a binary heap.
/// </summary>
/// <typeparam name="TElement">The type of objects to be stored.</typeparam>
internal sealed class MaxPriorityQueue<TElement>
{
    private readonly Comparison<TElement> priorityComparison;

    private TElement[] heap = new TElement[16];

    /// <summary>
    /// Initializes a new instance of the <see cref="MaxPriorityQueue{TElement}"/> class.
    /// </summary>
    /// <param name="priorityComparison">The comparison to use to compare elements.</param>
    public MaxPriorityQueue(Comparison<TElement> priorityComparison)
    {
        this.priorityComparison = priorityComparison ?? throw new ArgumentNullException(nameof(priorityComparison));
    }

    /// <summary>
    /// Gets the number of items in the queue.
    /// </summary>
    public int Count { get; private set; } = 0;

    /// <summary>
    /// Enqueues an item.
    /// </summary>
    /// <param name="element">The item to enqueue.</param>
    public void Enqueue(TElement element)
    {
        if (Count >= heap.Length)
        {
            Array.Resize(ref heap, Math.Max(heap.Length * 2, 1));
        }

        BubbleUp(Count++, element);
    }

    /// <summary>
    /// Retrieves the highest-priority item from the queue.
    /// </summary>
    /// <returns>The dequeued item.</returns>
    public TElement Dequeue()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("Queue is empty");
        }

        var element = heap[0];

        BubbleDown(0, heap[--Count]);

        // To avoid memory leak for reference types:
        // (usage of null-forgiving operator fine because it should never be retrieved)
        heap[Count] = default!; 

        return element;
    }

    /// <summary>
    /// Retrieves the highest-priority item from the queue, without actually removing it from the queue.
    /// </summary>
    /// <returns>The next item in the queue.</returns>
    public TElement Peek()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("Queue is empty");
        }

        return heap[0];
    }

    private void BubbleUp(int index, TElement element)
    {
        while (index > 0)
        {
            var parentIndex = (index - 1) / 2;
            ref var parent = ref heap[parentIndex];

            if (priorityComparison(parent, element) >= 0)
            {
                break;
            }

            heap[index] = parent;

            index = parentIndex;
        }

        heap[index] = element;
    }

    private void BubbleDown(int i, TElement element)
    {
        while (i != -1)
        {
            var dominatingIndex = -1;
            ref var dominating = ref element;

            var childIndex = 2 * i + 1;
            if (childIndex < Count)
            {
                ref var child = ref heap[childIndex];
                if (priorityComparison(child, dominating) > 0)
                {
                    dominatingIndex = childIndex;
                    dominating = ref child;
                }
            }

            if (++childIndex < Count)
            {
                ref var child = ref heap[childIndex];
                if (priorityComparison(child, dominating) > 0)
                {
                    dominatingIndex = childIndex;
                    dominating = ref child;
                }
            }

            heap[i] = dominating;

            i = dominatingIndex;
        }
    }
}
