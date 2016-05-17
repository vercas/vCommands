using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands
{
    using Commands;
    using Variables;
    using EventArguments;
    using Utilities;

    /// <summary>
    /// Represents a container of <see cref="vCommands.Commands.Command"/>s and <see cref="vCommands.Variables.IVariable"/>s.
    /// </summary>
    public interface ICommandContainer
    {
        #region Commands

        /// <summary>
        /// Registers the given command to the command container.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="overwrite">True to overwrite an existing command; otherwise false.</param>
        /// <param name="overwriteSameTypeOnly">True to only override a command of the same type, otherwise false.</param>
        /// <returns>True if the command was added; fakse if it already existed.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given command is null.</exception>
        bool RegisterCommand(Command cmd, bool overwrite = false, bool overwriteSameTypeOnly = true);

        /// <summary>
        /// Removes the command with the given name, if found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if the command was found and removed; otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        bool RemoveCommand(string name);

        /// <summary>
        /// Removes the command with the given name and type, if found.
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="T">The type of command to remove.</typeparam>
        /// <returns>True if the command was found and removed; otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        bool RemoveCommand<T>(string name);

        /// <summary>
        /// Gets the command registerd with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A <see cref="vCommands.Commands.Command"/> object if found; otherwise null.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        Command GetCommand(string name);

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
        TVar RegisterVariable<TVar>(TVar variable, bool overwrite = false) where TVar : IVariable;

        /// <summary>
        /// Removes the variable with the given name, if found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if the variable was found and removed; otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        bool RemoveVariable(string name);

        /// <summary>
        /// Gets the variable registered with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A <see cref="vCommands.Variables.IVariable"/> object if found; otherwise null.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        IVariable GetVariable(string name);

        /// <summary>
        /// Gets the underlying value of a variable.
        /// </summary>
        /// <typeparam name="T">The type of data to attempt to get.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when there is no variable with the given name.</exception>
        T GetValue<T>(string name);

        /// <summary>
        /// Gets the underlying value of a variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when there is no variable with the given name.</exception>
        string GetValue(string name);

        #endregion

        #region Events

        /// <summary>
        /// Raised when a command is added, removed or replaced.
        /// </summary>
        event TypedEventHandler<ICommandContainer, CommandMutationEventArgs> CommandMutation;

        #endregion
    }
}
