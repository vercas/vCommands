using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands
{
    using Commands;
    using Parsing;
    using Variables;
    using EventArguments;
    using Utilities;

    /// <summary>
    /// Represents a host of console commands.
    /// </summary>
    public sealed class CommandHost
    {
        //internal SortedList<string, Command> cmds = new SortedList<string, Command>();
        internal IDictionary<string, Command> cmds = new Dictionary<string, Command>();
        internal IDictionary<string, IVariable> vars = new Dictionary<string, IVariable>();

        private object cmds_locker = new object(), vars_locker = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.CommandHost"/> class.
        /// </summary>
        public CommandHost()
        {
            
        }

        #region Commands

        /// <summary>
        /// Registers the given command to the host.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="overwrite">True to overwrite an existing command; otherwise false.</param>
        /// <param name="overwriteSameTypeOnly">True to only override a command of the same type, otherwise false.</param>
        /// <returns>True if the command was added; fakse if it already existed.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given command is null.</exception>
        public bool RegisterCommand(Command cmd, bool overwrite = false, bool overwriteSameTypeOnly = true)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            //return cmds.TryAdd(cmd.Name, cmd);

            lock (cmds_locker)
            {
                Command existing = null;

                if (cmds.TryGetValue(cmd.Name, out existing))
                {
                    if (overwrite)
                    {
                        if (overwriteSameTypeOnly && existing.GetType() != cmd.GetType())
                            return false;

                        cmds[cmd.Name] = cmd;
                    }
                    else
                        return false;
                }
                else
                    cmds.Add(cmd.Name, cmd);
            }

            return true;
        }

        /// <summary>
        /// Removes the command with the given name, if found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if the command was found and removed; otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        public bool RemoveCommand(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            lock (cmds_locker)
                return cmds.Remove(name);
        }

        /// <summary>
        /// Removes the command with the given name and type, if found.
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="T">The type of command to remove.</typeparam>
        /// <returns>True if the command was found and removed; otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        public bool RemoveCommand<T>(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            lock (cmds_locker)
            {
                Command cmd;

                if (cmds.TryGetValue(name, out cmd))
                    if (cmd is T)
                        return cmds.Remove(name);
            }

            return false;
        }

        /// <summary>
        /// Gets the command registerd with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A <see cref="vCommands.Commands.Command"/> object if found; otherwise null.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        public Command GetCommand(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Command res = null;

            lock (cmds_locker)
                if (cmds.TryGetValue(name, out res))
                    return res;

            return null;
        }

        /// <summary>
        /// Evaluates the given command string and returns the result.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given command string is null.</exception>
        public EvaluationResult Evaluate(string command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            return Parser.Parse(command).Evaluate(new EvaluationContext(this));
        }

        /// <summary>
        /// Registers a default set of commands to the host.
        /// </summary>
        public void RegisterDefaultCommands()
        {
            DefaultCommands.Register(this);
        }

        #endregion

        #region Variables

        /// <summary>
        /// Registers the given command variable to the host.
        /// </summary>
        /// <typeparam name="TVar">The type of variable to register, implementing <see cref="vCommands.Variables.IVariable"/>.</typeparam>
        /// <param name="variable"></param>
        /// <param name="overwrite">True to override an existing variable with the same name if found; otherwise false.</param>
        /// <returns>The given variable.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given variable is null.</exception>
        public TVar RegisterVariable<TVar>(TVar variable, bool overwrite = false)
            where TVar : IVariable
        {
            if (variable == null)
                throw new ArgumentNullException("variable");

            lock (vars_locker)
            {
                if (vars.ContainsKey(variable.Name) && !overwrite)
                    throw new ArgumentException(string.Format("A variable with the same name already exists: {0}", variable.Name));

                vars[variable.Name] = variable;
            }

            return variable;
        }

        /// <summary>
        /// Removes the variable with the given name, if found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if the variable was found and removed; otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        public bool RemoveVariable(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            lock (vars_locker)
                return vars.Remove(name);
        }

        /// <summary>
        /// Gets the variable registered with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A <see cref="vCommands.Variables.IVariable"/> object if found; otherwise null.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        public IVariable GetVariable(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            IVariable res = null;

            lock (vars_locker)
                if (vars.TryGetValue(name, out res))
                    return res;

            return null;
        }

        /// <summary>
        /// Gets the underlying value of a variable.
        /// </summary>
        /// <typeparam name="T">The type of data to attempt to get.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when there is no variable with the given name.</exception>
        public T GetValue<T>(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            var variable = GetVariable(name);

            if (variable == null)
                throw new ArgumentException("There is no variable with that name.", "name");

            return variable.GetValue<T>();
        }

        /// <summary>
        /// Gets the underlying value of a variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when there is no variable with the given name.</exception>
        public string GetValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            var variable = GetVariable(name);

            if (variable == null)
                throw new ArgumentException("There is no variable with that name.", "name");

            return variable.StringValue;
        }

        #endregion

        #region Events

#if HCIE
        /// <summary>
        /// Raised before a command is invoked.
        /// </summary>
        public event TypedEventHandler<CommandHost, HostCommandInvocationEventArgs> CommandInvocation;

        /// <summary>
        /// Raisese the <see cref="vCommands.CommandHost.CommandInvocation"/> event.
        /// </summary>
        /// <param name="e">A <see cref="vCommands.EventArguments.HostCommandInvocationEventArgs"/> that contains event data.</param>
        internal void OnInvocation(HostCommandInvocationEventArgs e)
        {
            var ev = CommandInvocation;

            if (ev != null)
            {
                ev(this, e);
            }
        }
#endif

#if HVCE
        /// <summary>
        /// Raised before a command is invoked.
        /// </summary>
        public event TypedEventHandler<CommandHost, HostVariableChangeEventArgs> VariableChange;

        /// <summary>
        /// Raisese the <see cref="vCommands.CommandHost.VariableChange"/> event.
        /// </summary>
        /// <param name="e">A <see cref="vCommands.EventArguments.HostVariableChangeEventArgs"/> that contains event data.</param>
        internal void OnChange(HostVariableChangeEventArgs e)
        {
            var ev = VariableChange;

            if (ev != null)
            {
                ev(this, e);
            }
        }
#endif

        #endregion
    }
}
