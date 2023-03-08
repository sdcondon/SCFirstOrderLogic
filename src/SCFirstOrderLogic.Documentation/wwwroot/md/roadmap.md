# Roadmap

Long-term, this library will be "done" when I think that it does its "learning and experimentation" job adequately, and provides enough extension points for people to extend it should they so wish - perhaps even to the point of being useful in a production scenario.
I don't intend to add any particularly powerful or specialised inference logic to this package - the simple implementations suffice for its purpose.

Priorities at the time of writing:

* *As always, anything that's labelled as TODO in the code. Head over to [https://github.dev/sdcondon/SCFirstOrderLogic/](https://github.dev/sdcondon/SCFirstOrderLogic/) (GitHub's regular search functionality is still a bit naff) and do a case-sensitive search for TODO.*
* Nothing much else for the mo - certainly nothing breaking. I've been major version rev-ing this a bit too much (ultimately as a result of publishing it a bit before it was ready..), so turning my attention to other things for a while to let things settle.

At some point in the not-too-distant future:

* Improvements to inference algorithms. 
Handling of infinite loops when chaining, a linear resolution strategy, leveraging subsumption (e.g. a trie-like structure for fast lookup of subsuming clauses - SubsumptionTrie or somesuch, used by ResolutionQuery for its steps record instead of a dictionary), a linear resolution strategy etc.
As mentioned above and elsewhere, this package is more about the fundamentals than especially powerful inference logic - but we should probably cover such relatively fundamental problems and techniques - the inference demo is rather underwhelming at the moment..
