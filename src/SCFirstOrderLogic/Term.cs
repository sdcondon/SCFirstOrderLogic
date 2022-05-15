using SCFirstOrderLogic.SentenceFormatting;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a term within a sentence of first order logic.
    /// </summary>
    public abstract class Term
    {
        // TODO: visitor pattern, perhaps..
        ////public abstract T Accept<T>(ISentenceVisitor<T> visitor);
        
        /// <summary>
        /// Gets a value indicating whether the term is a ground term - that is, that it contains no variables.
        /// </summary>
        public abstract bool IsGroundTerm { get; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// <para/>
        /// NB: The implementation of this override creates a <see cref="SentenceFormatter"/> object and uses it to print the term.
        /// If the term is from a normalised sentence, its worth noting that this will not guarantee unique labelling of any normalisation terms
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
