![SCFirstOrderLogic Icon](src/SCFirstOrderLogic.png)

# SCFirstOrderLogic

Basic but fully functional and documented [first-order logic](https://en.wikipedia.org/wiki/First-order_logic) implementations. Includes:

* Implementations of both raw and CNF sentence representation. Multiple ways of instantiating raw sentences, including a language-integrated approach that allows (the domain to be modelled as an IEnumerable<T> and) sentences to be provided as lambda expressions.
* Knowledge base implementations that, while using very simple versions of their respective algorithms, do expose properties for the retrieval of proof details.
* Sentence formatting logic that allows for (customisable) unique labelling of standardised variables and Skolem functions across a set of sentences.

Created just for fun while reading chapters 8 and 9 of _Artificial Intelligence: A Modern Approach_ (3rd Edition - [ISBN 978-1292153964](https://www.google.com/search?q=isbn+978-1292153964)) - so may prove interesting to the C#-inclined reading the same book.
The ["official" C# repository](https://github.com/aimacode/aima-csharp/tree/master/aima-csharp) for the book does of course cover these chapters, but given that what I've found there isn't (IMHO) very useful, here we are.
The main goal here is for it to be a learning resource. As such, care has been taken to include decent XML documentation, functionality for explaining the results of queries, and explanatory inline comments in the source code where appropriate.
For production scenarios, there are obviously other better inference engines out there - that generally use more powerful logics than first-order.

For usage guidance, please consult the [user guide](./docs/user-guide).
