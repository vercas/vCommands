using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace vCommands.Parsing.Expressions
{
    /// <summary>
    /// Represents a constant expression - its result is always positive and the value never changes.
    /// </summary>
    public sealed class ConstantExpression
        : Expression
    {
        /// <summary>
        /// Gets the constant value of the expression.
        /// </summary>
        public String Value { get; private set; }

        /// <summary>
        /// Evaluates the current expression.
        /// </summary>
        /// <param name="status">Numerical status value of the evaluation.</param>
        /// <param name="output">Text output of the evaluation.</param>
        /// <param name="context">The context of the evaluation.</param>
        protected override void Evaluate(out int status, out string output, EvaluationContext context)
        {
            status = 0;
            output = Value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Parsing.Expressions.ConstantExpression"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given value is null.</exception>
        public ConstantExpression(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            this.Value = value;

            Seal();
        }

        /// <summary>
        /// Returns a string that represents the current constant expression.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Value) || Value.ToCharArray().Intersect(Parser.MustEscape).Any())
                return string.Format("\"{0}\"", Value.Replace("\\", "\\\\").Replace("\"", "\\\""));
            else
                return Value;
        }
    }
}
