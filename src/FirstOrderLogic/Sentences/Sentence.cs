using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Static factory methods for creating <see cref="Sentence{TDomain,TElement}"/> instances from LINQ expressions.
    /// </summary>
    public static class Sentence
    {
        private static readonly MethodInfo IfMethod;
        private static readonly MethodInfo IffMethod;
        private static readonly MethodInfo AnyMethod;
        private static readonly MethodInfo ShorthandAnyMethod2;
        private static readonly MethodInfo ShorthandAnyMethod3;
        private static readonly MethodInfo AllMethod;
        private static readonly MethodInfo ShorthandAllMethod2;
        private static readonly MethodInfo ShorthandAllMethod3;

        static Sentence()
        {
            IfMethod = typeof(Operators).GetMethod(nameof(Operators.If));

            IffMethod = typeof(Operators).GetMethod(nameof(Operators.Iff));

            AnyMethod = typeof(Enumerable).GetMethod(
                nameof(Enumerable.Any),
                new[]
                {
                    typeof(IEnumerable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
                    typeof(Func<,>).MakeGenericType(Type.MakeGenericMethodParameter(0), typeof(bool))
                });
            ShorthandAnyMethod2 = typeof(IEnumerableExtensions).GetMethod(
                nameof(IEnumerableExtensions.All),
                new[]
                {
                    typeof(IEnumerable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
                    typeof(Func<,,>).MakeGenericType(Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(0), typeof(bool))
                });
            ShorthandAnyMethod3 = typeof(IEnumerableExtensions).GetMethod(
                nameof(IEnumerableExtensions.All),
                new[]
                {
                    typeof(IEnumerable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
                    typeof(Func<,,,>).MakeGenericType(Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(0), typeof(bool))
                });

            AllMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.All));
            ShorthandAllMethod2 = typeof(IEnumerableExtensions).GetMethod(
                nameof(IEnumerableExtensions.All),
                new[]
                {
                    typeof(IEnumerable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
                    typeof(Func<,,>).MakeGenericType(Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(0), typeof(bool))
                });
            ShorthandAllMethod3 = typeof(IEnumerableExtensions).GetMethod(
                nameof(IEnumerableExtensions.All),
                new[]
                {
                    typeof(IEnumerable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
                    typeof(Func<,,,>).MakeGenericType(Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(0), typeof(bool))
                });
        }

        /// <summary>
        /// Creates and returns the <see cref="Sentence{TDomain, TElement}"/> instance that is logically equivalent to
        /// the proposition that a given lambda expression is guaranteed to evaluate as true for all possible domains.
        /// </summary>
        ///  <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
        /// <param name="lambda">The lambda expression.</param>
        /// <returns>The created sentence.</returns>
        public static Sentence<TDomain, TElement> Create<TDomain, TElement>(Expression<Predicate<TDomain>> lambda)
            where TDomain : IEnumerable<TElement>
        {
            if (!TryCreate<TDomain, TElement>(lambda, out var sentence))
            {
                throw new ArgumentException("Expression is not convertible to a sentence of first order logic", nameof(sentence));
            }

            return sentence;
        }

        /// <summary>
        /// Creates and returns the <see cref="Sentence{TDomain, TElement}"/> instance that is logically equivalent to
        /// the proposition that a given lambda expression is guaranteed to evaluate as true for all possible domains.
        /// </summary>
        ///  <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
        /// <param name="lambda">The lambda expression.</param>
        /// <returns>The created sentence.</returns>
        public static Sentence<IEnumerable<TElement>, TElement> Create<TElement>(Expression<Predicate<IEnumerable<TElement>>> lambda)
        {
            if (!TryCreate<TElement>(lambda, out var sentence))
            {
                throw new ArgumentException("Expression is not convertible to a sentence of first order logic", nameof(sentence));
            }

            return sentence;
        }

        /// <summary>
        /// Tries to create the <see cref="Sentence{TDomain, TElement}"/> instance that is logically equivalent to
        /// the proposition that a given lambda expression is guaranteed to evaluate as true for all possible domains.
        /// </summary>
        ///  <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
        /// <param name="lambda">The lambda expression.</param>
        /// <param name="sentence">The created sentence, or <see langword="null"/> on failure.</param>
        /// <returns>A value indicating whether or not creation was successful.</returns>
        public static bool TryCreate<TDomain, TElement>(Expression<Predicate<TDomain>> lambda, out Sentence<TDomain, TElement> sentence)
            where TDomain : IEnumerable<TElement>
        {
            return TryCreateSentence(lambda, out sentence);
        }

        /// <summary>
        /// Tries to create the <see cref="FOLSentence{TDomain, TElement}"/> instance that is logically equivalent to
        /// the proposition that a given lambda expression is guaranteed to evaluate as true for all possible domains.
        /// </summary>
        /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
        /// <param name="lambda">The lambda expression.</param>
        /// <param name="sentence">The created sentence, or <see langword="null"/> on failure.</param>
        /// <returns>A value indicating whether or not creation was successful.</returns>
        /// <remarks>
        /// This method serves as a shorthand for <see cref="TryCreate{TDomain, TElement}"/> where the domain is
        /// just <see cref="IEnumerable{TElement}"/> - which suffices when the domain contains no constants or ground
        /// predicates.
        /// </remarks>
        public static bool TryCreate<TElement>(Expression<Predicate<IEnumerable<TElement>>> lambda, out Sentence<IEnumerable<TElement>, TElement> sentence)
        {
            return TryCreateSentence(lambda, out sentence);
        }

        // Internally (i.e. for sub-sentences), lambda is not necessarily a single-arg predicate expression since we may have additional parameters
        // that are the variables defined by quantifiers. Hence this is separate to Sentence's public method.
        internal static bool TryCreateSentence<TDomain, TElement>(LambdaExpression lambda, out Sentence<TDomain, TElement> sentence)
            where TDomain : IEnumerable<TElement>
        {
            // NB: A different formulation might have created abstract ComplexSentence and AtomicSentence subclasses of sentence
            // with their own internal TryCreate methods. Until there is a reason to make the distinction though, we don't bother
            // adding that complexity here..

            // TODO-USABILITY: might be nice to return more info than just "false" - but can't rely on exceptions due to the way this works.
            return
                // Complex sentences:
                TryCreateNegation(lambda, out sentence)
                || TryCreateConjunction(lambda, out sentence)
                || TryCreateDisjunction(lambda, out sentence)
                || TryCreateEquivalence(lambda, out sentence)
                || TryCreateImplication(lambda, out sentence)
                || TryCreateQuantification(lambda, out sentence)
                // Atomic sentences:
                || TryCreateEquality(lambda, out sentence)
                || TryCreatePredicate(lambda, out sentence);
        }

        internal static bool TryCreateConjunction<TDomain, TElement>(LambdaExpression lambda, out Sentence<TDomain, TElement> sentence)
            where TDomain : IEnumerable<TElement>
        {
            if (lambda.Body is BinaryExpression binaryExpr && (binaryExpr.NodeType == ExpressionType.AndAlso || binaryExpr.NodeType == ExpressionType.And)
                && TryCreateSentence<TDomain, TElement>(MakeSubLambda(lambda, binaryExpr.Left), out var left)
                && TryCreateSentence<TDomain, TElement>(MakeSubLambda(lambda, binaryExpr.Right), out var right))
            {
                sentence = new Conjunction<TDomain, TElement>(left, right);
                return true;
            }

            sentence = null;
            return false;
        }

        internal static bool TryCreateConstant<TDomain, TElement>(LambdaExpression lambda, out Term<TDomain, TElement> term)
            where TDomain : IEnumerable<TElement>
        {
            if (typeof(TElement).IsAssignableFrom(lambda.Body.Type)) // Constants must be elements of the domain
            {
                if (lambda.Body is MemberExpression memberExpr
                    && typeof(TDomain).IsAssignableFrom(memberExpr.Expression.Type)) // TODO-ROBUSTNESS: Do we actually need to check if its accessing the domain-valued param (think of weird situations where its a domain-valued prop of an element or somat)..
                {
                    // TElement-valued property access of the domain is interpreted as a constant.
                    term = new Constant<TDomain, TElement>(memberExpr.Member);
                    return true;
                }
                else if (lambda.Body is MethodCallExpression methodCallExpr
                    && typeof(TDomain).IsAssignableFrom(methodCallExpr.Object.Type) // TODO-ROBUSTNESS: Do we actually need to check if its accessing the domain-valued param (think of weird situations where its a domain-valued prop of an element or somat)..
                    && methodCallExpr.Arguments.Count == 0)
                {
                    // TElement-valued parameterless method call of the domain is interpreted as a constant.
                    term = new Constant<TDomain, TElement>(methodCallExpr.Method);
                    return true;
                }
            }

            // TODO-USABILITY: Perhaps we should allow constants here too? For cases where the domain is of basic types, it seems
            // silly to require d => d.Hello rather than d => "Hello". For user provided types is of less value because
            // it requires an instantiation of the constant (where otherwise just working with interfaces is fine), but
            // as far as I can see there's no reason (aside from equality et al - probably solvable) to actually forbid it..?

            term = null;
            return false;
        }

        internal static bool TryCreateDisjunction<TDomain, TElement>(LambdaExpression lambda, out Sentence<TDomain, TElement> sentence)
            where TDomain : IEnumerable<TElement>
        {
            if (lambda.Body is BinaryExpression binaryExpr && (binaryExpr.NodeType == ExpressionType.OrElse || binaryExpr.NodeType == ExpressionType.Or)
                && TryCreateSentence<TDomain, TElement>(MakeSubLambda(lambda, binaryExpr.Left), out var left)
                && TryCreateSentence<TDomain, TElement>(MakeSubLambda(lambda, binaryExpr.Right), out var right))
            {
                sentence = new Disjunction<TDomain, TElement>(left, right);
                return true;
            }

            sentence = null;
            return false;
        }

        internal static bool TryCreateEquality<TDomain, TElement>(LambdaExpression lambda, out Sentence<TDomain, TElement> sentence)
            where TDomain : IEnumerable<TElement>
        {
            // TODO-ROBUSTNESS: ..and Object.Equals invocation? And others? How to think about map of different types of .NET equality to FOL "equals"?
            if (lambda.Body is BinaryExpression binaryExpr && binaryExpr.NodeType == ExpressionType.Equal
                && TryCreateTerm<TDomain, TElement>(MakeSubLambda(lambda, binaryExpr.Left), out var left)
                && TryCreateTerm<TDomain, TElement>(MakeSubLambda(lambda, binaryExpr.Right), out var right))
            {
                sentence = new Equality<TDomain, TElement>(left, right);
                return true;
            }

            sentence = null;
            return false;
        }

        internal static bool TryCreateEquivalence<TDomain, TElement>(LambdaExpression lambda, out Sentence<TDomain, TElement> sentence)
            where TDomain : IEnumerable<TElement>
        {
            // TODO-FEATURE: Would it be reasonable to also accept {sentence} == {sentence} here?

            if (lambda.Body is MethodCallExpression methodCallExpr && MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, IffMethod)
                && TryCreateSentence<TDomain, TElement>(MakeSubLambda(lambda, methodCallExpr.Arguments[0]), out var equivalent1)
                && TryCreateSentence<TDomain, TElement>(MakeSubLambda(lambda, methodCallExpr.Arguments[1]), out var equivalent2))
            {
                sentence = new Equivalence<TDomain, TElement>(equivalent1, equivalent2);
                return true;
            }

            sentence = null;
            return false;
        }

        internal static bool TryCreateExistentialQuantification<TDomain, TElement>(LambdaExpression lambda, out Sentence<TDomain, TElement> sentence)
            where TDomain : IEnumerable<TElement>
        {
            // TODO-MAINTAINABILITY: Ick. This is horrible. Can we recurse to make it more graceful without losing any more perf than we need to?

            if (lambda.Body is MethodCallExpression methodCallExpr)
            {
                if (MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, AnyMethod)
                    && methodCallExpr.Arguments[1] is LambdaExpression anyLambda // TODO-ROBUSTNESS: need better errors if they've e.g. attempted to use something other than a lambda..
                    && TryCreateSentence<TDomain, TElement>(MakeSubLambda(lambda, anyLambda), out var subSentence)
                    && TryCreateVariable(MakeSubLambda(lambda, anyLambda.Parameters[0]), out Variable<TDomain, TElement> variableTerm))
                {
                    sentence = new ExistentialQuantification<TDomain, TElement>(variableTerm, subSentence);
                    return true;
                }
                else if (MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, ShorthandAnyMethod2)
                    && methodCallExpr.Arguments[1] is LambdaExpression any2Lambda // TODO-ROBUSTNESS: need better errors if they've e.g. attempted to use something other than a lambda..
                    && TryCreateSentence<TDomain, TElement>(MakeSubLambda(lambda, any2Lambda), out var all2SubSentence)
                    && TryCreateVariable(MakeSubLambda(lambda, any2Lambda.Parameters[0]), out Variable<TDomain, TElement> variableTerm1Of2)
                    && TryCreateVariable(MakeSubLambda(lambda, any2Lambda.Parameters[1]), out Variable<TDomain, TElement> variableTerm2Of2))
                {
                    sentence = new ExistentialQuantification<TDomain, TElement>(
                        variableTerm1Of2,
                        new ExistentialQuantification<TDomain, TElement>(variableTerm2Of2, all2SubSentence));
                    return true;
                }
                else if (MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, ShorthandAnyMethod3)
                    && methodCallExpr.Arguments[1] is LambdaExpression any3Lambda // TODO-ROBUSTNESS: need better errors if they've e.g. attempted to use something other than a lambda..
                    && TryCreateSentence<TDomain, TElement>(MakeSubLambda(lambda, any3Lambda), out var all3SubSentence)
                    && TryCreateVariable(MakeSubLambda(lambda, any3Lambda.Parameters[0]), out Variable<TDomain, TElement> variableTerm1Of3)
                    && TryCreateVariable(MakeSubLambda(lambda, any3Lambda.Parameters[1]), out Variable<TDomain, TElement> variableTerm2Of3)
                    && TryCreateVariable(MakeSubLambda(lambda, any3Lambda.Parameters[2]), out Variable<TDomain, TElement> variableTerm3Of3))
                {
                    sentence = new UniversalQuantification<TDomain, TElement>(
                        variableTerm1Of3,
                        new ExistentialQuantification<TDomain, TElement>(
                            variableTerm2Of3,
                            new ExistentialQuantification<TDomain, TElement>(variableTerm3Of3, all3SubSentence)));
                    return true;
                }
            }

            sentence = null;
            return false;
        } 

        internal static bool TryCreateFunction<TDomain, TElement>(LambdaExpression lambda, out Term<TDomain, TElement> term)
            where TDomain : IEnumerable<TElement>
        {
            // NB: Here we verify that the value of the function is a domain element..
            if (typeof(TElement).IsAssignableFrom(lambda.Body.Type))
            {
                if (lambda.Body is MemberExpression memberExpr
                    && TryCreateTerm<TDomain, TElement>(MakeSubLambda(lambda, memberExpr.Expression), out var argument))
                {
                    // TElement-valued property access is interpreted as a unary function.
                    term = new Function<TDomain, TElement>(memberExpr.Member, new[] { argument });
                    return true;
                }
                else if (lambda.Body is MethodCallExpression methodCallExpr
                    && (methodCallExpr.Object != null || methodCallExpr.Arguments.Count > 0)) // NB: There must be at least one arg - otherwise its a constant, not a function..
                {
                    var arguments = new List<Term<TDomain, TElement>>();

                    // If the method is non-static, the instance it is operating on is the first arg of the predicate:
                    if (methodCallExpr.Object != null)
                    {
                        if (!TryCreateTerm<TDomain, TElement>(MakeSubLambda(lambda, methodCallExpr.Object), out var arg))
                        {
                            term = null;
                            return false;
                        }

                        arguments.Add(arg);
                    }

                    foreach (var argExpr in methodCallExpr.Arguments)
                    {
                        if (!TryCreateTerm<TDomain, TElement>(MakeSubLambda(lambda, argExpr), out var arg))
                        {
                            term = null;
                            return false;
                        }

                        arguments.Add(arg);
                    }

                    term = new Function<TDomain, TElement>(methodCallExpr.Method, arguments);
                    return true;
                }
            }

            term = null;
            return false;
        }

        internal static bool TryCreateImplication<TDomain, TElement>(LambdaExpression lambda, out Sentence<TDomain, TElement> sentence)
            where TDomain : IEnumerable<TElement>
        {
            if (lambda.Body is MethodCallExpression methodCallExpr && MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, IfMethod)
                && TryCreateSentence<TDomain, TElement>(MakeSubLambda(lambda, methodCallExpr.Arguments[0]), out var antecedent)
                && TryCreateSentence<TDomain, TElement>(MakeSubLambda(lambda, methodCallExpr.Arguments[1]), out var consequent))
            {
                sentence = new Implication<TDomain, TElement>(antecedent, consequent);
                return true;
            }

            sentence = null;
            return false;
        }

        internal static bool TryCreateNegation<TDomain, TElement>(LambdaExpression lambda, out Sentence<TDomain, TElement> sentence)
            where TDomain : IEnumerable<TElement>
        {
            if (lambda.Body is UnaryExpression unaryExpr && unaryExpr.NodeType == ExpressionType.Not
                && TryCreateSentence<TDomain, TElement>(MakeSubLambda(lambda, unaryExpr.Operand), out var operand))
            {
                sentence = new Negation<TDomain, TElement>(operand);
                return true;
            }
            else if (lambda.Body is BinaryExpression binaryExpr && binaryExpr.NodeType == ExpressionType.NotEqual
                && TryCreateTerm<TDomain, TElement>(MakeSubLambda(lambda, binaryExpr.Left), out var left)
                && TryCreateTerm<TDomain, TElement>(MakeSubLambda(lambda, binaryExpr.Right), out var right))
            {
                sentence = new Negation<TDomain, TElement>(new Equality<TDomain, TElement>(left, right));
                return true;
            }

            sentence = null;
            return false;
        }

        internal static bool TryCreatePredicate<TDomain, TElement>(LambdaExpression lambda, out Sentence<TDomain, TElement> sentence)
            where TDomain : IEnumerable<TElement>
        {
            if (lambda.Body.Type != typeof(bool))
            {
                sentence = null;
                return false;
            }

            if (lambda.Body is MemberExpression memberExpr && memberExpr.Expression != null) // Non-static field or property access
            {
                if (memberExpr.Expression == lambda.Parameters[0]) // i.e. is the domain parameter. Should try to find a cleaner way of doing this..
                {
                    // Boolean-valued property access on the domain parameter is interpreted as a ground predicate
                    sentence = new Predicate<TDomain, TElement>(memberExpr.Member, Array.Empty<Term<TDomain, TElement>>());
                    return true;
                }
                else if (TryCreateTerm<TDomain, TElement>(MakeSubLambda(lambda, memberExpr.Expression), out var argument))
                {
                    // Boolean-valued property access on a term is interpreted as a unary predicate.
                    sentence = new Predicate<TDomain, TElement>(memberExpr.Member, new[] { argument });
                    return true;
                }
            }
            else if (lambda.Body is MethodCallExpression methodCallExpr && methodCallExpr.Object != null) // Non-static mmethod call
            {
                var arguments = new List<Term<TDomain, TElement>>();

                // If the method is not against on the domain, the instance it is operating on is the first arg of the predicate:
                if (methodCallExpr.Object != lambda.Parameters[0]) // i.e. is the domain parameter. Should try to find a cleaner way of doing this..
                {
                    if (!TryCreateTerm<TDomain, TElement>(MakeSubLambda(lambda, methodCallExpr.Object), out var arg))
                    {
                        sentence = null;
                        return false;
                    }

                    arguments.Add(arg);
                }

                // Each of the method's args should be interpretable as a term.
                foreach (var argExpr in methodCallExpr.Arguments)
                {
                    if (!TryCreateTerm<TDomain, TElement>(MakeSubLambda(lambda, argExpr), out var arg))
                    {
                        sentence = null;
                        return false;
                    }

                    arguments.Add(arg);
                }

                sentence = new Predicate<TDomain, TElement>(methodCallExpr.Method, arguments);
                return true;
            }
            //// ... also to consider - certain things will fail the above but could be very sensibly interpreted
            //// as predicates. E.g. a Contains() call on a property that is a collection of TElements. Or indeed Any on
            //// an IEnumerable of them.

            sentence = null;
            return false;
        }

        internal static bool TryCreateQuantification<TDomain, TElement>(LambdaExpression lambda, out Sentence<TDomain, TElement> sentence)
            where TDomain : IEnumerable<TElement>
        {
            return TryCreateUniversalQuantification(lambda, out sentence)
                || TryCreateExistentialQuantification(lambda, out sentence);
        }

        internal static bool TryCreateTerm<TDomain, TElement>(LambdaExpression lambda, out Term<TDomain, TElement> sentence)
            where TDomain : IEnumerable<TElement>
        {
            return TryCreateFunction(lambda, out sentence)
                || TryCreateConstant(lambda, out sentence)
                || TryCreateVariable(lambda, out sentence);
        }

        internal static bool TryCreateUniversalQuantification<TDomain, TElement>(LambdaExpression lambda, out Sentence<TDomain, TElement> sentence)
            where TDomain : IEnumerable<TElement>
        {
            // TODO-MAINTAINABILITY: Ick. This is horrible. Can we recurse or something to make it more graceful without losing any more perf than we need to?

            if (lambda.Body is MethodCallExpression methodCallExpr)
            {
                if (MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, AllMethod)
                    && methodCallExpr.Arguments[1] is LambdaExpression allLambda // TODO-ROBUSTNESS: need better errors if they've e.g. attempted to use something other than a lambda..
                    && TryCreateSentence<TDomain, TElement>(MakeSubLambda(lambda, allLambda), out var allSubSentence)
                    && TryCreateVariable(MakeSubLambda(lambda, allLambda.Parameters[0]), out Variable<TDomain, TElement> variableTerm))
                {
                    sentence = new UniversalQuantification<TDomain, TElement>(variableTerm, allSubSentence);
                    return true;
                }
                else if (MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, ShorthandAllMethod2)
                    && methodCallExpr.Arguments[1] is LambdaExpression all2Lambda // TODO-ROBUSTNESS: need better errors if they've e.g. attempted to use something other than a lambda..
                    && TryCreateSentence<TDomain, TElement>(MakeSubLambda(lambda, all2Lambda), out var all2SubSentence)
                    && TryCreateVariable(MakeSubLambda(lambda, all2Lambda.Parameters[0]), out Variable<TDomain, TElement> variableTerm1Of2)
                    && TryCreateVariable(MakeSubLambda(lambda, all2Lambda.Parameters[1]), out Variable<TDomain, TElement> variableTerm2Of2))
                {
                    sentence = new UniversalQuantification<TDomain, TElement>(
                        variableTerm1Of2,
                        new UniversalQuantification<TDomain, TElement>(variableTerm2Of2, all2SubSentence));
                    return true;
                }
                else if (MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, ShorthandAllMethod3)
                    && methodCallExpr.Arguments[1] is LambdaExpression all3Lambda // TODO-ROBUSTNESS: need better errors if they've e.g. attempted to use something other than a lambda..
                    && TryCreateSentence<TDomain, TElement>(MakeSubLambda(lambda, all3Lambda), out var all3SubSentence)
                    && TryCreateVariable(MakeSubLambda(lambda, all3Lambda.Parameters[0]), out Variable<TDomain, TElement> variableTerm1Of3)
                    && TryCreateVariable(MakeSubLambda(lambda, all3Lambda.Parameters[1]), out Variable<TDomain, TElement> variableTerm2Of3)
                    && TryCreateVariable(MakeSubLambda(lambda, all3Lambda.Parameters[2]), out Variable<TDomain, TElement> variableTerm3Of3))
                {
                    sentence = new UniversalQuantification<TDomain, TElement>(
                        variableTerm1Of3,
                        new UniversalQuantification<TDomain, TElement>(
                            variableTerm2Of3,
                            new UniversalQuantification<TDomain, TElement>(variableTerm3Of3, all3SubSentence)));
                    return true;
                }
            }

            sentence = null;
            return false;
        }

        internal static bool TryCreateVariable<TDomain, TElement>(LambdaExpression expression, out Variable<TDomain, TElement> term)
            where TDomain : IEnumerable<TElement>
        {
            if (expression.Body is ParameterExpression parameterExpr
                && typeof(TElement).IsAssignableFrom(parameterExpr.Type))
            {
                term = new Variable<TDomain, TElement>(parameterExpr.Name);
                return true;
            }

            term = null;
            return false;
        }

        internal static bool TryCreateVariable<TDomain, TElement>(LambdaExpression expression, out Term<TDomain, TElement> term)
            where TDomain : IEnumerable<TElement>
        {
            var returnValue = TryCreateVariable(expression, out Variable<TDomain, TElement> variableTerm);
            term = variableTerm;
            return returnValue;
        }

        /// <remarks>
        /// TODO-MAINTAINABILITY: This was written when I was thinking about each sentence type
        /// exposing a lambda directly. If we're not going to do that then this does more work 
        /// than needed - don't need a sub-lambda..
        /// <para/>
        /// TODO-USABILITY: One way to introduce stronger type-safety here would be to add a (possibly
        /// empty) variable container - e.g. Expression<Predicate<TDomain, VariableContainer<TElement>>>.
        /// Or perhaps Expresison<Predicate<FOLScope<TDomain, TElement>>>. Something for v2...
        /// Would need to decide if the extra complexity (& perf hit with variable access?) is worth it.
        /// </remarks>
        private static LambdaExpression MakeSubLambda(LambdaExpression lambda, Expression body)
        {
            // TODO-ROBUSTNESS: Debug check that lambda contains body?
            // 

            // Flatten body that is a lamba (happens in quantifiers)
            if (body is LambdaExpression bodyLambda)
            {
                return Expression.Lambda(bodyLambda.Body, lambda.Parameters.Concat(bodyLambda.Parameters));
            }

            return Expression.Lambda(body, lambda.Parameters);
        }
    }
}
