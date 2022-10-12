# Roadmap

Clone the repo and use your IDE's TODO/Task list functionality (TODOs are generally expressed as `// TODO..`) to see individual things I want to do.
This library will be "done" when I think that it does its "learning and experimentation" job adequately, and provides enough extension points for people to build on it should they so wish.

Priorities at the time of writing:
* Find and zap any remaining bugs

Breaking changes lined up for v3:
* Probably change the ref params in ..Visitor{TState} classes - was thinking about value types,
but most of the time this isn't what we will want - can always add ..RefVisitor{TState}, or ..VisitorR{TState}
* Rule indexing support in the chaining knowledge bases - something similar to resolution's IClauseStore.
Will probably involve breaking changes to ctor of these KBs.

On the TODO list for later:
* A parser for sentences expressed as strings (mostly an excuse to play with ANTLR)