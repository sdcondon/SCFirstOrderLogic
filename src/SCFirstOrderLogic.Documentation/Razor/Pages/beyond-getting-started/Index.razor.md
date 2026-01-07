# Beyond Getting Started

This library is fairly small and its download count is rather low - putting a huge amount of effort into full documentation here would not be a great use of time.
Note, however, that a fair degree of effort *has* been put into the type and member documentation. That should be your primary resource for learning how to use this library.
Having said all that, some hopefully useful notes do follow here.

## Notes by Namespace

### `SCFirstOrderLogic`

* **CNF:** While it wasn't explicity mentioned in "getting started", the knowledge bases referenced in that document do of course make use of conjunctive normal form where appropriate.
  Classes for representation of CNF can be found alongside the "raw" formula types in the SCFirstOrderLogic namespace.
* **Equality:** The SCFirstOrderLogic namespace includes EqualityIdentifier, intended to be used as the identifier for the equality predicate.
  The various formula creation methods make use of this in created formulas where appropriate.

### `SCFirstOrderLogic.FormulaCreation.Linq`

As mentioned briefly in 'getting started', a language-integrated approach to formula creation is available. See [here](beyond-getting-started/language-integration.md) for details.

### `SCFirstOrderLogic.FormulaFormatting`

There is some formula formatting logic to be found in the FormulaFormatting namespace - which includes support for ensuring unique labelling of identifiers for standardised variables and Skolem functions across a set of formulas.
The sets of labels used can be specified by the caller (but defaults do exist).

### `SCFirstOrderLogic.TermIndexing`

Several types for fast term lookup are present in the TermIndexing namespace.
There are implementations of both discrimination trees and path indices.
An abstraction for the nodes of these structures allows for customisation of the underlying storage.
The only implementations provided in the library store things in memory, however.
