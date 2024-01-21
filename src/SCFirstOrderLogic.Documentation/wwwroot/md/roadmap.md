# Roadmap

Long-term, this library will be "done" when I think that it does its "learning and experimentation" job adequately, and provides enough extension points for people to extend it should they so wish - perhaps even to the point of being useful in a production scenario.
I don't intend to add any particularly powerful or specialised inference logic to this package - the simple implementations suffice for its purpose.

Priorities at the time of writing:

* *Easing back on this for the immediate future - working on other stuff.*

Further ahead:

* Improvements to inference algorithms. As mentioned above and elsewhere, this package is more about the fundamentals than especially powerful inference logic - but we should probably cover at least some of the relatively fundamental problems and techniques - the inference demo is rather underwhelming at the moment.. For example, some or all of:
  * Handling of infinite loops when chaining
  * More flexibility when chaining - while I'm still relatively new to this and haven't come across any explicit mention of it in what I've read, it feels like what is considered a definite clause should depend somewhat on a fairly flexible perspective w.r.t. positive and negative literals.
  * A linear resolution strategy
  * Leveraging subsumption (e.g. a trie-like structure for fast lookup of subsuming clauses - SubsumptionTrie or somesuch - used by new clause store(s)), etc.

