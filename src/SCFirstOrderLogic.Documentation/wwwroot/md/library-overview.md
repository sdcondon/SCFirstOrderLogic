﻿# Library Overview

Here is a quick overview of the namespaces found within the SCFirsrtOrderLogic library. Reading this should give you some helpful context for diving a little deeper:

* **`SCFirstOrderLogic`:** the root namespace contains classes representing individual elements of first order logic sentences (universal and existential quantifications, conjunctions, disjunctions, predicates, functions and so on).
  Instances of these classes are composed into tree structures that represent sentences of first order logic.
  This namespace also contains classes that represent sentences in [conjunctive normal form](https://en.wikipedia.org/wiki/Conjunctive_normal_form), as well as some that represent some well-known kinds of identifier - one for the equality predicate, one for standardised variables and one for Skolem functions.
  * **`Inference`:** intended as the top-level namespace for actual inference algorithms. Contains `IKnowledgeBase`, an interface for knowledge bases (stores of knowledge that callers can `Tell` and `Ask` things).
  * **`LanguageIntegration`:** contains classes to create FoL sentences from LINQ expressions (i.e. allowing sentences to be provided as lambda expressions).
  * **`SentenceCreation`:** logic to ease the creation of sentences (which the `LanguageIntegration` classes - see above - serve as an alternative to).
  * **`SentenceFormatting`:** logic for creating string representations of sentences. Includes logic for ensuring unique labelling of standardised variables and Skolem functions across a set of sentences.
  * **`SentenceManipulation`:** assorted logic for the manipulation of sentences. Contains some interfaces and base classes for sentence visitors and transformations.
    * **`Unification`:** utility logic for [unifying](https://en.wikipedia.org/wiki/Unification_(computer_science)) literals and sentences.
  * **`TermIndexing`:** term indexing data structures, for fast lookup of generalisations and instances of query terms. 
    There are discrimination tree and path tree implementations in here.
    Customisation of the backing store is allowed for via an abstraction for tree nodes, and both synchronous and asynchronous versions exist.

For a full type and member listing, the recommendation is to use the [FuGet package explorer](https://www.fuget.org/packages/SCFirstOrderLogic/) - though going through [getting started](getting-started.md) first is probably a good idea, if you haven't already.
