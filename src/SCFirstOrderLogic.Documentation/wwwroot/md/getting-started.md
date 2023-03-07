# Getting Started

NB: This guide assumes some familiarity with first-order logic. It won't explain first-order logic concepts - it'll just explain how to use FoL via this library.

## Writing Sentences

The first challenge is to write sentences. This can be done in several ways. This section will use the following sentence as an example:

`∀ g, c, IsGrandparentOf(g, c) ⇔ [∃ p, IsParentOf(g, p) ∧ IsParentOf(p, c)]`

..that is, the definition of a grandparent. We use this example because its very straightforward but includes a decent cross-section of FoL elements.

### Writing Sentences as Code - Directly

The most direct way to express sentences is to directly compose instances of the types found in the top-level `SCFirstOrderLogic` namespace into a tree structure, like this:

```
using SCFirstOrderLogic;

// Helper methods for creating your predicates (and functions) are recommended to avoid repetition:
Predicate IsGrandparent(Term grandparent, Term grandchild) => new(nameof(IsGrandparent), grandparent, grandchild);
Predicate IsParent(Term parent, Term child) => new(nameof(IsParent), parent, child);

// Now the sentence itself, with some intermediate variables so that it isn't completely unreadable:
var g = new VariableDeclaration("g");
var c = new VariableDeclaration("c");
var p = new VariableDeclaration("p");
var equivalenceLHS = IsGrandparent(g, c);
var equivalenceRHS = new ExistentialQuantification(p, new Conjunction(IsParent(g, p), IsParent(p, c)));
Sentence grandparentDefn = new UniversalQuantification(g, new UniversalQuantification(c, new Equivalence(equivalenceLHS, equivalenceRHS)));
```

Notice that:

* This is very "simple" in that it involves nothing other than the sentence types themselves, but is obviously far too verbose to be workable. Hence the alternatives.

### Writing Sentences as Code with SentenceFactory

The `SentenceCreation` namespace contains a static class called `SentenceFactory`. It is intended to be used via a `using static` directive,
and includes a number of static methods and properties to assist with succinct sentence creation. Here's how the example looks with this one:

```
using SCFirstOrderLogic;
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

// Helper methods for creating your predicates (and functions) are recommended to avoid repetition:
Predicate IsGrandparent(Term grandparent, Term grandchild) => new Predicate(nameof(IsGrandparent), grandparent, grandchild);
Predicate IsParent(Term parent, Term child) => new Predicate(nameof(IsParent), parent, child);

// Using the factory, the sentence itself is a little less verbose:
var grandparentDefn = ForAll(G, C, Iff(IsGrandparent(G, C), ThereExists(P, And(IsParent(G, P), IsParent(P, C)))));
```

Notice that:

* The factory provides `ForAll` and `ThereExists` methods for creating quantifications. There are overloads for declaring multiple variables at once.
* The factory provides `If` and `Iff` methods for creating implications and equivalences, respectively.
* The factory provides methods for conjunctions (`And`), disjunctions (`Or`) and negations (`Not`). See the next two examples if you really want to use C# operators for these.
* The factory provides `A` through `Z` as properties that return variable declarations with these letters as their symbol. There is also a `Var` method that allows you to specify an identifier.

### Writing Sentences as Code with OperableSentenceFactory

The `SentenceCreation` namespace also contains a static class called `OperableSentenceFactory`. It works similarly to `SentenceFactory`, but lets you use operators:

```
using SCFirstOrderLogic;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

// Helper methods for creating your predicates (and functions) are recommended to avoid repetition
// (NB: the return type here is OperablePredicate, not Predicate):
OperablePredicate IsGrandparent(Term grandparent, Term grandchild) => new Predicate(nameof(IsGrandparent), grandparent, grandchild);
OperablePredicate IsParent(Term parent, Term child) => new Predicate(nameof(IsParent), parent, child);

// This is the most succinct of the code-based approaches:
var grandparentDefn = ForAll(G, C, Iff(IsGrandparent(G, C), ThereExists(P, IsParent(G, P) & IsParent(P, C))));
```

Notice that:

* We've only used `&` (for a conjunction) above, but `|` works for disjunctions, and `!` for negations. In addition, `==` (applied to terms rather than sentences, of course) works for the equality predicate.
* Other aspects of this factory are the same as `SentenceFactory` - it also offers `ThereExists`, `ForAll`, `Iff`, `If` and single-letter variable declaration properties.
* The only proviso is that the supporting methods for domain-specific elements now need to use `Operable..` as their return type - which is easy as these types are implicitly convertible from the normal equivalents.

### Writing Sentences as Code with LanguageIntegration

The `LanguageIntegration` namespace contains classes for writing sentences in a language-integrated manner. The `SentenceFactory` in this namespace is based on the idea of
modelling the domain as an IEnumerable&lt;T&gt;, then expressing our sentence as a boolean-valued LINQ expression. Like this:

```
using SCFirstOrderLogic.LanguageIntegration;
using static SCFirstOrderLogic.LanguageIntegration.Operators; // Contains Iff an If methods

// The helper methods recommended for the other approaches become full interfaces
// when language integration is used (no implementation is needed):
interface IPerson
{
    bool IsParentOf(IPerson person);
    bool IsGrandparentOf(IPerson person);
}

// The sentence is expressed as a boolean-valued lambda that we are asserting will evaluate
// as true when provided an enumerable representing the domain. Quantifiers are provided
// via the "Any" and "All" LINQ to objects extension methods (the library provides some overloads
// for declaring multiple variables at once).
var grandparentDefn = 
    SentenceFactory.Create<IPerson>(d => d.All((g, c) => Iff(g.IsGrandparentOf(c), d.Any(p => g.IsParentOf(p) && p.IsParentOf(c)))));
```

Notice that:

* This is obviously non-trivial - more information can be found on the [language integration](beyond-getting-started/language-integration.md) page.

### Writing Sentences as Strings

The `SentenceCreation` namespace contains a `SentenceParser` class, that facilitates the expression of sentences as strings, like this:

```
using SCFirstOrderLogic.SentenceCreation;

var grandparentDefn = SentenceParser.Parse("∀ g, c, IsGrandparentOf(g, c) ⇔ [∃ p, IsParentOf(g, p) ∧ IsParentOf(p, c)]");
```

Notes:
* *The grammar definition is [here](https://github.com/sdcondon/SCFirstOrderLogic/blob/main/src/SCFirstOrderLogic/SentenceCreation/FirstOrderLogic.g4).*
* Writing strings that include the proper FoL symbols might be awkward, so the parser allows for some alternatives to be used. The following are recognised (NB all **case sensitive**):
  * `∀` ([U+21D2](https://www.google.com/search?q=U%2B21D2)) or `FOR-ALL` for universal quantifications
  * `∃` ([U+2203](https://www.google.com/search?q=U%2B2203)) or `THERE-EXISTS` for existential quantifications
  * `∧` ([U+2227](https://www.google.com/search?q=U%2B2227)) or `AND` for conjunctions
  * `∨` ([U+2228](https://www.google.com/search?q=U%2B2228)) or `OR` for disjunctions
  * `¬` ([U+00AC](https://www.google.com/search?q=U%2B00AC)) or `NOT` for negations
  * `⇒` ([U+21D2](https://www.google.com/search?q=U%2B21D2)), `->` or `=>` for implications
  * `⇔` ([U+21D4](https://www.google.com/search?q=U%2B21D4)), `<->` or `<=>` for equivalences
* You can use `[ ... ]` or `( ... )` for bracketing sub-sentences.
* Constant, variable, function and predicate identifiers must be alphanumeric (i.e. must match the regex `[A-Za-z0-9]+`).
* To create a predicate that refers to the `EqualitySymbol` type (and thus capable of being leveraged by KBs that have particular handling for equality, etc), use `{term} = {term}`.
* An identifier where a term is expected is interpreted as a variable reference if a matching declaration (from a quantification) is in scope - otherwise it is interpreted as a constant.
* All identifiers are **case sensitive**. This is, for example, something to double-check if you're seeing something intepreted as a constant that you intend as a variable reference.

## Storing Knowledge and Making Inferences

Once you have some sentences, storing them and making inferences is done with the aid of the types in the `Inference` namespace.
The most important type here is the `IKnowledgeBase` interface. The library includes a few very simple knowledge bases - one that
uses forward chaining, one that uses backward chaining, and one that uses resolution. Some examples follow, but first, here's our domain,
taken from section 9.3 of 'Artificial Intelligence: A Modern Approach':

```
using SCFirstOrderLogic;
using SCFirstOrderLogic.SentenceCreation;
using System.Linq;

var rules = new[]
{
    // "... it is a crime for an American to sell weapons to hostile nations":
    "∀ x, y, z, IsAmerican(x) ∧ IsWeapon(y) ∧ Sells(x, y, z) ∧ IsHostile(z) ⇒ IsCriminal(x)",

    // "Nono... has some missiles."
    "∃ x, IsMissile(x) ∧ Owns(Nono, x)",

    // "All of its missiles were sold to it by Colonel West":
    "∀ x, IsMissile(x) ∧ Owns(Nono, x) ⇒ Sells(West, x, Nono)",

    // We will also need to know that missiles are weapons: 
    "∀ x, IsMissile(x) ⇒ IsWeapon(x)",

    // And we must know that an enemy of America counts as "hostile":
    "∀ x, IsEnemyOf(x, America) ⇒ IsHostile(x)",

    // "West, who is American..":
    "IsAmerican(West)",

    // "The country Nono, an enemy of America..":
    "IsEnemyOf(Nono, America)",

}.Select(s => SentenceParser.Parse(s));
```

### Using Forward Chaining

```
using System;
using SCFirstOrderLogic.Inference; // For the "Tell" and "Ask" extension methods - IKnowledgeBase is very async
using SCFirstOrderLogic.Inference.ForwardChaining;

// .. paste the domain listing here ..

// Note that the knowledge base ctor has a "clause store" parameter.
// The clause store takes responsibility for the storage and lookup of
// individual clauses. The package provides only HashSetClauseStore, which stores
// things in memory. This is an extension point - you can create your own implementation
// of IClauseStore to use secondary storage and/or customised indexing, for example.
var kb = new ForwardChainingKnowledgeBase(new HashSetClauseStore());
kb.Tell(rules);
var querySentence = SentenceParser.Parse("IsCriminal(West)");

// Succinct way to get a true/false result:
Console.WriteLine(kb.Ask(querySentence)); // "True"

// Or, to get an explanation:
var query = kb.CreateQuery(querySentence);
query.Execute();
Console.WriteLine(query.Result); // "True"
Console.WriteLine(query.ResultExplanation); // A human-readable walkthrough of the proof tree(s)
// ..note that there are also other properties that facilitate programmatic exploration of the proof tree(s)
```

### Using Backward Chaining

```
using SCFirstOrderLogic.Inference; // For the "Tell" and "Ask" extension methods
using SCFirstOrderLogic.Inference.BackwardChaining;

// .. paste the domain listing here ..

// As with forward chaining, backward chaining KB uses a clause store.
// Also as with forward chaining, the only store provided by the library
// just stores things in memory.
var kb = new BackwardChainingKnowledgeBase(new DictionaryClauseStore());
kb.Tell(rules);
var result = kb.Ask(SentenceParser.Parse("IsCriminal(West)")); // == true
// ..Or can get an explanation in the same way as above
```

### Using Resolution

```
using SCFirstOrderLogic.Inference; // For the "Tell" and "Ask" extension methods
using SCFirstOrderLogic.Inference.Resolution;

// .. paste the domain listing here ..

// The resolution KB has a little more configurability/extensibility than the other two:
var kb = new ResolutionKnowledgeBase(new DelegateResolutionStrategy(
    new HashSetClauseStore(),
    DelegateResolutionStrategy.Filters.None,
    DelegateResolutionStrategy.PriorityComparisons.UnitPreference));

kb.Tell(rules);
var result = kb.Ask(SentenceParser.Parse("IsCriminal(West)")); // == true
// ..Or can get an explanation in the same way as above
```

Notice that:

* Formatting of sentences and query result explanations includes the appropriate symbols (∀, ⇔, ∃, ∧ and so on).
  Depending on your environment, you might need to take action so that these are outputted properly.
  E.g. For running on Windows it might be worth adding `Console.OutputEncoding = Encoding.Unicode;` to your application start-up.
* The `Tell`, `Ask`, `CreateQuery` and `Execute` methods used above are actually extension methods that are are synchronous wrappers around underlying async versions.
  The library has deep async support - because "real-world" KBs (or rather, real-world clause stores) will tend to need to do IO.

## Where Next?

Take a look at [beyond getting started](beyond-getting-started).
