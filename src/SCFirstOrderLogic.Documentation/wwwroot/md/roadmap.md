# Roadmap

Long-term, this library will be "done" when I think that it does its "learning and experimentation" job adequately, and provides enough extension points for people to extend it should they so wish - perhaps even to the point of being useful in a production scenario.
I don't intend to add any particularly powerful or specialised inference logic to this package - the simple implementations suffice for its purpose.

Priorities at the time of writing:

* A general review, figuring out if there's anything (among the TODOs or otherwise) that I want to do before starting to mess about with changes for v4.
Approaching this fairly sedately - will definitely be spending some time working on some of my other projects before I push v4 out, in any case.
Feedback is, as always, very welcome.
* *As always, anything that's labelled as TODO in the code. Clone the repo and use your IDE's TODO/Task list functionality (TODOs are generally expressed as `// TODO..` or `/// TODO`) to see individual things I want to do.*

Breaking changes on the list for v4 (though there's no particular timeline for this):

* [ ] I want to improve the extensibility of the simple resolution knowledge base, which will again likely mean breaking ctor changes.
The current approach that uses a store and two delegates is not flexible enough.
I'm quietly confident that if I replace all three with a strategy object (that in all likelihood will still use the store) I can allow for e.g. linear resolution, while still not making it more complicated than the "simple" name implies.
* [ ] Revisit sentence formatting. 
Label sets currently used by SentenceFormatter likely to become full interfaces (ILabeller, maybe) - though the first implementation created is likely to be LabelSetLabeller.
* [x] Move the SkolemFunctionSymbol and StandardisedVariableSymbol classes up out of the `SentenceManipulation` namespace.
Really the only reason that they're here is that I originally had all the CNF stuff in SentenceManipulation.
Now that the CNF types are in the root namespace, there's really no reason for these not to be as well.
* [x] May rename/otherwise break the SentenceManipulation.CNFExaminer class - it irks me somewhat..
* [ ] Also want to rename the clause store implementations
  * [x] BackwardChaining.SimpleClauseStore -> DictionaryClauseStore
  * [x] ForwardChaining.SimpleClauseStore -> HashSetClauseStore
  * [ ] Resolution.SimpleClauseStore -> HashSetClauseStore

On the back-burner, for later consideration:

* I'm almost certainly going to overheaul the collection types used in Predicate, Function, CNFClause and CNFSentence.
There are some robustness issues as it stands. Will probably move away from ReadOnlyCollection and towards System.Collections.Immutable.
Can be done in a non-breaking fashion.
* Take a look at creating a FoL syntax parser in the SentenceCreation namespace.
Did take a brief look at using ANTLR to do this, but the "all .NET" way of doing this is old and no longer supported.
If I go the route of using the Java tool, I'd want to do it via docker-integrated project (to avoid faffing around with JVM installation on my dev machine) - which would likely mean I'd want to put it into its own package.
Either that I just implement a recursive-descent parser myself (in which case it could maybe go in the core package) - but I'd honestly rather use ANTLR.
Either way, back burner for now.
