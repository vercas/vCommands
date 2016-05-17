using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace vCommands.Parsing.Expressions
{
    using Commands;

    /// <summary>
    /// Represents a command execution - a command name optionally followed by arguments.
    /// </summary>
    public sealed class CommandInvocationExpression
        : Expression
    {
        /// <summary>
        /// Gets or sets the toggle flag of the command.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when trying to set the property after the expression is sealed.</exception>
        public Toggler Toggle
        {
            get
            {
                return _toggle;
            }
            set
            {
                CheckSeal();

                _toggle = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the command.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when trying to set the property after the expression is sealed.</exception>
        public String CommandName
        {
            get
            {
                return _command;
            }
            set
            {
                CheckSeal();

                _command = value;
            }
        }

        /// <summary>
        /// Gets a read-only collection of arguments to the command.
        /// </summary>
        public ReadOnlyCollection<Expression> Arguments { get; internal set; }

        internal Toggler _toggle = Toggler.Neutral;
        internal string _command = null;
        internal List<Expression> args;
        internal Expression[] args_a = null;

        /// <summary>
        /// Evaluates the current expression.
        /// </summary>
        /// <param name="context">The context of the evaluation.</param>
        /// <param name="res">The variable which will contain the result of the evaluation.</param>
        protected override void Evaluate(EvaluationContext context, out EvaluationResult res)
        {
            Commands.Command cmd = null;

            if (context.State != null)
                cmd = context.State.GetCommand(CommandName);

            if (cmd == null)
                cmd = context.Host.GetCommand(CommandName);

            if (cmd == null)
            {
                res = new EvaluationResult(CommonStatusCodes.CommandNotFound, this
                    , string.Format(CultureInfo.InvariantCulture
                        , "Command not found: {0}"
                        , CommandName)
                    , this.CommandName);

                return;
            }

            res = cmd.Invoke(Toggle, context, args_a ?? args.ToArray());
            res.Expression = this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Parsing.Expressions.CommandInvocationExpression"/> class with the specified toggle flag, command name and optional arguments.
        /// </summary>
        /// <param name="toggle">optional; A flag indicating whether the command is toggled on, off or not toggled at all.</param>
        /// <param name="commandName">optional; The name of the command to execute.</param>
        /// <param name="arguments">optional; A set of arguments to provide at the start.</param>
        public CommandInvocationExpression(Toggler toggle = Toggler.Neutral, string commandName = null, IEnumerable<Expression> arguments = null)
        {
            if (arguments == null)
                args = new List<Expression>();
            else
                args = new List<Expression>(arguments);

            this._toggle = toggle;
            this._command = commandName;
            Arguments = new ReadOnlyCollection<Expression>(args);

            Sealed = false;
        }
        
        /// <summary>
        /// Adds the given expression as an argument.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>True if the argument was added (expression is not sealed); otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given expression is null.</exception>
        public bool AddArgument(Expression arg)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            if (Sealed)
                return false;

            args.Add(arg);

            return true;
        }

        /// <summary>
        /// Marks the expression as sealed, making it unchangeable.
        /// </summary>
        /// <returns>The current instance.</returns>
        public override void Seal()
        {
            base.Seal();

            args_a = args.ToArray();
        }

        /// <summary>
        /// Returns a string that represents the current conditional expression.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder(4096);

            if (_toggle != Toggler.Neutral)
            {
                sb.Append(_toggle == Toggler.On ? '+' : '-');
            }

            if (_command == null)
                sb.Append("!NULL COMMAND!");
            else if (_command.ToCharArray().Intersect(Parser.MustEscape).Any())
                sb.AppendFormat(CultureInfo.InvariantCulture
                    , "\"{0}\""
                    , _command.Replace("\\", "\\\\").Replace("\"", "\\\""));
            else
                sb.Append(_command);

            foreach (var a in args)
            {
                sb.Append(' ');

                if (a is ConstantExpression)
                    sb.Append(a);
                else
                    sb.AppendFormat(CultureInfo.InvariantCulture
                        , "[{0}]"
                        , a);
            }

            return sb.ToString();
        }
    }
}
