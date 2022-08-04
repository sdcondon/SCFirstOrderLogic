using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.Inference.Unification
{
    /// <summary>
    /// Most general unifier logic implemented as close as possible to the way it
    /// is stated in figure 9.1 of 'Artificial Intelligence: A Modern Approach'.
    /// For learning and benchmarking only - obviously this is a terrible way to 
    /// implement this in C#.
    /// </summary>
    public static class LiteralUnifier_FromAIaMA
    {
        /*
         * function UNIFY(x, y, θ) returns a substitution to make x and y identical
         *   inputs:
         *     x, a variable, constant, list, or compound expression
         *     y, a variable, constant, list, or compound expression
         *     θ, the substitution built up so far (optional, defaults to empty)
         *   
         *   if θ = failure then return failure
         *   else if x = y then return θ
         *   else if VARIABLE?(x) then return UNIFY-VAR(x, y, θ)
         *   else if VARIABLE?(y) then return UNIFY-VAR(y, x, θ)
         *   else if COMPOUND?(x) and COMPOUND?(y) then
         *     return UNIFY(x.ARGS, y.ARGS, UNIFY(x.OP, y.OP, θ))
         *   else if LIST?(x) and LIST?(y) then
         *     return UNIFY(x.REST, y.REST, UNIFY(x.FIRST, y.FIRST, θ))
         *   else return failure
         *
         * Figure 9.1
         * Russell, Stuart; Norvig, Peter. Artificial Intelligence: A Modern Approach, Global Edition (p. 328). Pearson Education Limited. Kindle Edition. 
         */
        public static Substitution Unify(object x, object y, Substitution? θ)
        {
            θ ??= new Substitution();

            if (θ == Substitution.Failure)
                return Substitution.Failure;
            else if (EffectivelyEqual(x, y))
                return θ;
            else if (IsVariable(x))
                return UnifyVar(x, y, θ);
            else if (IsVariable(y))
                return UnifyVar(y, x, θ);
            else if (IsCompound(x) && IsCompound(y))
                return Unify(ArgsOf(x), ArgsOf(y), Unify(OperatorOf(x), OperatorOf(y), θ));
            else if (IsList(x) && IsList(y))
                return Unify(RestOf(x), RestOf(y), Unify(FirstOf(x), FirstOf(y), θ));
            else
                return Substitution.Failure;
        }

        /*
         * function UNIFY-VAR(var, x, θ) returns a substitution
         *   if {var/val} ∈ θ then return UNIFY(val, x, θ)
         *   else if {x/val} ∈ θ then return UNIFY(var, val, θ)
         *   else if OCCUR-CHECK?(var, x) then return failure
         *   else return add {var /x} to θ
         *
         * Figure 9.1
         * Russell, Stuart; Norvig, Peter. Artificial Intelligence: A Modern Approach, Global Edition (p. 328). Pearson Education Limited. Kindle Edition. 
         */
        private static Substitution UnifyVar(object @var, object x, Substitution θ)
        {
            if (θ.TryGetValue(var, out var varval))
                return Unify(varval, x, θ);
            else if (θ.TryGetValue(x, out var xval))
                return Unify(@var, xval, θ);
            ////else if (OccurCheck(var, x))
            ////    return Substitution.Failure;
            else
                return θ.Add(@var, x);
        }

        private static bool EffectivelyEqual(object x, object y)
        {
            return (x, y) switch
            {
                (Constant cx, Constant cy) => cx.Symbol.Equals(cy.Symbol),
                (VariableReference vx, VariableReference vy) => vx.Declaration.Symbol.Equals(vy.Declaration.Symbol),
                (Operator ox, Operator oy) => ox.Symbol.Equals(oy.Symbol),
                (List<object> lx, List<object> ly) => lx.Count == 0 && ly.Count == 0, // NB: Important aspect of this implementation is that empty lists are considered equal

                // Otherwise its e.g. Conjunction, Disjunction, Equality, Equivalence, ExistentialQuantification, Function, Implication, Negation, Predicate, UniversalQuantification..
                // We could of course have identical conjunctions etc, but we'll discover that in due course if so. The difficulty (in terms of understanding) here is the vague notion of
                // equality, not clarified by the source material. What this is really doing is checking if the things under consideration are "atomic" (w.r.t. this algorithm) and
                // equal. The things that are "atomic" for this algorithm are constants, variable references, "operators" (i.e. function, predicate or operator symbols), and EMPTY argument
                // lists. Everything else needs to be recursed into to determine unifiability.
                _ => false 
            };
        }

        private static bool IsVariable(object x) => x is VariableReference;

        private static bool IsCompound(object x)
        {
            return x switch
            {
                Conjunction => true,
                Disjunction => true,
                Equivalence => true,
                ExistentialQuantification => true,
                Function => true,
                Implication => true,
                Negation => true,
                Predicate => true,
                UniversalQuantification => true,
                _ => false // e.g. Constant, VariableReference or List<object>..
            };
        }

        private static object OperatorOf(object x)
        {
            // not perfect, of course - e.g. there's nothing stopping a function from having the symbol "∧"
            return new Operator(x switch
            {
                Conjunction => "∧",
                Disjunction => "∨",
                Equivalence => "⇔",
                ExistentialQuantification => "∃",
                Function f => f.Symbol,
                Implication => "⇒",
                Negation => "¬",
                Predicate p => p.Symbol,
                UniversalQuantification => "∀",
                _ => throw new ArgumentException("Arg is not a compound", nameof(x)) // e.g. Constant, VariableReference or List<object>..
            });
        }

        private static object ArgsOf(object x)
        {
            return x switch
            {
                Conjunction c => new List<object> { c.Left, c.Right },
                Disjunction d => new List<object> { d.Left, d.Right },
                Equivalence e => new List<object> { e.Left, e.Right },
                ExistentialQuantification eq => eq.Sentence,
                Function f => f.Arguments.ToList<object>(),
                Implication i => new List<object> { i.Antecedent, i.Consequent },
                Negation n => n.Sentence,
                Predicate p => p.Arguments.ToList<object>(),
                UniversalQuantification uq => uq.Sentence,
                _ => throw new ArgumentException("Arg is not a compound", nameof(x)) // e.g. Constant, VariableReference or List<object>..
            };
        }

        private static bool IsList(object x) => x is List<object>;

        private static object FirstOf(object x)
        {
            if (x is not List<object> l)
            {
                throw new ArgumentException("Arg is not a list");
            }

            return l.First();
        }

        private static object RestOf(object x)
        {
            if (x is not List<object> l)
            {
                throw new ArgumentException("Arg is not a list");
            }

            return l.Skip(1).ToList();
        }

        // NB: we use the algorithm exactly as listed in the book, not even subsituting in a thrown exception
        // for the "failure" value - the book plays it a little fast and loose with types - I guess assuming
        // a dynamically typed system (but why then does it say that theta is a substitation..).
        // ..anyway, we create our own "substitution" type so that we can exactly copy the algorithm as listed
        // in the book
        public class Substitution
        {
            public object this[object x]
            {
                get => Mapping[x];
                set => Mapping[x] = value;
            }

            public IDictionary<object, object> Mapping { get; } = new Dictionary<object, object>();

            public bool TryGetValue(object x, [MaybeNullWhen(false)] out object val) => Mapping.TryGetValue(x, out val);

            public Substitution Add(object key, object value)
            {
                var s = new Substitution();
                foreach (var kvp in Mapping)
                {
                    s[kvp.Key] = kvp.Value;
                }

                s[key] = value;

                return s;
            }

            public static Substitution Failure { get; } = new Substitution();
        }

        private class Operator
        {
            public Operator(object symbol) => Symbol = symbol;

            public object Symbol { get; }

            public override bool Equals(object? obj) => obj is Operator other && Symbol.Equals(other.Symbol);

            public override int GetHashCode() => HashCode.Combine(Symbol);
        }
    }
}
