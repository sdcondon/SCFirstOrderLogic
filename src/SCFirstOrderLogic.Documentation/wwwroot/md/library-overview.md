# Library Overview

Here is a quick overview of the namespaces found within this library. Reading this should give you some helpful context for diving a little deeper:

* **`SCFirstOrderLogic`:** the root namespace contains classes representing individual elements of first order logic sentences (universal and existential quantifications, conjunctions, disjunctions, predicates, functions and so on).
  Instances of these classes are composed into tree structures that represent sentences of first order logic. This namespace also contains classes that represent sentences in [conjunctive normal form](https://en.wikipedia.org/wiki/Conjunctive_normal_form), as well as a class that represents the identifier of the equality predicate.
  * **`Inference`:** intended as the top-level namespace for actual inference algorithms. Directly contains `IKnowledgeBase`, an interface for knowledge bases (stores of knowledge that callers can `Tell` and `Ask` things).
    * **`BackwardChaining`:** contains an `IKnowledgeBase` implementation that uses a simple form of [backward chaining](https://en.wikipedia.org/wiki/Backward_chaining) - as well as some supporting types.
    * **`ForwardChaining`:** contains an `IKnowledgeBase` implementation that uses a simple form of [forward chaining](https://en.wikipedia.org/wiki/Forward_chaining) - as well as some supporting types.
    * **`Resolution`:** contains an `IKnowledgeBase` implementation that uses a simple form of [resolution](https://en.wikipedia.org/wiki/Resolution_(logic)) - as well as some supporting types.
  * **`LanguageIntegration`:** contains classes to create FoL sentences from LINQ expressions (i.e. allowing sentences to be provided as lambda expressions).
  * **`SentenceCreation`:** logic to ease the creation of sentences - which the language integration classes serve as an alternative to.
  * **`SentenceFormatting`:**: logic for creating string representations of sentences. Includes logic for ensuring unique labelling of standardised variables and Skolem functions across a set of sentences.
  * **`SentenceManipulation`:** assorted logic for the manipulation of sentences. Contains some interfaces and base classes for sentence visitors and transformations.
    * **`Unification`:** utility logic for [unifying](https://en.wikipedia.org/wiki/Unification_(computer_science)) literals and sentences.
