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
    public sealed class UserCommand
        : Command
    {
        /// <summary>
        /// The category given to all user commands.
        /// </summary>
        public static readonly String UniversalCategory = "User-defined Commands";

        /// <summary>
        /// The description given to all user commands.
        /// </summary>
        public static readonly String UniversalDescription = "User-defined command.";

        /// <summary>
        /// Gets the expression evaluated by this command.
        /// </summary>
        public Expression Expression { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Commands.UserCommand"/> class with the given name and function.
        /// </summary>
        /// <param name="name">The name of the command, used to find and invoke it.</param>
        /// <param name="expr">The expression to execute with the command.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when either of the given arguments is null.</exception>
        public UserCommand(string name, Expression expr)
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
            return Expression.Evaluate(context.WithUserArguments(args));
        }
    }
}
