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

            EvaluationResult res;

            if (context.Depth >= context.Host.MaxDepth)
                return new EvaluationResult(CommonStatusCodes.InvocationDepthExceeded, this, "Invocation depth exceeded.");

            try
            {
                Evaluate(context.WithChangedDepth(+1), out res);

                res.Expression = this;
                return res;
            }
            catch (Exception x)
            {
                return new EvaluationResult(CommonStatusCodes.ClrException, this, x.ToString(), x);
            }
        }

        /// <summary>
        /// Evaluates the current expression.
        /// </summary>
        /// <param name="context">The context of the evaluation.</param>
        /// <param name="res">The variable which will contain the result of the evaluation.</param>
        protected abstract void Evaluate(EvaluationContext context, out EvaluationResult res);
    }
}
