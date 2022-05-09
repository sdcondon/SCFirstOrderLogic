using System;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation
{
    /// <summary>
    /// Base class for transformations of <see cref="Sentence"/> instances.
    /// </summary>
    public abstract class SentenceTransformation
    {
        /// <summary>
        /// Applies this transformation to a <see cref="Sentence"/> instance.
        /// The default implementation simply invokes the Apply method appropriate to the actual type of the sentence.
        /// </summary>
        /// <param name="sentence">The sentence to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        public virtual Sentence ApplyTo(Sentence sentence)
        {
            // TODO-PERFORMANCE: Using "proper" visitor pattern will be (ever so slightly) faster than this,
            // as well as adhering better to the OCP - decide if its worth the extra complexity.
            return sentence switch
            {
                Conjunction conjunction => ApplyTo(conjunction),
                Disjunction disjunction => ApplyTo(disjunction),
                Equivalence equivalence => ApplyTo(equivalence),
                Implication implication => ApplyTo(implication),
                Negation negation => ApplyTo(negation),
                Predicate predicate => ApplyTo(predicate),
                Quantification quantification => ApplyTo(quantification),
                _ => throw new ArgumentException("Unsupported sentence type")
            };
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Conjunction"/> instance.
        /// The default implementation returns a <see cref="Conjunction"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="conjunction">The conjunction instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        protected virtual Sentence ApplyTo(Conjunction conjunction)
        {
            var left = ApplyTo(conjunction.Left);
            var right = ApplyTo(conjunction.Right);
            if (left != conjunction.Left || right != conjunction.Right)
            {
                return new Conjunction(left, right);
            }

            return conjunction;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Disjunction"/> instance.
        /// The default implementation returns a <see cref="Disjunction"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="conjunction">The <see cref="Disjunction"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        protected virtual Sentence ApplyTo(Disjunction disjunction)
        {
            var left = ApplyTo(disjunction.Left);
            var right = ApplyTo(disjunction.Right);
            if (left != disjunction.Left || right != disjunction.Right)
            {
                return new Disjunction(left, right);
            }

            return disjunction;
        }

        /// <summary>
        /// Applies this transformation to an <see cref="Equivalence"/> instance. 
        /// The default implementation returns an <see cref="Equivalence"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="equivalence">The <see cref="Equivalence"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        protected virtual Sentence ApplyTo(Equivalence equivalence)
        {
            var equivalent1 = ApplyTo(equivalence.Left);
            var equivalent2 = ApplyTo(equivalence.Right);
            if (equivalent1 != equivalence.Left || equivalent2 != equivalence.Right)
            {
                return new Equivalence(equivalent1, equivalent2);
            }

            return equivalence;
        }

        /// <summary>
        /// Applies this transformation to an <see cref="ExistentialQuantification"/> instance. 
        /// The default implementation returns an <see cref="ExistentialQuantification"/> for which the variable declaration is the result of <see cref="ApplyTo"/> on the existing declaration, and the sentence is the result of <see cref="ApplyTo"/> on the existing sentence.
        /// </summary>
        /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        protected virtual Sentence ApplyTo(ExistentialQuantification existentialQuantification)
        {
            var variable = ApplyTo(existentialQuantification.Variable);
            var sentence = ApplyTo(existentialQuantification.Sentence);
            if (variable != existentialQuantification.Variable || sentence != existentialQuantification.Sentence)
            {
                return new ExistentialQuantification(variable, sentence);
            }

            return existentialQuantification;
        }

        /// <summary>
        /// Applies this transformation to an <see cref="Implication"/> instance. 
        /// The default implementation returns an <see cref="Implication"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="implication">The <see cref="Implication"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        protected virtual Sentence ApplyTo(Implication implication)
        {
            var antecedent = ApplyTo(implication.Antecedent);
            var consequent = ApplyTo(implication.Consequent);

            if (antecedent != implication.Antecedent || consequent != implication.Consequent)
            {
                return new Implication(antecedent, consequent);
            }

            return implication;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Predicate"/> instance. 
        /// The default implementation returns a <see cref="Predicate"/> with the same Symbol and with an argument list that is the result of calling <see cref="ApplyTo"/> on all of the existing arguments.
        /// </summary>
        /// <param name="predicate">The <see cref="Predicate"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        protected virtual Sentence ApplyTo(Predicate predicate)
        {
            var arguments = predicate.Arguments.Select(a => ApplyTo(a)).ToList();

            if (arguments.Zip(predicate.Arguments, (x, y) => (x, y)).Any(t => t.x != t.y))
            {
                return new Predicate(predicate.Symbol, arguments);
            }

            return predicate;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Negation"/> instance. 
        /// The default implementation returns a <see cref="Negation"/> of the result of calling <see cref="ApplyTo"/> on the current sub-sentence.
        /// </summary>
        /// <param name="negation">The <see cref="Negation"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        protected virtual Sentence ApplyTo(Negation negation)
        {
            var sentence = ApplyTo(negation.Sentence);

            if (sentence != negation.Sentence)
            {
                return new Negation(sentence);
            }

            return negation;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Quantification"/> instance. 
        /// The default implementation simply invokes the <see cref="ApplyTo"/> method appropriate to the type of the quantification.
        /// </summary>
        /// <param name="quantification">The <see cref="Quantification"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        protected virtual Sentence ApplyTo(Quantification quantification)
        {
            return quantification switch
            {
                ExistentialQuantification existentialQuantification => ApplyTo(existentialQuantification),
                UniversalQuantification universalQuantification => ApplyTo(universalQuantification),
                _ => throw new ArgumentException($"Unsupported Quantification type '{quantification.GetType()}'")
            };
        }

        /// <summary>
        /// Applies this transformation to a <see cref="UniversalQuantification"/> instance. 
        /// The default implementation returns a <see cref="UniversalQuantification"/> for which the variable declaration is the result of <see cref="ApplyTo"/> on the existing declaration, and the sentence is the result of <see cref="ApplyTo"/> on the existing sentence.
        /// </summary>
        /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        protected virtual Sentence ApplyTo(UniversalQuantification universalQuantification)
        {
            var variable = ApplyTo(universalQuantification.Variable);
            var sentence = ApplyTo(universalQuantification.Sentence);
            if (variable != universalQuantification.Variable || sentence != universalQuantification.Sentence)
            {
                return new UniversalQuantification(variable, sentence);
            }

            return universalQuantification;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Term"/> instance.
        /// The default implementation simply invokes the <see cref="ApplyTo"/> method appropriate to the type of the term.
        /// </summary>
        /// <param name="term">The term to visit.</param>
        /// <returns>The transformed term.</returns>
        public virtual Term ApplyTo(Term term)
        {
            return term switch
            {
                Constant constant => ApplyTo(constant),
                VariableReference variable => ApplyTo(variable),
                Function function => ApplyTo(function),
                _ => throw new ArgumentException($"Unsupported Term type '{term.GetType()}'")
            };
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Constant"/> instance.
        /// The default implementation simply returns the constant unchanged.
        /// </summary>
        /// <param name="term">The constant to visit.</param>
        /// <returns>The transformed term.</returns>
        protected virtual Term ApplyTo(Constant constant)
        {
            return constant;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="VariableReference"/> instance.
        /// The default implementation returns a <see cref="VariableReference"/> of the result of calling <see cref="ApplyTo"/> on the current declaration.
        /// </summary>
        /// <param name="term">The variable to visit.</param>
        /// <returns>The transformed term.</returns>
        protected virtual Term ApplyTo(VariableReference variable)
        {
            var variableDeclaration = ApplyTo(variable.Declaration);
            if (variableDeclaration != variable.Declaration)
            {
                return new VariableReference(variableDeclaration);
            }

            return variable;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Function"/> instance.
        /// The default implementation returns a <see cref="Function"/> with the same Symbol and with an argument list that is the result of calling <see cref="ApplyTo"/> on each of the existing arguments.
        /// </summary>
        /// <param name="function">The function to visit.</param>
        /// <returns>The transformed term.</returns>
        protected virtual Term ApplyTo(Function function)
        {
            var arguments = function.Arguments.Select(a => ApplyTo(a)).ToList();

            if (arguments.Zip(function.Arguments, (x, y) => (x, y)).Any(t => t.x != t.y))
            {
                return new Function(function.Symbol, arguments);
            }

            return function;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="VariableDeclaration"/> instance.
        /// The default implementation simply returns the passed value.
        /// </summary>
        /// <param name="variableDeclaration">The <see cref="VariableDeclaration"/> instance to transform.</param>
        /// <returns>The transformed <see cref="VariableReference"/> declaration.</returns>
        protected virtual VariableDeclaration ApplyTo(VariableDeclaration variableDeclaration)
        {
            return variableDeclaration;
        }
    }
}
