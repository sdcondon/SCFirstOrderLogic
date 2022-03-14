using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm
{
    /// <summary>
    /// Representation of an individual clause (i.e. a disjunction of <see cref="CNFLiteral"/>s) of a first-order logic sentence in conjunctive normal form.
    /// </summary>
    public class CNFClause
    {
        private static readonly LiteralComparer literalComparer = new LiteralComparer();

        /// <summary>
        /// Initialises a new instance of the <see cref="CNFClause"/> class.
        /// </summary>
        /// <param name="sentence">The clause, represented as a <see cref="Sentence"/>.</param>
        /// <remarks>
        /// NB: Internal because it makes the assumption that the sentence is a disjunction of literals. If it were public we'd need to verify that.
        /// </remarks>
        internal CNFClause(Sentence sentence)
        {
            var literals = new SortedSet<CNFLiteral>(new LiteralComparer());
            new ClauseConstructor(literals).ApplyTo(sentence);
            Literals = literals; // TODO-ROBUSTNESS: would rather actually wrap this with something akin to an AsReadOnly, but not a huge deal..
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CNFClause"/> class from an enumerable of literals (removing any mutually-negating literals and duplicates as it does so).
        /// </summary>
        /// <param name="lambda">The set of literals to be included in the clause.</param>
        /// <remarks>
        /// While there is nothing stopping this being public from a robustness perspective, there is as yet
        /// no easy way for consumers to instantiate a literal on its own - making it pointless for the
        /// moment.
        /// </remarks>
        internal CNFClause(IEnumerable<CNFLiteral> literals)
        {
            // TODO-ROBUSTNESS: would rather actually wrap this with something akin to an AsReadOnly, but not a huge deal..
            Literals = new SortedSet<CNFLiteral>(literals, literalComparer);
        }

        /// <summary>
        /// Gets an instance of the empty clause.
        /// </summary>
        public static CNFClause Empty { get; } = new CNFClause(Array.Empty<CNFLiteral>());

        /// <summary>
        /// Gets the collection of literals that comprise this clause.
        /// </summary>
        /// <remarks>
        /// NB: Literals are ordered first by underlying atomic sentence (hash code), then by whether they are positive or not. This makes resolution easier.
        /// </remarks>
        public IReadOnlyCollection<CNFLiteral> Literals { get; }

        /// <summary>
        /// Gets a value indicating whether this is a Horn clause - that is, whether at most one of its literals is positive.
        /// </summary>
        public bool IsHornClause => Literals.Count(l => l.IsPositive) <= 1;

        /// <summary>
        /// Gets a value indicating whether this is a definite clause - that is, whether exactly one of its literals is positive.
        /// </summary>
        public bool IsDefiniteClause => Literals.Count(l => l.IsPositive) == 1;

        /// <summary>
        /// Gets a value indicating whether this is a goal clause - that is, whether none of its literals is positive.
        /// </summary>
        public bool IsGoalClause => Literals.Count(l => l.IsPositive) == 0;

        /// <summary>
        /// Gets a value indicating whether this is a unit clause - that is, whether it contains exactly one literal.
        /// </summary>
        public bool IsUnitClause => Literals.Count == 1;

        /// <summary>
        /// Gets a value indicating whether this is an empty clause (that implicitly evaluates to false). Can occur as a result of resolution.
        /// </summary>
        public bool IsEmpty => Literals.Count == 0;

        /// <summary>
        /// Resolves two clauses to potentially create some new clauses.
        /// </summary>
        /// <param name="clause1">The first of the clauses to resolve.</param>
        /// <param name="clause2">The second of the clauses to resolve.</param>
        /// <returns>A new clause.</returns>
        public static IEnumerable<CNFClause> Resolve(CNFClause clause1, CNFClause clause2)
        {
            //// Q1: Should we be discarding trivially true output clauses (that contain another complementary literal)?
            //// Q2: does any clause pair that contains more than one complementary literal pair necessarily only yield trivially true clauses? Seems like it must?
            //// This method could be simplified and made more performant depending on the answers to those questions, but the source material doesn't make
            //// this clear so I have erred on the side of caution..

            var resolvents = new List<SortedSet<CNFLiteral>>();
            var resolventPrototype = new SortedSet<CNFLiteral>(literalComparer);
            var literals1 = clause1.Literals.GetEnumerator();
            var literals2 = clause2.Literals.GetEnumerator();
            var moveNext1 = true;
            var moveNext2 = true;

            // Adds a literal to any existing resolvents & the resolvent prototype
            void AddToResolvents(CNFLiteral literal)
            {
                foreach (var resolvent in resolvents)
                {
                    resolvent.Add(literal);
                }

                resolventPrototype.Add(literal);
            }

            // Adds a new resolvent using the current resolvent prototype, as well as adding the two
            // complementary literals to any existing resolvents and the resolvent prototype.
            void AddResolvent(CNFLiteral literal, CNFLiteral complementaryLiteral)
            {
                foreach (var resolvent in resolvents)
                {
                    resolvent.Add(literal);
                    resolvent.Add(complementaryLiteral);
                }

                resolvents.Add(new SortedSet<CNFLiteral>(resolventPrototype, literalComparer));

                resolventPrototype.Add(literal);
                resolventPrototype.Add(complementaryLiteral);
            }

            // Attempts to move to the next literal in one or both input clauses - adding any remaining
            // literals in the other clause to the output if either of the clauses is exhausted
            bool MoveNext(bool moveNext1, bool moveNext2)
            {
                if (moveNext1 && !literals1.MoveNext())
                {
                    if (!moveNext2)
                    {
                        foreach (var resolvent in resolvents)
                        {
                            resolvent.Add(literals2.Current);
                        }
                    }

                    while (literals2.MoveNext())
                    {
                        foreach (var resolvent in resolvents)
                        {
                            resolvent.Add(literals2.Current);
                        }
                    }

                    return false;
                }

                if (moveNext2 && !literals2.MoveNext())
                {
                    foreach (var resolvent in resolvents)
                    {
                        resolvent.Add(literals1.Current);
                    }

                    while (literals1.MoveNext())
                    {
                        foreach (var resolvent in resolvents)
                        {
                            resolvent.Add(literals1.Current);
                        }
                    }

                    return false;
                }

                return true;
            }

            while (MoveNext(moveNext1, moveNext2))
            {
                var literal1 = literals1.Current;
                var literal2 = literals2.Current;

                if (CNFLiteralUnifier.TryCreate(literal1, literal2.Negate(), out _))
                {
                    AddResolvent(literal1, literal2);
                    moveNext1 = moveNext2 = true;
                }
                else
                {
                    var comparison = literalComparer.Compare(literal1, literal2);
                    AddToResolvents(comparison <= 0 ? literal1 : literal2);
                    moveNext1 = comparison <= 0;
                    moveNext2 = comparison >= 0;
                }
            }

            return resolvents.Select(r => new CNFClause(r));
        }

        /// <inheritdoc />
        public override string ToString() => Literals.Count == 0 ? "<FALSE>" : string.Join(" ∨ ", Literals);

        /// <inheritdoc />
        /// <remarks>
        /// Clauses that contain exactly the same set of literals are considered equal.
        /// </remarks>
        public override bool Equals(object obj)
        {
            if (!(obj is CNFClause clause) || Literals.Count != clause.Literals.Count)
            {
                return false;
            }

            foreach (var (xLiteral, yLiteral) in Literals.Zip(clause.Literals, (x, y) => (x, y)))
            {
                if (!xLiteral.Equals(yLiteral))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var literal in Literals)
            {
                hash.Add(literal);
            }

            return hash.ToHashCode();
        }

        private class ClauseConstructor : SentenceTransformation
        {
            private readonly ISet<CNFLiteral> literals;

            public ClauseConstructor(ISet<CNFLiteral> literals) => this.literals = literals;

            public override Sentence ApplyTo(Sentence sentence)
            {
                if (sentence is Disjunction)
                {
                    // The sentence is assumed to be a clause (i.e. a disjunction of literals) - so just skip past all the disjunctions at the root.
                    return base.ApplyTo(sentence);
                }
                else
                {
                    // Assume we've hit a literal. NB will throw if its not actually a literal.
                    literals.Add(new CNFLiteral(sentence));

                    // We don't need to look any further down the tree for the purposes of this class (though the CNFLiteral ctor, above,
                    // does so to figure out the details of the literal). So we can just return node rather than invoking base.ApplyTo. 
                    return sentence;
                }
            }
        }

        private class LiteralComparer : IComparer<CNFLiteral>
        {
            public int Compare(CNFLiteral x, CNFLiteral y)
            {
                var hashComparison = x.Predicate.GetHashCode().CompareTo(y.Predicate.GetHashCode());
                if (hashComparison != 0)
                {
                    return hashComparison;
                }

                return x.IsPositive.CompareTo(y.IsPositive);
            }
        }
    }
}
