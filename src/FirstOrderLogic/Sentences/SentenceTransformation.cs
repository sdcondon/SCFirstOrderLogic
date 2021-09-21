using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Base class for trnasformations of <see cref="Sentence{TDomain, TElement}"/> instances.
    /// </summary>
    /// <typeparam name="TDomain">the type of the domain of the sentences to transform.</typeparam>
    /// <typeparam name="TElement">The type of the elements of the sentences to transform.</typeparam>
    public abstract class SentenceTransformation<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        /// <summary>
        /// Transforms a <see cref="Sentence{TDomain, TElement}"/> instance.
        /// The default implementation simply invokes the Visit.. method appropriate to the actual type of the sentence.
        /// </summary>
        /// <param name="sentence">The sentence to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyToSentence(Sentence<TDomain, TElement> sentence)
        {
            return sentence switch
            {
                Conjunction<TDomain, TElement> conjunction => ApplyToConjunction(conjunction),
                Disjunction<TDomain, TElement> disjunction => ApplyToDisjunction(disjunction),
                Equality<TDomain, TElement> equality => ApplyToEquality(equality),
                Equivalence<TDomain, TElement> equivalence => ApplyToEquivalence(equivalence),
                ExistentialQuantification<TDomain, TElement> existentialQuantification => ApplyToExistentialQuantification(existentialQuantification),
                Implication<TDomain, TElement> implication => ApplyToImplication(implication),
                Negation<TDomain, TElement> negation => ApplyToNegation(negation),
                Predicate<TDomain, TElement> predicate => ApplyToPredicate(predicate),
                UniversalQuantification<TDomain, TElement> universalQuantification => ApplyToUniversalQuantification(universalQuantification),
                _ => throw new ArgumentException("Unsupported sentence type") // TODO: better exception message..
            };
        }

        /// <summary>
        /// Transforms a <see cref="Conjunction{TDomain, TElement}"/> instance.
        /// The default implementation returns a <see cref="Conjunction{TDomain, TElement}"/> of the result of calling <see cref="ApplyToSentence"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="conjunction">The conjunction instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyToConjunction(Conjunction<TDomain, TElement> conjunction)
        {
            var left = ApplyToSentence(conjunction.Left);
            var right = ApplyToSentence(conjunction.Right);
            if (left != conjunction.Left || right != conjunction.Right)
            {
                return new Conjunction<TDomain, TElement>(left, right);
            }

            return conjunction;
        }

        /// <summary>
        /// Transforms a <see cref="Disjunction{TDomain, TElement}"/> instance.
        /// The default implementation returns a <see cref="Disjunction{TDomain, TElement}"/> of the result of calling <see cref="ApplyToSentence"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="conjunction">The <see cref="Disjunction{TDomain, TElement}"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyToDisjunction(Disjunction<TDomain, TElement> disjunction)
        {
            var left = ApplyToSentence(disjunction.Left);
            var right = ApplyToSentence(disjunction.Right);
            if (left != disjunction.Left || right != disjunction.Right)
            {
                return new Disjunction<TDomain, TElement>(left, right);
            }

            return disjunction;
        }

        /// <summary>
        /// Transforms an <see cref="Equality{TDomain, TElement}"/> instance..
        /// The default implementation returns an <see cref="Equality{TDomain, TElement}"/> of the result of calling <see cref="ApplyToTerm"/> on both of the existing terms.
        /// </summary>
        /// <param name="equality">The <see cref="Equality{TDomain, TElement}"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyToEquality(Equality<TDomain, TElement> equality)
        {
            var left = ApplyToTerm(equality.Left);
            var right = ApplyToTerm(equality.Right);
            if (left != equality.Left || right != equality.Right)
            {
                return new Equality<TDomain, TElement>(left, right);
            }

            return equality;
        }

        /// <summary>
        /// Transforms an <see cref="Equivalence{TDomain, TElement}"/> instance. 
        /// The default implementation returns an <see cref="Equivalence{TDomain, TElement}"/> of the result of calling <see cref="ApplyToSentence"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="equivalence">The <see cref="Equivalence{TDomain, TElement}"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyToEquivalence(Equivalence<TDomain, TElement> equivalence)
        {
            var equivalent1 = ApplyToSentence(equivalence.Equivalent1);
            var equivalent2 = ApplyToSentence(equivalence.Equivalent2);
            if (equivalent1 != equivalence.Equivalent1 || equivalent2 != equivalence.Equivalent2)
            {
                return new Equivalence<TDomain, TElement>(equivalent1, equivalent2);
            }

            return equivalence;
        }

        /// <summary>
        /// Transforms a <see cref="ExistentialQuantification{TDomain, TElement}{TDomain, TElement}"/> instance. 
        /// The default implementation returns an <see cref="ExistentialQuantification{TDomain, TElement}"/> for which the variable declaration is the result of <see cref="ApplyToVariableDeclaration"/> on the existing declaration, and the sentence is the result of <see cref="ApplyToSentence"/> on the existing sentence.
        /// </summary>
        /// <param name="existentialQuantification">The <see cref="ExistentialQuantification{TDomain, TElement}"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyToExistentialQuantification(ExistentialQuantification<TDomain, TElement> existentialQuantification)
        {
            var variable = ApplyToVariableDeclaration(existentialQuantification.Variable);
            var sentence = ApplyToSentence(existentialQuantification.Sentence);
            if (variable != existentialQuantification.Variable || sentence != existentialQuantification.Sentence)
            {
                return new ExistentialQuantification<TDomain, TElement>(variable, sentence);
            }

            return existentialQuantification;
        }

        /// <summary>
        /// Transforms an <see cref="Implication{TDomain, TElement}"/> instance. 
        /// The default implementation returns an <see cref="Implication{TDomain, TElement}"/> of the result of calling <see cref="ApplyToSentence"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="implication">The <see cref="Implication{TDomain, TElement}"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyToImplication(Implication<TDomain, TElement> implication)
        {
            var antecedent = ApplyToSentence(implication.Antecedent);
            var consequent = ApplyToSentence(implication.Consequent);

            if (antecedent != implication.Antecedent || consequent != implication.Consequent)
            {
                return new Implication<TDomain, TElement>(antecedent, consequent);
            }

            return implication;
        }

        /// <summary>
        /// Transforms a <see cref="Negation{TDomain, TElement}"/> instance. 
        /// The default implementation returns an <see cref="Negation{TDomain, TElement}"/> of the result of calling <see cref="ApplyToSentence"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="negation">The <see cref="Negation{TDomain, TElement}"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyToNegation(Negation<TDomain, TElement> negation)
        {
            var sentence = ApplyToSentence(negation.Sentence);

            if (sentence != negation.Sentence)
            {
                return new Negation<TDomain, TElement>(sentence);
            }

            return negation;
        }

        /// <summary>
        /// Transforms a <see cref="Predicate{TDomain, TElement}"/> instance. 
        /// The default implementation returns an <see cref="Predicate{TDomain, TElement}"/> pointed at the same <see cref="MemberInfo"/> and with an argument list that is the result of calling <see cref="ApplyToTerm"/> on both of the existing arguments.
        /// </summary>
        /// <param name="predicate">The <see cref="Predicate{TDomain, TElement}"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyToPredicate(Predicate<TDomain, TElement> predicate)
        {
            var arguments = predicate.Arguments.Select(a => ApplyToTerm(a)).ToList();

            if (arguments.Zip(predicate.Arguments, (x, y) => (x, y)).Any(t => t.x != t.y))
            {
                return new Predicate<TDomain, TElement>(predicate.Member, arguments);
            }

            return predicate;
        }

        /// <summary>
        /// Transforms a <see cref="UniversalQuantification{TDomain, TElement}{TDomain, TElement}"/> instance. 
        /// The default implementation returns an <see cref="UniversalQuantification{TDomain, TElement}"/> for which the variable declaration is the result of <see cref="ApplyToVariableDeclaration"/> on the existing declaration, and the sentence is the result of <see cref="ApplyToSentence"/> on the existing sentence.
        /// </summary>
        /// <param name="universalQuantification">The <see cref="UniversalQuantification{TDomain, TElement}"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence{TDomain, TElement}"/>.</returns>
        public virtual Sentence<TDomain, TElement> ApplyToUniversalQuantification(UniversalQuantification<TDomain, TElement> universalQuantification)
        {
            var variable = ApplyToVariableDeclaration(universalQuantification.Variable);
            var sentence = ApplyToSentence(universalQuantification.Sentence);
            if (variable != universalQuantification.Variable || sentence != universalQuantification.Sentence)
            {
                return new UniversalQuantification<TDomain, TElement>(variable, sentence);
            }

            return universalQuantification;
        }

        /// <summary>
        /// Transforms a <see cref="Variable{TDomain, TElement}"/> instance, in the context of a variable declaration.
        /// The default implementation simply returns the passed value.
        /// </summary>
        /// <param name="variableDeclaration">The <see cref="Variable{TDomain, TElement}"/> instance to transform.</param>
        /// <returns>The transformed <see cref="Variable{TDomain, TElement}"/> declaration.</returns>
        public virtual Variable<TDomain, TElement> ApplyToVariableDeclaration(Variable<TDomain, TElement> variableDeclaration)
        {
            return variableDeclaration;
        }

        /// <summary>
        /// Transforms a <see cref="Term{TDomain, TElement}"/> instance.
        /// The default implementation simply invokes the Visit.. method appropriate to the type of the term.
        /// </summary>
        /// <param name="term">The term to visit.</param>
        /// <returns>The transformed term.</returns>
        public virtual Term<TDomain, TElement> ApplyToTerm(Term<TDomain, TElement> term)
        {
            return term switch
            {
                Constant<TDomain, TElement> constant => ApplyToConstant(constant),
                Variable<TDomain, TElement> variable => ApplyToVariable(variable),
                Function<TDomain, TElement> function => ApplyToFunction(function),
                _ => throw new ArgumentException()
            };
        }

        /// <summary>
        /// Transforms a constant.
        /// The default implementation simply returns the constant unchanged.
        /// </summary>
        /// <param name="term">The constant to visit.</param>
        /// <returns>The transformed term.</returns>
        public virtual Term<TDomain, TElement> ApplyToConstant(Constant<TDomain, TElement> constant)
        {
            return constant;
        }

        /// <summary>
        /// Transforms a variable instance.
        /// The default implementation simply returns the variable unchanged.
        /// <para/>
        /// Important: This method is only used to transform variable *references* in a sentence. For variable *declarations* that occur as part of a quantification sentence, the <see cref="ApplyToVariableDeclaration"/> method is used instead.
        /// This is because variable declarations must remain variable declarations under transformation. TODO: perhaps suggests that these should be two different types? Perhaps - going to follow LINQs lead for now, though..
        /// </summary>
        /// <param name="term">The variable to visit.</param>
        /// <returns>The transformed term.</returns>
        public virtual Term<TDomain, TElement> ApplyToVariable(Variable<TDomain, TElement> variable)
        {
            return variable;
        }

        /// <summary>
        /// Transforms a function.
        /// The default implementation returns an <see cref="Function{TDomain, TElement}"/> pointed at the same <see cref="MemberInfo"/> and with an argument list that is the result of calling <see cref="ApplyToTerm"/> on both of the existing arguments.
        /// </summary>
        /// <param name="function">The function to visit.</param>
        /// <returns>The transformed term.</returns>
        public virtual Term<TDomain, TElement> ApplyToFunction(Function<TDomain, TElement> function)
        {
            var arguments = function.Arguments.Select(a => ApplyToTerm(a)).ToList();

            if (arguments.Zip(function.Arguments, (x, y) => (x, y)).Any(t => t.x != t.y))
            {
                return new Function<TDomain, TElement>(function.Member, arguments);
            }

            return function;
        }
    }
}
