# SCFirstOrderLogic

Basic but fully functional and documented [first-order logic](https://en.wikipedia.org/wiki/First-order_logic) implementations.
Heavily influenced by ["Artificial Intelligence: A Modern Approach" (Russell & Norvig)](https://www.google.com/search?q=isbn+978-1292153964).
Intended first and foremost to assist with learning and experimentation, but does include extension points (and async support) so that it is at least conceivable that it could be (extended to be) useful in a production scenario.
Includes:

* Implementations of both raw and CNF sentence representation. Multiple ways of instantiating raw sentences, including a language-integrated approach that allows (the domain to be modelled as an IEnumerable&lt;T&gt; and) sentences to be provided as lambda expressions.
* Knowledge base implementations that, while using very simple versions of their respective algorithms, do expose properties for the retrieval of proof details.
* Sentence formatting logic that allows for (customisable) unique labelling of standardised variables and Skolem functions across a set of sentences.

As mentioned above, the main goal here is for it to be a resource for learning and experimentation. As such, care has also been taken to include:

* Good type and member documentation
* Functionality for explaining the results of queries
* [Source Link](https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/sourcelink), to allow for debugging; as well as explanatory inline comments in the source code where appropriate

Full documentation can be found via the [FlUnit repository readme](https://github.com/sdcondon/SCFirstOrderLogic).
