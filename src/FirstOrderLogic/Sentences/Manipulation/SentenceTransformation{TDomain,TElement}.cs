using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToKB.FirstOrderLogic.Sentences.Manipulation
{
    /// <summary>
    /// Base class for transformations of <see cref="Sentence{TDomain, TElement}"/> instances.
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain of the sentences to transform.</typeparam>
    /// <typeparam name="TElement">The type of the elements of the sentences to transform.</typeparam>
    public abstract class SentenceTransformation<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        /// <summary>
        /// Applies this transformation to a <see cref="Sentence{TDomain, TElement}"/> instance.
        /// The default implementation simply invokes the Apply method appropriate to the actual type of the sentence.
        /// </summary>
        /// <param name="sentence">The sentence to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyTo(Sentence<TDomain, TElement> sentence)
        {
            // TODO-PERFORMANCE: Using "proper" visitor pattern will be (ever so slightly) faster than this -
            // decide if its worth the extra complexity.
            return sentence switch
            {
                Conjunction<TDomain, TElement> conjunction => ApplyTo(conjunction),
                Disjunction<TDomain, TElement> disjunction => ApplyTo(disjunction),
                Equality<TDomain, TElement> equality => ApplyTo(equality),
                Equivalence<TDomain, TElement> equivalence => ApplyTo(equivalence),
                ExistentialQuantification<TDomain, TElement> existentialQuantification => ApplyTo(existentialQuantification),
                Implication<TDomain, TElement> implication => ApplyTo(implication),
                Negation<TDomain, TElement> negation => ApplyTo(negation),
                Predicate<TDomain, TElement> predicate => ApplyTo(predicate),
                UniversalQuantification<TDomain, TElement> universalQuantification => ApplyTo(universalQuantification),
                _ => throw new ArgumentException("Unsupported sentence type")
            };
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Conjunction{TDomain, TElement}"/> instance.
        /// The default implementation returns a <see cref="Conjunction{TDomain, TElement}"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="conjunction">The conjunction instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyTo(Conjunction<TDomain, TElement> conjunction)
        {
            var left = ApplyTo(conjunction.Left);
            var right = ApplyTo(conjunction.Right);
            if (left != conjunction.Left || right != conjunction.Right)
            {
                return new Conjunction<TDomain, TElement>(left, right);
            }

            return conjunction;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Disjunction{TDomain, TElement}"/> instance.
        /// The default implementation returns a <see cref="Disjunction{TDomain, TElement}"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="conjunction">The <see cref="Disjunction{TDomain, TElement}"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyTo(Disjunction<TDomain, TElement> disjunction)
        {
            var left = ApplyTo(disjunction.Left);
            var right = ApplyTo(disjunction.Right);
            if (left != disjunction.Left || right != disjunction.Right)
            {
                return new Disjunction<TDomain, TElement>(left, right);
            }

            return disjunction;
        }

        /// <summary>
        /// Applies this transformation to an <see cref="Equality{TDomain, TElement}"/> instance..
        /// The default implementation returns an <see cref="Equality{TDomain, TElement}"/> of the result of calling <see cref="ApplyTo"/> on both of the existing terms.
        /// </summary>
        /// <param name="equality">The <see cref="Equality{TDomain, TElement}"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyTo(Equality<TDomain, TElement> equality)
        {
            var left = ApplyTo(equality.Left);
            var right = ApplyTo(equality.Right);
            if (left != equality.Left || right != equality.Right)
            {
                return new Equality<TDomain, TElement>(left, right);
            }

            return equality;
        }

        /// <summary>
        /// Applies this transformation to an <see cref="Equivalence{TDomain, TElement}"/> instance. 
        /// The default implementation returns an <see cref="Equivalence{TDomain, TElement}"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="equivalence">The <see cref="Equivalence{TDomain, TElement}"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyTo(Equivalence<TDomain, TElement> equivalence)
        {
            var equivalent1 = ApplyTo(equivalence.Equivalent1);
            var equivalent2 = ApplyTo(equivalence.Equivalent2);
            if (equivalent1 != equivalence.Equivalent1 || equivalent2 != equivalence.Equivalent2)
            {
                return new Equivalence<TDomain, TElement>(equivalent1, equivalent2);
            }

            return equivalence;
        }

        /// <summary>
        /// Applies this transformation to an <see cref="ExistentialQuantification{TDomain, TElement}{TDomain, TElement}"/> instance. 
        /// The default implementation returns an <see cref="ExistentialQuantification{TDomain, TElement}"/> for which the variable declaration is the result of <see cref="ApplyTo"/> on the existing declaration, and the sentence is the result of <see cref="ApplyTo"/> on the existing sentence.
        /// </summary>
        /// <param name="existentialQuantification">The <see cref="ExistentialQuantification{TDomain, TElement}"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyTo(ExistentialQuantification<TDomain, TElement> existentialQuantification)
        {
            var variable = ApplyTo(existentialQuantification.Variable);
            var sentence = ApplyTo(existentialQuantification.Sentence);
            if (variable != existentialQuantification.Variable || sentence != existentialQuantification.Sentence)
            {
                return new ExistentialQuantification<TDomain, TElement>(variable, sentence);
            }

            return existentialQuantification;
        }

        /// <summary>
        /// Applies this transformation to an <see cref="Implication{TDomain, TElement}"/> instance. 
        /// The default implementation returns an <see cref="Implication{TDomain, TElement}"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="implication">The <see cref="Implication{TDomain, TElement}"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyTo(Implication<TDomain, TElement> implication)
        {
            var antecedent = ApplyTo(implication.Antecedent);
            var consequent = ApplyTo(implication.Consequent);

            if (antecedent != implication.Antecedent || consequent != implication.Consequent)
            {
                return new Implication<TDomain, TElement>(antecedent, consequent);
            }

            return implication;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Negation{TDomain, TElement}"/> instance. 
        /// The default implementation returns an <see cref="Negation{TDomain, TElement}"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="negation">The <see cref="Negation{TDomain, TElement}"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyTo(Negation<TDomain, TElement> negation)
        {
            var sentence = ApplyTo(negation.Sentence);

            if (sentence != negation.Sentence)
            {
                return new Negation<TDomain, TElement>(sentence);
            }

            return negation;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Predicate{TDomain, TElement}"/> instance. 
        /// The default implementation returns an <see cref="Predicate{TDomain, TElement}"/> pointed at the same <see cref="MemberInfo"/> and with an argument list that is the result of calling <see cref="ApplyTo"/> on both of the existing arguments.
        /// </summary>
        /// <param name="predicate">The <see cref="Predicate{TDomain, TElement}"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyTo(Predicate<TDomain, TElement> predicate)
        {
            var arguments = predicate.Arguments.Select(a => ApplyTo(a)).ToList();

            if (arguments.Zip(predicate.Arguments, (x, y) => (x, y)).Any(t => t.x != t.y))
            {
                return new Predicate<TDomain, TElement>(predicate.Member, arguments);
            }

            return predicate;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="UniversalQuantification{TDomain, TElement}{TDomain, TElement}"/> instance. 
        /// The default implementation returns an <see cref="UniversalQuantification{TDomain, TElement}"/> for which the variable declaration is the result of <see cref="ApplyTo"/> on the existing declaration, and the sentence is the result of <see cref="ApplyTo"/> on the existing sentence.
        /// </summary>
        /// <param name="universalQuantification">The <see cref="UniversalQuantification{TDomain, TElement}"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyTo(UniversalQuantification<TDomain, TElement> universalQuantification)
        {
            var variable = ApplyTo(universalQuantification.Variable);
            var sentence = ApplyTo(universalQuantification.Sentence);
            if (variable != universalQuantification.Variable || sentence != universalQuantification.Sentence)
            {
                return new UniversalQuantification<TDomain, TElement>(variable, sentence);
            }

            return universalQuantification;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="VariableDeclaration{TDomain, TElement}"/> instance.
        /// The default implementation simply returns the passed value.
        /// </summary>
        /// <param name="variableDeclaration">The <see cref="VariableDeclaration{TDomain, TElement}"/> instance to transform.</param>
        /// <returns>The transformed <see cref="Variable{TDomain, TElement}"/> declaration.</returns>
        public virtual VariableDeclaration<TDomain, TElement> ApplyTo(VariableDeclaration<TDomain, TElement> variableDeclaration)
        {
            return variableDeclaration;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Term{TDomain, TElement}"/> instance.
        /// The default implementation simply invokes the <see cref="ApplyTo"/> method appropriate to the type of the term.
        /// </summary>
        /// <param name="term">The term to visit.</param>
        /// <returns>The transformed term.</returns>
        public virtual Term<TDomain, TElement> ApplyTo(Term<TDomain, TElement> term)
        {
            return term switch
            {
                Constant<TDomain, TElement> constant => ApplyTo(constant),
                Variable<TDomain, TElement> variable => ApplyTo(variable),
                Function<TDomain, TElement> function => ApplyTo(function),
                _ => throw new ArgumentException()
            };
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Constant{TDomain, TElement}"/> instance.
        /// The default implementation simply returns the constant unchanged.
        /// </summary>
        /// <param name="term">The constant to visit.</param>
        /// <returns>The transformed term.</returns>
        public virtual Term<TDomain, TElement> ApplyTo(Constant<TDomain, TElement> constant)
        {
            return constant;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Variable{TDomain, TElement}"/> instance.
        /// The default implementation simply returns the variable unchanged.
        /// </summary>
        /// <param name="term">The variable to visit.</param>
        /// <returns>The transformed term.</returns>
        public virtual Term<TDomain, TElement> ApplyTo(Variable<TDomain, TElement> variable)
        {
            return variable;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Function{TDomain, TElement}"/> instance.
        /// The default implementation simply invokes the <see cref="ApplyTo"/> method appropriate to the type of the function.
        /// </summary>
        /// <param name="function">The function to visit.</param>
        /// <returns>The transformed function.</returns>
        public virtual Term<TDomain, TElement> ApplyTo(Function<TDomain, TElement> function)
        {
            return function switch
            {
                DomainFunction<TDomain, TElement> domainFunction => ApplyTo(domainFunction),
                SkolemFunction<TDomain, TElement> skolemFunction => ApplyTo(skolemFunction),
                _ => throw new ArgumentException()
            };
        }

        /// <summary>
        /// Applies this transformation to a <see cref="DomainFunction{TDomain, TElement}"/> instance.
        /// The default implementation returns a <see cref="DomainFunction{TDomain, TElement}"/> pointed at the same <see cref="MemberInfo"/> and with an argument list that is the result of calling <see cref="ApplyTo"/> on each of the existing arguments.
        /// </summary>
        /// <param name="domainFunction">The function to visit.</param>
        /// <returns>The transformed term.</returns>
        public virtual Term<TDomain, TElement> ApplyTo(DomainFunction<TDomain, TElement> domainFunction)
        {
            var arguments = domainFunction.Arguments.Select(a => ApplyTo(a)).ToList();

            if (arguments.Zip(domainFunction.Arguments, (x, y) => (x, y)).Any(t => t.x != t.y))
            {
                return new DomainFunction<TDomain, TElement>(domainFunction.Member, arguments);
            }

            return domainFunction;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="SkolemFunction{TDomain, TElement}"/> instance.
        /// The default implementation returns an <see cref="SkolemFunction{TDomain, TElement}"/> with the same label, and with an argument list that is the result of calling <see cref="ApplyTo"/> on each of the existing arguments.
        /// </summary>
        /// <param name="skolemFunction">The function to visit.</param>
        /// <returns>The transformed term.</returns>
        public virtual Term<TDomain, TElement> ApplyTo(SkolemFunction<TDomain, TElement> skolemFunction)
        {
            var arguments = skolemFunction.Arguments.Select(a => ApplyTo(a)).ToList();

            if (arguments.Zip(skolemFunction.Arguments, (x, y) => (x, y)).Any(t => t.x != t.y))
            {
                return new SkolemFunction<TDomain, TElement>(skolemFunction.Label, arguments);
            }

            return skolemFunction;
        }
    }
}
