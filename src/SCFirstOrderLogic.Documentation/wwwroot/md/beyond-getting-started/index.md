# Beyond Getting Started

This library is fairly small and its download count is rather low - putting a huge amount of effort into full documentation here would not be a great use of time.
Note, however, that a fair degree of effort *has* been put into the type and member documentation. That should be your primary resource for learning how to use this library.
Having said all that, some hopefully useful notes do follow here.

## Notes by Namespace

### `SCFirstOrderLogic`

* **CNF:** While it wasn't explicity mentioned in "getting started", the knowledge bases referenced in that document do of course make use of conjunctive normal form where appropriate.
  Classes for representation of CNF can be found alongside the "raw" sentence types in the SCFirstOrderLogic namespace.
  Note that the Sentence base class defines a `ToCNF` method.
* **Equality:** The SCFirstOrderLogic namespace includes EqualityIdentifier, intended to be used as the identifier for the equality predicate.
  The various sentence creation methods make use of this in created sentences where appropriate.

### `SCFirstOrderLogic.Inference`

* **Decorator knowledge bases:** this namespace directly contains some [decorator](https://en.wikipedia.org/wiki/Decorator_pattern) knowledge base implementations.
  * **EqualityAxiomisingKnowledgeBase:** None of the knowledge bases included in the package use particular techniques (e.g. demodulation, paramodulation) to handle equality.
  However, the Inference namespace does include EqualityAxiomisingKnowledgeBase, which adds rules pertaining to equality as knowledge is added.
  * **UniqueNamesAxiomisingKnowledgeBase:** Adds "axioms" for the unique names assumption as knowledge is added. That is, Adds "not equal" statements for all constant pairs whose identifiers do not match.

### `SCFirstOrderLogic.LanguageIntegration`

As mentioned briefly in 'getting started', a language-integrated approach to sentence creation is available. See [here](beyond-getting-started/language-integration.md) for details.

### `SCFirstOrderLogic.SentenceFormatting`

There is some sentence formatting logic to be found in the SentenceFormatting namespace - which includes support for ensuring unique labelling of identifiers for standardised variables and Skolem functions across a set of sentences.
The sets of labels used can be specified by the caller (but defaults do exist).

### `SCFirstOrderLogic.TermIndexing`

While they are not yet used by any of the clause store types in the library, several types for fast term lookup are now present in the TermIndexing namespace.
There are implementations of both discrimination trees and path indices.
An abstraction for the nodes of these structures allows for customisation of the underlying storage.
The only implementations provided in the library store things in memory, however.

## Learning Resources in the Repository

The source repository contains a few resources aside from the source of the library itself that people might find interesting and/or useful:

* **['ExampleDomains' project](https://github.com/sdcondon/SCFirstOrderLogic/tree/main/src/SCFirstOrderLogic.ExampleDomains):** Example domains, declared using each of the sentence creation options.
* **['Alternatives' project](https://github.com/sdcondon/SCFirstOrderLogic/tree/main/src/SCFirstOrderLogic.Alternatives):** this class library contains a number of alternative implementations of things.
  Some of these implementations are "rawer" recreations of algorithms as listed in the source material (these tend to have names suffixed with `_FromAIaMA`), others just exhibit slightly different performance or complexity characteristics.
  These have been created for learning and benchmarking.
* **['Benchmarks' project](https://github.com/sdcondon/SCFirstOrderLogic/tree/main/src/SCFirstOrderLogic.Benchmarks):** benchmark project for evaluating performance of the algorithms in the NuGet package and in the 'Alternatives' project.
