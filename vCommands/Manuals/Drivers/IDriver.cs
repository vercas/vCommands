using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Manuals.Drivers
{
    /// <summary>
    /// Represents a driver which accesses and displays a manual.
    /// </summary>
    public interface IDriver
    {
        /// <summary>
        /// Displays the given manual.
        /// </summary>
        /// <param name="context">A context under which the manual will be displayed.</param>
        /// <param name="m"></param>
        /// <returns></returns>
        EvaluationResult Display(EvaluationContext context, Manual m);

        /// <summary>
        /// Gets the name of the driver.
        /// </summary>
        String Name { get; }
    }
}
