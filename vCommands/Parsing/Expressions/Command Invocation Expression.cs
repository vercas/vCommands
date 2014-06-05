﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace vCommands.Parsing.Expressions
{
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
        public Boolean? Toggle
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

        internal bool? _toggle = null;
        internal string _command = null;
        internal List<Expression> args;
        internal Expression[] args_a = null;

        /// <summary>
        /// Evaluates the current expression.
        /// </summary>
        /// <param name="status">Numerical status value of the evaluation.</param>
        /// <param name="output">Text output of the evaluation.</param>
        /// <param name="context">The context of the evaluation.</param>
        protected override void Evaluate(out int status, out string output, EvaluationContext context)
        {
            if (context == null)
            {
                status = -5;
                output = "Missing context from which to look up command!";

                return;
            }

            var cmd = context.Host.GetCommand(CommandName);

            if (cmd == null)
            {
                status = -6;
                output = string.Format("Command does not exist: {0}", CommandName);

                return;
            }

            var res = cmd.Invoke(Toggle, context, args_a ?? args.ToArray());

            status = res.Status;
            output = res.Output;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Parsing.Expressions.CommandInvocationExpression"/> class with the specified toggle flag, command name and optional arguments.
        /// </summary>
        /// <param name="toggle">optional; A flag indicating whether the command is toggled on, off or not toggled at all.</param>
        /// <param name="commandName">optional; The name of the command to execute.</param>
        /// <param name="arguments">optional; A set of arguments to provide at the start.</param>
        public CommandInvocationExpression(bool? toggle = null, string commandName = null, IEnumerable<Expression> arguments = null)
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

            if (_toggle != null)
            {
                sb.Append(_toggle.Value ? '+' : '-');
            }

            if (_command == null)
                sb.Append("!NULL COMMAND!");
            else if (_command.ToCharArray().Intersect(Parser.MustEscape).Any())
                sb.AppendFormat("\"{0}\"", _command.Replace("\\", "\\\\").Replace("\"", "\\\""));
            else
                sb.Append(_command);

            foreach (var a in args)
            {
                sb.Append(' ');

                if (a is ConstantExpression)
                    sb.Append(a);
                else
                    sb.AppendFormat("[{0}]", a);
            }

            return sb.ToString();
        }
    }
}
