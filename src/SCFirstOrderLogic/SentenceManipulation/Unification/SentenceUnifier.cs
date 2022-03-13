using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation
{
    public class SentenceUnifier
    {
        public bool TryUnify(Sentence x, Sentence y, [NotNullWhen(returnValue: true)] out IDictionary<VariableReference, Term>? unifier)
        {
            unifier = new Dictionary<VariableReference, Term>();

            if (!TryUnify(x, y, unifier))
            {
                unifier = null;
                return false;
            }

            return true;
        }

        private bool TryUnify(Sentence x, Sentence y, IDictionary<VariableReference, Term> unifier)
        {
            // TODO-PERFORMANCE: Given the fundamentality of unification and the number of times that this could be called during inference,
            // it might be worth optimising it a little via a visitor-style design instead of this type switch..
            return (x, y) switch
            {
                (Conjunction conjunctionX, Conjunction conjunctionY) => TryUnify(conjunctionX, conjunctionY, unifier),
                (Disjunction disjunctionX, Disjunction disjunctionY) => TryUnify(disjunctionX, disjunctionY, unifier),
                (Equality equalityX, Equality equalityY) => TryUnify(equalityX, equalityY, unifier),
                (Equivalence equivalenceX, Equivalence equivalenceY) => TryUnify(equivalenceX, equivalenceY, unifier),
                //(ExistentialQuantification existentialQuantificationX, ExistentialQuantification existentialQuantificationY) => TryUnify(existentialQuantificationX, existentialQuantificationY, unifier),
                (Implication implicationX, Implication implicationY) => TryUnify(implicationX, implicationY, unifier),
                (Negation negationX, Negation negationY) => TryUnify(negationX, negationY, unifier),
                (Predicate predicateX, Predicate predicateY) => TryUnify(predicateX, predicateY, unifier),
                //(UniversalQuantification universalQuantificationX, UniversalQuantification universalQuantificationY) => TryUnify(universalQuantificationX, universalQuantificationY, unifier),
                _ => false
            };
        }

        private bool TryUnify(Conjunction x, Conjunction y, IDictionary<VariableReference, Term> unifier)
        {
            // BUG: Order shouldn't matter (but need to be careful about partially updating unifier)
            // perhaps Low and High (internal) props in conjunction?
            return TryUnify(x.Left, y.Left, unifier) && TryUnify(x.Right, y.Right, unifier);
        }

        private bool TryUnify(Disjunction x, Disjunction y, IDictionary<VariableReference, Term> unifier)
        {
            // BUG: Order shouldn't matter (but need to be careful about partially updating unifier)
            // perhaps Low and High (internal) props in conjunction? Or assume normalised ordering (which at the time of writing WE DONT DO)
            return TryUnify(x.Left, y.Left, unifier) && TryUnify(x.Right, y.Right, unifier);
        }

        private bool TryUnify(Equality x, Equality y, IDictionary<VariableReference, Term> unifier)
        {
            // BUG: Order shouldn't matter (but need to be careful about partially updating unifier)
            // perhaps Low and High (internal) props in conjunction? Or assume normalised ordering (which at the time of writing WE DONT DO)
            return TryUnify(x.Left, y.Left, unifier) && TryUnify(x.Right, y.Right, unifier);
        }

        private bool TryUnify(Equivalence x, Equivalence y, IDictionary<VariableReference, Term> unifier)
        {
            // BUG: Order shouldn't matter (but need to be careful about partially updating unifier)
            // perhaps Low and High (internal) props in conjunction?
            return TryUnify(x.Left, y.Left, unifier) && TryUnify(x.Right, y.Right, unifier);
        }

        ////private Sentence TryUnify(ExistentialQuantification x, ExistentialQuantification y, IDictionary<Variable, Term> unifier)
        ////{
        ////    var variable = ApplyToVariableDeclaration(existentialQuantification.Variable);
        ////    var sentence = ApplyToSentence(existentialQuantification.Sentence);
        ////    if (variable != existentialQuantification.Variable || sentence != existentialQuantification.Sentence)
        ////    {
        ////        return new ExistentialQuantification(variable, sentence);
        ////    }
        ////
        ////    return existentialQuantification;
        ////}

        private bool TryUnify(Implication x, Implication y, IDictionary<VariableReference, Term> unifier)
        {
            return TryUnify(x.Antecedent, y.Antecedent, unifier) && TryUnify(x.Consequent, y.Consequent, unifier);
        }

        private bool TryUnify(Negation x, Negation y, IDictionary<VariableReference, Term> unifier)
        {
            return TryUnify(x.Sentence, y.Sentence, unifier);
        }

        private bool TryUnify(Predicate x, Predicate y, IDictionary<VariableReference, Term> unifier)
        {
            if (!x.Symbol.Equals(y.Symbol))
            {
                return false;
            }

            foreach (var args in x.Arguments.Zip(y.Arguments, (x, y) => (x, y)))
            {
                if (!TryUnify(args.x, args.y, unifier))
                {
                    return false;
                }
            }

            return true;
        }

        ////private bool TryUnify(UniversalQuantification x, UniversalQuantification y, IDictionary<Variable, Term> unifier)
        ////{
        ////    var variable = ApplyToVariableDeclaration(universalQuantification.Variable);
        ////    var sentence = ApplyToSentence(universalQuantification.Sentence);
        ////    if (variable != universalQuantification.Variable || sentence != universalQuantification.Sentence)
        ////    {
        ////        return new UniversalQuantification(variable, sentence);
        ////    }
        ////
        ////    return universalQuantification;
        ////}

        private bool TryUnify(Term x, Term y, IDictionary<VariableReference, Term> unifier)
        {
            return (x, y) switch
            {
                (VariableReference variable, _) => TryUnify(variable, y, unifier),
                (_, VariableReference variable) => TryUnify(variable, x, unifier),
                (Function functionX, Function functionY) => TryUnify(functionX, functionY, unifier),
                _ => x.Equals(y), // only potential for equality is if they're both constants. Worth being explicit?
            };
        }

        private bool TryUnify(VariableReference variable, Term other, IDictionary<VariableReference, Term> unifier)
        {
            if (unifier.TryGetValue(variable, out var value))
            {
                return TryUnify(value, other, unifier);
            }
            else if (other is VariableReference otherVariable && unifier.TryGetValue(otherVariable, out value))
            {
                return TryUnify(variable, value, unifier);
            }
            else if (Occurs(variable, other))
            {
                return false;
            }
            else
            {
                // This substitution is not in the book, but is so that e.g. Knows(John, X) and Knows(Y, Mother(Y)) will have x = Mother(John), not x = Mother(Y)
                other = new VariableSubstituter(unifier).ApplyTo(other);
                unifier[variable] = other;
                return true;
            }
        }

        private bool TryUnify(Function x, Function y, IDictionary<VariableReference, Term> unifier)
        {
            if (!x.Symbol.Equals(y.Symbol))
            {
                return false;
            }

            foreach (var args in x.Arguments.Zip(y.Arguments, (x, y) => (x, y)))
            {
                if (!TryUnify(args.x, args.y, unifier))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool Occurs(VariableReference variable, Term term)
        {
            // TODO*-PERFORMANCE: GC impact when creating a bunch of these.. Mutability and pooling?
            var finder = new VariableFinder(variable);
            finder.ApplyTo(term);
            return finder.IsFound;
        }

        private class VariableFinder : SentenceTransformation
        {
            private readonly VariableReference variableReference;

            public VariableFinder(VariableReference variableReference) => this.variableReference = variableReference;

            public bool IsFound { get; private set; } = false;

            // TODO-PERFORMANCE: For performance, should probably override everything and stop as soon as IsFound is true.
            // And/or establish visitor pattern to make this easier..

            protected override Term ApplyTo(VariableReference variable)
            {
                if (variable.Equals(variableReference))
                {
                    IsFound = true;
                }

                return variable;
            }
        }

        private class VariableSubstituter : SentenceTransformation
        {
            private readonly IDictionary<VariableReference, Term> variableSubstitutions;

            public VariableSubstituter(IDictionary<VariableReference, Term> variableSubstitutions) => this.variableSubstitutions = variableSubstitutions;

            protected override Term ApplyTo(VariableReference variable)
            {
                if (variableSubstitutions.TryGetValue(variable, out var substitutedTerm))
                {
                    return substitutedTerm;
                }

                return variable;
            }
        }
    }
}
