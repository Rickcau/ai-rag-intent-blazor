﻿// Copyright (c) Microsoft. All rights reserved.

using System.Collections;
using System.Collections.Generic;

namespace api_ai_rag_intent.Plugins.SqlServer;

/// <summary>
/// A collector for Top N matches. Keeps only the best N matches by Score.
/// Automatically flushes out any not in the top N.
/// By default, items are not sorted by score until you call <see cref="TopNCollection{T}.SortByScore"/>.
/// </summary>
internal sealed class TopNCollection<T>(int maxItems) : IEnumerable<ScoredValue<T>>
{
    private readonly MinHeap<ScoredValue<T>> _heap = new(ScoredValue<T>.Min(), maxItems);
    private bool _sorted = false;

    /// <summary>
    /// Gets the maximum number of items allowed in the collection.
    /// </summary>
    public int MaxItems { get; } = maxItems;

    /// <summary>
    /// Gets the current number of items in the collection.
    /// </summary>
    public int Count => this._heap.Count;

    internal ScoredValue<T> this[int i] => this._heap[i];
    internal ScoredValue<T> Top => this._heap.Top;

    /// <summary>
    /// Resets the collection, allowing it to be reused.
    /// </summary>
    public void Reset()
    {
        this._heap.Clear();
    }

    /// <summary>
    /// Adds a single scored value to the collection.
    /// </summary>
    /// <param name="value">The scored value to add.</param>
    public void Add(ScoredValue<T> value)
    {
        if (this._sorted)
        {
            this._heap.Restore();
            this._sorted = false;
        }

        if (this._heap.Count == this.MaxItems)
        {
            // Queue is full. We will need to dequeue the item with lowest weight
            if (value.Score <= this.Top.Score)
            {
                // This score is lower than the lowest score on the queue right now. Ignore it
                return;
            }

            this._heap.RemoveTop();
        }

        this._heap.Add(value);
    }

    /// <summary>
    /// Adds a value with a specified score to the collection.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <param name="score">The score associated with the value.</param>
    public void Add(T value, double score)
    {
        this.Add(new ScoredValue<T>(value, score));
    }

    /// <summary>
    /// Sorts the collection in descending order by score.
    /// </summary>
    public void SortByScore()
    {
        if (!this._sorted && this._heap.Count > 0)
        {
            this._heap.SortDescending();
            this._sorted = true;
        }
    }

    /// <summary>
    /// Returns a list containing the scored values in the collection.
    /// </summary>
    /// <returns>A list of scored values.</returns>
    public IList<ScoredValue<T>> ToList()
    {
        var list = new List<ScoredValue<T>>(this.Count);
        for (int i = 0, count = this.Count; i < count; ++i)
        {
            list.Add(this[i]);
        }

        return list;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator for the collection.</returns>
    public IEnumerator<ScoredValue<T>> GetEnumerator()
    {
        return this._heap.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this._heap.GetEnumerator();
    }
}