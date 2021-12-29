namespace SCFirstOrderLogic.KnowledgeBases
{
    /// <summary>
    /// A store of knowledge expressed as sentences of first-order logic.
    /// </summary>
    public interface IKnowledgeBase
    {
        /// <summary>
        /// Tells the knowledge base that a given sentence can be assumed to hold true when answering queries.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        public void Tell(Sentence sentence);

        /// <summary>
        /// Asks the knowledge base if a given sentence necessarily holds true, given what it knows.
        /// </summary>
        /// <param name="query">The sentence to ask about.</param>
        /// <returns>True if the sentence is known to be true, false if it is known to be false or cannot be determined.</returns>
        public bool Ask(Sentence query);

        //// NB: No AskVars just yet..
    }
}
