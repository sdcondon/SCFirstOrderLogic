# Roadmap

Long-term, this library will be "done" when I think that it does its "learning and experimentation" job adequately, and provides enough extension points for people to extend it should they so wish - perhaps even to the point of being useful in a production scenario.

Priorities at the time of writing:

* *Nothing at the moment - working on other things for the time being*

Further ahead:

* More improvements to the inference algorithms in the Inference.Basic.Package:
  As mentioned above and elsewhere, this package is more about the fundamentals than especially powerful inference logic, but we should probably cover at least some of the relatively fundamental problems and techniques - the inference demo is rather underwhelming at the moment.
  For example, some or all of:
  * Further development of the linear resolution strategy, gradually heading towards SLD resolution (though if I get that far it will likely go in its own package).
  * Some improvements to the chaining knowledge bases. More flexibility when chaining. The current implementations are very constrained in their consideration of clauses, don't handle loops, etc.
* The next major version bump of the core package is likely to be an overhaul of the sentence formatting logic - I'm not very happy with it as it stands.
