using SCFirstOrderLogic.SentenceFormatting;

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

        /// <summary>
        /// Returns a string that represents the current object.
        /// <para/>
        /// NB: The implementation of this override creates a <see cref="SentenceFormatter"/> object and uses it to print the sentence.
        /// If the sentence has been normalised, its worth noting that this will not guarantee unique labelling of any normalisation terms
        /// (standardised variables or Skolem functions) across a set of sentences, or provide any choice as to the sets of labels used for
        /// normalisation terms. If you want either of these things, instantiate your own <see cref="SentenceFormatter"/> instance.
        /// <para/>
        /// Aside: I have wondered if it would perhaps better to just enforce explicit SentenceFormatter use. That would however be a PITA if you
        /// just want to print out your nice, simple sentence. It may even be non-normalised - in which case you definitely won't want to be
        /// messing around with sets of labels. So its important that this stays - to avoid a "barrier to entry" for the library.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => new SentenceFormatter().Print(this);
    }
}
