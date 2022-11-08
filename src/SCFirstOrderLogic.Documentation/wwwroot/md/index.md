# Home

The SCFirstOrderLogic NuGet package contains basic but fully functional and documented [first-order logic](https://en.wikipedia.org/wiki/First-order_logic) implementations.
It is heavily influenced by (chapters 8 and 9 of) [Artificial Intelligence: A Modern Approach" (Russell & Norvig)](https://www.google.com/search?q=isbn+978-1292153964).
It is intended first and foremost to assist with learning and experimentation, but does include extension points (and async support) so that it is at least conceivable that it could be (extended to be) useful in a production scenario.
Included are:

* Implementations of both raw and CNF sentence representation.
  Multiple ways of instantiating raw sentences, including a language-integrated approach that allows (the domain to be modelled as an IEnumerable&lt;T&gt; and) sentences to be provided as lambda expressions.
* Knowledge base implementations that, while using very simple versions of their respective algorithms, do use abstracted, asynchronous clause storage (thus providing an extension point) and expose properties for the retrieval of proof details.
* Sentence formatting logic that allows for (customisable) unique labelling of standardised variables and Skolem functions across a set of sentences.

As mentioned above, the main goal here is for it to be a resource for learning and experimentation.
As such, care has also been taken to include good type and member documentation, as well as [Source Link](https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/sourcelink) to allow for debugging - and explanatory inline comments in the source code where appropriate.

The recommended initial learning path is as follows:

1. **[Library Overview](library-overview.md):** This is the recommended first stop. Provides a quick overview of the library - useful context for diving a little deeper
1. **[Getting Started](getting-started.md):** Guidance for writing your first code that consumes this library
1. **[Beyond Getting Started](beyond-getting-started.md):** Brings attention to a few things not mentioned in 'getting started'
