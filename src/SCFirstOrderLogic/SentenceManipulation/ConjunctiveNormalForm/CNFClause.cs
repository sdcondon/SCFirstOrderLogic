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
        /// <summary>
        /// Initialises a new instance of the <see cref="CNFClause"/> class from a sentence that is a disjunction of literals (a literal being a predicate or a negated predicate).
        /// </summary>
        /// <param name="sentence">The clause, represented as a <see cref="Sentence"/>. An <see cref="ArgumentException"/> exception will be thrown if it is not a disjunction of literals.</param>
        public CNFClause(Sentence sentence)
        {
            var ctor = new ClauseConstructor();
            ctor.ApplyTo(sentence);
            Literals = ctor.Literals; // TODO-ROBUSTNESS: would rather actually wrap this with something akin to an AsReadOnly, but not a huge deal..
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CNFClause"/> class from an enumerable of literals (removing any mutually-negating literals and duplicates as it does so).
        /// </summary>
        /// <param name="literals">The set of literals to be included in the clause.</param>
        public CNFClause(IEnumerable<CNFLiteral> literals)
        {
            // TODO-ROBUSTNESS: would rather actually wrap this with something akin to an AsReadOnly, but not a huge deal..
            Literals = literals.ToArray();
        }

        /// <summary>
        /// Gets an instance of the empty clause.
        /// </summary>
        public static CNFClause Empty { get; } = new CNFClause(Array.Empty<CNFLiteral>());

        /// <summary>
        /// Gets the collection of literals that comprise this clause.
        /// </summary>
        public IReadOnlyCollection<CNFLiteral> Literals { get; }

        /// <summary>
        /// Gets a value indicating whether this is a Horn clause - that is, whether at most one of its literals is positive.
        /// <para/>
        /// TODO: No caching here, but the class is immutable so recalculating every time is wasted effort.
        /// Don't want to calculate on construction because this class is super low-level and we might never retrieve this property.
        /// In short - perhaps look at using a Lazy&lt;T&gt; at some point?
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
        public static IEnumerable<(CNFClause resolvent, CNFLiteralUnifier unifier)> Resolve(CNFClause clause1, CNFClause clause2)
        {
            // Yes, this is a slow implementation. It is simple, though - and thus will serve
            // well as a baseline for improvements. (I'm thinking of a CNFLiteralUnifier that accepts multiple
            // literals and examines the tree for them all "simultaneously" - i.e. do full resolution, not binary).
            foreach (var literal1 in clause1.Literals)
            {
                foreach (var literal2 in clause2.Literals)
                {
                    if (CNFLiteralUnifier.TryCreate(literal1, literal2.Negate(), out var unifier))
                    {
                        var resolventLiterals = new HashSet<CNFLiteral>(clause1.Literals
                            .Concat(clause2.Literals)
                            .Except(new[] { literal1, literal2 })
                            .Select(l => unifier.ApplyTo(l)));

                        var factoringCarriedOut = false;
                        var clauseIsTriviallyTrue = false;
                        do
                        {
                            factoringCarriedOut = false;
                            foreach (var rLiteral1 in resolventLiterals)
                            {
                                foreach (var rLiteral2 in resolventLiterals)
                                {
                                    if (!rLiteral1.Equals(rLiteral2) && CNFLiteralUnifier.TryCreate(rLiteral1, rLiteral2, out var factoringUnifier))
                                    {
                                        resolventLiterals = new HashSet<CNFLiteral>(resolventLiterals.Select(l => factoringUnifier.ApplyTo(l)));
                                        factoringCarriedOut = true;
                                        break;
                                    }

                                    if (rLiteral1.Predicate.Equals(rLiteral2) && rLiteral1.IsPositive != rLiteral2.IsPositive)
                                    {
                                        clauseIsTriviallyTrue = true;
                                        break;
                                    }
                                }

                                if (factoringCarriedOut || clauseIsTriviallyTrue)
                                {
                                    break;
                                }
                            }
                        }
                        while (factoringCarriedOut);

                        if (!clauseIsTriviallyTrue)
                        {
                            yield return (new CNFClause(resolventLiterals), unifier);
                        }
                    }
                }
            }
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
            public override Sentence ApplyTo(Sentence sentence)
            {
                if (sentence is Disjunction disjunction)
                {
                    // The sentence is assumed to be a clause (i.e. a disjunction of literals) - so just skip past all the disjunctions at the root.
                    return base.ApplyTo(disjunction);
                }
                else
                {
                    // Assume we've hit a literal. NB will throw if its not actually a literal.
                    Literals.Add(new CNFLiteral(sentence));

                    // We don't need to look any further down the tree for the purposes of this class (though the CNFLiteral ctor, above,
                    // does so to figure out the details of the literal). So we can just return node rather than invoking base.ApplyTo. 
                    return sentence;
                }
            }

            public HashSet<CNFLiteral> Literals { get; } = new HashSet<CNFLiteral>();
        }
    }
}
