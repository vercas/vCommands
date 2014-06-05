#if HCIE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.EventArguments
{
    using Commands;
    using Parsing.Expressions;
    
    /// <summary>
    /// Provides data for the <see cref="vCommands.CommandHost.CommandInvocation"/> event.
    /// </summary>
    public class HostCommandInvocationEventArgs
        : CommandInvocationEventArgs
    {
        /// <summary>
        /// Gets the command which was invoked.
        /// </summary>
        public Command Command { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.EventArguments.HostCommandInvocationEventArgs"/> class with the specified invoked command, invocation context, toggle status and arguments.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="context"></param>
        /// <param name="toggle"></param>
        /// <param name="args"></param>
        public HostCommandInvocationEventArgs(Command command, EvaluationContext context, bool? toggle, Expression[] args)
            : base(context, toggle, args)
        {
            this.Command = command;
        }
    }
}

#endif
