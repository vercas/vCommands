#if HVCE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.EventArguments
{
    using Variables;
    using Parsing.Expressions;
    
    /// <summary>
    /// Provides data for the <see cref="vCommands.CommandHost.VariableChange"/> event.
    /// </summary>
    public class HostVariableChangeEventArgs
        : VariableChangeEventArgs
    {
        /// <summary>
        /// Gets the variable which is being changed..
        /// </summary>
        public IVariable Variable { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.EventArguments.HostVariableChangeEventArgs"/> class with the specified variable, change context, current string value and argument.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="context"></param>
        /// <param name="csv"></param>
        /// <param name="arg"></param>
        public HostVariableChangeEventArgs(IVariable variable, EvaluationContext context, string csv, Expression arg)
            : base(context, csv, arg)
        {
            this.Variable = variable;
        }
    }
}

#endif
