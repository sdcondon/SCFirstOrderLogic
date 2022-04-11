# SCFirstOrderLogic User Guide

First and foremost, here is a quick overview of the namespaces found within this library:
 
* **`SCFirstOrderLogic`:** the root namespace contains classes representing individual elements of first order logic sentences ([universal](../../src/SCFirstOrderLogic/UniversalQuantification.cs) and [existential](../../src/SCFirstOrderLogic/ExistentialQuantification.cs) quantifications, [conjunctions](../../src/SCFirstOrderLogic/Conjunction.cs), [disjunctions](../../src/SCFirstOrderLogic/Disjunction.cs), [predicates](../../src/SCFirstOrderLogic/Predicate.cs), [functions](../../src/SCFirstOrderLogic/Function.cs) and so on).
Instances of these classes can be assembled into a tree structure to represent sentences of first order logic.
  * **`Inference`:** intended as the top-level namespace for actual inference algorithms. Directly contains an [interface for knowledge bases](../../src/SCFirstOrderLogic/Inference/IKnowledgeBase.cs) (stores of knowledge that can callers can `Tell` and `Ask` things).
    * **`Resolution`:** Contains an implementation of the knowledge base interface that uses a very simple form of [resolution](https://en.wikipedia.org/wiki/Resolution_(logic)).
    * **`Unification`:** Utility logic for [unifying](https://en.wikipedia.org/wiki/Unification_(computer_science)) literals and clauses. Also contains an interface for unifier stores, and a very simple implementation that just stores things in a list and searches through it.
  * **`LanguageIntegration`:** contains classes to create FoL sentences from LINQ expressions (i.e. allowing sentences to be provided as lambda expressions). For details, see the [language integration](./language-integration.md) page.
  * **`SentenceManipulation`:** Assorted logic for the creation and manipulation of sentences. Contains classes for conversion to and representation of [conjunctive normal form](https://en.wikipedia.org/wiki/Conjunctive_normal_form). Also contains a `SentenceFactory` static class (which includes a number of shorthand static helper methods for instantiating sentences) and a base class for sentence transformations.

For initial usage examples, see the [example domains](../../src/SCFirstOrderLogic.ExampleDomains) project (and, to a lesser extent, the [tests](../../src/SCFirstOrderLogic.Tests)).
Beyond that, see the XML documentation against the classes - which I hope is fairly decent.
