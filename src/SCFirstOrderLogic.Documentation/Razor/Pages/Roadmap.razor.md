# Roadmap

I'm *almost* at the point that I consider the core SCFirstOrderLogic package "done". At some point perhaps toward the middle of 2026, v9 might roll around, in which I might:

* Make some adjustments to the FormulaFormatting namespace - I'm not very happy with it as it stands
* Perhaps add a few things to the FormulaManipulation.Substitution namespace - slightly more advanced unification stuff.

..but, at the present time at least, I don't see this package as needing much more than that.

After that, might turn my attention to look at the inference package a bit more - maybe even implement some KBs that conceivably *could* be of use in a prod environment. For example:

* Further development of the linear resolution strategy, gradually heading towards SLD resolution.
* Some improvements to the chaining knowledge bases. More flexibility when chaining. The current implementations are very constrained in their consideration of clauses, don't handle loops, etc.
