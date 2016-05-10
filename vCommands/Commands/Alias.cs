using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Commands
{
    using Parsing.Expressions;

    /// <summary>
    /// Represents a command which can be executed in a console host.
    /// </summary>
    public sealed class Alias
        : Command
    {
        /// <summary>
        /// The category given to all aliases.
        /// </summary>
        public static readonly String UniversalCategory = "User-defined Aliases";

        /// <summary>
        /// The description given to all aliases.
        /// </summary>
        public static readonly String UniversalDescription = "User-defined alias.";

        /// <summary>
        /// Gets the expression evaluated by this command.
        /// </summary>
        public String Expression { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Commands.Alias"/> class with the given name and function.
        /// </summary>
        /// <param name="name">The name of the command, used to find and invoke it.</param>
        /// <param name="expr">The expression to execute with the command.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when either of the given arguments is null.</exception>
        public Alias(string name, string expr)
            : base(name, UniversalCategory, UniversalDescription)
        {
            if (expr == null)
                throw new ArgumentNullException("expr");

            this.Expression = expr;
        }

        /// <summary>
        /// Invokes the underlying function of the command.
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="context">The context under which the command is invoked.</param>
        /// <param name="args">The results of evaluating each argument given to the command.</param>
        /// <returns>A status code accompanied by text output.</returns>
        protected override EvaluationResult InvokeInternal(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            string[] inputs = new string[args.Length + 1];

            inputs[0] = Expression;

            for (int i = 0; i < args.Length; i++)
            {
                var evalRes = args[i].Evaluate(context);

                if (!evalRes.TruthValue)
                    return new EvaluationResult(CommonStatusCodes.ArgumentEvaluationFailure, null, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                inputs[i + 1] = evalRes.Output;
            }

            return Parsing.Parser.Parse(string.Join(" ", inputs)).Evaluate(context);
        }
    }
}
