# Beyond Getting Started

Once you have worked through [getting started](./getting-started.md), the best approach for getting to grips with SCFirstOrderLogic is to experiment with the [example domains](../../src/SCFirstOrderLogic.ExampleDomains) (and/or domains of your own making), consulting the type and member documentation as you go.

You may also find the information provided below useful:

## Functionality not Mentioned in 'Getting Started'

There are a number of pieces of functionality that are not mentioned in 'getting stated', but are worth noting:

* **Equality:** The top-level namespace `SCFirstOrderLogic` includes [`EqualitySymbol`](../../src/SCFirstOrderLogic/EqualitySymbol.cs), intended to be used as the symbol for the equality predicate.
The various sentence creation methods make use of this in created sentences where appropriate.
None of the knowledge bases here use particular techniques (e.g. demodulation, paramodulation) to handle equality. However, the `Inference` namespace does include [`EqualityAxiomisingKnowledgeBase`](../../src/SCFirstOrderLogic/Inference/EqualityAxiomisingKnowledgeBase.cs), which is a decorator applied to an inner knowledge base - and adds rules pertaining to equality as knowledge is added.
* **Sentence Formatting:** There is some sentence formatting logic to be found in the `SentenceFormatting` namespace - which includes support for ensuring unique labelling of symbols for standardised variables and Skolem functions across a set of sentences.
The sets of labels used can be specified by the caller (but defaults do exist).
* **CNF:** While it wasn't explicity mentioned in "getting started", the knowledge bases referenced in that document do of course make use of conjunctive normal form where appropriate. Classes for representation of CNF can be found alongside the "raw" sentence types in the `SCFirstOrderLogic` namespace.

## Learning Resources in the Repository

The repo contains a few resources aside from the source of the library itself that people might find interesting:

* **['Alternatives' project](../../src/SCFirstOrderLogic.Alternatives):** this class library contains a number of alternative implementations of things. Some of these implementations are "rawer" recreations of algorithms as listed in the source material (these tend to have names suffixed with `_FromAIaMA`), others just exhibit slightly different performance or complexity characteristics. These have been created for learning and benchmarking.
* **['Benchmarks' project](../../src/SCFirstOrderLogic.Benchmarks):** benchmark project for evaluating performance of the algorithms in the NuGet package and in the 'Alternatives' project.
