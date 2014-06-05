using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace vCommands.Parsing.Expressions
{
    /// <summary>
    /// Represents a conditional statement - a condition with a required truth value, followed by a primary action and, optionally, a secondary action.
    /// </summary>
    public sealed class ConditionalExpression
        : Expression
    {
        /// <summary>
        /// Gets or sets the truth value of the condition necessary to trigger the primary action.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when trying to set the property after the expression is sealed.</exception>
        public Boolean TruthValue
        {
            get
            {
                return _value;
            }
            set
            {
                CheckSeal();

                _value = value;
            }
        }

        /// <summary>
        /// Gets or sets the expression which acts as a condition.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when trying to set the property after the expression is sealed.</exception>
        public Expression Condition
        {
            get
            {
                return _condition;
            }
            set
            {
                CheckSeal();

                _condition = value;
            }
        }

        /// <summary>
        /// Gets or sets the expression which evaluates when the condition meets the required truth value.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when trying to set the property after the expression is sealed.</exception>
        public Expression PrimaryAction
        {
            get
            {
                return _primary;
            }
            set
            {
                CheckSeal();

                _primary = value;
            }
        }

        /// <summary>
        /// Gets or sets the expression which evaluates when the condition does not meet the required truth value.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when trying to set the property after the expression is sealed.</exception>
        public Expression SecondaryAction
        {
            get
            {
                return _secondary;
            }
            set
            {
                if (Sealed)
                    throw new InvalidOperationException("Expression is sealed! Its properties may no longer be changed.");

                _secondary = value;
            }
        }

        bool _value = false;
        Expression _condition = null, _primary = null, _secondary = null;

        /// <summary>
        /// Evaluates the condition, and if it meets the required truth value, evaluates the primary action, otherwise the secondary action, if any.
        /// </summary>
        /// <param name="status">Numerical status value of the evaluation.</param>
        /// <param name="output">Text output of the evaluation.</param>
        /// <param name="context">The context of the evaluation.</param>
        protected override void Evaluate(out int status, out string output, EvaluationContext context)
        {
            if (Condition == null)
            {
                status = -2;
                output = "Missing condition expression!";

                return;
            }

            if (PrimaryAction == null)
            {
                status = -3;
                output = "Missing primary action expression!";

                return;
            }

            var evalRes = Condition.Evaluate(context);

            if (evalRes.TruthValue == this.TruthValue)
                evalRes = _primary.Evaluate(context);
            else if (_secondary != null)
                evalRes = _secondary.Evaluate(context);

            status = evalRes.Status;
            output = evalRes.Output;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Parsing.Expressions.ConditionalExpression"/> class with the specified truth value, condition and actions.
        /// </summary>
        /// <param name="value">optional; The truth value against which the condition is checked to determine which action is executed.</param>
        /// <param name="condition">optional; The expression which acts as a condition.</param>
        /// <param name="primaryAction">optional; The expression which evaluates when the condition meets the required truth value.</param>
        /// <param name="secondaryAction">optional; The expression which evaluates when the condition does not meet the required truth value.</param>
        public ConditionalExpression(bool value = false, Expression condition = null, Expression primaryAction = null, Expression secondaryAction = null)
        {
            this._value = value;
            this._condition = condition;
            this._primary = primaryAction;
            this._secondary = secondaryAction;

            Sealed = false;
        }

        /// <summary>
        /// Returns a string that represents the current conditional expression.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder(4096);

            sb.Append(_condition);
            sb.Append(TruthValue ? " ? " : " ! ");
            sb.Append(_primary);

            if (_secondary != null)
            {
                sb.Append(" : ");
                sb.Append(_secondary);
            }

            return sb.ToString();
        }
    }
}
