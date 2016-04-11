using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Utilities
{
    using Parsing.Expressions;

    /// <summary>
    /// Contains utilitary methods for dealing with arguments and expressions.
    /// </summary>
    public static class ArgumentsAndExpressions
    {
        /// <summary>
        /// Evaluates all the given arguments under the given context. In case of error, a descriptive result is returned with the given error status.
        /// </summary>
        /// <remarks>
        /// When the single result is non-null, an error has occurred and that result should be forwarded by the callee. The array will be null.
        /// <para>When all arguments were evaluated successfully, the single result will be null and the array will contain the results of every evaluated argument.</para>
        /// </remarks>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <param name="errorStatus">In case an argument failed to evaluate, a result is returned with this specific error status.</param>
        /// <returns>A tuple containing a single evaluation result for the whole operation and the evaluation results of all arguments. Only one of them is non-null.</returns>
        public static Tuple<EvaluationResult, EvaluationResult[]> Evaluate(EvaluationContext context, Expression[] args, int errorStatus)
        {
            var ers = new EvaluationResult[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                var evalRes = args[i].Evaluate(context);

                if (!evalRes.TruthValue)
                    return new Tuple<EvaluationResult, EvaluationResult[]>(new EvaluationResult(errorStatus, null, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output)), null);

                ers[i] = evalRes;
            }

            return new Tuple<EvaluationResult, EvaluationResult[]>(null, ers);
        }
    }
}
