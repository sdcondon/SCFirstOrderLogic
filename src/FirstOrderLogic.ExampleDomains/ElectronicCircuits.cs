using System.Collections.Generic;

namespace LinqToKB.FirstOrderLogic.ExampleDomains.ElectronicCircuits
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
        // TODO! Other examples are more complete..

        //// Unary functions:
        ICircuitElement GateType { get; }
    }

    public static class KnowledgeBaseExtensions
    {
        // Usage (note the separation of concerns for knowledge base implementation and the domain):
        //
        // var kb = new ResolutionKnowledgeBase<ICircuitElements, ICircuitElement>(); // ..or a different KB implementation - none implemented yet
        // kb.AddElectronicCircuitAxioms();
        // kb.Tell(..facts about the specific problem..);
        // .. though the real value of LinqToKB would be in allowing something like kb.Bind(myDomainAdapter); for runtime "constants"
        // kb.Ask(..my query..);
        //
        // Would this be better as a public read-only axioms collection and an IKnowledgeBase extension to tell multiple facts at once?
        // i.e. kb.Tell(ElectronicCircuitKnowledge.Axioms); ..could also gracefully provide theorems then, and also allows for axiom
        // examination without a KB instance..

        public static void AddElectronicCircuitAxioms(this IKnowledgeBase<ICircuitElements, ICircuitElement> knowledgeBase)
        {
            // TODO! Other examples are more complete..
        }
    }
}
