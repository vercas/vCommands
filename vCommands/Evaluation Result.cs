using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands
{
    using Parsing.Expressions;

    /// <summary>
    /// Represents the result of evaluating a <see cref="vCommands.Parsing.Expressions.Expression"/>.
    /// </summary>
    public sealed class EvaluationResult
    {
        /// <summary>
        /// A result whic contains no output text and status zero.
        /// </summary>
        public static EvaluationResult EmptyPositive = new EvaluationResult(0, string.Empty);

        /// <summary>
        /// Gets the numerical status of the result.
        /// </summary>
        /// <remarks>
        /// 0 means True/positive, anything else is False/negative.
        /// </remarks>
        public int Status { get; internal set; }

        /// <summary>
        /// Gets the truth value interpretede from the status.
        /// </summary>
        public Boolean TruthValue { get; internal set; }

        /// <summary>
        /// Gets the resulted text output of the expression.
        /// </summary>
        public String Output { get; internal set; }

        /// <summary>
        /// Gets the expression which evaluated to the current result.
        /// </summary>
        public Expression Expression { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.EvaluationResult"/> class with the specified status, output and optional expression.
        /// </summary>
        /// <param name="status">Numerical status indicating the success of the evaluation.</param>
        /// <param name="output">The text output of the evaluation.</param>
        /// <param name="exp">optional; The expression which evaluated to the current result.</param>
        public EvaluationResult(int status, string output, Expression exp = null)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            this.Status = status;
            this.TruthValue = status == 0;
            this.Output = output;
        }
    }
}
