﻿using SCFirstOrderLogic.SentenceCreation.Antlr;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.SentenceCreation;

/// <summary>
/// Visitor that transforms from a syntax tree generated by ANTLR to a <see cref="Term"/> instance
/// </summary>
internal class TermTransformation : FirstOrderLogicBaseVisitor<Term>
{
    private readonly SentenceParserOptions options;
    private readonly IEnumerable<VariableDeclaration> variablesInScope;

    public TermTransformation(SentenceParserOptions options, IEnumerable<VariableDeclaration> variablesInScope)
    {
        this.options = options;
        this.variablesInScope = variablesInScope;
    }

    public override Term VisitVariableOrConstant([NotNull] FirstOrderLogicParser.VariableOrConstantContext context)
    {
        var identifier = options.GetVariableOrConstantIdentifier(context.IDENTIFIER().Symbol.Text);
        var matchingVariableDeclaration = variablesInScope.SingleOrDefault(v => v.Identifier.Equals(identifier));
        if (matchingVariableDeclaration != null)
        {
            // identifier matches a variable that is in scope - interpret as a reference to it
            return new VariableReference(matchingVariableDeclaration);
        }
        else
        {
            // identifier doesn't match any variable in scope - interpret as a zero arity function
            return new Function(identifier);
        }
    }

    public override Term VisitFunction([NotNull] FirstOrderLogicParser.FunctionContext context)
    {
        return new Function(
            options.GetFunctionIdentifier(context.IDENTIFIER().Symbol.Text),
            context.argumentList()._elements.Select(e => Visit(e)));
    }
}
