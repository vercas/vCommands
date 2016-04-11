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
        /// <param name="context">The context under which the change occurs.</param>
        /// <param name="value">The expression representing the value to change the variable to.</param>
        /// <param name="ct">The means of extracting the actual value from the expression.</param>
        /// <returns>Result of the operation.</returns>
        EvaluationResult ChangeValue(EvaluationContext context, Expression value, ChangeType ct);

        /// <summary>
        /// Raised before the value of the variable is changed.
        /// </summary>
        event TypedEventHandler<IVariable, VariableChangeEventArgs> Change;
    }

    /// <summary>
    /// Possible ways to change the value of a variable from an expression evaluation result.
    /// </summary>
    public enum ChangeType
    {
        /// <summary>
        /// The value will be extracted exclusively from the data.
        /// </summary>
        FromData = 0x01,
        /// <summary>
        /// The value will be extracted exclusively from the output.
        /// </summary>
        FromOutput = 0x02,
        /// <summary>
        /// The value will be extracted from the data, and if that fails, from the output.
        /// </summary>
        FromDataOrOutput = 0x04,
        /// <summary>
        /// The value will be extracted from the output, and if that fails, from the data.
        /// </summary>
        FromOutputOrData = 0x08,
    }
}
