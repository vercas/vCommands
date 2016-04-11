using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Manuals.Drivers
{
    using Utilities;

    /// <summary>
    /// A driver which prints the manual in the command output.
    /// </summary>
    /// <remarks>
    /// This class has no internal state, so it is recommended to use <see cref="vCommands.Manuals.Drivers.OutputDriver.Instance"/> whenever an instance is needed.
    /// </remarks>
    public class OutputDriver
        : IDriver
    {
        #region Constants and Statics

        /// <summary>
        /// The character used to separate section indexes.
        /// </summary>
        public const string SectionIndexesSeparator = ".";

        /// <summary>
        /// The character used to separate section titles and indexes.
        /// </summary>
        public const string SectionTitlesSeparator = " ";

        /// <summary>
        /// A line which separates the actual manual contents from the index, abstract and title.
        /// </summary>
        public const string LargeSeparator = "--------------------------------";

        static OutputDriver inst = new OutputDriver();

        /// <summary>
        /// Gets an instance of <see cref="vCommands.Manuals.Drivers.OutputDriver"/>.
        /// </summary>
        public static OutputDriver Instance { get { return inst; } }

        #endregion

        #region IDriver Members

        /// <summary>
        /// Formats the given manual and outputs it in the result.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given context or manual are null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given manual has a null title.</exception>
        public virtual EvaluationResult Display(EvaluationContext context, Manual m)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (m == null)
                throw new ArgumentNullException("m");

            if (m.Title == null)
                throw new ArgumentException("The given manual has a null title.");

            StringBuilder b = new StringBuilder();

            b.AppendLine("Title:");
            b.Append('\t');
            b.AppendLine(m.Title);
            b.AppendLine();

            if (m.Abstract != null)
            {
                b.AppendLine("Abstract:");
                b.Append('\t');
                b.AppendLine(m.Abstract);
                b.AppendLine();
            }

            var sectionIndexes = new Dictionary<Section, string>();

            b.AppendLine("Index:");

            int i = 0;
            foreach (var s in m.Sections)
                ManualSections.indexSection(b, s, ++i, "\t", sectionIndexes, SectionTitlesSeparator, SectionIndexesSeparator);

            b.AppendLine();
            b.AppendLine(LargeSeparator);

            foreach (var s in m.Sections)
                displaySection(b, s, sectionIndexes);

            return new EvaluationResult(CommonStatusCodes.Success, null, b.ToString());
        }

        /// <summary>
        /// Gets the name of the driver.
        /// </summary>
        public string Name
        {
            get { return "output"; }
        }

        #endregion

        #region Utilities

        void displaySection(StringBuilder b, Section s, IDictionary<Section, string> d)
        {
            b.AppendLine();

            b.Append(d[s]);
            b.Append(SectionTitlesSeparator);
            b.AppendLine(s.Title);

            b.AppendLine();

            b.AppendLine(s.Body);

            //b.AppendLine();

            foreach (var s2 in s.Subsections)
                displaySection(b, s2, d);
        }

        #endregion
    }
}
