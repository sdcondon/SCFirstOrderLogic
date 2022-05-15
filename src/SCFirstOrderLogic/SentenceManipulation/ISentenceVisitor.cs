#if FALSE
using System;

namespace SCFirstOrderLogic.SentenceManipulation
{
    /// <summary>
    /// Useful base class for visitors of <see cref="Sentence"/> instances.
    /// </summary>
    public interface ISentenceVisitor
    {
        /// <summary>
        /// Visits a <see cref="Sentence"/> instance.
        /// </summary>
        /// <param name="sentence">The sentence to visit.</param>
        public void Visit(Sentence sentence);

        /// <summary>
        /// Visits a <see cref="Conjunction"/> instance.
        /// </summary>
        /// <param name="conjunction">The conjunction instance to visit.</param>
        protected void Visit(Conjunction conjunction);

        /// <summary>
        /// Visits a <see cref="Disjunction"/> instance.
        /// </summary>
        /// <param name="conjunction">The <see cref="Disjunction"/> instance to visit.</param>
        protected void Visit(Disjunction disjunction);

        /// <summary>
        /// Visits an <see cref="Equivalence"/> instance. 
        /// </summary>
        /// <param name="equivalence">The <see cref="Equivalence"/> instance to visit.</param>
        protected void Visit(Equivalence equivalence);

        /// <summary>
        /// Visits an <see cref="ExistentialQuantification"/> instance. 
        /// </summary>
        /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to visit.</param>
        protected void Visit(ExistentialQuantification existentialQuantification);

        /// <summary>
        /// Visits an <see cref="Implication"/> instance. 
        /// </summary>
        /// <param name="implication">The <see cref="Implication"/> instance to visit.</param>
        protected void Visit(Implication implication);

        /// <summary>
        /// Visits a <see cref="Predicate"/> instance. 
        /// </summary>
        /// <param name="predicate">The <see cref="Predicate"/> instance to visit.</param>
        protected void Visit(Predicate predicate);

        /// <summary>
        /// Visits a <see cref="Negation"/> instance. 
        /// </summary>
        /// <param name="negation">The <see cref="Negation"/> instance to visit.</param>
        protected void Visit(Negation negation);

        /// <summary>
        /// Visits a <see cref="Quantification"/> instance. 
        /// </summary>
        /// <param name="quantification">The <see cref="Quantification"/> instance to visit.</param>
        protected void Visit(Quantification quantification);

        /// <summary>
        /// Visits a <see cref="UniversalQuantification"/> instance. 
        /// </summary>
        /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to visit.</param>
        protected void Visit(UniversalQuantification universalQuantification);

        /// <summary>
        /// Visits a <see cref="Term"/> instance.
        /// </summary>
        /// <param name="term">The term to visit.</param>
        public void Visit(Term term);

        /// <summary>
        /// Visits a <see cref="Constant"/> instance.
        /// </summary>
        /// <param name="term">The constant to visit.</param>
        protected void Visit(Constant constant);

        /// <summary>
        /// Visits a <see cref="VariableReference"/> instance.
        /// </summary>
        /// <param name="term">The variable to visit.</param>
        protected void Visit(VariableReference variable);

        /// <summary>
        /// Visits a <see cref="Function"/> instance.
        /// </summary>
        /// <param name="function">The function to visit.</param>
        protected void Visit(Function function);

        /// <summary>
        /// Visits a <see cref="VariableDeclaration"/> instance.
        /// The default implementation doesn't do anything.
        /// </summary>
        /// <param name="variableDeclaration">The <see cref="VariableDeclaration"/> instance to visit.</param>
        protected void Visit(VariableDeclaration variableDeclaration);
    }
}
#endif