using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Parsing.Expressions
{
    using Utilities;

    /// <summary>
    /// Represents a console command expression.
    /// </summary>
    public abstract class Expression
        : Sealable
    {
        /// <summary>
        /// Evaluates the current expression, returning the result.
        /// </summary>
        /// <returns></returns>
        public EvaluationResult Evaluate(EvaluationContext context)
        {
            if (context == null)
                throw new ArgumentException("context");

            int status = int.MaxValue;
            string output = string.Empty;

            try
            {
                Evaluate(out status, out output, context);
            }
            catch (Exception x)
            {
                status = -1;
                output = x.ToString();
            }

            return new EvaluationResult(status, output, this);
        }

        /// <summary>
        /// Evaluates the current expression.
        /// </summary>
        /// <param name="status">Numerical status value of the evaluation.</param>
        /// <param name="output">Text output of the evaluation.</param>
        /// <param name="context">The context of the evaluation.</param>
        protected abstract void Evaluate(out int status, out string output, EvaluationContext context);
    }
}
