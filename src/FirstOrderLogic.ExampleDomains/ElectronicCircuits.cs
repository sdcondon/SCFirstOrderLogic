using LinqToKB.FirstOrderLogic.KnowledgeBases;
using System.Collections.Generic;

namespace FirstOrderLogic.ExampleDomains
{
    public static class ElectronicCircuits
    {
        static ElectronicCircuits()
        {
            KnowledgeBase = null; // TODO!


        }

        public static IKnowledgeBase<ICircuitElements, ICircuitElement> KnowledgeBase { get; }

        /// <summary>
        /// First order logic domain for electronic circuit elements.
        /// </summary>
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


            //// Unary functions:
            ICircuitElement GateType { get; }
        }
    }
}
