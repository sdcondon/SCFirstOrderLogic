# Home

The [SCFirstOrderLogic NuGet package](https://www.nuget.org/packages/SCFirstOrderLogic) contains basic but fully functional and documented [first-order logic](https://en.wikipedia.org/wiki/First-order_logic) implementations for .NET.
Included are:

* Implementations of both raw and CNF sentence representation. 
* Multiple ways of instantiating raw sentences, ranging from string parsing, through a number of code-based approaches, all the way to a language-integrated approach that allows (the domain to be modelled as an IEnumerable&lt;T&gt; and) sentences to be provided as lambda expressions.
* Sentence manipulation logic - base classes for sentence visitors and transformations, as well as some implementations - e.g. normalisation, variable substitutions and basic unification logic.
* Sentence formatting logic that allows for (customisable) unique labelling of standardised variables and Skolem functions across a set of sentences.
* Index structures for terms and clauses, with node abstractions to allow for consumer-provided backing stores. Specifically, we have discrimination tree, path tree and feature vector index implementations.
* Some abstractions for knowledge base types to implement.

Accompanying the core SCFirstOrderLogic package are two supporting packages:

* SCFirstOrderLogic.Inference.Basic: Very basic first-order logic knowledge base implementations that use the models defined by the SCFirstOrderLogic package. Not useful for anything resembling a production scenario, but perhaps useful as a tool for learning, and as a starting point for more sophisticated implementations.
* SCFirstOrderLogic.ExampleDomains: A few simple first-order logic domains declared using the models found in the SCFirstOrderLogic package - for use in tests and demos.

The recommended initial learning path is as follows:

1. **[Library Overview](library-overview.md):** This is the recommended first stop. Provides a quick overview of the core and basic inference packages - useful context for diving a little deeper
1. **[Getting Started](getting-started.md):** Guidance for writing your first code that consumes these libraries
1. **[Beyond Getting Started](beyond-getting-started):** Brings attention to a few things not mentioned in 'getting started'
