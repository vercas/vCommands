using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Utilities
{
    using Manuals;

    internal class ManualSections
    {
        public static bool Check(Section initial, Section target)
        {
            if (initial == target)
                return true;

            return Check(initial.Subsections, target);
        }

        public static bool Check(IEnumerable<Section> list, Section tar)
        {
            foreach (var s in list)
            {
                if (s == tar)
                    return true;

                if (Check(s.Subsections, tar))
                    return true;
            }

            return false;
        }

        public static void indexSection(StringBuilder b, Section s, int i, string p, IDictionary<Section, string> d, string SectionTitlesSeparator, string SectionIndexesSeparator)
        {
            b.Append(p);
            b.Append(i);
            b.Append(SectionTitlesSeparator);
            b.AppendLine(s.Title);

            if (d != null)
                d[s] = p + i;

            p += i.ToString() + SectionIndexesSeparator;

            int j = 0;

            foreach (var s2 in s.Subsections)
                indexSection(b, s2, ++j, p, d, SectionTitlesSeparator, SectionIndexesSeparator);
        }
    }
}
