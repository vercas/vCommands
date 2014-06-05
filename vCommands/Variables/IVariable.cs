using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Variables
{
    using EventArguments;
    using Parsing.Expressions;
    using Utilities;

    /// <summary>
    /// Defines the functionality of a command variable.
    /// </summary>
    public interface IVariable
    {
        /// <summary>
        /// Gets or sets the string representing the value of the variable.
        /// </summary>
        String StringValue { get; set; }

        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Gets a brief description of the variable.
        /// </summary>
        String Abstract { get; }

        /// <summary>
        /// Gets the underlying value of the variable.
        /// </summary>
        /// <typeparam name="T">The type of data to attempt to get.</typeparam>
        /// <returns></returns>
        T GetValue<T>();

        /// <summary>
        /// Changes the underlying value of the variable under the given context and according to the given value expression.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        EvaluationResult ChangeValue(EvaluationContext context, Expression value);

        /// <summary>
        /// Raised before the value of the variable is changed.
        /// </summary>
        event TypedEventHandler<IVariable, VariableChangeEventArgs> Change;
    }
}
