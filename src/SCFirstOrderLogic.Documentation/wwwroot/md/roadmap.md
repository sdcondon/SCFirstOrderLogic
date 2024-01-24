# Roadmap

Long-term, this library will be "done" when I think that it does its "learning and experimentation" job adequately, and provides enough extension points for people to extend it should they so wish - perhaps even to the point of being useful in a production scenario.
I don't intend to add any particularly powerful or specialised inference logic to this package - the simple implementations suffice for its purpose.

Priorities at the time of writing:

* *Easing back on this for the immediate future - working on other stuff.*

Further ahead:

* Next up likely to be more indexing stuff - specifically, to facilitate finding subsuming/subsumed clauses.
  Might go very old-school and build on discrimination/path trees for this, but will probably at least try to go slightly less old-school and have a crack at a feature vector index implementation.
* Improvements to inference algorithms. As mentioned above and elsewhere, this package is more about the fundamentals than especially powerful inference logic - but we should probably cover at least some of the relatively fundamental problems and techniques - the inference demo is rather underwhelming at the moment.. For example, some or all of:
  * Create a clause store or two that leverages subsumption.
  * Handling of loops when chaining
  * More flexibility when chaining. Obviously the current implementations are very constrained in their consideration of clauses.
  * A linear resolution strategy


