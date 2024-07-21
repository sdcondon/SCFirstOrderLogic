# Roadmap

Long-term, this library will be "done" when I think that it does its "learning and experimentation" job adequately, and provides enough extension points for people to extend it should they so wish - perhaps even to the point of being useful in a production scenario.
I don't intend to add any particularly powerful or specialised inference logic to this package - the simple implementations suffice for its purpose.

Priorities at the time of writing:

* A feature vector index implementation in a new ClauseIndexing namespace of the core package.
* A linear resolution strategy in the Inference.Basic package.

Further ahead:

* Some improvements to the inference algorithms in Inference.Basic package.
  As mentioned above and elsewhere, this package is more about the fundamentals than especially powerful inference logic, but we should probably cover at least some of the relatively fundamental problems and techniques - the inference demo is rather underwhelming at the moment.
  For example, some or all of:
  * Some clause store implementations that leverage subsumption
  * Handling of loops when chaining
  * More flexibility when chaining. The current implementations are very constrained in their consideration of clauses.
* Breaking changes for v7:
  * I don't like the fact that the model includes Constant as its own type.
    There is of course no real need for it - a constant is just a function with arity zero.
    I have kept it so far to keep notions of equality "simple" in the face of the parser allowing both (e.g.) `f` and `f()`.
    I would argue that in most cases a caller including e.g. both `f` and `f()` in parsed statements would expect the two to NOT be considered the same from an equality perspective.
    While this is easily accomplised (by using identifiers that are more than just the symbol text), I would **also** argue that the default parser using just "plain" strings as identifiers is a good, intuitive behaviour.
    But I really want to sort it out - after all, dealing with something that should really just be a parsing concern in the model isn't a great state of affairs.
    So I'm likely to do this in v7 - and account for it in the parser by (tweaking the delegates that the parser ctor accepts and) splitting the `Basic` parser into two - one that just uses strings (making `f` and `f()` identical) and one that uses an identifier that encodes whether parentheses were included or not (making `f` and `f()` different).
