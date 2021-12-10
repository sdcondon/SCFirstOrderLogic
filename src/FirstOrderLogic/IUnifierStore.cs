using LinqToKB.FirstOrderLogic.Sentences;
using System.Collections.Generic;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Interface for types that facilitate lookup of all of the unifiers (i.e. sets of variable assignments) such
    /// that a given query unifies (i.e. matches, once the variable assignents are made) with some known sentence.
    /// <para/>
    /// This is a process that underpins first-order logic inference algorithms. We have an abstraction for it because
    /// stores of different sizes and natures will have different requirements for how the known sentences are stored
    /// (w.r.t. indexing approach, primary vs secondary storage, and so on) in order to be acceptably performant.
    /// <para/>
    /// TBD: given the potential for secondary storage here, should these methods be async?
    /// </summary>
    public interface IUnifierStore
    {
        /// <summary>
        /// Registers a sentence with the store.
        /// </summary>
        /// <param name="sentence">The sentence to store.</param>
        void Store(Sentence sentence);

        /// <summary>
        /// Returns all unifiers such that the query q unifies with some sentence in the store.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDictionary<Variable, Constant>> Fetch(Sentence sentence);
    }
}
