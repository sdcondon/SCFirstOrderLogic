// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.FormulaManipulation;
using SCFirstOrderLogic.FormulaManipulation.Normalisation;

namespace SCFirstOrderLogic;

/// <summary>
/// Representation of a <see cref="Formula"/> in conjunctive normal form (CNF).
/// </summary>
public class CNFFormula_WithTypeSwitchCtorVisitors
{
    /// <summary>
    /// Initialises a new instance of the <see cref="CNFFormula_WithTypeSwitchCtorVisitors"/> class, implicitly converting the provided formula to CNF in the process.
    /// </summary>
    /// <param name="formula">The formula to (convert and) represent.</param>
    public CNFFormula_WithTypeSwitchCtorVisitors(Formula formula)
    {
        var cnfFormula = CNFConversion.ApplyTo(formula);
        var clauses = new List<CNFClause_WithTypeSwitchCtorVisitors>();
        new CNFClauseFinder(clauses).Visit(cnfFormula);
        // WOULD-BE-A-BUG-IF-THIS-WERE-PROD-CODE: Potential equality bug on hash code collision..
        Clauses = clauses.OrderBy(c => c.GetHashCode()).ToArray();
    }

    /// <summary>
    /// Gets the collection of clauses that comprise this CNF formula.
    /// </summary>
    public IReadOnlyCollection<CNFClause_WithTypeSwitchCtorVisitors> Clauses { get; }

    /// <summary>
    /// Defines the (implicit) conversion of a <see cref="Formula"/> instance to a <see cref="CNFFormula_WithoutTypeSwitch"/> instance.
    /// </summary>
    /// <param name="formula">The formula to convert.</param>
    public static implicit operator CNFFormula_WithTypeSwitchCtorVisitors(Formula formula) => new(formula);

    /// <summary>
    /// Formula visitor that constructs a set of <see cref="CNFClause_WithoutTypeSwitch"/> objects from a <see cref="Formula"/> in CNF.
    /// </summary>
    private class CNFClauseFinder : RecursiveFormulaVisitor
    {
        private readonly ICollection<CNFClause_WithTypeSwitchCtorVisitors> clauses;

        public CNFClauseFinder(ICollection<CNFClause_WithTypeSwitchCtorVisitors> clauses) => this.clauses = clauses;

        /// <inheritdoc />
        public override void Visit(Formula formula)
        {
            if (formula is Conjunction conjunction)
            {
                // The expression is already in CNF - so the root down until the individual clauses will all be Conjunctions - we just skip past those.
                Visit(conjunction);
            }
            else
            {
                // We've hit a clause.
                // Afterwards, we don't need to look any further down the tree for the purposes of this class (though the CNFClause ctor that
                // we invoke here does so to figure out the details of the clause). So we can just return rather than invoking base.Visit. 
                clauses.Add(new CNFClause_WithTypeSwitchCtorVisitors(formula));
            }
        }
    }
}
