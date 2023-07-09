// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SCFirstOrderLogic.TermIndexing
{
    /// <summary>
    /// An implementation of an in-memory discrimination tree for <see cref="Term"/>s.
    /// </summary>
    /// <typeparam name="TValue">The type of value attached for each term.</typeparam>
    /// <seealso href="https://www.google.com/search?q=discrimination+tree"/>
    // NB: not a TODO just yet, but - while it's not terrible - there are a few aspects of this
    // class that aren't great from a performance perspective. Notably, while the recursive iterator
    // approach used for the retrieval methods may be easy to understand, it will make a lot of heap
    // allocations - increasing GC pressure. The priority thus far has just been to get it working.
    // TODO-V5-BREAKING: Should really allow for secondary storage extensibility.
    // Adding async support (breaking change) and allowing for different node
    // implementations should do the trick - i.e. make the current concrete node classes
    // abstract in some way, and add a ctor that allows passing in the root node. Everything
    // else can stay common, probably.
    public class DiscriminationTree<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscriminationTree{TValue}"/> class that is empty to begin with.
        /// </summary>
        public DiscriminationTree()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscriminationTree{TValue}"/> class with some initial content.
        /// </summary>
        public DiscriminationTree(IEnumerable<KeyValuePair<Term, TValue>> content)
        {
            foreach (var kvp in content)
            {
                Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// <para>
        /// Gets the root node of the tree.
        /// </para>
        /// <para>
        /// NB: In "normal" usage consumers shouldn't need to access this property.
        /// However, discrimination trees are a specific, well-known data structure, and 
        /// learning and experimentation is a key priority for this library. As such, 
        /// interrogability of the internal structure is considered a useful feature.
        /// </para>
        /// </summary>
        public InternalNode Root { get; } = new InternalNode();

        /// <summary>
        /// Adds a <see cref="Term"/> to the tree.
        /// </summary>
        /// <param name="term">The term to add.</param>
        /// <param name="value">The value to associate with the added term.</param>
        public void Add(Term term, TValue value)
        {
            if (term == null)
            {
                throw new ArgumentNullException(nameof(term));
            }

            var queryElements = new ElementInfoTransformation().ApplyTo(term).GetEnumerator();
            try
            {
                InternalNode currentNode = Root;
                IElementInfo? currentQueryElement = queryElements.MoveNext() ? queryElements.Current : null;
                while (currentQueryElement != null)
                {
                    var nextQueryElement = queryElements.MoveNext() ? queryElements.Current : null;

                    if (nextQueryElement != null)
                    {
                        currentNode = currentNode.GetOrAddInternalChild(currentQueryElement);
                    }
                    else
                    {
                        currentNode.AddLeafChild(currentQueryElement, value);
                    }

                    currentQueryElement = nextQueryElement;
                }
            }
            finally
            {
                queryElements.Dispose();
            }
        }

        /// <summary>
        /// Attempts to retrieve the value associated with a specific term.
        /// </summary>
        /// <param name="term">The term to retrieve the associated value of.</param>
        /// <param name="value">Will be populated with the retrieved value.</param>
        /// <returns>True if and only if a value was successfully retrieved.</returns>
        public bool TryGetExact(Term term, out TValue? value)
        {
            if (term == null)
            {
                throw new ArgumentNullException(nameof(term));
            }

            INode currentNode = Root;
            foreach (var queryElement in new ElementInfoTransformation().ApplyTo(term))
            {
                if (!currentNode.Children.TryGetValue(queryElement, out currentNode!))
                {
                    value = default;
                    return false;
                }
            }

            // NB: can safely grab Value here because the current node must ALWAYS be a LeafNode at this point.
            // It is not possible for prefix element info enumerations to occur, because the the number of
            // elements in the path is always one more than the summation of the ChildElementCounts (NB: used in
            // equality check) of all encountered elements.
            value = currentNode.Value;
            return true;
        }

        /// <summary>
        /// Retrieves all values associated with instances of a given term. That is, all values associated with
        /// terms that can be obtained from the given term by applying a variable substitution to it.
        /// </summary>
        /// <param name="term">The term to query for.</param>
        /// <returns>An enumerable of the value associated with each of the matching terms.</returns>
        public IEnumerable<TValue> GetInstances(Term term)
        {
            if (term == null)
            {
                throw new ArgumentNullException(nameof(term));
            }

            var queryElements = new ElementInfoTransformation().ApplyTo(term).ToList();

            IEnumerable<TValue> ExpandNodes(
                IReadOnlyDictionary<IElementInfo, INode> nodes,
                int queryElementIndex,
                VariableBindings variableBindings)
            {
                var queryElement = queryElements[queryElementIndex];

                if (queryElement is VariableInfo)
                {
                    foreach (var value in ExpandVariableMatches(nodes, queryElementIndex, Enumerable.Empty<IElementInfo>(), 1, variableBindings))
                    {
                        yield return value;
                    }
                }
                else if (nodes.TryGetValue(queryElement, out var childNode))
                {
                    foreach (var value in ExpandNode(childNode, queryElementIndex + 1, variableBindings))
                    {
                        yield return value;
                    }
                }
            }

            IEnumerable<TValue> ExpandVariableMatches(
                IReadOnlyDictionary<IElementInfo, INode> nodes,
                int queryElementIndex,
                IEnumerable<IElementInfo> parentVariableMatch,
                int parentUnexploredBranchCount,
                VariableBindings priorVariableBindings)
            {
                foreach (var (elementInfo, node) in nodes)
                {
                    // NB: there's an obvious possible performance improvement here - cache
                    // the descendent nodes we need to jump to rather than figuring it out afresh
                    // each time. i.e. A "jump list". Might implement this at a later date - not a TODO for now.
                    var variableMatch = parentVariableMatch.Append(elementInfo);
                    var unexploredBranchCount = parentUnexploredBranchCount + elementInfo.ChildElementCount - 1;

                    if (unexploredBranchCount > 0)
                    {
                        foreach (var value in ExpandVariableMatches(
                            node.Children,
                            queryElementIndex,
                            variableMatch,
                            unexploredBranchCount,
                            priorVariableBindings))
                        {
                            yield return value;
                        }
                    }
                    else
                    {
                        var variableBindings = priorVariableBindings;

                        var isVariableMatch = VariableBindings.TryAddOrMatchBinding(
                            ((VariableInfo)queryElements[queryElementIndex]).Ordinal,
                            variableMatch.ToArray(),
                            ref variableBindings);

                        if (isVariableMatch)
                        {
                            foreach (var value in ExpandNode(node, queryElementIndex + 1, variableBindings))
                            {
                                yield return value;
                            }
                        }
                    }
                }
            }

            IEnumerable<TValue> ExpandNode(
                INode node,
                int queryElementIndex,
                VariableBindings variableBindings)
            {
                if (queryElementIndex < queryElements.Count)
                {
                    foreach (var value in ExpandNodes(node.Children, queryElementIndex, variableBindings))
                    {
                        yield return value;
                    }
                }
                else
                {
                    // We can safely grab Value here because node MUST be a LeafNode at this point - ultimately because of how
                    // ElementInfoTransformation works (which controls both the structure of the tree and queryElements here).
                    yield return node.Value;
                }
            }

            return ExpandNodes(Root.Children, 0, new VariableBindings());
        }

        /// <summary>
        /// Retrieves all values associated with generalisations of a given term. That is, all values associated with
        /// terms from which the given term can be obtained by applying a variable substitution to them.
        /// </summary>
        /// <param name="term">The term to query for.</param>
        /// <returns>An enumerable of the value associated with each of the matching terms.</returns>
        public IEnumerable<TValue> GetGeneralisations(Term term)
        {
            if (term == null)
            {
                throw new ArgumentNullException(nameof(term));
            }

            var queryElements = new ElementInfoTransformation().ApplyTo(term).ToArray();

            IEnumerable<TValue> ExpandNode(INode node, int queryElementIndex, VariableBindings variableBindings)
            {
                foreach (var (childElement, childNode) in node.Children)
                {
                    bool isVariableMatch = false;
                    var nextQueryElementOffset = 1;
                    var childVariableBindings = variableBindings;

                    if (childElement is VariableInfo variableInfo)
                    {
                        // Here we are setting nextQueryInfoOffset to the size of the whole subtree
                        // with queryInfos[queryInfoIndex] at its root (i.e. the subtree that we are
                        // matching to the variable).
                        var unexploredBranchCount = queryElements[queryElementIndex].ChildElementCount;
                        while (unexploredBranchCount > 0)
                        {
                            unexploredBranchCount += queryElements[queryElementIndex + nextQueryElementOffset].ChildElementCount - 1;
                            nextQueryElementOffset++;
                        }

                        // Now we need to verify subtree's consistency with any existing variable binding, add a binding if needed,
                        // and set isVariableMatch appropriately.
                        isVariableMatch = VariableBindings.TryAddOrMatchBinding(
                            variableInfo.Ordinal,
                            queryElements[queryElementIndex..(queryElementIndex + nextQueryElementOffset)],
                            ref childVariableBindings);
                    }

                    if (isVariableMatch || childElement.Equals(queryElements[queryElementIndex]))
                    {
                        var nextQueryElementIndex = queryElementIndex + nextQueryElementOffset;

                        if (nextQueryElementIndex < queryElements.Length)
                        {
                            foreach (var value in ExpandNode(childNode, nextQueryElementIndex, childVariableBindings))
                            {
                                yield return value;
                            }
                        }
                        else
                        {
                            // We can safely grab Value here because childNode MUST be a LeafNode at this point - ultimately because of how
                            // ElementInfoTransformation works (which controls both the structure of the tree and queryElements here).
                            yield return childNode.Value;
                        }
                    }
                }
            }

            return ExpandNode(Root, 0, new VariableBindings());
        }

        // public IEnumerable<TValue> GetUnifications(Term term) -- not yet..

        /// <summary>
        /// Interface shared by all nodes of a <see cref="DiscriminationTree{TValue}"/>.
        /// </summary>
        public interface INode
        {
            /// <summary>
            /// Gets the child nodes of this node, keyed by objects that describe the element represented by the child.
            /// </summary>
            IReadOnlyDictionary<IElementInfo, INode> Children { get; }

            /// <summary>
            /// Gets the value attached to a leaf node - throws an exception for internal nodes.
            /// </summary>
            TValue Value { get; }
        }

        /// <summary>
        /// Representation of an internal node (that is, a node without an attached value) of a discrimination tree.
        /// </summary>
        public sealed class InternalNode : INode
        {
            private readonly Dictionary<IElementInfo, INode> children = new();

            /// <inheritdoc/>
            // NB: we don't bother wrapping children in a ReadOnlyDict to stop unscrupulous
            // users from casting. Would be more mem for a real edge case.. 
            public IReadOnlyDictionary<IElementInfo, INode> Children => children;

            /// <inheritdoc/>
            public TValue Value => throw new NotSupportedException("Internal node - has no value");

            internal InternalNode GetOrAddInternalChild(IElementInfo elementInfo)
            {
                if (!Children.TryGetValue(elementInfo, out var node))
                {
                    node = new InternalNode();
                    children.Add(elementInfo, node);
                }

                return (InternalNode)node;
            }

            internal void AddLeafChild(IElementInfo elementInfo, TValue value)
            {
                if (!children.TryAdd(elementInfo, new LeafNode(value)))
                {
                    throw new ArgumentException("Key already present", nameof(elementInfo));
                }
            }
        }

        /// <summary>
        /// Representation of a leaf node (that is, a node with an attached value) of a discrimination tree.
        /// </summary>
        public sealed class LeafNode : INode
        {
            private static readonly ReadOnlyDictionary<IElementInfo, INode> emptyChildren = new(new Dictionary<IElementInfo, INode>());

            internal LeafNode(TValue value) => Value = value;

            /// <inheritdoc/>
            public IReadOnlyDictionary<IElementInfo, INode> Children => emptyChildren;

            /// <inheritdoc/>
            public TValue Value { get; }
        }

        /// <summary>
        /// Interface for the types that describe elements of a term.
        /// Instances of this interface are associated with each non-root element of a discrimination tree.
        /// </summary>
        public interface IElementInfo
        {
            /// <summary>
            /// Gets the number of child elements of the element described by this object.
            /// </summary>
            int ChildElementCount { get; }
        }

        /// <summary>
        /// Information about a function, for storage against a node of a <see cref="DiscriminationTree{TValue}"/>.
        /// </summary>
        /// <param name="Identifier">The identifier of the represented function.</param>
        /// <param name="ArgumentCount">The number of arguments of the represented function.</param>
        public sealed record FunctionInfo(object Identifier, int ArgumentCount) : IElementInfo
        {
            /// <inheritdoc/>
            public int ChildElementCount => ArgumentCount;
        }

        /// <summary>
        /// Information about a constant, for storage against a node of a <see cref="DiscriminationTree{TValue}"/>.
        /// </summary>
        /// <param name="Identifier">The identifier of the represented constant.</param>
        public sealed record ConstantInfo(object Identifier) : IElementInfo
        {
            /// <inheritdoc/>
            public int ChildElementCount => 0;
        }

        /// <summary>
        /// Information about a variable, for storage against a node of a <see cref="DiscriminationTree{TValue}"/>.
        /// </summary>
        /// <param name="Ordinal">
        /// The ordinal of the represented variable - that is, the index of its position in a list of variables that
        /// exist in the term, in the order in which they are first encountered by a depth-first traversal.
        /// </param>
        public sealed record VariableInfo(int Ordinal) : IElementInfo
        {
            /// <inheritdoc/>
            public int ChildElementCount => 0;
        }

        /// <summary>
        /// Transformation logic that converts <see cref="Term"/>s into the equivalent path of <see cref="IElementInfo"/>s for storage in or querying of a discrimination tree.
        /// That is, converts terms into an enumerable that represents a depth-first traversal of their constituent elements.
        /// </summary>
        private class ElementInfoTransformation
        {
            // TODO-PERFORMANCE: a dictionary is almost certainly overkill given the low number of vars likely to
            // appear in any given term. Plain old list likely to perform better. Test me.
            private readonly Dictionary<object, int> variableIdMap = new(); 

            public IEnumerable<IElementInfo> ApplyTo(Term term)
            {
                return term switch
                {
                    Constant constant => ApplyTo(constant),
                    VariableReference variable => ApplyTo(variable),
                    Function function => ApplyTo(function),
                    _ => throw new ArgumentException($"Unrecognised Term type '{term.GetType()}'", nameof(term))
                };
            }

            public IEnumerable<IElementInfo> ApplyTo(Constant constant)
            {
                yield return new ConstantInfo(constant.Symbol);
            }

            public IEnumerable<IElementInfo> ApplyTo(Function function)
            {
                yield return new FunctionInfo(function.Symbol, function.Arguments.Count);

                foreach (var argument in function.Arguments)
                {
                    foreach (var node in ApplyTo(argument))
                    {
                        yield return node;
                    }
                }
            }

            public IEnumerable<IElementInfo> ApplyTo(VariableReference variable)
            {
                // Variable declarations are "ordinalised" (probably not the "right" terminology - need to look this up).
                // That is, converted into the ordinal of where they first appear in a depth-first traversal of the term.
                // This is useful because it means e.g. F(X, X) is transformed to a path that is identical to the transformation of F(Y, Y),
                // but different to the transformation of F(X, Y).
                if (!variableIdMap.TryGetValue(variable.Symbol, out var ordinal))
                {
                    ordinal = variableIdMap[variable.Symbol] = variableIdMap.Count;
                }

                yield return new VariableInfo(ordinal);
            }
        }

        private class VariableBindings
        {
            // TODO-ZZ: does this need to be a dictionary - we should always encounter variables in ordinal order, i think?
            private readonly Dictionary<int, IElementInfo[]> map;

            public VariableBindings() => map = new();

            private VariableBindings(IEnumerable<KeyValuePair<int, IElementInfo[]>> content) => map = new(content);

            // overall performance would perhaps be better with a tree instead of copying a dictionary.. worth a test at some point perhaps.
            // (perhaps after looking at substitution trees in general). Might not be worth the complexity though.
            public static bool TryAddOrMatchBinding(int ordinal, IElementInfo[] value, ref VariableBindings bindings)
            {
                // NB the "one-way" nature of this binding means this logic can be
                // simpler than that of unification (see LiteralUnifier) - here, we just need to check for equality.
                // This behaviour will need to CHANGE if we add unifier discovery logic to the tree.
                if (!bindings.map.TryGetValue(ordinal, out var existingBinding))
                {
                    bindings = new VariableBindings(bindings.map.Append(KeyValuePair.Create(ordinal, value)));
                    return true;
                }

                if (existingBinding.Length != value.Length)
                {
                    return false;
                }

                for (int i = 0; i < existingBinding.Length; i++)
                {
                    if (!existingBinding[i].Equals(value[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
