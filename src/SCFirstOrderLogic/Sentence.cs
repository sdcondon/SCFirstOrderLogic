using SCFirstOrderLogic.SentenceManipulation;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a sentence of first order logic.
    /// </summary>
    public abstract partial class Sentence
    {
        // TODO.. proper visitor pattern probably useful for transformations and others..
        ////public abstract T Accept<T>(ISentenceVisitor<T> visitor);

        // Validation of a sentence would be nice - though the only problems I can think of
        // is a variable declaration the symbol of which is equal to one already in scope,
        // or a reference to a non-declared variable (on the assumption that we don't want to
        // assume universal quantification for these).
        // Perhaps better implemented as a visitor..
        ////public virtual bool Validate()

        //// NB: No operator overloads (&& for conjunctions, || for disjunctions etc) here to keep
        //// the core sentence classes as lean and mean as possible. C# / FoL syntax mapping
        //// better achieved with LINQ - see LanguageIntegration namespace.

        /// <inheritdoc />
        public override string ToString() => SentenceFormatter.Print(this); // Just for now..
    }
}
