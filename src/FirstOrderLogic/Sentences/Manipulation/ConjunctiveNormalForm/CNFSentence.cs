// Copied wholesale from LinqToKB.PredicateLogic..
#if false
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderlLogic.Sentences.Manipulation.ConjunctiveNormalForm
{
    /// <summary>
    /// Representation of a predicate lambda expression in conjunctive normal form (CNF).
    /// </summary>
    /// <typeparam name="TModel">The type that the literals of this expression refer to.</typeparam>
    public class CNFExpression<TModel>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="CNFExpression{TModel}"/> class, implicitly converting the provided lambda to CNF in the process.
        /// </summary>
        /// <param name="lambda">The predicate to represent.</param>
        public CNFExpression(Expression<Predicate<TModel>> lambda)
        {
            Lambda = new CNFConverter().VisitAndConvert(lambda, nameof(CNFConverter));
            var clauses = new List<CNFClause<TModel>>();
            new ExpressionConstructor(this, clauses).Visit(Lambda.Body);
            Clauses = clauses.AsReadOnly();
        }

        /// <summary>
        /// Gets the expression (in conjunctive normal form) as a lambda.
        /// </summary>
        public Expression<Predicate<TModel>> Lambda { get; }

        /// <summary>
        /// Gets the collection of clauses that comprise this expression.
        /// </summary>
        public IReadOnlyCollection<CNFClause<TModel>> Clauses { get; }

        /// <summary>
        /// Linq expression visitor that converts the visited expression to conjunctive normal form.
        /// </summary>
        private class CNFConverter : ExpressionVisitor
        {
            private readonly NegationNormalFormConverter negationNormalFormConverter = new NegationNormalFormConverter();
            private readonly OrDistributor orDistributor = new OrDistributor();

            /// <inheritdoc />
            public override Expression Visit(Expression node)
            {
                // NB: the way this works implicitly treats everything that is not an AndAlso, OrElse or a Not as an atomic sentence - 
                // which should make the library somewhat flexible in what kinds of models it acts against.
                // TODO-ROBUSTNESS: Should probably add And (&) and Or (|) as well, for good measure?
                // TODO-MAINTAINABILITY: this feels like a more fundamental bit of logic than specific to CNF? Considering a redesign where PLExpression<> is instantiable..?

                // Need to completely convert to NNF before distributing ORs,
                // else stuff will be missed. Hence the separate subconverters.
                node = negationNormalFormConverter.Visit(node);
                node = orDistributor.Visit(node);
                return node;
            }
        }

        /// <summary>
        /// Expression visitor that converts to negation normal form by repeated elimination of double negatives and application of de Morgans laws.
        /// </summary>
        private class NegationNormalFormConverter : ExpressionVisitor
        {
            /// <inheritdoc />
            public override Expression Visit(Expression node)
            {
                if (node is UnaryExpression u && u.NodeType == ExpressionType.Not)
                {
                    if (u.Operand is UnaryExpression not && not.NodeType == ExpressionType.Not)
                    {
                        // Eliminate double negative: ¬(¬P) ≡ P
                        node = not.Operand;
                    }
                    else if (u.Operand is BinaryExpression andAlso && andAlso.NodeType == ExpressionType.AndAlso)
                    {
                        // Apply de Morgan: ¬(P ∧ Q) ≡ (¬P ∨ ¬Q)
                        node = Expression.OrElse(Expression.Not(andAlso.Left), Expression.Not(andAlso.Right));
                    }
                    else if (u.Operand is BinaryExpression orElse && orElse.NodeType == ExpressionType.OrElse)
                    {
                        // Apply de Morgan: ¬(P ∨ Q) ≡ (¬P ∧ ¬Q)
                        node = Expression.AndAlso(Expression.Not(orElse.Left), Expression.Not(orElse.Right));
                    }
                }

                return base.Visit(node);
            }
        }

        /// <summary>
        /// Expression visitor that recursively distributes disjunctions over conjunctions.
        /// </summary>
        private class OrDistributor : ExpressionVisitor
        {
            /// <inheritdoc />
            public override Expression Visit(Expression node)
            {
                if (node is BinaryExpression b && b.NodeType == ExpressionType.OrElse)
                {
                    if (b.Right is BinaryExpression andAlsoRight && andAlsoRight.NodeType == ExpressionType.AndAlso)
                    {
                        // Apply distribution of ∨ over ∧: (α ∨ (β ∧ γ)) ≡ ((α ∨ β) ∧ (α ∨ γ))
                        // NB the "else if" below is fine (i.e. we don't need a seperate case for if they are both &&s)
                        // since if b.Left is also an &&, well end up distributing over it once we recurse down as far
                        // as the Expression.OrElses we create here.
                        node = Expression.AndAlso(
                            Expression.OrElse(b.Left, andAlsoRight.Left),
                            Expression.OrElse(b.Left, andAlsoRight.Right));
                    }
                    else if (b.Left is BinaryExpression andAlsoLeft && andAlsoLeft.NodeType == ExpressionType.AndAlso)
                    {
                        // Apply distribution of ∨ over ∧: ((β ∧ γ) ∨ α) ≡ ((β ∨ α) ∧ (γ ∨ α))
                        node = Expression.AndAlso(
                            Expression.OrElse(andAlsoLeft.Left, b.Right),
                            Expression.OrElse(andAlsoLeft.Right, b.Right));
                    }
                }

                return base.Visit(node);
            }
        }

        /// <summary>
        /// Expression visitor that constructs a set of <see cref="CNFClause{TModel}"/> objects from a lambda in CNF.
        /// </summary>
        private class ExpressionConstructor : ExpressionVisitor
        {
            private readonly CNFExpression<TModel> owner;
            private readonly List<CNFClause<TModel>> clauses;

            public ExpressionConstructor(CNFExpression<TModel> owner, List<CNFClause<TModel>> clauses) => (this.owner, this.clauses) = (owner, clauses);

            /// <inheritdoc />
            public override Expression Visit(Expression node)
            {
                if (node is BinaryExpression andAlso && andAlso.NodeType == ExpressionType.AndAlso)
                {
                    // The expression is already in CNF - so the root down until the individual clauses will all be AndAlso - we just skip past those.
                    return base.Visit(node);
                }
                else
                {
                    // We've hit a clause.
                    // NB: CNFClause accepts a lambda - not just an Expression. This is for maximum flexibility,
                    // so that e.g. clauses can be evaluated individually should we so wish. Performance hit to build, but meh..
                    // So, we need to create a lambda here - easy enough - we just point at the parameters from the
                    // overall expression (after all, this is just a sub-expression).
                    clauses.Add(new CNFClause<TModel>(Expression.Lambda<Predicate<TModel>>(node, owner.Lambda.Parameters)));

                    // We don't need to look any further down the tree for the purposes of this class (though the CNFClause ctor, above,
                    // does so to figure out the details of the clause). So we can just return node rather than invoking base.Visit. 
                    return node;
                }
            }
        }
    }
}
#endif