# SCFirstOrderLogic User Guide

First and foremost, here is a quick overview of the namespaces found within this library:
 
* **`SCFirstOrderLogic`:** the root namespace contains classes representing individual elements of first order logic sentences ([universal](../../src/SCFirstOrderLogic/UniversalQuantification.cs) and [existential](../../src/SCFirstOrderLogic/ExistentialQuantification.cs) quantifications, [conjunctions](../../src/SCFirstOrderLogic/Conjunction.cs), [disjunctions](../../src/SCFirstOrderLogic/Disjunction.cs), [predicates](../../src/SCFirstOrderLogic/Predicate.cs), [functions](../../src/SCFirstOrderLogic/Function.cs) and so on).
Instances of these classes can be assembled into a tree structure to represent sentences of first order logic.
  * **`Inference`:** intended as the top-level namespace for actual inference algorithms. Directly contains an [interface for knowledge bases](../../src/SCFirstOrderLogic/Inference/IKnowledgeBase.cs) (stores of knowledge that can callers can `Tell` and `Ask` things).
    * **`Resolution`:** Contains an implementation of the knowledge base interface that uses a very simple form of [resolution](https://en.wikipedia.org/wiki/Resolution_(logic)) - as well as some supporting types. These supporting types include an interface for clause stores, and a very simple implementation that just stores clauses in a list.
    * **`Unification`:** Utility logic for [unifying](https://en.wikipedia.org/wiki/Unification_(computer_science)) literals.
  * **`LanguageIntegration`:** contains classes to create FoL sentences from LINQ expressions (i.e. allowing sentences to be provided as lambda expressions). For details, see the [language integration](./language-integration.md) page.
  * **`SentenceCreation`:** Logic to ease the creation of sentences. Contains the [`SentenceFactory`](../../src/SCFirstOrderLogic/SentenceCreation/SentenceFactory.cs) and [`OperableSentenceFactory`](../../src/SCFirstOrderLogic/SentenceCreation/OperableSentenceFactory.cs) static classes (which both include a number of shorthand static helper methods for instantiating sentences).
  * **`SentenceFormatting`:** Logic for creating string representations of sentences. Includes logic for ensuring unique labelling of standardised variables and Skolem functions (which the symbol representations themselves don't concern themselves with).
  * **`SentenceManipulation`:** Assorted logic for the manipulation of sentences. Contains some interfaces and base classes for sentence visitors and transformations. Also contains classes for conversion to and representation of [conjunctive normal form](https://en.wikipedia.org/wiki/Conjunctive_normal_form).

For initial usage examples, see the [example domains](../../src/SCFirstOrderLogic.ExampleDomains) project (and, to a lesser extent, the [tests](../../src/SCFirstOrderLogic.Tests)).
Beyond that, see the XML documentation against the classes - which I hope is fairly decent.
