using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Commands
{
    using EventArguments;
    using Parsing.Expressions;
    using Utilities;

    /// <summary>
    /// Represents a command which can be executed in a console host.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// The default category for commands (when null is specified).
        /// </summary>
        public static readonly string DefaultCategory = "Miscellaneous";

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public String Name { get; internal set; }

        /// <summary>
        /// Gets the category of the command.
        /// </summary>
        public String Category { get; internal set; }

        /// <summary>
        /// Gets a brief description of the command.
        /// </summary>
        public String Abstract { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Commands.Command"/> class with the given name and function.
        /// </summary>
        /// <param name="name">The name of the command, used to find and invoke it.</param>
        /// <param name="category">The named category under which the command goes.</param>
        /// <param name="description">The description of the command, for displaying in help text.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name string is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given name or category strings are empty or consist only of white-space characters..</exception>
        public Command(string name, string category, string description)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Command name may not be empty or consist only of white-space characters.", "name");

            if (category != null && string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category must be null for default, or must contain characters other than white-spaces.", "category");

            this.Name = name;
            this.Category = category ?? DefaultCategory;
            this.Abstract = description ?? string.Empty;
        }

        /// <summary>
        /// Invokes the underlying function of the command.
        /// </summary>
        /// <param name="toggle">The toggle status of the command.</param>
        /// <param name="context">The context under which the command is invoked.</param>
        /// <param name="args">The results of evaluating each argument given to the command.</param>
        /// <returns>A status code accompanied by text output.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given argument array is null.</exception>
        public EvaluationResult Invoke(Toggler toggle, EvaluationContext context, params Expression[] args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            var e1 = new CommandInvocationEventArgs(context, toggle, args);

            OnInvocation(e1);

            if (e1.Cancel)
                return new EvaluationResult(CommonStatusCodes.InvocationCanceled, null, e1.CancelReason ?? "Invocation has stopped.");

#if HCIE
            var e2 = new HostCommandInvocationEventArgs(this, context, toggle, args);

            context.Host.OnInvocation(e2);

            if (e2.Stop)
                return new EvaluationResult(CommonStatusCodes.InvocationCanceled, null, e2.StopReason ?? "Invocation has stopped.");
#endif

            try
            {
                return this.InvokeInternal(toggle, context, args);
            }
            catch (Exception x)
            {
                return new EvaluationResult(CommonStatusCodes.ClrException, null, x.ToString(), x);
            }
        }

        /// <summary>
        /// Raised before the command is invoked.
        /// </summary>
        public event TypedEventHandler<Command, CommandInvocationEventArgs> Invocation;

        /// <summary>
        /// Raisese the <see cref="vCommands.Commands.Command.Invocation"/> event.
        /// </summary>
        /// <param name="e">A <see cref="vCommands.EventArguments.CommandInvocationEventArgs"/> that contains event data.</param>
        protected virtual void OnInvocation(CommandInvocationEventArgs e)
        {
            Invocation?.Invoke(this, e);
        }

        /// <summary>
        /// Invokes the underlying logic of the command.
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected abstract EvaluationResult InvokeInternal(Toggler toggle, EvaluationContext context, Expression[] args);
    }
}
