![SCFirstOrderLogic Icon](src/SCFirstOrderLogic.png)

# SCFirstOrderLogic

[![NuGet version (SCFirstOrderLogic)](https://img.shields.io/nuget/v/SCFirstOrderLogic.svg?style=flat-square)](https://www.nuget.org/packages/SCFirstOrderLogic/) [![NuGet downloads (SCFirstOrderLogic)](https://img.shields.io/nuget/dt/SCFirstOrderLogic.svg?style=flat-square)](https://www.nuget.org/packages/SCFirstOrderLogic/)

Basic but fully functional and documented [first-order logic](https://en.wikipedia.org/wiki/First-order_logic) implementations.
Heavily influenced by ["Artificial Intelligence: A Modern Approach" (Russell & Norvig)](https://www.google.com/search?q=isbn+978-1292153964).
Intended first and foremost to assist with learning and experimentation, but does include extension points (and async support) so that it is at least conceivable that it could be (extended to be) useful in a production scenario.
Includes:

* Implementations of both raw and CNF sentence representation. Multiple ways of instantiating raw sentences, including a language-integrated approach that allows (the domain to be modelled as an IEnumerable&lt;T&gt; and) sentences to be provided as lambda expressions.
* Knowledge base implementations that, while using very simple versions of their respective algorithms, do expose properties for the retrieval of proof details.
* Sentence formatting logic that allows for (customisable) unique labelling of standardised variables and Skolem functions across a set of sentences.

As mentioned above, the main goal here is for it to be a resource for learning and experimentation.
As such, care has also been taken to include good type and member documentation, as well as [Source Link](https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/sourcelink) to allow for debugging - and explanatory inline comments in the source code where appropriate.

## Documentation

For now at least, full documentation can be found in the docs folder of this repository. Specifically, we have:

* **[User Guide](https://github.com/sdcondon/SCFirstOrderLogic/blob/main/docs/user-guide/README.md):** The user guide is fairly sparse so far, but should suffice for people to get up and running
  * **[Library Overview](https://github.com/sdcondon/SCFirstOrderLogic/blob/main/docs/user-guide/library-overview.md):** This is the recommended first stop. Provides a quick overview of the library - useful context for diving a little deeper
  * **[Getting Started](https://github.com/sdcondon/SCFirstOrderLogic/blob/main/docs/user-guide/getting-started.md):** Guidance for writing your first code that consumes this library
  * **[Beyond Getting Started](https://github.com/sdcondon/SCFirstOrderLogic/blob/main/docs/user-guide/beyond-getting-started.md):** Brings attention to a few things not mentioned in 'getting started'
  * **[Language Integration](https://github.com/sdcondon/SCFirstOrderLogic/blob/main/docs/user-guide/language-integration.md):** An explanation of how the language integration aspects of this library work
* **[Roadmap](https://github.com/sdcondon/SCFirstOrderLogic/blob/main/docs/roadmap.md):** Full project tracking would be overkill for the moment, so there's just a bullet list to organise my thoughts

## See Also

Like this? It might also be worth checking out:

* [SCClassicalPlanning](https://github.com/sdcondon/SCClassicalPlanning): Basic classical planning implementations. Based on chapter 10 of "Artificial Intelligence: A Modern Approach" - though perhaps a _little_ more loosely than this repo is based on chapters 8 and 9. Specifically, introduces a couple more PDDL concepts that the book doesn't bother with (but should IMO). Depends on this library (and [SCGraphTheory.Search](https://github.com/sdcondon/SCGraphTheory.Search)).
* [SCPropositionalLogic](https://github.com/sdcondon/SCPropositionalLogic): Basic propositional logic implementations. Based on chapter 7 of "Artificial Intelligence: A Modern Approach". Mostly just a precursor to this repo - I haven't bothered publishing this one to NuGet - but might be of use if you find this repo a bit too much.
* [AIMA Code](https://github.com/aimacode/aima-csharp): I mention this only because I feel like I should. This is the "official" C# repository for "Artificial Intelligence: A Modern Approach" - and it is utterly, irretrievably useless.