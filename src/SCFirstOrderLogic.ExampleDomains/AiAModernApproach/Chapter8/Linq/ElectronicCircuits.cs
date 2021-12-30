using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter8.ElectronicCircuits.Linq
{
    /// <summary>
    /// First order logic domain for electronic circuit elements.
    /// </summary>
    /// <remarks>
    /// (TODO: comment on the same interface being used for would ordinarily be different types. This
    /// is a consequence of not engaging with the type system that much in the interests of simplicity.
    /// Not necessairly a big deal - if we do want to apply this to real domain elements (for verification
    /// or whatever), this interface could always be an adapter..
    /// Perhaps to revisit later..
    /// </remarks>
    public interface ICircuitElements : IEnumerable<ICircuitElement>
    {
        ICircuitElement SignalOn { get; }

        ICircuitElement SignalOff { get; }

        ICircuitElement GateTypeAnd { get; }

        ICircuitElement GateTypeOr { get; }

        ICircuitElement GateTypeXOr { get; }
    }

    /// <summary>
    /// Interface for elements of the electronic circuits first order logic domain.
    /// </summary>
    public interface ICircuitElement
    {
        //// Unary predicates:
        bool IsGate { get; }
        bool IsTerminal { get; }
        bool IsCircuit { get; }

        //// Binary predicates:
        // TODO! The other examples are more complete..

        //// Unary functions:
        ICircuitElement GateType { get; }
    }

    /// <summary>
    /// Container for fundamental knowledge about the <see cref="ICircuitElements"/> domain.
    /// </summary>
    public static class ElectronicCircuitKnowledge
    {
        // Usage (note the separation of concerns for knowledge base implementation and the domain):
        //
        // var kb = new ResolutionKnowledgeBase<ICircuitElements, ICircuitElement>(); // ..or a different KB implementation - none implemented yet
        // kb.Tell(ElectronicCircuitKnowledge.Axioms);
        // kb.Tell(..facts about the specific problem..);
        // .. though the real value of lanaguage integration would be in allowing something like kb.Bind(domainAdapter, opts),
        //    where domainAdapter is an ICircuitElements, to specify known constants in a way that is easily integrable with other code
        // kb.Ask(..my query..);
        public static IReadOnlyCollection<Expression<Predicate<ICircuitElements>>> Axioms { get; } = new List<Expression<Predicate<ICircuitElements>>>()
        {
            //// TODO! The other examples are more complete..

        }.AsReadOnly();
    }
}
