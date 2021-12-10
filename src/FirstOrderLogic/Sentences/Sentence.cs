using LinqToKB.FirstOrderLogic.Sentences.Manipulation;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a sentence of first order logic.
    /// </summary>
    public abstract partial class Sentence
    {
        // TODO.. proper visitor pattern probably useful for transformations and others..
        ////public abstract T Accept<T>(ISentenceVisitor<T> visitor);

        public override string ToString() => SentencePrinter.Print(this); // Just for now..
    }
}
