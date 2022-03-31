# SCFirstOrderLogic User Guide

First and foremost, here is a quick overview of the namespaces found within this library:
 
* **`SCFirstOrderLogic`:** the root namespace contains classes representing individual elements of first order logic sentences ([universal](../../src/SCFirstOrderLogic/UniversalQuantification.cs) and [existential](../../src/SCFirstOrderLogic/ExistentialQuantification.cs) quantifications, [conjunctions](../../src/SCFirstOrderLogic/Conjunction.cs), [disjunctions](../../src/SCFirstOrderLogic/Disjunction.cs), [predicates](../../src/SCFirstOrderLogic/Predicate.cs), [functions](../../src/SCFirstOrderLogic/Function.cs) and so on).
Together, these form a tree structure to represent sentences.
  * **`Inference`:** intended as the top-level namespace for actual inference algorithms. Directly contains an [interface for knowledge bases](../../src/SCFirstOrderLogic/Inference/IKnowledgeBase.cs) (stores of knowledge that can callers can `Tell` and `Ask` things).
    * **`Resolution`:** Contains an implementation of the knowledge base interface that uses a very simple form of [resolution](https://en.wikipedia.org/wiki/Resolution_(logic)).
    * **`Unification`:** Utility logic for unifying literals and clauses. Also contains an interface for unifier stores, and a very simple implementation that just stores things in a list and searches through it.
  * **`LanguageIntegration`:** contains classes to create FoL sentences from LINQ expressions (i.e. allowing sentences to be provided as lambda expressions). For details, see the [language integration](./language-integration.md) page.
  * **`SentenceManipulation`:** Intended as the top-level namespace for logic related to manipulation of sentences. Directly contains a `SentenceFactory` static class (which includes a number of shorthand static helper methods for instantiating sentences) and a base class for sentence transformations. Also contains a sub-namespace:
    * **`ConjunctiveNormalForm`:** Utility classes for conversion to and representation of sentences in conjunctive normal form.

For initial usage examples, see the [example domains](../../src/SCFirstOrderLogic.ExampleDomains) project (and, to a lesser extent, the [tests](../../src/SCFirstOrderLogic.Tests)).
Beyond that, see the XML documentation against the classes - which I hope is fairly decent.
