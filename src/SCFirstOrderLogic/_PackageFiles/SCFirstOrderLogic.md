The SCFirstOrderLogic NuGet package contains basic but fully functional and documented [first-order logic](https://en.wikipedia.org/wiki/First-order_logic) implementations for .NET.
Included are:

* Implementations of both raw and CNF sentence representation. 
* Multiple ways of instantiating raw sentences, ranging from string parsing, through a number of code-based approaches, all the way to a language-integrated approach that allows (the domain to be modelled as an IEnumerable&lt;T&gt; and) sentences to be provided as lambda expressions.
* Sentence manipulation logic - base classes for sentence visitors and transformations, as well as some implementations - e.g. normalisation, variable substitutions and basic unification logic.
* Sentence formatting logic that allows for (customisable) unique labelling of standardised variables and Skolem functions across a set of sentences.
* Index structures for terms and clauses, with node abstractions to allow for consumer-provided backing stores. Specifically, we have discrimination tree, path tree and feature vector index implementations.
* Some abstractions for knowledge base implementations to implement.

Accompanying the core SCFirstOrderLogic package are two supporting packages:

* [SCFirstOrderLogic.ExampleDomains](https://www.nuget.org/packages/SCFirstOrderLogic.ExampleDomains): A few simple first-order logic domains declared using the models found in the SCFirstOrderLogic package - for use in tests and demos.
* [SCFirstOrderLogic.Inference.Basic](https://www.nuget.org/packages/SCFirstOrderLogic.Inference.Basic): Very basic first-order logic knowledge base implementations that use the models defined by the SCFirstOrderLogic package. Not useful for anything resembling a production scenario, but perhaps useful as a tool for learning and experimentation.

Full documentation can be found [here](https://sdcondon.net/SCFirstOrderLogic/).
