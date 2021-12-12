namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a existential quantification sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>∃ {variable}, {sentence}</code>
    /// </summary>
    public class ExistentialQuantification : Quantification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExistentialQuantification"/> class.
        /// </summary>
        /// <param name="variable">The variable declared by this quantification.</param>
        /// <param name="sentence">The sentence that this quantification applies to.</param>
        public ExistentialQuantification(VariableDeclaration variable, Sentence sentence)
            : base(variable, sentence)
        {
        }
    }
}
