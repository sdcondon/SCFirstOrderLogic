Full documentation can be found [here](https://sdcondon.net/SCFirstOrderLogic/).

The SCFirstOrderLogic NuGet package contains basic but fully functional and documented [first-order logic](https://en.wikipedia.org/wiki/First-order_logic) implementations for .NET.
Included are:

* A tree model for formulas.
* A streamlined model for formulas in conjunctive normal form - a set of clauses rather than a tree.
* Multiple ways of instantiating raw formulas, ranging from string parsing, through a number of code-based approaches, all the way to a language-integrated approach that allows (the domain to be modelled as an IEnumerable&lt;T&gt; and) formulas to be provided as lambda expressions.
* Formula manipulation logic - base classes for formula visitors and transformations, as well as some fundamental implementations - normalisation, variable substitutions and basic unification logic.
* Formula formatting logic that allows for (customisable) unique labelling of identifiers (e.g. those of standardised variables and Skolem functions) across a set of formulas.
* Index structures for terms and clauses, with node abstractions to allow for consumer-provided backing stores. Specifically, we have discrimination tree, path tree and feature vector index implementations.

Accompanying the core SCFirstOrderLogic package are a few supporting packages:

* [SCFirstOrderLogic.Inference.Abstractions](https://www.nuget.org/packages/SCFirstOrderLogic.Inference.Abstractions): Some abstractions for knowledge base implementations to use.
* [SCFirstOrderLogic.Inference.Basic](https://www.nuget.org/packages/SCFirstOrderLogic.Inference.Basic): Very basic first-order logic knowledge base implementations that use the models defined by the SCFirstOrderLogic package. Not useful for anything resembling a production scenario, but perhaps useful as a tool for learning and experimentation.
* [SCFirstOrderLogic.ExampleDomains](https://www.nuget.org/packages/SCFirstOrderLogic.ExampleDomains): A few simple first-order logic domains declared using the models found in the SCFirstOrderLogic package - for use in tests and demos.
