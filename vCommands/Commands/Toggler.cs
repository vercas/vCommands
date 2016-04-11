using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Commands
{
    /// <summary>
    /// Represents possible togglers for command executions.
    /// </summary>
    public enum Toggler
    {
        /// <summary>
        /// No toggler.
        /// </summary>
        Neutral = 0,
        /// <summary>
        /// On toggler.
        /// </summary>
        On = 1,
        /// <summary>
        /// Off toggler.
        /// </summary>
        Off = 2,
    }
}
