using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace vCommands.EventArguments
{
    using Parsing.Expressions;
    
    /// <summary>
    /// Provides data for the <see cref="vCommands.Commands.Command.Invocation"/> event.
    /// </summary>
    public class CommandInvocationEventArgs
        : ContextuallyCancellableEventArgs
    {
        /// <summary>
        /// Gets the toggle status of the command.
        /// </summary>
        public Boolean? Toggle { get; internal set; }

        /// <summary>
        /// Gets the arguments passed to the command.
        /// </summary>
        public ReadOnlyCollection<Expression> Arguments { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.EventArguments.CommandInvocationEventArgs"/> class with the specified invocation context, toggle status and arguments.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="toggle"></param>
        /// <param name="args"></param>
        public CommandInvocationEventArgs(EvaluationContext context, bool? toggle, Expression[] args)
            : base(context, false, null)
        {
            this.Toggle = toggle;
            this.Arguments = new ReadOnlyCollection<Expression>(args);
        }
    }
}
