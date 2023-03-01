# Roadmap

Long-term, this library will be "done" when I think that it does its "learning and experimentation" job adequately, and provides enough extension points for people to extend it should they so wish - perhaps even to the point of being useful in a production scenario.
I don't intend to add any particularly powerful or specialised inference logic to this package - the simple implementations suffice for its purpose.

Priorities at the time of writing:

* *As always, anything that's labelled as TODO in the code. Head over to [https://github.dev/sdcondon/SCFirstOrderLogic/](https://github.dev/sdcondon/SCFirstOrderLogic/) (GitHub's regular search functionality is still a bit naff) and do a case-sensitive search for TODO.*
* Nothing much else for the mo. I've been major version rev-ing this a bit too much (ultimately as a result of publishing it a bit before it was ready..), so turning my attention to other things for a while to let things settle.

On the back-burner, for later consideration:

* Improvements to inference algorithms may appear at some point. Handling of infinite loops when chaining, leveraging subsumption (e.g. a trie-like structure for fast lookup of subsuming clauses - SubsumptionTrie or somesuch, used by ResolutionQuery for its steps record instead of a dictionary) etc.
As alluded to above and elsewhere, this package is more about the fundamentals than especially powerful inference logic - but we should probably cover such relatively fundamental problems and techniques.
* Take a look at creating a FoL syntax parser in the SentenceCreation namespace.
Did take a brief look at using ANTLR to do this, but the "all .NET" way of doing this is old and no longer supported.
If I go the route of using the Java tool, I'd want to do it via docker-integrated project (to avoid faffing around with JVM installation on my dev machine) - which would likely mean I'd want to put it into its own package.
Either that I just implement a recursive-descent parser myself (in which case it could maybe go in the core package) - but I'd honestly rather use ANTLR.
Either way, back burner for now.