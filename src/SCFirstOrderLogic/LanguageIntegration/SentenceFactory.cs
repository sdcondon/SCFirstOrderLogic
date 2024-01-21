// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SCFirstOrderLogic.LanguageIntegration
{
    /// <summary>
    /// Static factory methods for creating <see cref="Sentence"/> instances from LINQ expressions operating on an (<see cref="IEnumerable{T}"/>) object representing the domain.
    /// The conventions used can be found in library documentation.
    /// </summary>
    public static class SentenceFactory
    {
        private static readonly MethodInfo IfMethod;
        private static readonly MethodInfo IffMethod;
        private static readonly MethodInfo AnyMethod;
        private static readonly MethodInfo ShorthandAnyMethod2;
        private static readonly MethodInfo ShorthandAnyMethod3;
        private static readonly MethodInfo AllMethod;
        private static readonly MethodInfo ShorthandAllMethod2;
        private static readonly MethodInfo ShorthandAllMethod3;

        static SentenceFactory()
        {
            IfMethod = typeof(Operators).GetMethod(nameof(Operators.If)) ?? throw new NotSupportedException("Couldn't find 'If' method");

            IffMethod = typeof(Operators).GetMethod(nameof(Operators.Iff)) ?? throw new NotSupportedException("Couldn't find 'Iff' method");

            AnyMethod = typeof(Enumerable).GetMethod(
                nameof(Enumerable.Any),
                new[]
                {
                    typeof(IEnumerable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
                    typeof(Func<,>).MakeGenericType(Type.MakeGenericMethodParameter(0), typeof(bool))
                }) ?? throw new NotSupportedException("Couldn't find 'Any' method");

            ShorthandAnyMethod2 = typeof(IEnumerableExtensions).GetMethod(
                nameof(IEnumerableExtensions.All),
                new[]
                {
                    typeof(IEnumerable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
                    typeof(Func<,,>).MakeGenericType(Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(0), typeof(bool))
                }) ?? throw new NotSupportedException("Couldn't find 2-param 'Any' method");

            ShorthandAnyMethod3 = typeof(IEnumerableExtensions).GetMethod(
                nameof(IEnumerableExtensions.All),
                new[]
                {
                    typeof(IEnumerable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
                    typeof(Func<,,,>).MakeGenericType(Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(0), typeof(bool))
                }) ?? throw new NotSupportedException("Couldn't find 3-param 'Any' method");

            AllMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.All)) ?? throw new Exception("Couldn't find 'All' method");

            ShorthandAllMethod2 = typeof(IEnumerableExtensions).GetMethod(
                nameof(IEnumerableExtensions.All),
                new[]
                {
                    typeof(IEnumerable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
                    typeof(Func<,,>).MakeGenericType(Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(0), typeof(bool))
                }) ?? throw new NotSupportedException("Couldn't find 2-param 'All' method");

            ShorthandAllMethod3 = typeof(IEnumerableExtensions).GetMethod(
                nameof(IEnumerableExtensions.All),
                new[]
                {
                    typeof(IEnumerable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
                    typeof(Func<,,,>).MakeGenericType(Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(0), Type.MakeGenericMethodParameter(0), typeof(bool))
                }) ?? throw new NotSupportedException("Couldn't find 3-param 'All' method");
        }

        /// <summary>
        /// Creates and returns the <see cref="Sentence"/> instance that is logically equivalent to
        /// the proposition that a given lambda expression is guaranteed to evaluate as true for all possible domains.
        /// </summary>
        /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
        /// <param name="lambda">The lambda expression.</param>
        /// <returns>The created sentence.</returns>
        public static Sentence Create<TElement>(Expression<Predicate<IEnumerable<TElement>>> lambda)
        {
            if (!TryCreate<TElement>(lambda, out var sentence))
            {
                throw new ArgumentException("Expression is not convertible to a sentence of first order logic", nameof(lambda));
            }

            return sentence;
        }

        /// <summary>
        /// Creates and returns the <see cref="Sentence"/> instance that is logically equivalent to
        /// the proposition that a given lambda expression is guaranteed to evaluate as true for all possible domains.
        /// </summary>
        /// <param name="lambda">The lambda expression.</param>
        /// <returns>The created sentence.</returns>
        public static Sentence Create<TDomain, TElement>(Expression<Predicate<TDomain>> lambda)
            where TDomain : IEnumerable<TElement>
        {
            if (!TryCreate<TDomain, TElement>(lambda, out var sentence))
            {
                throw new ArgumentException("Expression is not convertible to a sentence of first order logic", nameof(lambda));
            }

            return sentence;
        }

        /// <summary>
        /// <para>
        /// Tries to create the <see cref="Sentence"/> instance that is logically equivalent to
        /// the proposition that a given lambda expression is guaranteed to evaluate as true for all possible domains.
        /// </para>
        /// <para>
        /// This method serves as a shorthand for <see cref="TryCreate{TDomain, TElement}"/> where the domain is
        /// just <c>IEnumerable&lt;TElement&gt;</c> - which suffices when the domain contains no constants or ground
        /// predicates.
        /// </para>
        /// </summary>
        /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
        /// <param name="lambda">The lambda expression.</param>
        /// <param name="sentence">The created sentence, or <see langword="null"/> on failure.</param>
        /// <returns>A value indicating whether or not creation was successful.</returns>
        public static bool TryCreate<TElement>(Expression<Predicate<IEnumerable<TElement>>> lambda, [NotNullWhen(returnValue: true)] out Sentence? sentence)
        {
            return TryCreateSentence<IEnumerable<TElement>, TElement>(lambda.Body, out sentence);
        }

        /// <summary>
        /// Tries to create the <see cref="Sentence"/> instance that is logically equivalent to
        /// the proposition that a given lambda expression is guaranteed to evaluate as true for all possible domains.
        /// </summary>
        /// <typeparam name="TDomain">The type of the domain. The domain must be modelled as an <see cref="IEnumerable{T}"/> of TElement instances.</typeparam>
        /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
        /// <param name="lambda">The lambda expression.</param>
        /// <param name="sentence">The created sentence, or <see langword="null"/> on failure.</param>
        /// <returns>A value indicating whether or not creation was successful.</returns>
        public static bool TryCreate<TDomain, TElement>(Expression<Predicate<TDomain>> lambda, [NotNullWhen(returnValue: true)] out Sentence? sentence)
            where TDomain : IEnumerable<TElement>
        {
            return TryCreateSentence<TDomain, TElement>(lambda.Body, out sentence);
        }

        // TODO-ZZ-QOL: might be nice to return more info than just "false" on failure. that is, return an InstantiationOutcome or somesuch instead of a bool
        private static bool TryCreateSentence<TDomain, TElement>(Expression expression, [NotNullWhen(returnValue: true)] out Sentence? sentence)
            where TDomain : IEnumerable<TElement>
        {
            return
                // Complex sentences:
                TryCreateNegation<TDomain, TElement>(expression, out sentence)
                || TryCreateConjunction<TDomain, TElement>(expression, out sentence)
                || TryCreateDisjunction<TDomain, TElement>(expression, out sentence)
                || TryCreateEquivalence<TDomain, TElement>(expression, out sentence)
                || TryCreateImplication<TDomain, TElement>(expression, out sentence)
                || TryCreateUniversalQuantification<TDomain, TElement>(expression, out sentence)
                || TryCreateExistentialQuantification<TDomain, TElement>(expression, out sentence)
                // Atomic sentences:
                || TryCreateEquality<TDomain, TElement>(expression, out sentence)
                || TryCreatePredicate<TDomain, TElement>(expression, out sentence);
        }

        /// <summary>
        /// Tries to create a <see cref="Conjunction"/> from an expression acting on the domain (and any relevant variables and constants) of the form:
        /// <code>{expression} {&amp;&amp; or &amp;} {expression}</code>
        /// </summary>
        private static bool TryCreateConjunction<TDomain, TElement>(Expression expression, [NotNullWhen(returnValue: true)] out Sentence? sentence)
            where TDomain : IEnumerable<TElement>
        {
            if (expression is BinaryExpression binaryExpr && (binaryExpr.NodeType == ExpressionType.AndAlso || binaryExpr.NodeType == ExpressionType.And)
                && TryCreateSentence<TDomain, TElement>(binaryExpr.Left, out var left)
                && TryCreateSentence<TDomain, TElement>(binaryExpr.Right, out var right))
            {
                sentence = new Conjunction(left, right);
                return true;
            }

            sentence = null;
            return false;
        }

        private static bool TryCreateConstant<TDomain, TElement>(Expression expression, [NotNullWhen(returnValue: true)] out Term? term)
            where TDomain : IEnumerable<TElement>
        {
            if (typeof(TElement).IsAssignableFrom(expression.Type)) // Constants must be elements of the domain
            {
                if (expression is MemberExpression memberExpr
                    && typeof(TDomain).IsAssignableFrom(memberExpr.Expression?.Type)) // BUG-MINOR: Should check if its accessing the domain-valued param (think of weird situations where its a domain-valued prop of an element or somat)..
                {
                    // TElement-valued property access of the domain is interpreted as a constant.
                    term = new Constant(new MemberConstantIdentifier(memberExpr.Member));
                    return true;
                }
                else if (expression is MethodCallExpression methodCallExpr
                    && typeof(TDomain).IsAssignableFrom(methodCallExpr.Object?.Type) // BUG-MINOR: Should check if its accessing the domain-valued param (think of weird situations where its a domain-valued prop of an element or somat)..
                    && methodCallExpr.Arguments.Count == 0)
                {
                    // TElement-valued parameterless method call of the domain is interpreted as a constant.
                    term = new Constant(new MemberConstantIdentifier(methodCallExpr.Method));
                    return true;
                }
            }

            // Perhaps we should allow literals here too? For cases where the domain is of primitive types, it seems
            // silly to require d => d.Hello rather than d => "Hello". For user provided types is of less value because
            // it requires an instantiation of the constant (where otherwise just working with interfaces is fine), but
            // as far as I can see there's no reason (aside from equality et al - probably solvable) to actually forbid it..?

            term = null;
            return false;
        }

        /// <summary>
        /// Tries to create a <see cref="Disjunction"/> from an expression acting on the domain (and any relevant variables and constants) of the form:
        /// <code>{expression} {|| or |} {expression}</code>
        /// </summary>
        private static bool TryCreateDisjunction<TDomain, TElement>(Expression expression, [NotNullWhen(returnValue: true)] out Sentence? sentence)
            where TDomain : IEnumerable<TElement>
        {
            if (expression is BinaryExpression binaryExpr && (binaryExpr.NodeType == ExpressionType.OrElse || binaryExpr.NodeType == ExpressionType.Or)
                && TryCreateSentence<TDomain, TElement>(binaryExpr.Left, out var left)
                && TryCreateSentence<TDomain, TElement>(binaryExpr.Right, out var right))
            {
                sentence = new Disjunction(left, right);
                return true;
            }

            sentence = null;
            return false;
        }

        /// <summary>
        /// Tries to create a <see cref="Predicate"/> with the <see cref="EqualityIdentifier"/> from an expression acting on the domain (and any relevant variables and constants) of the form:
        /// <code>{expression} == {expression}</code>
        /// </summary>
        private static bool TryCreateEquality<TDomain, TElement>(Expression expression, [NotNullWhen(returnValue: true)] out Sentence? sentence)
            where TDomain : IEnumerable<TElement>
        {
            // TODO-ZZ-QOL: ..and Object.Equals invocation? And others? How to think about map of different types of .NET equality to FOL "equals"?
            if (expression is BinaryExpression binaryExpr && binaryExpr.NodeType == ExpressionType.Equal
                && TryCreateTerm<TDomain, TElement>(binaryExpr.Left, out var left)
                && TryCreateTerm<TDomain, TElement>(binaryExpr.Right, out var right))
            {
                sentence = new Predicate(EqualityIdentifier.Instance, left, right);
                return true;
            }

            sentence = null;
            return false;
        }

        /// <summary>
        /// Tries to create a <see cref="Equivalence"/> from an expression acting on the domain (and any relevant variables and constants) of the form:
        /// <code>Operators.Iff({expression}, {expression})</code>
        /// (Consumers are encouraged to include <c>using static SCFirstOrderLogic.Operators;</c> to make this a little shorter)
        /// </summary>
        private static bool TryCreateEquivalence<TDomain, TElement>(Expression expression, [NotNullWhen(returnValue: true)] out Sentence? sentence)
            where TDomain : IEnumerable<TElement>
        {
            if (expression is MethodCallExpression methodCallExpr && MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, IffMethod)
                && TryCreateSentence<TDomain, TElement>(methodCallExpr.Arguments[0], out var equivalent1)
                && TryCreateSentence<TDomain, TElement>(methodCallExpr.Arguments[1], out var equivalent2))
            {
                sentence = new Equivalence(equivalent1, equivalent2);
                return true;
            }

            sentence = null;
            return false;
        }

        /// <summary>
        /// Tries to create a <see cref="ExistentialQuantification"/> from an expression acting on the domain (and any relevant variables and constants) of the form:
        /// <code>{domain}.Any({variable} => {expression})</code>
        /// </summary>
        private static bool TryCreateExistentialQuantification<TDomain, TElement>(Expression expression, [NotNullWhen(returnValue: true)] out Sentence? sentence)
            where TDomain : IEnumerable<TElement>
        {
            // TODO-ZZ-QOL: would be nice to have better errors if they've e.g. attempted to use something other than a lambda..
            if (expression is MethodCallExpression methodCallExpr)
            {
                if (MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, AnyMethod)
                    && methodCallExpr.Arguments[1] is LambdaExpression anyLambda
                    && TryCreateSentence<TDomain, TElement>(anyLambda.Body, out var subSentence)
                    && TryCreateVariableDeclaration<TDomain, TElement>(anyLambda.Parameters[0], out VariableDeclaration? declaration))
                {
                    sentence = new ExistentialQuantification(declaration, subSentence);
                    return true;
                }
                else if (MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, ShorthandAnyMethod2)
                    && methodCallExpr.Arguments[1] is LambdaExpression any2Lambda
                    && TryCreateSentence<TDomain, TElement>(any2Lambda.Body, out var all2SubSentence)
                    && TryCreateVariableDeclaration<TDomain, TElement>(any2Lambda.Parameters[0], out VariableDeclaration? declaration1Of2)
                    && TryCreateVariableDeclaration<TDomain, TElement>(any2Lambda.Parameters[1], out VariableDeclaration? declaration2Of2))
                {
                    sentence = new ExistentialQuantification(
                        declaration1Of2,
                        new ExistentialQuantification(declaration2Of2, all2SubSentence));
                    return true;
                }
                else if (MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, ShorthandAnyMethod3)
                    && methodCallExpr.Arguments[1] is LambdaExpression any3Lambda
                    && TryCreateSentence<TDomain, TElement>(any3Lambda.Body, out var all3SubSentence)
                    && TryCreateVariableDeclaration<TDomain, TElement>(any3Lambda.Parameters[0], out VariableDeclaration? declaration1Of3)
                    && TryCreateVariableDeclaration<TDomain, TElement>(any3Lambda.Parameters[1], out VariableDeclaration? declaration2Of3)
                    && TryCreateVariableDeclaration<TDomain, TElement>(any3Lambda.Parameters[2], out VariableDeclaration? declaration3Of3))
                {
                    sentence = new UniversalQuantification(
                        declaration1Of3,
                        new ExistentialQuantification(
                            declaration2Of3,
                            new ExistentialQuantification(declaration3Of3, all3SubSentence)));
                    return true;
                }
            }

            sentence = null;
            return false;
        }

        private static bool TryCreateFunction<TDomain, TElement>(Expression expression, [NotNullWhen(returnValue: true)] out Term? term)
            where TDomain : IEnumerable<TElement>
        {
            // NB: Here we verify that the value of the function is a domain element..
            if (typeof(TElement).IsAssignableFrom(expression.Type))
            {
                if (expression is MemberExpression memberExpr
                    && memberExpr.Expression != null
                    && TryCreateTerm<TDomain, TElement>(memberExpr.Expression, out var argument))
                {
                    // TElement-valued property access is interpreted as a unary function.
                    term = new Function(new MemberFunctionIdentifier(memberExpr.Member), new[] { argument });
                    return true;
                }
                else if (expression is MethodCallExpression methodCallExpr
                    && (methodCallExpr.Object != null || methodCallExpr.Arguments.Count > 0)) // NB: There must be at least one arg - otherwise its a constant, not a function..
                {
                    var arguments = new List<Term>();

                    // If the method is non-static, the instance it is operating on is the first arg of the predicate:
                    if (methodCallExpr.Object != null)
                    {
                        if (!TryCreateTerm<TDomain, TElement>(methodCallExpr.Object, out var arg))
                        {
                            term = null;
                            return false;
                        }

                        arguments.Add(arg);
                    }

                    foreach (var argExpr in methodCallExpr.Arguments)
                    {
                        if (!TryCreateTerm<TDomain, TElement>(argExpr, out var arg))
                        {
                            term = null;
                            return false;
                        }

                        arguments.Add(arg);
                    }

                    term = new Function(new MemberFunctionIdentifier(methodCallExpr.Method), arguments);
                    return true;
                }
            }

            term = null;
            return false;
        }

        /// <summary>
        /// Tries to create a <see cref="Implication"/> from an expression acting on the domain (and any relevant variables and constants) of the form:
        /// <code>Operators.If({expression}, {expression})</code>
        /// (Consumers are encouraged to include <c>using static SCFirstOrderLogic.LanguageIntegration.Operators;</c> to make this a little shorter)
        /// </summary>
        private static bool TryCreateImplication<TDomain, TElement>(Expression expression, [NotNullWhen(returnValue: true)] out Sentence? sentence)
            where TDomain : IEnumerable<TElement>
        {
            if (expression is MethodCallExpression methodCallExpr && MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, IfMethod)
                && TryCreateSentence<TDomain, TElement>(methodCallExpr.Arguments[0], out var antecedent)
                && TryCreateSentence<TDomain, TElement>(methodCallExpr.Arguments[1], out var consequent))
            {
                sentence = new Implication(antecedent, consequent);
                return true;
            }

            sentence = null;
            return false;
        }

        /// <summary>
        /// Tries to create a <see cref="Negation"/> from an expression acting on the domain (and any relevant variables and constants) of the form:
        /// <code>!{expression}</code>
        /// We also interpret <c>!=</c> as a negation of an equality.
        /// </summary>
        private static bool TryCreateNegation<TDomain, TElement>(Expression expression, [NotNullWhen(returnValue: true)] out Sentence? sentence)
            where TDomain : IEnumerable<TElement>
        {
            if (expression is UnaryExpression unaryExpr && unaryExpr.NodeType == ExpressionType.Not
                && TryCreateSentence<TDomain, TElement>(unaryExpr.Operand, out var operand))
            {
                sentence = new Negation(operand);
                return true;
            }
            else if (expression is BinaryExpression binaryExpr && binaryExpr.NodeType == ExpressionType.NotEqual
                && TryCreateTerm<TDomain, TElement>(binaryExpr.Left, out var left)
                && TryCreateTerm<TDomain, TElement>(binaryExpr.Right, out var right))
            {
                sentence = new Negation(new Predicate(EqualityIdentifier.Instance, left, right));
                return true;
            }

            sentence = null;
            return false;
        }

        /// <summary>
        /// Tries to create a <see cref="MemberPredicateIdentifier"/> from an expression acting on the domain (and any relevant variables and constants) that
        /// is a boolean-valued property or method call on an element object, or a boolean-valued property or method call on a domain object.
        /// </summary>
        private static bool TryCreatePredicate<TDomain, TElement>(Expression expression, [NotNullWhen(returnValue: true)] out Sentence? sentence)
            where TDomain : IEnumerable<TElement>
        {
            if (expression.Type != typeof(bool))
            {
                sentence = null;
                return false;
            }

            if (expression is MemberExpression memberExpr && memberExpr.Expression != null) // Non-static field or property access
            {
                if (memberExpr.Expression.Type == typeof(TDomain)) // BUG-MINOR: no guarantee that this is the domain param of the original lambda... requires passing domain param down through the whole process..
                {
                    // Boolean-valued property access on the domain parameter is interpreted as a ground predicate
                    sentence = new Predicate(new MemberPredicateIdentifier(memberExpr.Member), Array.Empty<Term>());
                    return true;
                }
                else if (TryCreateTerm<TDomain, TElement>(memberExpr.Expression, out var argument))
                {
                    // Boolean-valued property access on a term is interpreted as a unary predicate.
                    sentence = new Predicate(new MemberPredicateIdentifier(memberExpr.Member), new[] { argument });
                    return true;
                }
            }
            else if (expression is MethodCallExpression methodCallExpr && methodCallExpr.Object != null) // Non-static mmethod call
            {
                var arguments = new List<Term>();

                // If the method is not against the domain, the instance it is operating on is the first arg of the predicate:
                if (methodCallExpr.Object.Type != typeof(TDomain)) // BUG-MINOR: in theory objs of that type might also not be "the" domain param. requires passing domain param down through the whole process..
                {
                    if (!TryCreateTerm<TDomain, TElement>(methodCallExpr.Object, out var arg))
                    {
                        sentence = null;
                        return false;
                    }

                    arguments.Add(arg);
                }

                // Each of the method's args should be interpretable as a term.
                foreach (var argExpr in methodCallExpr.Arguments)
                {
                    if (!TryCreateTerm<TDomain, TElement>(argExpr, out var arg))
                    {
                        sentence = null;
                        return false;
                    }

                    arguments.Add(arg);
                }

                sentence = new Predicate(new MemberPredicateIdentifier(methodCallExpr.Method), arguments);
                return true;
            }
            //// ... also to consider - certain things will fail the above but could be very sensibly interpreted
            //// as predicates. E.g. a Contains() call on a property that is a collection of TElements. Or indeed Any on
            //// an IEnumerable of them.

            sentence = null;
            return false;
        }

        private static bool TryCreateTerm<TDomain, TElement>(Expression expression, [NotNullWhen(returnValue: true)] out Term? sentence)
            where TDomain : IEnumerable<TElement>
        {
            return TryCreateFunction<TDomain, TElement>(expression, out sentence)
                || TryCreateConstant<TDomain, TElement>(expression, out sentence)
                || TryCreateVariable<TDomain, TElement>(expression, out sentence);
        }

        /// <summary>
        /// Tries to create a <see cref="UniversalQuantification"/> from an expression acting on the domain (and any relevant variables and constants) of the form:
        /// <code>{domain}.All({variable} => {expression})</code>
        /// </summary>
        private static bool TryCreateUniversalQuantification<TDomain, TElement>(Expression expression, [NotNullWhen(returnValue: true)] out Sentence? sentence)
            where TDomain : IEnumerable<TElement>
        {
            // TODO-ZZ-QOL: also would be nice to have better errors if they've e.g. attempted to use something other than a lambda..
            if (expression is MethodCallExpression methodCallExpr)
            {
                if (MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, AllMethod)
                    && methodCallExpr.Arguments[1] is LambdaExpression allLambda 
                    && TryCreateSentence<TDomain, TElement>(allLambda.Body, out var allSubSentence)
                    && TryCreateVariableDeclaration<TDomain, TElement>(allLambda.Parameters[0], out VariableDeclaration? declaration))
                {
                    sentence = new UniversalQuantification(declaration, allSubSentence);
                    return true;
                }
                else if (MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, ShorthandAllMethod2)
                    && methodCallExpr.Arguments[1] is LambdaExpression all2Lambda
                    && TryCreateSentence<TDomain, TElement>(all2Lambda.Body, out var all2SubSentence)
                    && TryCreateVariableDeclaration<TDomain, TElement>(all2Lambda.Parameters[0], out VariableDeclaration? declaration1Of2)
                    && TryCreateVariableDeclaration<TDomain, TElement>(all2Lambda.Parameters[1], out VariableDeclaration? declaration2Of2))
                {
                    sentence = new UniversalQuantification(
                        declaration1Of2,
                        new UniversalQuantification(declaration2Of2, all2SubSentence));
                    return true;
                }
                else if (MemberInfoEqualityComparer.Instance.Equals(methodCallExpr.Method, ShorthandAllMethod3)
                    && methodCallExpr.Arguments[1] is LambdaExpression all3Lambda
                    && TryCreateSentence<TDomain, TElement>(all3Lambda.Body, out var all3SubSentence)
                    && TryCreateVariableDeclaration<TDomain, TElement>(all3Lambda.Parameters[0], out VariableDeclaration? declaration1Of3)
                    && TryCreateVariableDeclaration<TDomain, TElement>(all3Lambda.Parameters[1], out VariableDeclaration? declaration2Of3)
                    && TryCreateVariableDeclaration<TDomain, TElement>(all3Lambda.Parameters[2], out VariableDeclaration? declaration3Of3))
                {
                    sentence = new UniversalQuantification(
                        declaration1Of3,
                        new UniversalQuantification(
                            declaration2Of3,
                            new UniversalQuantification(declaration3Of3, all3SubSentence)));
                    return true;
                }
            }

            sentence = null;
            return false;
        }

        private static bool TryCreateVariableDeclaration<TDomain, TElement>(Expression expression, [NotNullWhen(returnValue: true)] out VariableDeclaration? variableDeclaration)
            where TDomain : IEnumerable<TElement>
        {
            if (expression is ParameterExpression parameterExpr
                && typeof(TElement).IsAssignableFrom(parameterExpr.Type)
                && parameterExpr.Name != null)
            {
                variableDeclaration = new VariableDeclaration(parameterExpr.Name);
                return true;
            }

            variableDeclaration = null;
            return false;
        }

        private static bool TryCreateVariable<TDomain, TElement>(Expression expression, [NotNullWhen(returnValue: true)] out Term? term)
            where TDomain : IEnumerable<TElement>
        {
            if (expression is ParameterExpression parameterExpr
                && typeof(TElement).IsAssignableFrom(parameterExpr.Type)
                && parameterExpr.Name != null)
            {
                // NB: doesn't refer to a singular variable declaration object - but since equality is correctly implemented, not a huge deal. Maybe revisit later?
                term = new VariableReference(new VariableDeclaration(parameterExpr.Name));
                return true;
            }

            term = null;
            return false;
        }
    }
}
