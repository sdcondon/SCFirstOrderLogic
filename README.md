![SCFirstOrderLogic Icon](src/SCFirstOrderLogic-128.png)

# SCFirstOrderLogic

[![NuGet version (SCFirstOrderLogic)](https://img.shields.io/nuget/v/SCFirstOrderLogic.svg?style=flat-square)](https://www.nuget.org/packages/SCFirstOrderLogic/) 
[![NuGet downloads (SCFirstOrderLogic)](https://img.shields.io/nuget/dt/SCFirstOrderLogic.svg?style=flat-square)](https://www.nuget.org/packages/SCFirstOrderLogic/) 
[![Commits since latest release](https://img.shields.io/github/commits-since/sdcondon/SCFirstOrderLogic/latest?style=flat-square)](https://github.com/sdcondon/SCFirstOrderLogic/compare/4.1.2...main)

This repository contains the source code for the SCFirstOrderLogic NuGet package - along with its tests, performance benchmarks, and documentation website.
It also contains some example domains and alternative algorithm implementations, which are used in the tests and performance benchmarks. 

## Package Documentation

For documentation of the package itself, see https://sdcondon.net/SCFirstOrderLogic/.

## Source Documentation

I have not written up any documentation of the source (e.g. repo summary, design discussion, build guidance) - and likely won't unless someone else expresses an interest in contributing.
Once cloned, it should "just work" as far as compilation is concerned.
The only thing perhaps worthy of note is that it uses [Antlr4BuildTasks](https://github.com/kaby76/Antlr4BuildTasks) to invoke ANTLR to generate the FoL-parsing code.
ANTLR is a Java tool - something Antlr4BuildTasks handles by downloading the JRE to your system for you - see its docs for details.

## Issues and Contributions

I'm not expecting anyone to want to get involved given the low download figures on NuGet, but please feel free to do so.
I do keep an eye on the [issues](https://github.com/sdcondon/SCFirstOrderLogic/issues) tab, and will add a CONTRIBUTING.md if anyone drops me a message expressing interest.
Do bear in mind that I have a very particular scope in mind for the library, though - see the [roadmap](https://sdcondon.net/SCFirstOrderLogic/roadmap.md) for details.

## See Also

You might also be interested in:

* [My GitHub Profile](https://github.com/sdcondon): My profile README lists several of my more interesting public projects.
* My other AI-related stuff: 
  * [SCClassicalPlanning](https://github.com/sdcondon/SCClassicalPlanning): Basic classical planning implementations. Based on chapter 10 of "Artificial Intelligence: A Modern Approach" - though perhaps a _little_ more loosely than this repo is based on chapters 8 and 9. Specifically, introduces a couple more PDDL concepts that the book doesn't bother with (but should IMO). Depends on this library (and [SCGraphTheory.Search](https://github.com/sdcondon/SCGraphTheory.Search)).
  * [SCPropositionalLogic](https://github.com/sdcondon/SCPropositionalLogic): Basic propositional logic implementations. Based on chapter 7 of "Artificial Intelligence: A Modern Approach". Mostly just a precursor to this repo - I haven't bothered publishing this one to NuGet - but might be of use if you find this repo a bit too much.
* A selection of other people's AI-related stuff:
  * [Infer.NET](https://github.com/dotnet/infer): Bayesian inference engine. Bayesian logic of course being strictly more powerful than first-order logic.
  * [AIMA Code - C#](https://github.com/aimacode/aima-csharp): I mention this only because I feel like I should. This is the "official" C# repository for "Artificial Intelligence: A Modern Approach" - and it is.. err.. not great.
