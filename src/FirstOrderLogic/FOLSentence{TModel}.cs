using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a sentence of first order logic.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <remarks>
    /// See Fig 8.3...
    /// </remarks>
    public abstract class FOLSentence<TModel>
    {
        ////protected FOLSentence(Expression<Predicate<IEnumerable<TModel>>> lambda) => Lambda = lambda;

        // Ultimately might be useful for verification - but can add this later..
        ////public Expression<Predicate<IEnumerable<TModel>>> Lambda { get; }

        /// <summary>
        /// Tries to create the <see cref="FOLSentence{TModel}"/> instance that is logically equivalent to
        /// the proposition that a given lambda expression is guaranteed to evaluate as true for all possible domains.
        /// </summary>
        /// <param name="lambda">The lambda expression.</param>
        /// <param name="sentence">The created sentence, or <see langword="null"/> on failure.</param>
        /// <returns>A value indicating whether or not creation was successful.</returns>
        public static bool TryCreate(Expression<Predicate<IEnumerable<TModel>>> lambda, out FOLSentence<TModel> sentence)
        {
            // TODO-USABILITY: might be nice to return more info than just "false" - but can't rely on exceptions due to the way this works.
            return FOLComplexSentence<TModel>.TryCreate(lambda, out sentence)
                || FOLAtomicSentence<TModel>.TryCreate(lambda, out sentence);
        }
    }

    /// <summary>
    /// Representation of a complex sentence of first order logic - that is, one that is composed of one or more sub-sentences.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <remarks>
    /// NB: This is a largely pointless mid-level abstract class because it adds no members - it exists only because the BNF representation
    /// I'm working from makes the distinction between complex and atomic sentences - which to be fair is useful distinction to make when learning
    /// FOL.
    /// </remarks>
    public abstract class FOLComplexSentence<TModel> : FOLSentence<TModel>
    {
        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TModel>>> lambda, out FOLSentence<TModel> sentence)
        {
            return FOLNegation<TModel>.TryCreate(lambda, out sentence)
                || FOLConjunction<TModel>.TryCreate(lambda, out sentence)
                || FOLDisjunction<TModel>.TryCreate(lambda, out sentence)
                || FOLQuantification<TModel>.TryCreate(lambda, out sentence);
        }
    }

    /// <summary>
    /// Representation of an atomic sentence of first order logic - that is, one that is not composed of sub-sentences.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <remarks>
    /// NB: This is a largely pointless mid-level abstract class because it adds no members - it exists only because the BNF representation
    /// I'm working from makes the distinction between complex and atomic sentences - which to be fair is useful distinction to make when learning
    /// FOL.
    /// </remarks>
    public abstract class FOLAtomicSentence<TModel> : FOLSentence<TModel>
    {
        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TModel>>> lambda, out FOLSentence<TModel> sentence)
        {
            return FOLPredicate<TModel>.TryCreate(lambda, out sentence)
                || FOLEquality<TModel>.TryCreate(lambda, out sentence);
        }
    }

    /// <summary>
    /// Representation of an negation sentence of first order logic.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class FOLNegation<TModel> : FOLComplexSentence<TModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FOLNegation{TModel}"/> class.
        /// </summary>
        /// <param name="sentence">The sentence that is negated.</param>
        public FOLNegation(FOLSentence<TModel> sentence) => Sentence = sentence;

        /// <summary>
        /// Gets the sentence that is negated.
        /// </summary>
        public FOLSentence<TModel> Sentence { get; }

        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TModel>>> lambda, out FOLSentence<TModel> sentence)
        {
            if (lambda.Body is UnaryExpression unaryExpr && unaryExpr.NodeType == ExpressionType.Not
                && FOLSentence<TModel>.TryCreate(lambda.MakeSubPredicateExpr(unaryExpr.Operand), out var operand))
            {
                sentence = new FOLNegation<TModel>(operand);
                return true;
            }

            sentence = null;
            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is FOLNegation<TModel> negation && Sentence.Equals(negation.Sentence);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Sentence);
    }

    /// <summary>
    /// Representation of a conjunction sentence of first order logic.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class FOLConjunction<TModel> : FOLComplexSentence<TModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FOLConjunction{TModel}"/> class.
        /// </summary>
        /// <param name="left">The left side of the conjunction.</param>
        /// <param name="right">The right side of the conjunction.</param>
        public FOLConjunction(FOLSentence<TModel> left, FOLSentence<TModel> right) => (Left, Right) = (left, right);

        /// <summary>
        /// Gets the left side of the conjunction.
        /// </summary>
        public FOLSentence<TModel> Left { get; }

        /// <summary>
        /// Gets the right side of the conjunction.
        /// </summary>
        public FOLSentence<TModel> Right { get; }

        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TModel>>> lambda, out FOLSentence<TModel> sentence)
        {
            if (lambda.Body is BinaryExpression binaryExpr && (binaryExpr.NodeType == ExpressionType.AndAlso || binaryExpr.NodeType == ExpressionType.And)
                && FOLSentence<TModel>.TryCreate(lambda.MakeSubPredicateExpr(binaryExpr.Left), out var left)
                && FOLSentence<TModel>.TryCreate(lambda.MakeSubPredicateExpr(binaryExpr.Right), out var right))
            {
                sentence = new FOLConjunction<TModel>(left, right);
                return true;
            }

            sentence = null;
            return false;
        }

        // TODO: Equality & HashCode
    }

    /// <summary>
    /// Representation of a disjunction sentence of first order logic.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class FOLDisjunction<TModel> : FOLComplexSentence<TModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FOLDisjunction{TModel}"/> class.
        /// </summary>
        /// <param name="left">The left side of the disjunction.</param>
        /// <param name="right">The right side of the disjunction.</param>
        public FOLDisjunction(FOLSentence<TModel> left, FOLSentence<TModel> right) => (Left, Right) = (left, right);

        /// <summary>
        /// Gets the left side of the disjunction.
        /// </summary>
        public FOLSentence<TModel> Left { get; }

        /// <summary>
        /// Gets the right side of the disjunction.
        /// </summary>
        public FOLSentence<TModel> Right { get; }

        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TModel>>> lambda, out FOLSentence<TModel> sentence)
        {
            if (lambda.Body is BinaryExpression binaryExpr && (binaryExpr.NodeType == ExpressionType.OrElse || binaryExpr.NodeType == ExpressionType.Or)
                && FOLSentence<TModel>.TryCreate(lambda.MakeSubPredicateExpr(binaryExpr.Left), out var left)
                && FOLSentence<TModel>.TryCreate(lambda.MakeSubPredicateExpr(binaryExpr.Right), out var right))
            {
                sentence = new FOLDisjunction<TModel>(left, right);
                return true;
            }

            sentence = null;
            return false;
        }

        // TODO: Equality & HashCode
    }

    // Not explicitly needed (can just use ¬P OR Q) - and no C# that it makes sense to map to it.
    // TODO-FEATURE: For convenience, create an Implies(bool antecedent, bool consequent) method the implementation of which is correct. Implement this class, looking specifically for that method.
    ////public class FOLImplication<TModel> : FOLComplexSentence<TModel>
    ////{
    ////}

    // Not explicitly needed (can just use (¬P OR Q) AND (¬Q OR P)) - and no C# that it makes sense to map to it.
    // TODO-FEATURE: For convenience, create an Iff(bool p, bool q) method the implementation of which is correct. Implement this class, looking specifically for that method.
    ////public class FOLBiconditional<TModel> : FOLComplexSentence<TModel>
    ////{
    ////}

    public abstract class FOLQuantification<TModel> : FOLComplexSentence<TModel>
    {
        protected FOLQuantification(FOLVariableTerm<TModel> variable, FOLSentence<TModel> sentence) => (Variable, Sentence) = (variable, sentence);

        public FOLSentence<TModel> Sentence { get; }

        public FOLVariableTerm<TModel> Variable { get; }

        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TModel>>> lambda, out FOLSentence<TModel> sentence)
        {
            return FOLUniversalQuantification<TModel>.TryCreate(lambda, out sentence)
                || FOLExistentialQuantification<TModel>.TryCreate(lambda, out sentence);
        }

        // TODO: Equality & HashCode
    }

    public class FOLUniversalQuantification<TModel> : FOLQuantification<TModel>
    {
        private static readonly (Module Module, int MetadataToken) AllMethod;

        static FOLUniversalQuantification()
        {
            var allMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.All));
            AllMethod = (allMethod.Module, allMethod.MetadataToken);
        }

        public FOLUniversalQuantification(FOLVariableTerm<TModel> variable, FOLSentence<TModel> sentence)
            : base(variable, sentence)
        {
        }

        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TModel>>> lambda, out FOLSentence<TModel> sentence)
        {
            if (lambda.Body is MethodCallExpression methodCallExpr && (methodCallExpr.Method.Module, methodCallExpr.Method.MetadataToken) == AllMethod
                && FOLSentence<TModel>.TryCreate(lambda.MakeSubPredicateExpr2(methodCallExpr.Arguments[1]), out var subSentence)
                // TODO: Ugh, so ugly, and might not be a lambda if theyve used e.g. a method - but we should handle this more gracefully than an InvalidCast
                && FOLVariableTerm<TModel>.TryCreate(lambda.MakeSubLambda(((LambdaExpression)methodCallExpr.Arguments[1]).Parameters[0]), out FOLVariableTerm<TModel> variableTerm))
            {
                sentence = new FOLUniversalQuantification<TModel>(variableTerm, subSentence);
                return true;
            }

            sentence = null;
            return false;
        }

        // TODO: Equality & HashCode
    }

    public class FOLExistentialQuantification<TModel> : FOLQuantification<TModel>
    {
        private static readonly (Module Module, int MetadataToken) AnyMethod;

        static FOLExistentialQuantification()
        {
            var anyMethod = typeof(Enumerable).GetMethod(
                nameof(Enumerable.Any),
                new[]
                {
                    typeof(IEnumerable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
                    typeof(Func<,>).MakeGenericType(Type.MakeGenericMethodParameter(0), typeof(bool))
                });
            AnyMethod = (anyMethod.Module, anyMethod.MetadataToken);
        }

        public FOLExistentialQuantification(FOLVariableTerm<TModel> variable, FOLSentence<TModel> sentence)
            : base(variable, sentence)
        {
        }

        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TModel>>> lambda, out FOLSentence<TModel> sentence)
        {
            if (lambda.Body is MethodCallExpression methodCallExpr && (methodCallExpr.Method.Module, methodCallExpr.Method.MetadataToken) == AnyMethod
                && FOLSentence<TModel>.TryCreate(lambda.MakeSubPredicateExpr2(methodCallExpr.Arguments[1]), out var subSentence)
                // TODO: Ugh, so ugly, and might not be a lambda if theyve used e.g. a method - but we should handle this more gracefully than an InvalidCast
                && FOLVariableTerm<TModel>.TryCreate(lambda.MakeSubLambda(((LambdaExpression)methodCallExpr.Arguments[1]).Parameters[0]), out FOLVariableTerm<TModel> variableTerm)) 
            {
                sentence = new FOLExistentialQuantification<TModel>(variableTerm, subSentence);
                return true;
            }

            sentence = null;
            return false;
        }

        // TODO: Equality & HashCode
    }

    public class FOLEquality<TModel> : FOLAtomicSentence<TModel>
    {
        public FOLEquality(FOLTerm<TModel> left, FOLTerm<TModel> right) => (Left, Right) = (left, right);

        public FOLTerm<TModel> Left { get; }

        public FOLTerm<TModel> Right { get; }

        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TModel>>> lambda, out FOLSentence<TModel> sentence)
        {
            if (lambda.Body is BinaryExpression binaryExpr && binaryExpr.NodeType == ExpressionType.Equal // TODO-ROBUSTNESS: ..and Object.Equals invocation? And others? How to think about map of different types of .NET equality to FOL "equals"?
                && FOLTerm<TModel>.TryCreate(lambda.MakeSubLambda(binaryExpr.Left), out var left)
                && FOLTerm<TModel>.TryCreate(lambda.MakeSubLambda(binaryExpr.Right), out var right))
            {
                sentence = new FOLEquality<TModel>(left, right);
                return true;
            }

            sentence = null;
            return false;
        }

        // TODO: Equality & HashCode
    }

    // TODO: could be ground or unary or binary or.. n-ary.
    public class FOLPredicate<TModel> : FOLAtomicSentence<TModel>
    {
        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TModel>>> lambda, out FOLSentence<TModel> sentence)
        {
            // TODO!
            sentence = null;
            return false;
        }

        // TODO: Equality & HashCode
    }
}
