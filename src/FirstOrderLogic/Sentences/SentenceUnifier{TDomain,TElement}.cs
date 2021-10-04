using LinqToKB.FirstOrderLogic.InternalUtilities;
using System.Collections.Generic;
using System.Linq;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    public class SentenceUnifier<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        public bool TryUnify(
            Sentence<TDomain, TElement> x,
            Sentence<TDomain, TElement> y,
            out IDictionary<Variable<TDomain, TElement>, Term<TDomain, TElement>> unifier)
        {
            unifier = new Dictionary<Variable<TDomain, TElement>, Term<TDomain, TElement>>();
            return TryUnify(x, y, unifier);
        }

        private bool TryUnify(
            Sentence<TDomain, TElement> x,
            Sentence<TDomain, TElement> y,
            IDictionary<Variable<TDomain, TElement>, Term<TDomain, TElement>> unifier)
        {
            // TODO-PERFORMANCE: Given the fundamentality of unification and the number of times that this could be called during inference,
            // it might be worth optimising it a little via a visitor-style design instead of this type switch..
            return (x, y) switch
            {
                (Conjunction<TDomain, TElement> conjunctionX, Conjunction<TDomain, TElement> conjunctionY) => TryUnify(conjunctionX, conjunctionY, unifier),
                (Disjunction<TDomain, TElement> disjunctionX, Disjunction<TDomain, TElement> disjunctionY) => TryUnify(disjunctionX, disjunctionY, unifier),
                (Equality<TDomain, TElement> equalityX, Equality<TDomain, TElement> equalityY) => TryUnify(equalityX, equalityY, unifier),
                (Equivalence<TDomain, TElement> equivalenceX, Equivalence<TDomain, TElement> equivalenceY) => TryUnify(equivalenceX, equivalenceY, unifier),
                //(ExistentialQuantification<TDomain, TElement> existentialQuantificationX, ExistentialQuantification<TDomain, TElement> existentialQuantificationY) => TryUnify(existentialQuantificationX, existentialQuantificationY, unifier),
                (Implication<TDomain, TElement> implicationX, Implication<TDomain, TElement> implicationY) => TryUnify(implicationX, implicationY, unifier),
                (Negation<TDomain, TElement> negationX, Negation<TDomain, TElement> negationY) => TryUnify(negationX, negationY, unifier),
                (Predicate<TDomain, TElement> predicateX, Predicate<TDomain, TElement> predicateY) => TryUnify(predicateX, predicateY, unifier),
                //(UniversalQuantification<TDomain, TElement> universalQuantificationX, UniversalQuantification<TDomain, TElement> universalQuantificationY) => TryUnify(universalQuantificationX, universalQuantificationY, unifier),
                _ => false
            };
        }

        private bool TryUnify(
            Conjunction<TDomain, TElement> x,
            Conjunction<TDomain, TElement> y,
            IDictionary<Variable<TDomain, TElement>, Term<TDomain, TElement>> unifier)
        {
            // BUG: Order shouldn't matter (but need to be careful about partially updating unifier)
            // perhaps Low and High (internal) props in conjunction?
            return TryUnify(x.Left, y.Left, unifier) && TryUnify(x.Right, y.Right, unifier);
        }

        public bool TryUnify(
            Disjunction<TDomain, TElement> x,
            Disjunction<TDomain, TElement> y,
            IDictionary<Variable<TDomain, TElement>, Term<TDomain, TElement>> unifier)
        {
            // BUG: Order shouldn't matter (but need to be careful about partially updating unifier)
            // perhaps Low and High (internal) props in conjunction?
            return TryUnify(x.Left, y.Left, unifier) && TryUnify(x.Right, y.Right, unifier);
        }

        public bool TryUnify(
            Equality<TDomain, TElement> x,
            Equality<TDomain, TElement> y,
            IDictionary<Variable<TDomain, TElement>, Term<TDomain, TElement>> unifier)
        {
            // BUG: Order shouldn't matter (but need to be careful about partially updating unifier)
            // perhaps Low and High (internal) props in conjunction?
            return TryUnify(x.Left, y.Left, unifier) && TryUnify(x.Right, y.Right, unifier);
        }

        public bool TryUnify(
            Equivalence<TDomain, TElement> x,
            Equivalence<TDomain, TElement> y,
            IDictionary<Variable<TDomain, TElement>, Term<TDomain, TElement>> unifier)
        {
            // BUG: Order shouldn't matter (but need to be careful about partially updating unifier)
            // perhaps Low and High (internal) props in conjunction?
            return TryUnify(x.Equivalent1, y.Equivalent1, unifier) && TryUnify(x.Equivalent2, y.Equivalent2, unifier);
        }

        ////public virtual Sentence<TDomain, TElement> TryUnify(
        ////    ExistentialQuantification<TDomain, TElement> x,
        ////    ExistentialQuantification<TDomain, TElement> y,
        ////    IDictionary<Variable<TDomain, TElement>, Term<TDomain, TElement>> unifier)
        ////{
        ////    var variable = ApplyToVariableDeclaration(existentialQuantification.Variable);
        ////    var sentence = ApplyToSentence(existentialQuantification.Sentence);
        ////    if (variable != existentialQuantification.Variable || sentence != existentialQuantification.Sentence)
        ////    {
        ////        return new ExistentialQuantification<TDomain, TElement>(variable, sentence);
        ////    }
        ////
        ////    return existentialQuantification;
        ////}

        public bool TryUnify(
            Implication<TDomain, TElement> x,
            Implication<TDomain, TElement> y,
            IDictionary<Variable<TDomain, TElement>, Term<TDomain, TElement>> unifier)
        {
            return TryUnify(x.Antecedent, y.Antecedent, unifier) && TryUnify(x.Consequent, y.Consequent, unifier);
        }

        public bool TryUnify(
            Negation<TDomain, TElement> x,
            Negation<TDomain, TElement> y,
            IDictionary<Variable<TDomain, TElement>, Term<TDomain, TElement>> unifier)
        {
            return TryUnify(x.Sentence, y.Sentence, unifier);
        }

        public bool TryUnify(
            Predicate<TDomain, TElement> x,
            Predicate<TDomain, TElement> y,
            IDictionary<Variable<TDomain, TElement>, Term<TDomain, TElement>> unifier)
        {
            if (!MemberInfoEqualityComparer.Instance.Equals(x.Member, y.Member))
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

        ////public bool TryUnify(
        ////    UniversalQuantification<TDomain, TElement> x,
        ////    UniversalQuantification<TDomain, TElement> y,
        ////    IDictionary<Variable<TDomain, TElement>, Term<TDomain, TElement>> unifier)
        ////{
        ////    var variable = ApplyToVariableDeclaration(universalQuantification.Variable);
        ////    var sentence = ApplyToSentence(universalQuantification.Sentence);
        ////    if (variable != universalQuantification.Variable || sentence != universalQuantification.Sentence)
        ////    {
        ////        return new UniversalQuantification<TDomain, TElement>(variable, sentence);
        ////    }
        ////
        ////    return universalQuantification;
        ////}

        public bool TryUnify(
            Term<TDomain, TElement> x,
            Term<TDomain, TElement> y,
            IDictionary<Variable<TDomain, TElement>, Term<TDomain, TElement>> unifier)
        {
            return (x, y) switch
            {
                (Variable<TDomain, TElement> variable, _) => TryUnify(variable, y, unifier),
                (_, Variable<TDomain, TElement> variable) => TryUnify(variable, x, unifier),
                (Function<TDomain, TElement> functionX, Function<TDomain, TElement> functionY) => TryUnify(functionX, functionY, unifier),
                _ => x.Equals(y), // only potential for equality is if they're both constants. Worth being explicit?
            };
        }

        public bool TryUnify(
            Variable<TDomain, TElement> variable,
            Term<TDomain, TElement> other,
            IDictionary<Variable<TDomain, TElement>, Term<TDomain, TElement>> unifier)
        {
            Term<TDomain, TElement> value;
            if (unifier.TryGetValue(variable, out value))
            {
                return TryUnify(other, value, unifier);
            }
            else if (other is Variable<TDomain, TElement> otherVariable && unifier.TryGetValue(otherVariable, out value))
            {
                return TryUnify(variable, value, unifier);
            }
            ////else if (Occurs(variable, other))
            ////{
            ////    return false;
            ////}
            else
            {
                unifier[variable] = other;
                return true;
            }
        }

        public bool TryUnify(
            Function<TDomain, TElement> x,
            Function<TDomain, TElement> y,
            IDictionary<Variable<TDomain, TElement>, Term<TDomain, TElement>> unifier)
        {
            if (!MemberInfoEqualityComparer.Instance.Equals(x.Member, y.Member))
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
    }
}
