# Library Overview

Here is a quick overview of the namespaces found within the SCFirstOrderLogic library. Reading this should give you some helpful context for diving a little deeper:

* **`SCFirstOrderLogic`:** the root namespace contains classes representing individual elements of first order logic formulas (universal and existential quantifications, conjunctions, disjunctions, predicates, functions and so on).
  Instances of these classes are composed into tree structures that represent formulas of first order logic.
  This namespace also contains classes that represent formulas in [conjunctive normal form](https://en.wikipedia.org/wiki/Conjunctive_normal_form) in a streamlined manner (i.e. as a set of clauses rather than a single tree structure).
  Finally, it contains a class that represents the identifier of a well-known predicate - equality.
  * **`ClauseIndexing`:** clause indexing data structures, for fast lookup of stored clauses that subsume or are subsumed by a query clause.
    Specifically, there are feature vector index implementations in here.
    Customisation of the backing store is allowed for via an abstraction for tree nodes, and both synchronous and asynchronous versions exist.
    * **`Features:`** a couple of well-known features to use in feature vector index instances - max depth and occurence count.
    The use of these is of course optional - you can use your own feature specifications, too.
  * **`FormulaCreation`:** logic to ease the creation of formulas. Directly contains a formula parser, as well as a couple of static factory classes for the succinct creation of formulas.
    All formula creation options aside from the specialised ones (see below) are demonstrated in [getting started](getting-started.md).
    * **`Linq:`** logic for creating formulas in a language-integrated manner. Essentially allows for creating formulas from LINQ expressions.
    * **`Specialised:`** Formula creation logic for use in certain specialised situations.
  * **`FormulaFormatting`:** logic for creating string representations of formulas. Includes logic for ensuring unique labelling of e.g. standardised variables and Skolem functions across a set of formulas.
  * **`FormulaManipulation`:** contains some interfaces and base classes for the manipulation of formulas - formula visitors and transformations.
    * **`Normalisation`:** formula manipulation & interrogation logic related to normalisation. For example; transformation to CNF, and restandardisation of variables in a formula already in CNF.
    * **`Substitution`:** formula manipulation & interrogation logic for working with variables. For example; a variable substitution representation, as well as logic for identifying generalisations, instances, unifications, subsumed clauses etc.
  * **`TermIndexing`:** term indexing data structures, for fast lookup of generalisations and instances of query terms. 
    There are discrimination tree and path tree implementations in here.
    Customisation of the backing store is allowed for via an abstraction for tree nodes, and both synchronous and asynchronous versions exist.

For a full type and member listing, the recommendation is to use the [FuGet package explorer](https://www.fuget.org/packages/SCFirstOrderLogic/) - though going through [getting started](getting-started.md) first is probably a good idea, if you haven't already.
