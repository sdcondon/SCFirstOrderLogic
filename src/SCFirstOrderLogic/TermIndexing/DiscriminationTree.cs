using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.TermIndexing
{
    /// <summary>
    /// An implementation of an in-memory discrimination tree for <see cref="Term"/>s - specifically, one for which the attached values are the terms themselves.
    /// </summary>
    /// <seealso href="https://www.google.com/search?q=discrimination+tree"/>
    public class DiscriminationTree
    {
        private readonly DiscriminationTree<Term> actualTree;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscriminationTree"/> class that is empty to begin with.
        /// </summary>
        public DiscriminationTree()
        {
            actualTree = new();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscriminationTree"/> class with some initial content.
        /// </summary>
        public DiscriminationTree(IEnumerable<Term> content)
        {
            actualTree = new(content.Select(t => KeyValuePair.Create(t, t)));
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
        public DiscriminationTree<Term>.InternalNode Root => actualTree.Root;

        /// <summary>
        /// Adds a <see cref="Term"/> to the tree.
        /// </summary>
        /// <param name="term">The term to add.</param>
        public void Add(Term term) => actualTree.Add(term, term);

        /// <summary>
        /// Determines whether an exact match to a given term is contained within the tree.
        /// </summary>
        /// <param name="term">The term to query for.</param>
        /// <returns>True if and only if the term is contained within the tree.</returns>
        public bool Contains(Term term) => actualTree.TryGetExact(term, out _);

        /// <summary>
        /// Retrieves all instances of a given term. That is, all terms that can be
        /// obtained from the given term by applying a variable substitution to it.
        /// </summary>
        /// <param name="term">The term to query for.</param>
        /// <returns>An enumerable of all matching terms.</returns>
        public IEnumerable<Term> GetInstances(Term term) => actualTree.GetInstances(term);

        /// <summary>
        /// Retrieves all generalisations of a given term. That is, all terms from which
        /// the given term can be obtained by applying a variable substitution to them.
        /// </summary>
        /// <param name="term">The term to query for.</param>
        /// <returns>An enumerable of all matching terms.</returns>
        public IEnumerable<Term> GetGeneralisations(Term term) => actualTree.GetGeneralisations(term);
    }
}
