# LinqToKB.FirstOrderLogic

Very simple [First-order logic](https://en.wikipedia.org/wiki/First-order_logic) knowledge base implementations that use LINQ expressions for knowledge representation and queries.

Created just for fun while reading _Artificial Intelligence: A Modern Approach_ (3rd Edition - [ISBN 978-1292153964](https://www.google.com/search?q=isbn+978-1292153964)) - so may prove interesting to the .NET-inclined reading the same book.
The main goal here is for it to be a learning resource - as such, care has been taken to include decent XML documentation and explanatory inline comments where helpful.
For real-world scenarios, there are other better inference engines out there.

Also see [LinqToKB.PropositionalLogic](https://github.com/sdcondon/LinqToKB.PropositionalLogic) - which I strongly suspect this library will end up depending on.

Benefits of using LINQ expressions:
* Your sentences of propositional logic can be expressed in the familiar, plain-old C#, with the operators you would expect (`&&`, `||` and `!` - NB not `&` and `|`, yet..) fulfilling their usual roles.
* Further, your rules are expressed in code that can be executed directly against the domain model (which is useful for verification purposes, probably..)
* LINQ already includes much of the plumbing to make this happen - expression trees, visitor classes etc - meaning that there isn't actually a huge amount that the library needs to add.

Drawbacks of using LINQ expressions:
* LINQ expressions are perhaps larger and slower to work with than custom-built expression classes would be. So this may not be the best choice where performance requirements are particularly stringent.
* By using C#, there is a danger in confusing C# operators with the elements of propositional logic that they are mapped to - creating a risk to learning outcomes.
That is, while it may be intuitive to map the C# `||` operator to a disjunction in PL, they do of course represent distinct things.
Compared to uses of LINQ such as LINQ to SQL (where the system being mapped to is very obviously distinct), it is perhaps less obvious that there IS still a different system (propositional logic) being mapped to here. This is important to bear in mind while working with this library.
* C# doesn't map any near as neatly and unambiguously to the world of first-order logic as it does to propositional logic (see [LinqToKnowledgeBase.PropositionalLogic](https://github.com/sdcondon/LinqToKnowledgeBase.PropositionalLogic)). This is the biggest challenge of implementing this library - I'm not actually 100% convinced that this is doable in a particularly robust & usable way, but there's one way to find out..

## Usage

TODO!

The first thing is that our domain objects should probably be constrained to implement `IEnumerable<>`. This
is similar to LINQ to objects and perhaps even suggests a representation of the `` and `` quantifiers - namely,
the `All` and `Any` extension methods respectively. Where there is more uncertainty is in how we represent
predicates and functions. One option

* (Object type): *: Implicit predicate (is of type..)
* Boolean-valued property: Unary predicate? Could also be a Boolean-valued unary function? Perhaps convention based
* Collection-valued property:


```csharp
var kb = new KnowledgeBase<MyModel>();
kb.Tell(d => o.Any()); // d for domain, o for object..
```
