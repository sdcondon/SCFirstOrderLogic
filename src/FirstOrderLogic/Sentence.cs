using LinqToKB.FirstOrderLogic.SentenceManipulation;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a sentence of first order logic.
    /// </summary>
    public abstract class Sentence
    {
        // TODO.. proper visitor pattern probably useful for transformations and others..
        ////public abstract T Accept<T>(ISentenceVisitor<T> visitor);

        public override string ToString() => SentencePrinter.Print(this); // Just for now..
    }
}
