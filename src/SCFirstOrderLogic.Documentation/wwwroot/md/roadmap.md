# Roadmap

Long-term, this library will be "done" when I think that it does its "learning and experimentation" job adequately, and provides enough extension points for people to extend it should they so wish - perhaps even to the point of being useful in a production scenario.
I don't intend to add any particularly powerful or specialised inference logic to this package - the simple implementations suffice for its purpose.

Priorities at the time of writing:

* Improvements in the area of query execution robustness (in the face of re-execution, re-entry etc)
* *As always, anything that's labelled as TODO in the code. Clone the repo and use your IDE's TODO/Task list functionality (TODOs are generally expressed as `// TODO..` or `/// TODO`) to see individual things I want to do.*

Breaking changes on the list for v4 (though there's no particular timeline for this):

* I want to improve the extensibility of the simple resolution knowledge base, which will again likely mean breaking ctor changes.
The current approach that uses a store and two delegates is not flexible enough.
I'm quietly confident that if I replace all three with a strategy object (that in all likelihood will still use the store) I can allow for e.g. linear resolution, while still not making it TOO complicated to fulfill the "learning" part of this project's remit.
* Revisit sentence formatting. 
Label sets currently used by SentenceFormatter likely to become full interfaces (ILabeller, maybe) - though the first implementation created is likely to be LabelSetLabeller.

On the back-burner, for later consideration:

* Take a look at creating a FoL syntax parser in the SentenceCreation namespace.
Did take a brief look at using ANTLR to do this, but the "all .NET" way of doing this is old and no longer supported.
If I go the route of using the Java tool, and want to do it via docker-integrated project - which would likely mean I'd want to put it into its own package.
Either that I just implement a recursive-descent parser myself (in which case it could maybe go in the core package) - but I'd honestly rather use ANTLR.
Either way, back burner for now.
