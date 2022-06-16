using System;

namespace SCFirstOrderLogic.SentenceManipulation
{
    /// <summary>
    /// Base class for recursive visitors of <see cref="Sentence"/> instances.
    /// </summary>
    public abstract class RecursiveSentenceVisitor_WithTypeSwitch : ISentenceVisitor, ITermVisitor
    {
        /// <summary>
        /// Visits a <see cref="Sentence"/> instance.
        /// </summary>
        /// <param name="sentence">The sentence to visit.</param>
        /// <returns>The transformed <see cref="Sentence"/>.</returns>
        public virtual void Visit(Sentence sentence)
        {
            switch (sentence)
            {
                case Conjunction conjunction:
                    Visit(conjunction);
                    break;
                case Disjunction disjunction:
                    Visit(disjunction);
                    break;
                case Equivalence equivalence:
                    Visit(equivalence);
                    break;
                case Implication implication:
                    Visit(implication);
                    break;
                case Negation negation:
                    Visit(negation);
                    break;
                case Predicate predicate:
                    Visit(predicate);
                    break;
                case Quantification quantification:
                    Visit(quantification);
                    break;
                default:
                    throw new ArgumentException($"Unsupported sentence type '{sentence.GetType()}'", nameof(sentence));
            };
        }

        /// <summary>
        /// Visits a <see cref="Conjunction"/> instance.
        /// The default implementation just visits both of the sub-sentences.
        /// </summary>
        /// <param name="conjunction">The conjunction instance to visit.</param>
        public virtual void Visit(Conjunction conjunction)
        {
            Visit(conjunction.Left);
            Visit(conjunction.Right);
        }

        /// <summary>
        /// Visits a <see cref="Disjunction"/> instance.
        /// The default implementation just visits the both of the sub-sentences.
        /// </summary>
        /// <param name="disjunction">The <see cref="Disjunction"/> instance to visit.</param>
        public virtual void Visit(Disjunction disjunction)
        {
            Visit(disjunction.Left);
            Visit(disjunction.Right);
        }

        /// <summary>
        /// Visits an <see cref="Equivalence"/> instance. 
        /// The default implementation just visits both of the sub-sentences.
        /// </summary>
        /// <param name="equivalence">The <see cref="Equivalence"/> instance to visit.</param>
        public virtual void Visit(Equivalence equivalence)
        {
            Visit(equivalence.Left);
            Visit(equivalence.Right);
        }

        /// <summary>
        /// Visits an <see cref="ExistentialQuantification"/> instance. 
        /// The default implementation just visits the variable declaration and sentence.
        /// </summary>
        /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to visit.</param>
        public virtual void Visit(ExistentialQuantification existentialQuantification)
        {
            Visit(existentialQuantification.Variable);
            Visit(existentialQuantification.Sentence);
        }

        /// <summary>
        /// Visits an <see cref="Implication"/> instance. 
        /// The default implementation just visits both of the sub-sentences.
        /// </summary>
        /// <param name="implication">The <see cref="Implication"/> instance to visit.</param>
        public virtual void Visit(Implication implication)
        {
            Visit(implication.Antecedent);
            Visit(implication.Consequent);
        }

        /// <summary>
        /// Visits a <see cref="Predicate"/> instance. 
        /// The default implementation just visits each of the arguments.
        /// </summary>
        /// <param name="predicate">The <see cref="Predicate"/> instance to visit.</param>
        public virtual void Visit(Predicate predicate)
        {
            foreach (var argument in predicate.Arguments)
            {
                Visit(argument);
            }
        }

        /// <summary>
        /// Visits a <see cref="Negation"/> instance. 
        /// The default implementation just visits the sub-sentence.
        /// </summary>
        /// <param name="negation">The <see cref="Negation"/> instance to visit.</param>
        public virtual void Visit(Negation negation)
        {
            Visit(negation.Sentence);
        }

        /// <summary>
        /// Visits a <see cref="Quantification"/> instance. 
        /// The default implementation simply invokes the Visit method appropriate to the type of the quantification.
        /// </summary>
        /// <param name="quantification">The <see cref="Quantification"/> instance to visit.</param>
        public virtual void Visit(Quantification quantification)
        {
            switch (quantification)
            {
                case ExistentialQuantification existentialQuantification:
                    Visit(existentialQuantification);
                    break;
                case UniversalQuantification universalQuantification:
                    Visit(universalQuantification);
                    break;
                default:
                    throw new ArgumentException($"Unsupported Quantification type '{quantification.GetType()}'", nameof(quantification));
            }
        }

        /// <summary>
        /// Visits a <see cref="UniversalQuantification"/> instance. 
        /// The default implementation just visits the variable declaration and sentence.
        /// </summary>
        /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to visit.</param>
        public virtual void Visit(UniversalQuantification universalQuantification)
        {
            Visit(universalQuantification.Variable);
            Visit(universalQuantification.Sentence);
        }

        /// <summary>
        /// Visits a <see cref="Term"/> instance.
        /// The default implementation simply invokes the Visit method appropriate to the type of the term.
        /// </summary>
        /// <param name="term">The term to visit.</param>
        public virtual void Visit(Term term)
        {
            switch (term)
            {
                case Constant constant:
                    Visit(constant);
                    break;
                case VariableReference variable:
                    Visit(variable);
                    break;
                case Function function:
                    Visit(function);
                    break;
                default:
                    throw new ArgumentException($"Unsupported Term type '{term.GetType()}'", nameof(term));
            }
        }

        /// <summary>
        /// Visits a <see cref="Constant"/> instance.
        /// The default implementation doesn't do anything.
        /// </summary>
        /// <param name="term">The constant to visit.</param>
        public virtual void Visit(Constant constant)
        {
        }

        /// <summary>
        /// Visits a <see cref="VariableReference"/> instance.
        /// The default implementation just visits the variable declaration.
        /// </summary>
        /// <param name="variable">The variable reference to visit.</param>
        public virtual void Visit(VariableReference variable)
        {
            Visit(variable.Declaration);
        }

        /// <summary>
        /// Visits a <see cref="Function"/> instance.
        /// The default implementation just visits each of the arguments.
        /// </summary>
        /// <param name="function">The function to visit.</param>
        public virtual void Visit(Function function)
        {
            foreach (var argument in function.Arguments)
            {
                Visit(argument);
            }
        }

        /// <summary>
        /// Visits a <see cref="VariableDeclaration"/> instance.
        /// The default implementation doesn't do anything.
        /// </summary>
        /// <param name="variableDeclaration">The <see cref="VariableDeclaration"/> instance to visit.</param>
        public virtual void Visit(VariableDeclaration variableDeclaration)
        {
        }
    }
}
