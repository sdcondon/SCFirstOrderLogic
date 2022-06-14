using System;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation
{
    /// <summary>
    /// Alternative version of <see cref="SentenceTransformation"/> that is a <see cref="ISentenceVisitor{Sentence}"/> and a <see cref="ITermVisitor{Term}"/>.
    /// </summary>
    public class SentenceTransformation_IsSentenceVisitor : ISentenceVisitor<Sentence>, ITermVisitor<Term>
    {
        /// <summary>
        /// Applies this transformation to a <see cref="Conjunction"/> instance.
        /// The default implementation returns a <see cref="Conjunction"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="conjunction">The conjunction instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        public virtual void Visit(Conjunction conjunction, ref Sentence transformedSentence)
        {
            Sentence left = null;
            Sentence right = null;
            conjunction.Left.Accept(this, ref left);
            conjunction.Right.Accept(this, ref right);
            if (left != conjunction.Left || right != conjunction.Right)
            {
                transformedSentence = new Conjunction(left, right);
            }
            else
            {
                transformedSentence = conjunction;
            }
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Disjunction"/> instance.
        /// The default implementation returns a <see cref="Disjunction"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="disjunction">The <see cref="Disjunction"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        public virtual void Visit(Disjunction disjunction, ref Sentence transformedSentence)
        {
            Sentence left = null;
            Sentence right = null;
            disjunction.Left.Accept(this, ref left);
            disjunction.Right.Accept(this, ref right);
            if (left != disjunction.Left || right != disjunction.Right)
            {
                transformedSentence = new Disjunction(left, right);
            }
            else
            {
                transformedSentence = disjunction;
            }
        }

        /// <summary>
        /// Applies this transformation to an <see cref="Equivalence"/> instance. 
        /// The default implementation returns an <see cref="Equivalence"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="equivalence">The <see cref="Equivalence"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        public virtual void Visit(Equivalence equivalence, ref Sentence transformedSentence)
        {
            Sentence left = null;
            Sentence right = null;
            equivalence.Left.Accept(this, ref left);
            equivalence.Right.Accept(this, ref right);
            if (left != equivalence.Left || right != equivalence.Right)
            {
                transformedSentence = new Equivalence(left, right);
            }
            else
            {
                transformedSentence = equivalence;
            }
        }

        /// <summary>
        /// Applies this transformation to an <see cref="ExistentialQuantification"/> instance. 
        /// The default implementation returns an <see cref="ExistentialQuantification"/> for which the variable declaration is the result of <see cref="ApplyTo"/> on the existing declaration, and the sentence is the result of <see cref="ApplyTo"/> on the existing sentence.
        /// </summary>
        /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        public virtual void Visit(ExistentialQuantification existentialQuantification, ref Sentence transformedSentence)
        {
            VariableDeclaration variableDeclaration = null;
            Sentence sentence = null;
            Visit(existentialQuantification.Variable, ref variableDeclaration);
            existentialQuantification.Sentence.Accept(this, ref sentence);
            if (variableDeclaration != existentialQuantification.Variable || sentence != existentialQuantification.Sentence)
            {
                transformedSentence = new ExistentialQuantification(variableDeclaration, sentence);
            }
            else
            {
                transformedSentence = existentialQuantification;
            }
        }

        /// <summary>
        /// Applies this transformation to an <see cref="Implication"/> instance. 
        /// The default implementation returns an <see cref="Implication"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-sentences.
        /// </summary>
        /// <param name="implication">The <see cref="Implication"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        public virtual void Visit(Implication implication, ref Sentence transformedSentence)
        {
            Sentence antecedent = null;
            Sentence consequent = null;
            implication.Antecedent.Accept(this, ref antecedent);
            implication.Consequent.Accept(this, ref consequent);

            if (antecedent != implication.Antecedent || consequent != implication.Consequent)
            {
                transformedSentence = new Implication(antecedent, consequent);
            }
            else
            {
                transformedSentence = implication;
            }
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Predicate"/> instance. 
        /// The default implementation returns a <see cref="Predicate"/> with the same Symbol and with an argument list that is the result of calling <see cref="ApplyTo"/> on all of the existing arguments.
        /// </summary>
        /// <param name="predicate">The <see cref="Predicate"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        public virtual void Visit(Predicate predicate, ref Sentence transformedSentence)
        {
            var arguments = predicate.Arguments.Select(a =>
            {
                Term transformedArgument = null;
                a.Accept(this, ref transformedArgument);
                return transformedArgument;
            }).ToList();

            if (arguments.Zip(predicate.Arguments, (x, y) => (x, y)).Any(t => t.x != t.y))
            {
                transformedSentence = new Predicate(predicate.Symbol, arguments);
            }
            else
            {
                transformedSentence = predicate;
            }
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Negation"/> instance. 
        /// The default implementation returns a <see cref="Negation"/> of the result of calling <see cref="ApplyTo"/> on the current sub-sentence.
        /// </summary>
        /// <param name="negation">The <see cref="Negation"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        public virtual void Visit(Negation negation, ref Sentence transformedSentence)
        {
            Sentence sentence = null;
            negation.Sentence.Accept(this, ref sentence);

            if (sentence != negation.Sentence)
            {
                transformedSentence = new Negation(sentence);
            }
            else
            {
                transformedSentence = negation;
            }
        }

        /// <summary>
        /// Applies this transformation to a <see cref="UniversalQuantification"/> instance. 
        /// The default implementation returns a <see cref="UniversalQuantification"/> for which the variable declaration is the result of <see cref="ApplyTo"/> on the existing declaration, and the sentence is the result of <see cref="ApplyTo"/> on the existing sentence.
        /// </summary>
        /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        public virtual void Visit(UniversalQuantification universalQuantification, ref Sentence transformedSentence)
        {
            VariableDeclaration variableDeclaration = null;
            Sentence sentence = null;
            Visit(universalQuantification.Variable, ref variableDeclaration);
            universalQuantification.Sentence.Accept(this, ref sentence);
            if (variableDeclaration != universalQuantification.Variable || sentence != universalQuantification.Sentence)
            {
                transformedSentence = new UniversalQuantification(variableDeclaration, sentence);
            }
            else
            {
                transformedSentence = universalQuantification;
            }
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Constant"/> instance.
        /// The default implementation simply returns the constant unchanged.
        /// </summary>
        /// <param name="term">The constant to visit.</param>
        /// <returns>The transformed term.</returns>
        public virtual void Visit(Constant constant, ref Term transformedTerm)
        {
            transformedTerm = constant;
        }

        /// <summary>
        /// Applies this transformation to a <see cref="VariableReference"/> instance.
        /// The default implementation returns a <see cref="VariableReference"/> of the result of calling <see cref="ApplyTo"/> on the current declaration.
        /// </summary>
        /// <param name="term">The variable to visit.</param>
        /// <returns>The transformed term.</returns>
        public virtual void Visit(VariableReference variable, ref Term transformedTerm)
        {
            VariableDeclaration variableDeclaration = null;
            Visit(variable.Declaration, ref variableDeclaration);

            if (variableDeclaration != variable.Declaration)
            {
                transformedTerm = new VariableReference(variableDeclaration);
            }
            else
            {
                transformedTerm = variable;
            }
        }

        /// <summary>
        /// Applies this transformation to a <see cref="Function"/> instance.
        /// The default implementation returns a <see cref="Function"/> with the same Symbol and with an argument list that is the result of calling <see cref="ApplyTo"/> on each of the existing arguments.
        /// </summary>
        /// <param name="function">The function to visit.</param>
        /// <returns>The transformed term.</returns>
        public virtual void Visit(Function function, ref Term transformedTerm)
        {
            var arguments = function.Arguments.Select(a =>
            {
                Term transformedArgument = null;
                a.Accept(this, ref transformedArgument);
                return transformedArgument;
            }).ToList();

            if (arguments.Zip(function.Arguments, (x, y) => (x, y)).Any(t => t.x != t.y))
            {
                transformedTerm = new Function(function.Symbol, arguments);
            }
            else
            {
                transformedTerm = function;
            }
        }

        /// <summary>
        /// Applies this transformation to a <see cref="VariableDeclaration"/> instance.
        /// The default implementation simply returns the passed value.
        /// </summary>
        /// <param name="variableDeclaration">The <see cref="VariableDeclaration"/> instance to transform.</param>
        /// <returns>The transformed <see cref="VariableReference"/> declaration.</returns>
        public virtual void Visit(VariableDeclaration variableDeclaration, ref VariableDeclaration transformedVariableDeclaration)
        {
            transformedVariableDeclaration = variableDeclaration;
        }
    }
}
