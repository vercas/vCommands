﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace vCommands
{
    using Parsing.Expressions;

    /// <summary>
    /// Represents the context in which a <see cref="vCommands.Parsing.Expressions.Expression"/> is evaluated.
    /// </summary>
    public sealed class EvaluationContext
    {
        /// <summary>
        /// Gets the <see cref="vCommands.CommandHost"/> under which the evaluation occurs.
        /// </summary>
        public CommandHost Host { get; internal set; }

        /// <summary>
        /// Gets a read-only collection of argument expressions to a user command.
        /// </summary>
        public ReadOnlyCollection<Expression> UserArguments { get; internal set; }

        /// <summary>
        /// Gets a dictionary of iterators available in the context.
        /// </summary>
        public Dictionary<string, string> Locals { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.EvaluationContext"/> class with the specified status and output.
        /// </summary>
        /// <param name="host">The <see cref="vCommands.CommandHost"/> under which the evaluation occurs.</param>
        /// <param name="userArguments">optional; An list of expressions as arguments to a user command.</param>
        /// <param name="locals">optional; A list of local variables (pairs of names and values).</param>
        public EvaluationContext(CommandHost host, IList<Expression> userArguments = null, IDictionary<string, string> locals = null)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            this.Host = host;

            if (userArguments != null)
                this.UserArguments = new ReadOnlyCollection<Expression>(userArguments);

            if (locals == null)
                Locals = new Dictionary<string, string>();
            else
                Locals = new Dictionary<string, string>(locals);
        }

        /// <summary>
        /// Creates a new <see cref="vCommands.EvaluationContext"/> with the properties of the current one, but with the given user argument list.
        /// </summary>
        /// <param name="userArguments">A list of expressions as arguments to a user command.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given user argument list is null.</exception>
        public EvaluationContext WithUserArguments(IList<Expression> userArguments)
        {
            if (userArguments == null)
                throw new ArgumentNullException("userArguments");

            return new EvaluationContext(this.Host, userArguments);
        }

        /// <summary>
        /// Creates a new <see cref="vCommands.EvaluationContext"/> with the properties of the current one, but with the given local.
        /// </summary>
        /// <param name="name">The name of the local, as it will be accessed.</param>
        /// <param name="value">The value of the local.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when either of the given arguments is null.</exception>
        public EvaluationContext WithLocal(string name, string value)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (value == null)
                throw new ArgumentNullException("value");

            var ret = new EvaluationContext(this.Host, this.UserArguments, this.Locals);
            ret.Locals[name] = value;

            return ret;
        }

        /// <summary>
        /// Creates a new <see cref="vCommands.EvaluationContext"/> with the properties of the current one, but with the given iterator.
        /// </summary>
        /// <param name="locals">A list of local variables (pair of name and value).</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given arguments is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when a key or a value in the enumerable is null.</exception>
        public EvaluationContext WithLocal(IEnumerable<KeyValuePair<string, string>> locals)
        {
            if (locals == null)
                throw new ArgumentNullException("locals");

            var ret = new EvaluationContext(this.Host, this.UserArguments, this.Locals);

            foreach (var l in locals)
            {
                if (l.Key == null)
                    throw new ArgumentException("A local contains a null key.");
                if (l.Value == null)
                    throw new ArgumentException("A local contains a null value.");

                ret.Locals[l.Key] = l.Value;
            }

            return ret;
        }
    }
}
