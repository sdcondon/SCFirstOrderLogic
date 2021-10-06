# LinqToKB.FirstOrderLogic

Experimental [first-order logic](https://en.wikipedia.org/wiki/First-order_logic) knowledge base implementations that use LINQ expressions for knowledge representation and queries.
That is, rather than directly giving the knowledge base sentences of first order logic (which obvisouly can't be done in C#), we (represent domains as IEnumerable&lt;TElement&gt; and) `Tell` the knowledge base bool-valued expressions that are guaranteed to be true for all models that it will be asked about - which the knowledge base then converts into the entailed sentences of first order logic.

Created just for fun while reading _Artificial Intelligence: A Modern Approach_ (3rd Edition - [ISBN 978-1292153964](https://www.google.com/search?q=isbn+978-1292153964)) - so may prove interesting to the .NET-inclined reading the same book.
The main goal here is for it to be a learning resource - as such, care has been taken to include decent XML documentation and explanatory inline comments where helpful.
For real-world scenarios, there are other better inference engines out there.

Benefits of using LINQ expressions:
* Your sentences of propositional logic can be expressed in the familiar, plain-old C#, with the operators you would expect (e.g. `&&`, `||` and `!`). The mapping from bool-valued expressions to FoL sentences is straightforward.
* Further, your rules are expressed in code that can be executed directly against the domain model, if an implementation exists (which is useful for verification purposes, probably..)
* LINQ already includes much of the plumbing to make this happen - expression trees, visitor classes etc - making this library simple and maintainable.

Drawbacks of using LINQ expressions:
* Allowing for constants declared at runtime raises some (surmountable) challenges. More on this below.
* By using C#, there is a danger in confusing C# operators with the elements of first order logic that they are mapped to - creating a risk to learning outcomes.
That is, while it may be intuitive to map the C# `||` operator to a disjunction in PL, they do of course represent very different things.
Compared to uses of LINQ such as LINQ to SQL (where the system being mapped to is very obviously distinct), it is perhaps less obvious that there IS still a different system (first order logic) being mapped to here. This is important to bear in mind while working with this library.

## Usage

First off, some important notes regarding overall approach that should make it easier to get to grips with the library:

* As mentioned above, the general approach is to have domain types (can be - in fact is recommended to be - just an interface) that implement IEnumerable&lt;TElement&gt;, where TElement is a type for elements of the domain. This approach provides a few useful qualities:
  * Using IEnumerable&lt;T&gt; gives us an obvious choice for representing quantifiers - the `All` extension method for universal quantification, and `Any` for existential quantification.
  * Allowing the domain to be some type that implements this interface  - as opposed to assuming that a domain is IEnumerable&lt;TElement&gt; itself - is useful because it allows constants and ground predicates to be defined on the domain type.
* You will note that it doesn't really engage with the .NET type system beyond establishing type-safety based on the terminology used in the domain.
That is, there's nothing in here about being able to use the fact that a domain element (as an object in .NET) is of a particular type or not.
Instead, predicates must be defined as properties or methods on a singular TElement type. On the upside, this keeps things simple. On the downside, this means you can't be quite as expressive as you might be hoping.
And as mitigations to the downside:
  1. if you actually need to map to a real instantiated domain (for e.g. validation purposes), writing an adapter that implements predicates based on runtime type should be relatively straightforward. If that doesn't quite make sense, I'm hoping it will once you look at the examples..
  2. if this works, I may revisit this to see what extensions can be made in this area.

The mapping between bool-valued C# expressions (that act on the TDomain) and FoL sentences can be found in the table below.
Note that the overriding rule is that FoL sentences are entailed by the assertion that the C# expression is guaranteed to evaluate to "true" for all models.
See the [ExampleDomains project](./src/FirstOrderLogic.ExampleDomains) for some examples (and note that no knowledge bases are actually implemented yet - this is a WIP).

| **FoL** | **FoL Syntax** | **C# Expression** |
| --- | --- | --- |
| Conjunction | `{sentence} ∧ {sentence}` | `{expression} {&& or &} {expression}` |
|Disjunction|`{sentence} ∨ {sentence}`|`{expression} {\|\| or \|} {expression}`|
|Material equivalence|`{sentence} ⇔ {sentence}`|`Operators.Iff({expression}, {expression})` *|
|Material implication|`{sentence} ⇒ {sentence}`|`Operators.If({expression}, {expression})` *|
|Negation|`¬{sentence}`|`!{expression}`|
|Existential quantification|`∃ {variable}, {sentence}`|`{domain}.Any({variable} => {expression})` †|
|Universal quantification|`∀ {variable}, {sentence}`|`{domain}.All({variable} => {expression})` †|
|Equality|`{sentence} = {sentence}`|`{expression} == {expression}`|
|Predicate|`{predicate symbol}({term}, ..)`|A boolean-valued property or method call on a TElement, or a boolean-valued property or method call on TDomain (for ground predicates).|
|Constant|`{constant symbol}`|Access of a TElement-valued property or parameterless method on TDomain ‡|
|Function|`{function symbol}({term}, ...)`|Invocation of a TElement-valued method on TElement that accepts only TElement-valued paramaters, or access of a TElement-valued property of TElement|
|Variable|`{variable symbol}`|A variable from the lambda passed to All or Any|

\* C# lacks a single operator appropriate for material equivalence and implication, so LinqToKB offers some shorthand methods in the `Operators` static class. Library consumers are encouraged use `using static LinqToKB.FirstOrderLogic.Operators;` where appropriate

† LinqToKB also defines some more overloads of `All` and `Any` that accept multiple parameters - which can help with keeping expressions simple when there are multiple variables involved

‡ How to deal effectively with constants is an open question. At present, they can be declared as TElement-valued props on the domain, but this doesn't facilitate existential instantiation, let alone "real world" scenarios where being able to define constants at run-time is surely a pretty fundamental requirement. Keyed collections of constants seem the obvious approach, but exactly how is the question. Convention-based (e.g. any IDictionary<string, TElement> on the domain type assumed to be a collection of constants, or - better - a TElement valued indexer..) or something stronger (e.g. domains *must* implement IRuntimeConstantContainer&lt;TElement&gt; which defines a string-keyed, TElement-valued indexer?), or something else? The latter seems like it might be needed given the need for algorithms to define constants on the fly.

