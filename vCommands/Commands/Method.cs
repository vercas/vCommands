using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Commands
{
    using Parsing.Expressions;

    /// <summary>
    /// Encapsulates a method which can be executed by a console command.
    /// </summary>
    /// <param name="toggle">The toggle status of the command.</param>
    /// <param name="context">The context under which the command is invoked.</param>
    /// <param name="args">The results of evaluating each argument given to the command.</param>
    /// <returns>A status code accompanied by text output.</returns>
    public delegate EvaluationResult CommandMethod(bool? toggle, EvaluationContext context, Expression[] args);

    /// <summary>
    /// Represents a command which can be executed in a console host.
    /// </summary>
    public sealed class MethodCommand
        : Command
    {
        /// <summary>
        /// Gets the delegate of the method executed by the command.
        /// </summary>
        public CommandMethod Method { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Commands.MethodCommand"/> class with the given name and function.
        /// </summary>
        /// <param name="category">The named category under which the command goes.</param>
        /// <param name="name">The name of the command, used to find and invoke it.</param>
        /// <param name="description">The description of the command, for displaying in help text.</param>
        /// <param name="function">The underlying method to be invoked by the command.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when either of the given arguments is null.</exception>
        public MethodCommand(string name, string category, string description, CommandMethod function)
            : base(name, category, description)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            this.Method = function;
        }

        /// <summary>
        /// Invokes the underlying function of the command.
        /// </summary>
        /// <param name="toggle">The toggle status of the command.</param>
        /// <param name="context">The context under which the command is invoked.</param>
        /// <param name="args">The results of evaluating each argument given to the command.</param>
        /// <returns>A status code accompanied by text output.</returns>
        protected override EvaluationResult _Invoke(bool? toggle, EvaluationContext context, Expression[] args)
        {
            return Method.Invoke(toggle, context, args);
        }
    }
}
