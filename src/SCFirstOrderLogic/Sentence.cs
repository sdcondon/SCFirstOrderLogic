using SCFirstOrderLogic.SentenceManipulation;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a sentence of first order logic.
    /// <para/>
    /// NB: There are no "intuitive" operator overloads (&amp; for conjunctions, | for disjunctions etc) here,
    /// to keep the core sentence classes as lean and mean as possible. C# / FoL syntax mapping
    /// is better achieved with LINQ - see the types in the LanguageIntegration namespace. Or, if you're
    /// really desperate for sentence operators, also see the <see cref="OperableSentenceFactory"/> class, which
    /// is something of a compromise.
    /// <para/>
    /// Also, note that there's no validation method (for e.g. catching of variable declaration the
    /// symbol of which is equal to one already in scope, or of references to non-declared variables).
    /// We'll do this during the normalisation process if and when we want it. Again, this is to keep the
    /// core classes as dumb (and thus flexible) as possible.
    /// </summary>
    public abstract class Sentence
    {
        // TODO.. proper visitor pattern probably useful for transformations and others..
        ////public abstract T Accept<T>(ISentenceVisitor<T> visitor);
        ////public abstract void Accept(ISentenceVisitor visitor);

        /// <inheritdoc />
        public override string ToString() => SentenceFormatter.Print(this); // Just for now..
    }
}
