// Copied wholesale from LinqToKB.PredicateLogic..
#if false
using LinqToKB.PropositionalLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderlLogic.SentenceManipulation.ConjunctiveNormalForm
{
    /// <summary>
    /// Representation of an individual clause of a predicate expression in conjunctive normal form - that is, a disjunction of literals (<see cref="PLLiteral{TModel}"/>s).
    /// </summary>
    /// <typeparam name="TModel">The type that the literals of this clause refer to.</typeparam>
    public class CNFClause<TModel>
    {
        private static readonly LiteralComparer literalComparer = new LiteralComparer();

        /// <summary>
        /// Initialises a new instance of the <see cref="CNFClause{TModel}"/> class.
        /// </summary>
        /// <param name="lambda">The clause, represented as a lambda expression.</param>
        /// <remarks>
        /// NB: Internal because it makes the assumption that the lambda is a disjunction of literals. If it were public we'd need to verify that.
        /// </remarks>
        internal CNFClause(Expression<Predicate<TModel>> lambda)
        {
            Lambda = lambda; // Assumed to be a disjunction of literals
            var literals = new SortedSet<PLLiteral<TModel>>(new LiteralComparer());
            new ClauseConstructor(this, literals).Visit(lambda.Body);
            Literals = literals; // TODO-ROBUSTNESS: would rather actually wrap this with something akin to an AsReadOnly, but not a huge deal..
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CNFClause{TModel}"/> class from an enumerable of literals (removing any mutually-negating literals and duplicates as it does so).
        /// </summary>
        /// <param name="lambda">The set of literals to be included in the clause.</param>
        /// <remarks>
        /// While there is nothing stopping this being public from a robustness perspective, there is as yet
        /// no easy way for consumers to instantiate a literal on its own - making it pointless for the
        /// moment.
        /// </remarks>
        internal CNFClause(IEnumerable<PLLiteral<TModel>> literals)
        {
            // TODO-ROBUSTNESS: would rather actually wrap this with something akin to an AsReadOnly, but not a huge deal..
            Literals = new SortedSet<PLLiteral<TModel>>(literals, literalComparer);

            if (Literals.Count == 0)
            {
                // It is an important maxim of propositional logic that empty clauses evaluate to false
                Lambda = Expression.Lambda<Predicate<TModel>>(Expression.Constant(false), Expression.Parameter(typeof(TModel)));
            }
            else
            {
                // NB: The literals here will often be part of completely separate expressions - so will
                // refer to different parameter expression objects. To combine their lambdas into a single lambda,
                // we need to unify the referenced parameter expressions into a single one.
                var parameterReplacer = new ParameterReplacer<TModel>(Literals.First().Lambda.Parameters.Single().Name);

                var node = parameterReplacer.Visit(Literals.First().Lambda.Body);
                foreach (var literal in Literals.Skip(1))
                {
                    node = Expression.OrElse(node, parameterReplacer.Visit(literal.Lambda.Body));
                }

                Lambda = Expression.Lambda<Predicate<TModel>>(node, parameterReplacer.NewParameter);
            }
        }

        /// <summary>
        /// Returns an instance of the empty clause.
        /// </summary>
        public static CNFClause<TModel> Empty { get; } = new CNFClause<TModel>(Array.Empty<PLLiteral<TModel>>());

        /// <summary>
        /// Gets a representation of this clause as a lambda expression.
        /// </summary>
        public Expression<Predicate<TModel>> Lambda { get; }

        /// <summary>
        /// Gets the collection of literals that comprise this clause.
        /// </summary>
        public IReadOnlyCollection<PLLiteral<TModel>> Literals { get; }

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
        public bool IsUnitClause => Literals.Count() == 1;

        /// <summary>
        /// Gets a value indicating whether this is an empty clause (that implicitly evaluates to false). Can occur as a result of resolution.
        /// </summary>
        public bool IsEmpty => Literals.Count() == 0;

        /// <summary>
        /// Resolves two clauses to potentially create some new clauses.
        /// </summary>
        /// <param name="clause1">The first of the clauses to resolve.</param>
        /// <param name="clause2">The second of the clauses to resolve.</param>
        /// <returns>A new clause.</returns>
        /// <remarks>
        /// NB: IMO, at the time of writing, http://logic.stanford.edu/intrologic/notes/chapter_05.html is a far better resource 
        /// on propositional resolution than the book mentioned in the readme.
        /// </remarks>
        public static IEnumerable<CNFClause<TModel>> Resolve(CNFClause<TModel> clause1, CNFClause<TModel> clause2)
        {
            //// Q1: Should we be discarding trivially true output clauses (that contain another complementary literal)?
            //// Q2: does any clause pair that contains more than one complementary literal pair necessarily only yield trivially true clauses? Seems like it must?
            //// This method could be simplified and made more performant depending on the answers to those questions, but the source material doesn't make
            //// this clear so I have erred on the side of caution..

            var resolvents = new List<SortedSet<PLLiteral<TModel>>>();
            var resolventPrototype = new SortedSet<PLLiteral<TModel>>(literalComparer);
            var literals1 = clause1.Literals.GetEnumerator();
            var literals2 = clause2.Literals.GetEnumerator();
            var moveNext1 = true;
            var moveNext2 = true;

            // Adds a literal to any existing resolvents & the resolvent prototype
            void AddToResolvents(PLLiteral<TModel> literal)
            {
                foreach (var resolvent in resolvents)
                {
                    resolvent.Add(literal);
                }

                resolventPrototype.Add(literal);
            }

            // Adds a new resolvent using the current resolvent prototype, as well as adding the two
            // complementary literals to any existing resolvents and the resolvent prototype.
            void AddResolvent(PLLiteral<TModel> literal, PLLiteral<TModel> complementaryLiteral)
            {
                foreach (var resolvent in resolvents)
                {
                    resolvent.Add(literal);
                    resolvent.Add(complementaryLiteral);
                }

                resolvents.Add(new SortedSet<PLLiteral<TModel>>(resolventPrototype, literalComparer));

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

                if (literal1.AtomicSentence.Equals(literal2.AtomicSentence) && literal1.IsNegated != literal2.IsNegated)
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

            return resolvents.Select(r => new CNFClause<TModel>(r));
        }

        /// <inheritdoc />
        public override string ToString() => string.Join(" ∨ ", Literals);

        /// <inheritdoc />
        /// <remarks>
        /// Clauses that contain exactly the same set of literals are considered equal.
        /// </remarks>
        public override bool Equals(object obj)
        {
            if (!(obj is CNFClause<TModel> clause) || Literals.Count != clause.Literals.Count)
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

        private class ClauseConstructor : ExpressionVisitor
        {
            private readonly CNFClause<TModel> owner;
            private readonly ISet<PLLiteral<TModel>> literals;

            public ClauseConstructor(CNFClause<TModel> owner, ISet<PLLiteral<TModel>> literals) => (this.owner, this.literals) = (owner, literals);

            public override Expression Visit(Expression node)
            {
                if (node is BinaryExpression orElse && orElse.NodeType == ExpressionType.OrElse)
                {
                    // The expression is guaranteed to be a disjunction of literals - so the root down until the individual clauses will all be OrElse - we just skip past those.
                    return base.Visit(node);
                }
                else
                {
                    // We've hit a literal.
                    // NB: PLLiteral accepts a lambda - not just an Expression. This is for maximum flexibility,
                    // so that e.g. literals can be evaluated individually should we so wish. Performance hit to build, but meh..
                    // So, we need to create a lambda here - easy enough - we just re-use the parameters from the
                    // overall clause.
                    literals.Add(new PLLiteral<TModel>(Expression.Lambda<Predicate<TModel>>(node, owner.Lambda.Parameters)));

                    // We don't need to look any further down the tree for the purposes of this class (though the PLLiteral ctor, above,
                    // does so to figure out the details of the literal). So we can just return node rather than invoking base.Visit. 
                    return node;
                }
            }
        }

        private class LiteralComparer : IComparer<PLLiteral<TModel>>
        {
            public int Compare(PLLiteral<TModel> x, PLLiteral<TModel> y)
            {
                var hashComparison = x.AtomicSentence.GetHashCode().CompareTo(y.AtomicSentence.GetHashCode());
                if (hashComparison != 0)
                {
                    return hashComparison;
                }

                return x.IsPositive.CompareTo(y.IsPositive);
            }
        }
    }
}
#endif