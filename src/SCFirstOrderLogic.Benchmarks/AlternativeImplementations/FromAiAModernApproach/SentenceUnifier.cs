////using System;
////using System.Collections.Generic;
////using System.Linq;

////namespace SCFirstOrderLogic.SentenceManipulation
////{
////    /*
////     * function UNIFY(x, y, θ) returns a substitution to make x and y identical
////     *   inputs:
////     *     x, a variable, constant, list, or compound expression
////     *     y, a variable, constant, list, or compound expression
////     *     θ, the substitution built up so far (optional, defaults to empty)
////     *   
////     *   if θ = failure then return failure
////     *   else if x = y then return θ
////     *   else if VARIABLE?(x) then return UNIFY-VAR(x, y, θ)
////     *   else if VARIABLE?(y) then return UNIFY-VAR(y, x, θ)
////     *   else if COMPOUND?(x) and COMPOUND?(y) then
////     *     return UNIFY(x .ARGS, y.ARGS, UNIFY(x.OP, y.OP, θ))
////     *   else if LIST?(x) and LIST?(y) then
////     *     return UNIFY(x .REST, y.REST, UNIFY(x.FIRST, y.FIRST, θ))
////     *   else return failure
////     *   
////     * function UNIFY-VAR(var, x, θ) returns a substitution
////     * if {var/val} ∈ θ then return UNIFY(val, x, θ)
////     * else if {x/val} ∈ θ then return UNIFY(var, val, θ)
////     * else if OCCUR-CHECK?(var, x) then return failure
////     * else return add {var /x} to θ
////     *
////       Russell, Stuart; Norvig, Peter. Artificial Intelligence: A Modern Approach, Global Edition (p. 328). Pearson Education Limited. Kindle Edition. 
////     */
////    public class SentenceUnifier
////    {
////        public bool Unify(Sentence x, Sentence y, out IDictionary<Variable, Term> unifier)
////        {
////            unifier = new Dictionary<Variable, Term>();

////            if (TryUnify(x, y, unifier))
////            {
////                unifier = null;
////                return false;
////            }

////            return true;
////        }

////        private Substitution Unify(Sentence x, Sentence y, Substitution substitution)
////        {
////            // TODO-PERFORMANCE: Given the fundamentality of unification and the number of times that this could be called during inference,
////            // it might be worth optimising it a little via a visitor-style design instead of this type switch..
////            return (x, y) switch
////            {
////                (Conjunction conjunctionX, Conjunction conjunctionY) => TryUnify(conjunctionX, conjunctionY, unifier),
////                (Disjunction disjunctionX, Disjunction disjunctionY) => TryUnify(disjunctionX, disjunctionY, unifier),
////                (Equality equalityX, Equality equalityY) => TryUnify(equalityX, equalityY, unifier),
////                (Equivalence equivalenceX, Equivalence equivalenceY) => TryUnify(equivalenceX, equivalenceY, unifier),
////                //(ExistentialQuantification existentialQuantificationX, ExistentialQuantification existentialQuantificationY) => TryUnify(existentialQuantificationX, existentialQuantificationY, unifier),
////                (Implication implicationX, Implication implicationY) => TryUnify(implicationX, implicationY, unifier),
////                (Negation negationX, Negation negationY) => TryUnify(negationX, negationY, unifier),
////                (Predicate predicateX, Predicate predicateY) => TryUnify(predicateX, predicateY, unifier),
////                //(UniversalQuantification universalQuantificationX, UniversalQuantification universalQuantificationY) => TryUnify(universalQuantificationX, universalQuantificationY, unifier),
////                _ => false
////            };
////        }

////        private bool TryUnify(Conjunction x, Conjunction y, IDictionary<Variable, Term> unifier)
////        {
////            // BUG: Order shouldn't matter (but need to be careful about partially updating unifier)
////            // perhaps Low and High (internal) props in conjunction?
////            return TryUnify(x.Left, y.Left, unifier) && TryUnify(x.Right, y.Right, unifier);
////        }

////        private bool TryUnify(Disjunction x, Disjunction y, IDictionary<Variable, Term> unifier)
////        {
////            // BUG: Order shouldn't matter (but need to be careful about partially updating unifier)
////            // perhaps Low and High (internal) props in conjunction?
////            return TryUnify(x.Left, y.Left, unifier) && TryUnify(x.Right, y.Right, unifier);
////        }

////        private bool TryUnify(Equality x, Equality y, IDictionary<Variable, Term> unifier)
////        {
////            // BUG: Order shouldn't matter (but need to be careful about partially updating unifier)
////            // perhaps Low and High (internal) props in conjunction?
////            return TryUnify(x.Left, y.Left, unifier) && TryUnify(x.Right, y.Right, unifier);
////        }

////        private bool TryUnify(Equivalence x, Equivalence y, IDictionary<Variable, Term> unifier)
////        {
////            // BUG: Order shouldn't matter (but need to be careful about partially updating unifier)
////            // perhaps Low and High (internal) props in conjunction?
////            return TryUnify(x.Left, y.Left, unifier) && TryUnify(x.Right, y.Right, unifier);
////        }

////        ////private Sentence TryUnify(ExistentialQuantification x, ExistentialQuantification y, IDictionary<Variable, Term> unifier)
////        ////{
////        ////    var variable = ApplyToVariableDeclaration(existentialQuantification.Variable);
////        ////    var sentence = ApplyToSentence(existentialQuantification.Sentence);
////        ////    if (variable != existentialQuantification.Variable || sentence != existentialQuantification.Sentence)
////        ////    {
////        ////        return new ExistentialQuantification(variable, sentence);
////        ////    }
////        ////
////        ////    return existentialQuantification;
////        ////}

////        private bool TryUnify(Implication x, Implication y, IDictionary<Variable, Term> unifier)
////        {
////            return TryUnify(x.Antecedent, y.Antecedent, unifier) && TryUnify(x.Consequent, y.Consequent, unifier);
////        }

////        private bool TryUnify(Negation x, Negation y, IDictionary<Variable, Term> unifier)
////        {
////            return TryUnify(x.Sentence, y.Sentence, unifier);
////        }

////        private bool TryUnify(Predicate x, Predicate y, IDictionary<Variable, Term> unifier)
////        {
////            if (!x.Symbol.Equals(y.Symbol))
////            {
////                return false;
////            }

////            foreach (var args in x.Arguments.Zip(y.Arguments, (x, y) => (x, y)))
////            {
////                if (!TryUnify(args.x, args.y, unifier))
////                {
////                    return false;
////                }
////            }

////            return true;
////        }

////        ////private bool TryUnify(UniversalQuantification x, UniversalQuantification y, IDictionary<Variable, Term> unifier)
////        ////{
////        ////    var variable = ApplyToVariableDeclaration(universalQuantification.Variable);
////        ////    var sentence = ApplyToSentence(universalQuantification.Sentence);
////        ////    if (variable != universalQuantification.Variable || sentence != universalQuantification.Sentence)
////        ////    {
////        ////        return new UniversalQuantification(variable, sentence);
////        ////    }
////        ////
////        ////    return universalQuantification;
////        ////}

////        private bool TryUnify(Term x, Term y, IDictionary<Variable, Term> unifier)
////        {
////            return (x, y) switch
////            {
////                (Variable variable, _) => TryUnify(variable, y, unifier),
////                (_, Variable variable) => TryUnify(variable, x, unifier),
////                (Function functionX, Function functionY) => TryUnify(functionX, functionY, unifier),
////                _ => x.Equals(y), // only potential for equality is if they're both constants. Worth being explicit?
////            };
////        }

////        private bool TryUnify(Variable variable, Term other, IDictionary<Variable, Term> unifier)
////        {
////            if (unifier.TryGetValue(variable, out var value))
////            {
////                return TryUnify(other, value, unifier);
////            }
////            else if (other is Variable otherVariable && unifier.TryGetValue(otherVariable, out value))
////            {
////                return TryUnify(variable, value, unifier);
////            }
////            ////else if (Occurs(variable, other))
////            ////{
////            ////    return false;
////            ////}
////            else
////            {
////                unifier[variable] = other;
////                return true;
////            }
////        }

////        private bool TryUnify(Function x, Function y, IDictionary<Variable, Term> unifier)
////        {
////            if (!x.Symbol.Equals(y.Symbol))
////            {
////                return false;
////            }

////            foreach (var args in x.Arguments.Zip(y.Arguments, (x, y) => (x, y)))
////            {
////                if (!TryUnify(args.x, args.y, unifier))
////                {
////                    return false;
////                }
////            }

////            return true;
////        }

////        // NB: we use the algorithm exactly as listed in the book, not even subsituting in a thrown exception
////        // for the "failure" value - the book plays it a little fast and loose with types - i guess assuming
////        // a dynamically typed system (but why then does it say that theta is a substitation..).
////        // ..anyway, we create our own "substitution" type so that we can exactly copy the algorithm as listed
////        // in the book
////        private class Substitution
////        {
////            private readonly IDictionary<Variable, Term> variables;

////            private Substitution(bool isFailure)
////            {
////                IsFailure = isFailure;
////                variables = new Dictionary<Variable, Term>();
////            }

////            public IDictionary<Variable, Term> Mapping => !IsFailure ? variables : throw new InvalidOperationException();

////            public bool IsFailure { get; }
////        }
////    }
////}
