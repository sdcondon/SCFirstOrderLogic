using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a sentence of first order logic.
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public abstract class Sentence<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        ////protected FOLSentence(LambdaExpression lambda) => Lambda = lambda;

        // TODO-FEATURE: Ultimately might be useful for verification - but can add this later..
        // NB: Not Expression<Predicate<TDomain>> since sub-sentences will probably also include
        // variables from quantifiers.
        // TODO-USABILITY: One way to introduce stronger type-safety here would be to add a (possibly
        // empty variable container - e.g. Expression<Predicate<TDomain, VariableContainer<TElement>>>.
        // Would need to decide if the extra complexity is worth it.
        ////public LambdaExpression Lambda { get; }

        // Internally (i.e. for sub-sentences), lambda is not necessarily a single-arg predicate expression since we may have additional parameters
        // that are the variables defined by quantifiers. Hence this is separate to Sentence's public method.
        internal static bool TryCreate(LambdaExpression lambda, out Sentence<TDomain, TElement> sentence)
        {
            // NB: A different formulation might have created abstract ComplexSentence and AtomicSentence subclasses of sentence
            // with their own internal TryCreate methods. Until there is a reason to make the distinction though, we don't bother
            // adding that complexity here..

            // TODO-USABILITY: might be nice to return more info than just "false" - but can't rely on exceptions due to the way this works.
            return
                // Complex sentences:
                Negation<TDomain, TElement>.TryCreate(lambda, out sentence)
                || Conjunction<TDomain, TElement>.TryCreate(lambda, out sentence)
                || Disjunction<TDomain, TElement>.TryCreate(lambda, out sentence)
                || Equivalence<TDomain, TElement>.TryCreate(lambda, out sentence)
                || Implication<TDomain, TElement>.TryCreate(lambda, out sentence)
                || Quantification<TDomain, TElement>.TryCreate(lambda, out sentence)
                // Atomic sentences:
                || Equality<TDomain, TElement>.TryCreate(lambda, out sentence)
                || Predicate<TDomain, TElement>.TryCreate(lambda, out sentence);
        }
    }
}
