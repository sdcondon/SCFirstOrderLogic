using SCFirstOrderLogic;
using SCFirstOrderLogic.SentenceFormatting;
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of an individual clause (i.e. a disjunction of <see cref="Literal"/>s) of a first-order logic sentence in conjunctive normal form.
    /// </summary>
    public class CNFClause : IEquatable<CNFClause>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="CNFClause"/> class from a sentence that is a disjunction of literals (a literal being a predicate or a negated predicate).
        /// </summary>
        /// <param name="sentence">The clause, represented as a <see cref="Sentence"/>. An <see cref="ArgumentException"/> exception will be thrown if it is not a disjunction of literals.</param>
        public CNFClause(Sentence sentence)
        {
            var ctor = new ClauseConstructor();
            ctor.Visit(sentence);

            // We *could* actually use an immutable type to stop unscrupulous users from making it mutable by casting, but
            // its a super low-level class and I'd rather err on the side of using the smallest & simplest implementation possible.
            // Note that we order literals - which is important to justifiably consider the clause "normalised".
            // BUG: Possible problems when hash code collisions occur. Probably worth a more robust approach at some point - but
            // clause equality will be checked a LOT during resolution..
            Literals = ctor.Literals.OrderBy(l => l.GetHashCode()).ToArray();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CNFClause"/> class from an enumerable of literals (removing any mutually-negating literals and duplicates as it does so).
        /// </summary>
        /// <param name="literals">The set of literals to be included in the clause.</param>
        public CNFClause(IEnumerable<Literal> literals)
        {
            // We *could* actually use an immutable type to stop unscrupulous users from making it mutable by casting, but
            // its a super low-level class and I'd rather err on the side of using the simplest/smallest implementation possible.
            // Note that we order literals - important to justifiably consider the clause "normalised".
            Literals = literals.OrderBy(l => l.GetHashCode()).ToArray();
        }

        /// <summary>
        /// Gets an instance of the empty clause.
        /// </summary>
        public static CNFClause Empty { get; } = new CNFClause(Array.Empty<Literal>());

        /// <summary>
        /// Gets the collection of literals that comprise this clause (ordered by hash code).
        /// </summary>
        public IReadOnlyCollection<Literal> Literals { get; }

        /// <summary>
        /// Gets a value indicating whether this is a Horn clause - that is, whether at most one of its literals is positive.
        /// <para/>
        /// No caching here, but the class is immutable so recalculating every time is wasted effort, strictly speaking.
        /// Various things we could do, but for the moment I'm erring on the side of doing nothing, on the grounds that these
        /// properties are unlikely to be on the "hot" path of any given applicaiton.
        /// </summary>
        public bool IsHornClause => Literals.Count(l => l.IsPositive) <= 1;

        /// <summary>
        /// Gets a value indicating whether this is a definite clause - that is, whether exactly one of its literals is positive.
        /// <para/>
        /// NB: this means that the clause can be written in the form L₁ ∧ L₂ ∧ .. ∧ Lₙ ⇒ L, where none of the literals is negated
        /// - or is simply a single non-negated literal.
        /// </summary>
        public bool IsDefiniteClause => Literals.Count(l => l.IsPositive) == 1;

        /// <summary>
        /// Gets a value indicating whether this is a goal clause - that is, whether none of its literals is positive.
        /// </summary>
        public bool IsGoalClause => !Literals.Any(l => l.IsPositive);

        /// <summary>
        /// Gets a value indicating whether this is a unit clause - that is, whether it contains exactly one literal.
        /// </summary>
        public bool IsUnitClause => Literals.Count == 1;

        /// <summary>
        /// Gets a value indicating whether this is an empty clause (that by convention evaluates to false). Can occur as a result of resolution.
        /// </summary>
        public bool IsEmpty => Literals.Count == 0;

        /// <summary>
        /// Constructs and returns a clause that is the same as this one, except for the
        /// fact that all variable references are replaced with new ones.
        /// </summary>
        /// <returns>
        /// A clause that is the same as this one, except for the fact that all variable
        /// references are replaced with new ones.
        /// </returns>
        public CNFClause Restandardize()
        {
            var mapping = new Dictionary<StandardisedVariableSymbol, StandardisedVariableSymbol>();

            StandardisedVariableSymbol GetOrAddNewSymbol(StandardisedVariableSymbol oldSymbol)
            {
                if (!mapping!.TryGetValue(oldSymbol, out var newSymbol))
                {
                    newSymbol = mapping[oldSymbol] = new StandardisedVariableSymbol(oldSymbol.OriginalVariableScope, oldSymbol.OriginalSentence);
                }

                return newSymbol;
            }

            Term VisitTerm(Term term) => term switch
            {
                Constant c => c,
                VariableReference v => new VariableReference(GetOrAddNewSymbol((StandardisedVariableSymbol)v.Symbol)),
                Function f => new Function(f.Symbol, f.Arguments.Select(a => VisitTerm(a)).ToArray()),
                _ => throw new ArgumentException($"Unexpected term type '{term.GetType()}' encountered", nameof(term)),
            };

            Predicate VisitPredicate(Predicate predicate) => new Predicate(predicate.Symbol, predicate.Arguments.Select(a => VisitTerm(a)).ToArray());

            Literal VisitLiteral(Literal literal) => new Literal(VisitPredicate(literal.Predicate), literal.IsNegated);

            return new CNFClause(Literals.Select(l => VisitLiteral(l)));
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// <para/>
        /// NB: The implementation of this override creates a <see cref="SentenceFormatter"/> object and uses it to format the clause.
        /// Note that this will not guarantee unique labelling of normalisation terms (standardised variables or Skolem functions)
        /// across multiple calls, or provide any choice as to the sets of labels used for normalisation terms. If you want either
        /// of these things, instantiate your own <see cref="SentenceFormatter"/> instance.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => new SentenceFormatter().Format(this);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is CNFClause clause && Equals(clause);

        /// <inheritdoc />
        /// <remarks>
        /// Clauses that contain exactly the same collection of literals are considered equal.
        /// </remarks>
        public bool Equals(CNFClause? other)
        {
            if (other == null || Literals.Count != other.Literals.Count)
            {
                return false;
            }

            foreach (var (xLiteral, yLiteral) in Literals.Zip(other.Literals, (x, y) => (x, y)))
            {
                if (!xLiteral.Equals(yLiteral))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Clauses that contain exactly the same collection of literals are considered equal.
        /// </remarks>
        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var literal in Literals)
            {
                hash.Add(literal);
            }

            return hash.ToHashCode();
        }

        private class ClauseConstructor : RecursiveSentenceVisitor
        {
            public HashSet<Literal> Literals { get; } = new HashSet<Literal>();

            public override void Visit(Sentence sentence)
            {
                if (sentence is Disjunction disjunction)
                {
                    // The sentence is assumed to be a clause (i.e. a disjunction of literals) - so just skip past all the disjunctions at the root.
                    base.Visit(disjunction);
                }
                else
                {
                    // Assume we've hit a literal. NB will throw if its not actually a literal.
                    // Afterwards, we don't need to look any further down the tree for the purposes of this class (though the Literal ctor that
                    // we invoke here does so to figure out the details of the literal). So we can just return rather than invoking base.Visit.
                    Literals.Add(new Literal(sentence));
                }
            }
        }
    }
}
