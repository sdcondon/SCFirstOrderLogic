Basic but fully functional and documented [first-order logic](https://en.wikipedia.org/wiki/First-order_logic) implementations.
Heavily influenced by ["Artificial Intelligence: A Modern Approach" (Russell & Norvig)](https://www.google.com/search?q=isbn+978-1292153964).
Intended first and foremost to assist with learning and experimentation, but does include extension points (and async support) so that it is at least conceivable that it could be (extended to be) useful in a production scenario.
Includes:

* Implementations of both raw and CNF sentence representation. Multiple ways of instantiating raw sentences, ranging from string parsing, through a number of code-based approaches, all the way to a language-integrated approach that allows (the domain to be modelled as an IEnumerable&lt;T&gt; and) sentences to be provided as lambda expressions.
* Knowledge base implementations that, while using very simple (i.e. not production ready) versions of their respective algorithms, do use abstracted, asynchronous clause storage (thus providing an extension point) and expose properties for the retrieval of proof details.
* Sentence formatting logic that allows for (customisable) unique labelling of standardised variables and Skolem functions across a set of sentences.

As mentioned above, the main goal here is for it to be a resource for learning and experimentation.
As such, care has also been taken to include good type and member documentation, as well as [Source Link](https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/sourcelink) to allow for debugging - and explanatory inline comments in the source code where appropriate.

Full documentation can be found [here](https://sdcondon.net/SCFirstOrderLogic/).
