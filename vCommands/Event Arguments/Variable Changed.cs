using System;

namespace vCommands.EventArguments
{
    /// <summary>
    /// Provides data for the <see cref="vCommands.Variables.IVariable.Changed"/> event.
    /// </summary>
    public class VariableChangedEventArgs
        : EventArgs
    {
        /// <summary>
        /// Gets the current string value of the variable.
        /// </summary>
        public String OldStringValue { get; internal set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.EventArguments.VariableChangedEventArgs"/> class with the specified old string value.
        /// </summary>
        /// <param name="osv">Old string value.</param>
        public VariableChangedEventArgs(string osv)
        {
            this.OldStringValue = osv;
        }
    }
}
