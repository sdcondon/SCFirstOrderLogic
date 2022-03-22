# SCFirstOrderLogic User Guide

For initial usage examples, see the [tests](../../src/SCFirstOrderLogic.Tests) and the [example domains](../../src/SCFirstOrderLogic.ExampleDomains) project.

There follows a quick overview of the namespaces found within this library:
 
* **`SCFirstOrderLogic`:** the root namespace contains classes representing individual elements of first order logic sentences. Together, these form a tree structure to represent sentences. Of particular note is the `Sentence` base class, which includes a number of shorthand static helper methods for instantiating sentences.
  * **`KnowledgeBases`:** contains the interface for knowledge bases - stores of knowledge that can callers can `Tell` and `Ask` things.
  * **`LanguageIntegration`:** contains classes to create FoL sentences from LINQ expressions (i.e. allowing sentences to be provided as lambda expressions). For details, see the [language integration](./language-integration.md) page.
  * **`SentenceManipulation`:** Utility classes for manipulating sentences. Contains base class for sentence transformation classes as well as some sub-namespaces:
    * **`ConjunctiveNormalForm`:** Utility classes for conversion to and representation of sentences in conjunctive normal form.
    * **`Unification`:** Utility classes for determining the unifier for pairs of sentences, and for finding pairs of sentences that unify.