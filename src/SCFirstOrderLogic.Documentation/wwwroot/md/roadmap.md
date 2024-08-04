# Roadmap

Long-term, this library will be "done" when I think that it does its "learning and experimentation" job adequately, and provides enough extension points for people to extend it should they so wish - perhaps even to the point of being useful in a production scenario.
I don't intend to add any particularly powerful or specialised inference logic to this package - the simple implementations suffice for its purpose.

Priorities at the time of writing:

* v7 of the core package, including:
  * (done - pre-release published) `Constant` type dropped from model.
    There's never been any real need for it - a constant is just a function with arity zero.
  * (done - pre-release published) Some namespace changes
  * (WIP) A feature vector index implementation in a new ClauseIndexing namespace.

Further ahead:

* Some improvements to the inference algorithms in Inference.Basic package.
  As mentioned above and elsewhere, this package is more about the fundamentals than especially powerful inference logic, but we should probably cover at least some of the relatively fundamental problems and techniques - the inference demo is rather underwhelming at the moment.
  For example, some or all of:
  * A linear resolution strategy in the Inference.Basic package.
  * Some clause store implementations that leverage subsumption via feature vector indices
  * Handling of loops when chaining
  * More flexibility when chaining. The current implementations are very constrained in their consideration of clauses.
