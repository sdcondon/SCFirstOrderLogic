namespace SCFirstOrderLogic.SentenceManipulation
{
    /// <summary>
    /// <para>
    /// Interface for visitors of <see cref="Sentence"/> instances.
    /// </para>
    /// <para>
    /// NB: This interface (in comparison to the non-generic <see cref="ISentenceVisitor"/>) is specifically for visitors that facilitate
    /// state accumulation outside of the visitor instance itself - state that is passed to the visitor by reference.
    /// </para>
    /// </summary>
    /// <typeparam name="TState">The type of state that this visitor works with.</typeparam>
    public interface ISentenceVisitorR<TState>
    {
        /// <summary>
        /// Visits a <see cref="Conjunction"/> instance.
        /// </summary>
        /// <param name="conjunction">The conjunction instance to visit.</param>
        /// <param name="state">A reference to the state of this visitation.</param>
        void Visit(Conjunction conjunction, ref TState state);

        /// <summary>
        /// Visits a <see cref="Disjunction"/> instance.
        /// </summary>
        /// <param name="disjunction">The <see cref="Disjunction"/> instance to visit.</param>
        /// <param name="state">A reference to the state of this visitation.</param>
        void Visit(Disjunction disjunction, ref TState state);

        /// <summary>
        /// Visits a <see cref="Equivalence"/> instance. 
        /// </summary>
        /// <param name="equivalence">The <see cref="Equivalence"/> instance to visit.</param>
        /// <param name="state">A reference to the state of this visitation.</param>
        void Visit(Equivalence equivalence, ref TState state);

        /// <summary>
        /// Visits an <see cref="ExistentialQuantification"/> instance. 
        /// </summary>
        /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to visit.</param>
        /// <param name="state">A reference to the state of this visitation.</param>
        void Visit(ExistentialQuantification existentialQuantification, ref TState state);

        /// <summary>
        /// Visits an <see cref="Implication"/> instance. 
        /// </summary>
        /// <param name="implication">The <see cref="Implication"/> instance to visit.</param>
        /// <param name="state">A reference to the state of this visitation.</param>
        void Visit(Implication implication, ref TState state);

        /// <summary>
        /// Visits a <see cref="Negation"/> instance. 
        /// </summary>
        /// <param name="negation">The <see cref="Negation"/> instance to visit.</param>
        /// <param name="state">A reference to the state of this visitation.</param>
        void Visit(Negation negation, ref TState state);

        /// <summary>
        /// Visits a <see cref="Predicate"/> instance. 
        /// </summary>
        /// <param name="predicate">The <see cref="Predicate"/> instance to visit.</param>
        /// <param name="state">A reference to the state of this visitation.</param>
        void Visit(Predicate predicate, ref TState state);

        /// <summary>
        /// Visits a <see cref="UniversalQuantification"/> instance. 
        /// </summary>
        /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to visit.</param>
        /// <param name="state">A reference to the state of this visitation.</param>
        void Visit(UniversalQuantification universalQuantification, ref TState state);
    }
}
