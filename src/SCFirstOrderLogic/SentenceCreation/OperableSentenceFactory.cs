using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SCFirstOrderLogic.SentenceCreation
{
    /// <summary>
    /// Shorthand static factory methods for <see cref="Sentence"/> instances. Intended to be used with a 'using static' directive to make method invocations acceptably succinct:
    /// <code>using static SCFirstOrderLogic.SentenceManipulation.OperableSentenceFactory;</code>
    /// This class provides a compromise between <see cref="SentenceFactory"/> and the full language integration provided by the types in the LanguageIntegration namespace.
    /// The objects returned by this factory (are implicitly convertible to <see cref="Sentence"/>s and) override operators in a way that is perhaps intuitive: | for disjunctions, &amp; for conjunctions, ! for negations, and == for the equality predicate.
    /// For domain-specific sentence elements (predicates, functions &amp; constants), their operable surrogate is implicitly convertible FROM the regular type, so the recommendation is to create appropriate properties and methods to create them - declaring
    /// the type as Operable.. so that they can be operated on. For example:
    /// <code>OperablePredicate MyBinaryPredicate(Term arg1, Term arg2) => new Predicate(nameof(MyBinaryPredicate), arg1, arg2);</code>
    /// ..which means you can then write things like:
    /// <code>ForAll(X, ThereExists(Y, !MyBinaryPredicate(X, Y) | MyOtherPredicate(Y)));</code>
    /// <para/>
    /// <strong>N.B.</strong> The real sentence classes do not define these operators to keep them as lean and mean as possible.
    /// In fact, the author's worry is that these operators aren't a good idea at all - because of the resulting wishy-washy mix of C# syntax and first-order logic concepts.
    /// Using the full LINQ integration (via <see cref="LanguageIntegration.SentenceFactory"/>) is strongly recommended instead of this class - because it has a much clearer and cleaner definition for how C# is mapped to FoL logic ("this expression would evaluate as true when invoked on an IEnumerable representing the domain").
    /// However, libraries should empower people, not constrain them, so here we are.
    /// </summary>
    public static class OperableSentenceFactory
    {
        /// <summary>
        /// Shorthand factory method for a new <see cref="OperableUniversalQuantification"/> instance.
        /// </summary>
        /// <param name="variableDeclaration">The variable declaration.</param>
        /// <param name="sentence">The body sentence that refers to the declared variable.</param>
        /// <returns>A new <see cref="OperableUniversalQuantification"/> instance.</returns>
        public static OperableSentence ForAll(OperableVariableDeclaration variableDeclaration, OperableSentence sentence) =>
            new OperableUniversalQuantification(variableDeclaration, sentence);

        /// <summary>
        /// Shorthand factory method for a (tree of) new <see cref="OperableUniversalQuantification"/> instances that declares two universally quantified variables.
        /// </summary>
        /// <param name="variableDeclaration1">The first variable declaration.</param>
        /// <param name="variableDeclaration2">The second variable declaration.</param>
        /// <param name="sentence">The body sentence that refers to the declared variables.</param>
        /// <returns>A new <see cref="OperableUniversalQuantification"/> instance.</returns>
        public static OperableSentence ForAll(OperableVariableDeclaration variableDeclaration1, OperableVariableDeclaration variableDeclaration2, OperableSentence sentence) =>
            new OperableUniversalQuantification(variableDeclaration1, new OperableUniversalQuantification(variableDeclaration2, sentence));

        /// <summary>
        /// Shorthand factory method for a (tree of) new <see cref="OperableUniversalQuantification"/> instances that declares three universally quantified variables.
        /// </summary>
        /// <param name="variableDeclaration1">The first variable declaration.</param>
        /// <param name="variableDeclaration2">The second variable declaration.</param>
        /// <param name="variableDeclaration3">The third variable declaration.</param>
        /// <param name="sentence">The body sentence that refers to the declared variables.</param>
        /// <returns>A new <see cref="OperableUniversalQuantification"/> instance.</returns>
        public static OperableSentence ForAll(OperableVariableDeclaration variableDeclaration1, OperableVariableDeclaration variableDeclaration2, OperableVariableDeclaration variableDeclaration3, OperableSentence sentence) =>
            new OperableUniversalQuantification(variableDeclaration1, new OperableUniversalQuantification(variableDeclaration2, new OperableUniversalQuantification(variableDeclaration3, sentence)));

        /// <summary>
        /// Shorthand factory method for a new <see cref="OperableExistentialQuantification"/> instance.
        /// </summary>
        /// <param name="variableDeclaration">The variable declaration.</param>
        /// <param name="sentence">The body sentence that refers to the declared variable.</param>
        /// <returns>A new <see cref="OperableExistentialQuantification"/> instance.</returns>
        public static OperableSentence ThereExists(OperableVariableDeclaration variableDeclaration, OperableSentence sentence) =>
            new OperableExistentialQuantification(variableDeclaration, sentence);

        /// <summary>
        /// Shorthand factory method for a (tree of) new <see cref="OperableExistentialQuantification"/> instances that declares two universally quantified variables.
        /// </summary>
        /// <param name="variableDeclaration1">The first variable declaration.</param>
        /// <param name="variableDeclaration2">The second variable declaration.</param>
        /// <param name="sentence">The body sentence that refers to the declared variables.</param>
        /// <returns>A new <see cref="OperableExistentialQuantification"/> instance.</returns>
        public static OperableSentence ThereExists(OperableVariableDeclaration variableDeclaration1, OperableVariableDeclaration variableDeclaration2, OperableSentence sentence) =>
            new OperableExistentialQuantification(variableDeclaration1, new OperableUniversalQuantification(variableDeclaration2, sentence));

        /// <summary>
        /// Shorthand factory method for a (tree of) new <see cref="OperableExistentialQuantification"/> instances that declares three universally quantified variables.
        /// </summary>
        /// <param name="variableDeclaration1">The first variable declaration.</param>
        /// <param name="variableDeclaration2">The second variable declaration.</param>
        /// <param name="variableDeclaration3">The third variable declaration.</param>
        /// <param name="sentence">The body sentence that refers to the declared variables.</param>
        /// <returns>A new <see cref="OperableExistentialQuantification"/> instance.</returns>
        public static OperableSentence ThereExists(OperableVariableDeclaration variableDeclaration1, OperableVariableDeclaration variableDeclaration2, OperableVariableDeclaration variableDeclaration3, OperableSentence sentence) =>
            new OperableExistentialQuantification(variableDeclaration1, new OperableExistentialQuantification(variableDeclaration2, new OperableExistentialQuantification(variableDeclaration3, sentence)));

        /// <summary>
        /// Shorthand factory method for a new <see cref="OperableImplication"/> instance.
        /// </summary>
        /// <param name="antecedent">The antecedent sentence of the implication.</param>
        /// <param name="consequent">The consequent sentence of the implication.</param>
        /// <returns>A new <see cref="OperableImplication"/> instance.</returns>
        public static OperableSentence If(OperableSentence antecedent, OperableSentence consequent) =>
            new OperableImplication(antecedent, consequent);

        /// <summary>
        /// Shorthand factory method for a new <see cref="OperableEquivalence"/> instance.
        /// </summary>
        /// <param name="left">The left-hand operand of the equivalence.</param>
        /// <param name="right">The right-hand operand of the equivalence.</param>
        /// <returns>A new <see cref="OperableEquivalence"/> instance.</returns>
        public static OperableSentence Iff(OperableSentence left, OperableSentence right) =>
            new OperableEquivalence(left, right);

        /// <summary>
        /// Shorthand factory method for a new <see cref="OperablePredicate"/> instance with the <see cref="EqualitySymbol.Instance"/> symbol.
        /// </summary>
        /// <param name="left">The left-hand operand of the equality.</param>
        /// <param name="right">The right-hand operand of the equality.</param>
        /// <returns>A new <see cref="OperablePredicate"/> instance.</returns>
        public static OperableSentence AreEqual(OperableTerm left, OperableTerm right) =>
            new OperablePredicate(EqualitySymbol.Instance, left, right);

        /// <summary>
        /// Shorthand factory method for a new <see cref="OperableVariableDeclaration"/> instance.
        /// </summary>
        /// <param name="symbol">The symbol of the variable.</param>>
        /// <returns>A new <see cref="OperableVariableDeclaration"/> instance.</returns>
        public static OperableVariableDeclaration Var(object symbol) =>
            new OperableVariableDeclaration(symbol);

        #region VariableDeclarations
        //// I'm still unconvinced that these properties are a good idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "A".
        /// </summary>
        public static OperableVariableDeclaration A => new(nameof(A));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "B".
        /// </summary>
        public static OperableVariableDeclaration B => new(nameof(B));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "C".
        /// </summary>
        public static OperableVariableDeclaration C => new(nameof(C));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "D".
        /// </summary>
        public static OperableVariableDeclaration D => new(nameof(D));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "E".
        /// </summary>
        public static OperableVariableDeclaration E => new(nameof(E));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "F".
        /// </summary>
        public static OperableVariableDeclaration F => new(nameof(F));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "G".
        /// </summary>
        public static OperableVariableDeclaration G => new(nameof(G));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "H".
        /// </summary>
        public static OperableVariableDeclaration H => new(nameof(H));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "I".
        /// </summary>
        public static OperableVariableDeclaration I => new(nameof(I));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "J".
        /// </summary>
        public static OperableVariableDeclaration J => new(nameof(J));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "K".
        /// </summary>
        public static OperableVariableDeclaration K => new(nameof(K));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "L".
        /// </summary>
        public static OperableVariableDeclaration L => new(nameof(L));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "M".
        /// </summary>
        public static OperableVariableDeclaration M => new(nameof(M));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "N".
        /// </summary>
        public static OperableVariableDeclaration N => new(nameof(N));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "O".
        /// </summary>
        public static OperableVariableDeclaration O => new(nameof(O));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "P".
        /// </summary>
        public static OperableVariableDeclaration P => new(nameof(P));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "Q".
        /// </summary>
        public static OperableVariableDeclaration Q => new(nameof(Q));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "R".
        /// </summary>
        public static OperableVariableDeclaration R => new(nameof(R));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "S".
        /// </summary>
        public static OperableVariableDeclaration S => new(nameof(S));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "T".
        /// </summary>
        public static OperableVariableDeclaration T => new(nameof(T));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "U".
        /// </summary>
        public static OperableVariableDeclaration U => new(nameof(U));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "V".
        /// </summary>
        public static OperableVariableDeclaration V => new(nameof(V));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "W".
        /// </summary>
        public static OperableVariableDeclaration W => new(nameof(W));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "X".
        /// </summary>
        public static OperableVariableDeclaration X => new(nameof(X));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "Y".
        /// </summary>
        public static OperableVariableDeclaration Y => new(nameof(Y));

        /// <summary>
        /// Gets a new <see cref="OperableVariableDeclaration"/> for a variable with the symbol "Z".
        /// </summary>
        public static OperableVariableDeclaration Z => new(nameof(Z));

        #endregion

        /// <summary>
        /// Surrogate for <see cref="Sentence"/> instances that defines |, &amp; and ! operators to create disjunctions, conjunctions and negations respectively.
        /// Instances are implicitly convertible to the equivalent <see cref="Sentence"/> instance.
        /// </summary>
        public abstract class OperableSentence
        {
            /// <summary>
            /// Composes two <see cref="OperableSentence"/> instances together by forming a conjunction from them.
            /// </summary>
            /// <param name="left">The left-hand operand of the conjunction.</param>
            /// <param name="right">The right-hand operand of the conjunction.</param>
            /// <returns>A new <see cref="OperableConjunction"/> instance.</returns>
            public static OperableSentence operator &(OperableSentence left, OperableSentence right) => new OperableConjunction(left, right);

            /// <summary>
            /// Composes two <see cref="OperableSentence"/> instances together by forming a disjunction from them.
            /// </summary>
            /// <param name="left">The left-hand operand of the disjunction.</param>
            /// <param name="right">The right-hand operand of the disjunction.</param>
            /// <returns>A new <see cref="OperableDisjunction"/> instance.</returns>
            public static OperableSentence operator |(OperableSentence left, OperableSentence right) => new OperableDisjunction(left, right);

            /// <summary>
            /// Creates the negation of an <see cref="OperableSentence"/> instance.
            /// </summary>
            /// <param name="operand">The sentence to negate.</param>
            /// <returns>A new <see cref="OperableNegation"/> instance.</returns>
            public static OperableSentence operator !(OperableSentence operand) => new OperableNegation(operand);

            /// <summary>
            /// Implicitly converts an <see cref="OperableSentence"/> instance to an equivalent <see cref="Sentence"/>.
            /// </summary>
            /// <param name="sentence">The sentence to convert.</param>
            public static implicit operator Sentence(OperableSentence sentence) => sentence switch
            {
                OperableConjunction conjunction => new Conjunction(conjunction.Left, conjunction.Right),
                OperableDisjunction disjunction => new Disjunction(disjunction.Left, disjunction.Right),
                OperableEquivalence equivalence => new Equivalence(equivalence.Left, equivalence.Right),
                OperableExistentialQuantification existentialQuantification => new ExistentialQuantification(existentialQuantification.Variable, existentialQuantification.Sentence),
                OperableImplication implication => new Implication(implication.Antecedent, implication.Consequent),
                OperableNegation negation => new Negation(negation.Sentence),
                OperablePredicate predicate => new Predicate(predicate.Symbol, predicate.Arguments.Select(a => (Term)a).ToArray()),
                OperableUniversalQuantification universalQuantification => new UniversalQuantification(universalQuantification.Variable, universalQuantification.Sentence),
                _ => throw new ArgumentException($"Unsupported OperableSentence type '{sentence.GetType()}'", nameof(sentence))
            };
        }

        /// <summary>
        /// Surrogate for <see cref="Conjunction"/> instances that derives from <see cref="OperableSentence"/> and can thus be operated on with the |, &amp; and ! operators
        /// to create disjunctions, conjunctions and negations respectively. NB constructor is intentionally not public - should only be created via the &amp; operator acting on
        /// <see cref="OperableSentence"/> instances.
        /// </summary>
        public sealed class OperableConjunction : OperableSentence
        {
            internal OperableConjunction(OperableSentence left, OperableSentence right) => (Left, Right) = (left, right);

            internal OperableSentence Left { get; }

            internal OperableSentence Right { get; }
        }

        /// <summary>
        /// Surrogate for <see cref="Constant"/> instances that derives from <see cref="OperableTerm"/> and can thus be operated on with the  == operator
        /// to create equality predicate instances. N.B. constructor is intentionally not public - can be implicitly converted to from <see cref="Constant"/>
        /// instances. E.g.
        /// <code>OperableConstant MyConstant { get; } = new Constant(nameof(MyConstant));</code>
        /// </summary>
        public sealed class OperableConstant : OperableTerm
        {
            internal OperableConstant(object symbol) => Symbol = symbol;

            internal object Symbol { get; }

            /// <summary>
            /// Implicitly converts a <see cref="Constant"/> instance to an equivalent <see cref="OperableConstant"/>.
            /// </summary>
            /// <param name="constant">The constant to convert.</param>
            public static implicit operator OperableConstant(Constant constant) => new(constant.Symbol);
        }

        /// <summary>
        /// Surrogate for <see cref="Conjunction"/> instances that derives from <see cref="OperableSentence"/> and can thus be operated on with |, &amp; and ! operators
        /// to create disjunctions, conjunctions and negations respectively. N.B. constructor is intentionally not public - should only be created via the | operator acting on
        /// <see cref="OperableSentence"/> instances.
        /// </summary>
        public sealed class OperableDisjunction : OperableSentence
        {
            internal OperableDisjunction(OperableSentence left, OperableSentence right) => (Left, Right) = (left, right);

            internal OperableSentence Left { get; }

            internal OperableSentence Right { get; }
        }

        /// <summary>
        /// Surrogate for <see cref="Equivalence"/> instances that derives from <see cref="OperableSentence"/> and can thus be operated on with |, &amp; and ! operators
        /// to create disjunctions, conjunctions and negations respectively. N.B. constructor is intentionally not public - should only be created via the
        /// <see cref="Iff"/> method.
        /// </summary>
        public sealed class OperableEquivalence : OperableSentence
        {
            internal OperableEquivalence(OperableSentence left, OperableSentence right) => (Left, Right) = (left, right);

            internal OperableSentence Left { get; }

            internal OperableSentence Right { get; }
        }

        /// <summary>
        /// Surrogate for <see cref="ExistentialQuantification"/> instances that derives from <see cref="OperableSentence"/> and can thus be operated on with |, &amp; and ! operators
        /// to create disjunctions, conjunctions and negations respectively. N.B. constructor is intentionally not public - should only be created via the
        /// <see cref="ThereExists(OperableVariableDeclaration, OperableSentence)"/> method (or its overrides).
        /// </summary>
        public sealed class OperableExistentialQuantification : OperableSentence
        {
            internal OperableExistentialQuantification(OperableVariableDeclaration variable, OperableSentence sentence) => (Variable, Sentence) = (variable, sentence);

            internal OperableVariableDeclaration Variable { get; }

            internal OperableSentence Sentence { get; }
        }

        /// <summary>
        /// Surrogate for <see cref="Function"/> instances that derives from <see cref="OperableTerm"/> and can thus be operated on with the  == operator
        /// to create equality predicate instances. N.B. constructor is intentionally not public - can be implicitly converted to from <see cref="Function"/>
        /// instances. E.g.
        /// <code>OperableFunction MyFunction(OperableTerm arg1) => new Function(nameof(MyFunction), arg1);</code>
        /// </summary>
        public class OperableFunction : OperableTerm
        {
            internal OperableFunction(object symbol, params OperableTerm[] arguments)
                : this(symbol, (IList<OperableTerm>)arguments)
            {
            }

            internal OperableFunction(object symbol, IList<OperableTerm> arguments)
            {
                Symbol = symbol;
                Arguments = new ReadOnlyCollection<OperableTerm>(arguments);
            }

            internal ReadOnlyCollection<OperableTerm> Arguments { get; }

            internal object Symbol { get; }

            /// <summary>
            /// Implicitly converts a <see cref="Function"/> instance to an equivalent <see cref="OperableFunction"/>.
            /// </summary>
            /// <param name="function">The function to convert.</param>
            public static implicit operator OperableFunction(Function function) => new(function.Symbol, function.Arguments.Select(a => (OperableTerm)a).ToArray());
        }

        /// <summary>
        /// Surrogate for <see cref="Implication"/> instances that derives from <see cref="OperableSentence"/> and can thus be operated on with |, &amp; and ! operators
        /// to create disjunctions, conjunctions and negations respectively. N.B. constructor is intentionally not public - should only be created via the
        /// <see cref="If"/> method.
        /// </summary>
        public sealed class OperableImplication : OperableSentence
        {
            internal OperableImplication(OperableSentence antecedent, OperableSentence consequent) => (Antecedent, Consequent) = (antecedent, consequent);

            internal OperableSentence Antecedent { get; }

            internal OperableSentence Consequent { get; }
        }

        /// <summary>
        /// Surrogate for <see cref="Negation"/> instances that derives from <see cref="OperableSentence"/> and can thus be operated on with |, &amp; and ! operators
        /// to create disjunctions, conjunctions and negations respectively. N.B. constructor is intentionally not public - should only be created via the ! operator acting on
        /// <see cref="OperableSentence"/> instances.
        /// </summary>
        public sealed class OperableNegation : OperableSentence
        {
            internal OperableNegation(OperableSentence sentence) => Sentence = sentence;

            internal OperableSentence Sentence { get; }
        }

        /// <summary>
        /// Surrogate for <see cref="Predicate"/> instances that derives from <see cref="OperableSentence"/> and can thus be operated on with |, &amp; and ! operators
        /// to create disjunctions, conjunctions and negations respectively. N.B. constructor is intentionally not public - can be implicitly converted to from <see cref="Predicate"/>
        /// instances. E.g.
        /// <code>OperablePredicate MyPredicate(OperableTerm arg1) => new Predicate(nameof(MyPredicate), arg1);</code>
        /// </summary>
        public class OperablePredicate : OperableSentence
        {
            internal OperablePredicate(object symbol, params OperableTerm[] arguments)
                : this(symbol, (IList<OperableTerm>)arguments)
            {
            }

            internal OperablePredicate(object symbol, IList<OperableTerm> arguments)
            {
                Symbol = symbol;
                Arguments = new ReadOnlyCollection<OperableTerm>(arguments);
            }

            internal ReadOnlyCollection<OperableTerm> Arguments { get; }

            internal object Symbol { get; }

            /// <summary>
            /// Implicitly converts a <see cref="Predicate"/> instance to an equivalent <see cref="OperablePredicate"/>.
            /// </summary>
            /// <param name="predicate">The predicate to convert.</param>
            public static implicit operator OperablePredicate(Predicate predicate) => new(predicate.Symbol, predicate.Arguments.Select(a => (OperableTerm)a).ToArray());
        }

        /// <summary>
        /// Surrogate for <see cref="Term"/> instances that defines an == operator to create equality predicates.
        /// Instances are implicitly convertible to the equivalent <see cref="Term"/> instance.
        /// </summary>
#pragma warning disable CS0660, CS0661
        // Overrides == but not Equals and HashCode. Overriding these would make no sense. Needing to disable these warnings is
        // hopefully a good example of why I want to keep this approach to creating sentences well away from the actual sentence classes..
        public abstract class OperableTerm
        {
            /// <summary>
            /// Composes two <see cref="OperableTerm"/> instances together by creating an equality predicate. That is, an <see cref="OperablePredicate"/> with the <see cref="EqualitySymbol.Instance"/> symbol.
            /// </summary>
            /// <param name="left">The left-hand side of the equality.</param>
            /// <param name="right">The right-hand side of the equality.</param>
            /// <returns>A new <see cref="OperablePredicate"/> instance.</returns>
            public static OperableSentence operator ==(OperableTerm left, OperableTerm right) => new OperablePredicate(EqualitySymbol.Instance, left, right);

            /// <summary>
            /// Composes two <see cref="OperableTerm"/> instances together by creating a negation of the equality predicate. That is, an <see cref="OperableNegation"/> acting on an <see cref="OperablePredicate"/> with the <see cref="EqualitySymbol.Instance"/> symbol.
            /// </summary>
            /// <param name="left">The left-hand side of the inequality.</param>
            /// <param name="right">The right-hand side of the inequality.</param>
            /// <returns>A new <see cref="OperableNegation"/> instance.</returns>
            public static OperableSentence operator !=(OperableTerm left, OperableTerm right) => new OperableNegation(new OperablePredicate(EqualitySymbol.Instance, left, right));

            /// <summary>
            /// Implicitly converts a <see cref="OperableTerm"/> instance to an equivalent <see cref="Term"/>.
            /// </summary>
            /// <param name="term">The term to convert.</param>
            public static implicit operator Term(OperableTerm term) => term switch
            {
                OperableConstant constant => new Constant(constant.Symbol),
                OperableFunction function => new Function(function.Symbol, function.Arguments.Select(a => (Term)a).ToArray()),
                OperableVariableReference variableReference => new VariableReference(variableReference.Declaration.Symbol),
                _ => throw new ArgumentException("Unsupported TermSurrogate type"),
            };

            /// <summary>
            /// Implicitly converts a <see cref="OperableTerm"/> instance to an equivalent <see cref="Term"/>.
            /// </summary>
            /// <param name="term">The term to convert.</param>
            public static implicit operator OperableTerm(Term term) => term switch
            {
                Constant constant => new OperableConstant(constant.Symbol),
                Function function => new OperableFunction(function.Symbol, function.Arguments.Select(a => (OperableTerm)a).ToArray()),
                VariableReference variableReference => new OperableVariableReference(variableReference.Declaration.Symbol),
                _ => throw new ArgumentException("Unsupported TermSurrogate type"),
            };
        }
#pragma warning restore CS0660, CS0661

        /// <summary>
        /// Surrogate for <see cref="UniversalQuantification"/> instances that derives from <see cref="OperableSentence"/> and can thus be operated on with |, &amp; and ! operators
        /// to create disjunctions, conjunctions and negations respectively. N.B. constructor is intentionally not public - should only be created via the
        /// <see cref="ForAll(OperableVariableDeclaration, OperableSentence)"/> method (or its overrides).
        /// </summary>
        public sealed class OperableUniversalQuantification : OperableSentence
        {
            internal OperableUniversalQuantification(OperableVariableDeclaration variable, OperableSentence sentence) => (Variable, Sentence) = (variable, sentence);

            internal OperableVariableDeclaration Variable { get; }

            internal OperableSentence Sentence { get; }
        }

        /// <summary>
        /// Surrogate for <see cref="VariableDeclaration"/> instances.
        /// </summary>
        public sealed class OperableVariableDeclaration
        {
            internal OperableVariableDeclaration(object symbol) => Symbol = symbol;

            internal object Symbol { get; }

            /// <summary>
            /// Implicitly converts an <see cref="OperableVariableDeclaration"/> instance to an equivalent <see cref="VariableDeclaration"/>.
            /// </summary>
            /// <param name="declaration">The declaration to convert.</param>
            public static implicit operator VariableDeclaration(OperableVariableDeclaration declaration) => new(declaration.Symbol);

            /// <summary>
            /// Implicitly converts a <see cref="VariableDeclaration"/> instance to an equivalent <see cref="OperableVariableDeclaration"/>.
            /// </summary>
            /// <param name="declaration">The declaration to convert.</param>
            public static implicit operator OperableVariableDeclaration(VariableDeclaration declaration) => new(declaration.Symbol);

            /// <summary>
            /// Implicitly converts an <see cref="OperableVariableDeclaration"/> instance to an <see cref="OperableVariableReference"/> referring to that variable.
            /// </summary>
            /// <param name="declaration">The declaration to convert.</param>
            public static implicit operator OperableVariableReference(OperableVariableDeclaration declaration) => new(declaration);
        }

        /// <summary>
        /// Surrogate for <see cref="VariableReference"/> instances that derives from <see cref="OperableTerm"/> and can thus be operated on with the == and != operators
        /// to create equality and negation of equality respectively.
        /// </summary>
        public sealed class OperableVariableReference : OperableTerm
        {
            internal OperableVariableReference(OperableVariableDeclaration declaration) => Declaration = declaration;

            internal OperableVariableReference(object symbol) => Declaration = new OperableVariableDeclaration(symbol);

            internal OperableVariableDeclaration Declaration { get; }

            internal object Symbol => Declaration.Symbol;
        }
    }
}
