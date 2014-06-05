﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace vCommands.Parsing.Expressions
{
    /// <summary>
    /// Represents a series of expressions.
    /// </summary>
    public sealed class SeriesExpression
        : Expression
    {
        /// <summary>
        /// Gets a read-only collection of arguments to the command.
        /// </summary>
        public ReadOnlyCollection<Expression> Subexpressions { get; internal set; }

        internal List<Expression> exprs;

        /// <summary>
        /// Evaluates the expression in the series, yielding the concatenated outputs and the status of the last evaluation.
        /// </summary>
        /// <param name="status">Numerical status value of the evaluation.</param>
        /// <param name="output">Text output of the evaluation.</param>
        /// <param name="context">The context of the evaluation.</param>
        protected override void Evaluate(out int status, out string output, EvaluationContext context)
        {
            status = 0;
            StringBuilder outputGatherer = new StringBuilder(4096);

            for (int i = 0; i < exprs.Count; i++)
            {
                var evalRes = exprs[i].Evaluate(context);

                status = evalRes.Status;
                outputGatherer.Append(evalRes.Output);
            }

            output = outputGatherer.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Parsing.Expressions.ConditionalExpression"/> class with the specified toggle flag, command name and optional arguments.
        /// </summary>
        /// <param name="subexpressions">optional; An enumeration of expressions to add to the series.</param>
        public SeriesExpression(IEnumerable<Expression> subexpressions)
        {
            Sealed = false;

            if (subexpressions == null)
                exprs = new List<Expression>();
            else
                exprs = new List<Expression>(subexpressions);

            Subexpressions = new ReadOnlyCollection<Expression>(exprs);
        }

        /// <summary>
        /// Adds the given expression to the series.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>True if the expression was added (series is not sealed); otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given expression is null.</exception>
        public bool AddExpression(Expression arg)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            if (Sealed)
                return false;

            exprs.Add(arg);

            return true;
        }

        /// <summary>
        /// Returns a string that represents the current series of expressions.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join("; ", this.exprs);
        }
    }
}
