# Roadmap

Long-term, this library will be "done" when I think that it does its "learning and experimentation" job adequately, and provides enough extension points for people to extend it should they so wish - perhaps even to the point of being useful in a production scenario.
I don't intend to add any particularly powerful or specialised inference logic to this package - the simple implementations suffice for its purpose.

Priorities at the time of writing:

* An async version of a discrimination tree, that allows for passing in the root node (thus allowing for implementations that e.g. use secondary storage)
* To accompany the discrimination tree, a path index implementation for terms.

Further ahead:

* Improvements to inference algorithms. 
Handling of infinite loops when chaining, a linear resolution strategy, leveraging subsumption (e.g. a trie-like structure for fast lookup of subsuming clauses - SubsumptionTrie or somesuch - used by new clause store(s)), etc.
As mentioned above and elsewhere, this package is more about the fundamentals than especially powerful inference logic - but we should probably cover such relatively fundamental problems and techniques - the inference demo is rather underwhelming at the moment..
