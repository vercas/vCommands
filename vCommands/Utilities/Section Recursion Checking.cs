using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Utilities
{
    using Manual;

    internal class SectionRecursionChecking
    {
        public static bool Check(Section initial, Section target)
        {
            if (initial == target)
                return false;

            return Check(initial.Subsections, target);
        }

        public static bool Check(IEnumerable<Section> list, Section tar)
        {
            foreach (var s in list)
            {
                if (s == tar)
                    return false;

                if (!Check(s.Subsections, tar))
                    return false;
            }

            return true;
        }
    }
}
