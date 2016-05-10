using System;

namespace vCommands.EventArguments
{
    using Commands;
    
    /// <summary>
    /// Provides data for the <see cref="vCommands.CommandHost.CommandMutation"/> event.
    /// </summary>
    public class HostCommandMutationEventArgs
        : EventArgs
    {
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public String CommandName { get; internal set; }

        /// <summary>
        /// Gets the old command.
        /// </summary>
        public Command OldCommand { get; internal set; }

        /// <summary>
        /// Gets the new command.
        /// </summary>
        public Command NewCommand { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.EventArguments.HostCommandMutationEventArgs"/> class with the specified command name, old command value, and new command value.
        /// </summary>
        /// <param name="name">Name of command.</param>
        /// <param name="oldCommand">Old command; null if the command was just added.</param>
        /// <param name="newCommand">New commandl null if the command was just removed.</param>
        public HostCommandMutationEventArgs(string name, Command oldCommand, Command newCommand)
            : base()
        {
            this.CommandName = name;
            this.OldCommand = oldCommand;
            this.NewCommand = newCommand;
        }
    }
}
