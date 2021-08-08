# LinqToKB.FirstOrderLogic

Very simple [First-order logic](https://en.wikipedia.org/wiki/First-order_logic) knowledge base implementations that use LINQ expressions for knowledge representation and queries.

Created just for fun while reading _Artificial Intelligence: A Modern Approach_ (3rd Edition - [ISBN 978-1292153964](https://www.google.com/search?q=isbn+978-1292153964)) - so may prove interesting to the .NET-inclined reading the same book.
The main goal here is for it to be a learning resource - as such, care has been taken to include decent XML documentation and explanatory inline comments where helpful.
For real-world scenarios, there are other better inference engines out there.

Also see [LinqToKB.PropositionalLogic](https://github.com/sdcondon/LinqToKB.PropositionalLogic) - which I strongly suspect this library will end up depending on.

Benefits of using LINQ expressions:
* Your sentences of propositional logic can be expressed in the familiar, plain-old C#, with the operators you would expect (e.g. `&&`, `||` and `!`) fulfilling their usual roles.
* Further, your rules are expressed in code that can be executed directly against the domain model (which is useful for verification purposes, probably..)
* LINQ already includes much of the plumbing to make this happen - expression trees, visitor classes etc - meaning that there isn't actually a huge amount that the library needs to add.

Drawbacks of using LINQ expressions:
* LINQ expressions are perhaps larger and slower to work with than custom-built expression classes would be. So this may not be the best choice where performance requirements are particularly stringent.
* By using C#, there is a danger in confusing C# operators with the elements of first order logic that they are mapped to - creating a risk to learning outcomes.
That is, while it may be intuitive to map the C# `||` operator to a disjunction in PL, they do of course represent distinct things.
Compared to uses of LINQ such as LINQ to SQL (where the system being mapped to is very obviously distinct), it is perhaps less obvious that there IS still a different system (first order logic) being mapped to here. This is important to bear in mind while working with this library.

## Usage

First off, some important notes regarding overall approach that should make it easier to get to grips with the library:

* The general approach is to have domain types (can be, in fact is recommended to be just an interface) that implement IEnumerable<T>, where T is a type for elements of the domain. This approach provides a few useful qualities:
  * Using IEnumerable&lt;T&gt; gives us an obvious choice for representing quantifiers - All for universal quantification, and Any for existential quantification. It also
  * Allowing the domain to be some type that implements this interface  - as opposed to assuming that a domain is IEnumerable<T> itself - is useful because it allows constants and ground predicates to be defined on the domain type.
* You will note that it doesn't really engage with the .NET type system beyond establishing type-safety based on the terminology used in the domain.
That is, there's nothing in here about being able to use the fact that a domain element (as an object in .NET) is of a particular type or not.
Instead, predicates must be defined as properties or methods on a singular TElement type. On the plus side, this keeps things simple. On the downside, this means you can't be quite as expressive as you might be hoping.
And as mitigations to the downside:
  1. if you actually need to map to a real instantiated domain (for e.g. validstion purposes), writing an adapter that implements predicates based on runtime type should be relatively straightforward. If that doesn't quite make sense, I'm hoping it will once you look at the examples..
  2. if this works, I may revisit this to see what extensions can be made in this area.


TODO


```csharp
var kb = new KnowledgeBase<MyModel>();
kb.Tell(d => o.Any()); // d for domain, o for object..
```
