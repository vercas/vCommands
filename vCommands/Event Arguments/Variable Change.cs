using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace vCommands.EventArguments
{
    using Parsing.Expressions;
    
    /// <summary>
    /// Provides data for the <see cref="vCommands.Variables.IVariable.Change"/> event.
    /// </summary>
    public class VariableChangeEventArgs
        : ContextuallyCancellableEventArgs
    {
        /// <summary>
        /// Gets the current string value of the variable.
        /// </summary>
        public String CurrentStringValue { get; internal set; }

        /// <summary>
        /// Gets the argument passed to the variable.
        /// </summary>
        public Expression Argument { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.EventArguments.VariableChangeEventArgs"/> class with the specified change context, current string value and argument.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="csv">Current string value.</param>
        /// <param name="arg"></param>
        public VariableChangeEventArgs(EvaluationContext context, string csv, Expression arg)
            : base(context, false, null)
        {
            this.CurrentStringValue = csv;
            this.Argument = arg;
        }
    }
}
