using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Manuals
{
    /// <summary>
    /// Possible locations to look up for finding a manual.
    /// </summary>
    [Flags]
    public enum ManualLookupLocation
    {
        /// <summary>
        /// The lookup will include the title of a manual.
        /// </summary>
        ManualTitle = 1,
        /// <summary>
        /// The lookup will include the abstract of a manual.
        /// </summary>
        ManualAbstract = 2,

        /// <summary>
        /// The lookup will include titles of sections.
        /// </summary>
        SectionTitles = 4,
        /// <summary>
        /// The lookup will include bodies of sections.
        /// </summary>
        SectionBodies = 8,

        /// <summary>
        /// The whole manual will be looked up.
        /// </summary>
        WholeManual = ManualTitle | ManualAbstract | SectionTitles | SectionBodies,
    }
}
