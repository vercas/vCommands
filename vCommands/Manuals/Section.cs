using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace vCommands.Manuals
{
    using Utilities;

    /// <summary>
    /// Represents a section in a manual.
    /// </summary>
    public class Section
        : Sealable
    {
        #region Properties and Fields

        string title = null, body = null;
        List<Section> subs = null;
        ReadOnlyCollection<Section> subsro = null;

        /// <summary>
        /// Gets the title of the section.
        /// </summary>
        public String Title
        {
            get
            {
                return title;
            }
            set
            {
                CheckSeal();

                title = value;
            }
        }

        /// <summary>
        /// Gets the body of the section.
        /// </summary>
        public String Body
        {
            get
            {
                return body;
            }
            set
            {
                CheckSeal();

                body = value;
            }
        }

        /// <summary>
        /// Gets a read only sequential collection of subsections of the current section.
        /// </summary>
        public ReadOnlyCollection<Section> Subsections
        {
            get
            {
                return subsro;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Manuals.Section"/> class.
        /// </summary>
        public Section()
        {
            this.subs = new List<Section>();
            this.subsro = new ReadOnlyCollection<Section>(subs);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Manuals.Section"/> class with the specified title and body.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="body"></param>
        public Section(string title, string body)
            : this()
        {
            this.title = title;
            this.body = body;
        }

        #region Subsections

        /// <summary>
        /// Adds the given section as a subsection to the current section.
        /// </summary>
        /// <param name="sub"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given subsection is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given section is identical to the current one -or- the given section is contained within the hierarcy of the current one -or- the current section is contained within the hierarchy of the given one -or- the current section already contains a subsection with the title of the given section.</exception>
        public void AddSubsection(Section sub)
        {
            if (sub == null)
                throw new ArgumentNullException("sub");

            if (sub == this)
                throw new ArgumentException("Cannot add the current section as subsection to itself.");

            if (ManualSections.Check(sub, this))
                throw new ArgumentException("The current section is already found within the subsection hierarchy of the given section.");
            if (ManualSections.Check(this, sub))
                throw new ArgumentException("The given section is already found within the subsection hierarchy of the current section.");

            if (subs.Where(s => s.title == sub.title).Any())
                throw new ArgumentException("There already is a subsection with that title.");

            CheckSeal();

            subs.Add(sub);
        }

        /// <summary>
        /// Adds all sections from the given enumeration as subsections to the current section.
        /// </summary>
        /// <param name="subs"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given enumeration is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the enumeration contains a null section -or- an enumerated section is identical to the current one -or- an enumerated section is contained within the hierarcy of the current one -or- the current section is contained within the hierarchy of an enumerated one -or- the current section already contains a subsection with the title of an enumerated section.</exception>
        public void AddSubsections(IEnumerable<Section> subs)
        {
            if (subs == null)
                throw new ArgumentNullException("subs");

            try
            {
                foreach (var sub in subs)
                    AddSubsection(sub);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentException("The enumeration contains a null section.");
            }
        }

        /// <summary>
        /// Removes the given subection from the current section, if found.
        /// </summary>
        /// <param name="sub"></param>
        /// <returns>True if found and removed; false if not found.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given subsection is null.</exception>
        public bool RemoveSubsection(Section sub)
        {
            if (sub == null)
                throw new ArgumentNullException("sub");

            return subs.Remove(sub);
        }

        /// <summary>
        /// Removes the subsection with the given title from the current section, if found.
        /// </summary>
        /// <param name="title"></param>
        /// <returns>True if found and removed; false if not found.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given title is null.</exception>
        public bool RemoveSubsection(string title)
        {
            if (title == null)
                throw new ArgumentNullException("title");

            Section sub = null;

            sub = subs.Where(s => s.title == title).FirstOrDefault();

            if (sub == null)
                return false;

            return subs.Remove(sub);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Seals the section, making it unchangeable and refuse to add new subsections.
        /// </summary>
        /// <remarks>
        /// Subsections are sealed as well.
        /// </remarks>
        public override void Seal()
        {
            base.Seal();

            foreach (var ss in this.subs)
                ss.Seal();
        }

        /// <summary>
        /// Determines whether the given <see cref="System.Object"/> is equal to the current <see cref="vCommands.Manuals.Section"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current section.</param>
        /// <returns>true if the specified object is equal to the current section; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            //  If the object is not a section, it will become null.

            return this.Equals(obj as Section);
        }

        /// <summary>
        /// Determines whether the given <see cref="vCommands.Manuals.Section"/> is equal to the current <see cref="vCommands.Manuals.Section"/>.
        /// </summary>
        /// <param name="obj">The section to compare with the current section.</param>
        /// <returns>true if the specified section is equal to the current section; otherwise, false.</returns>
        public bool Equals(Section obj)
        {
            if ((object)obj == null)    //  Casting to object avoids usage of the overloaded operator.
                return false;

            return (this.title == obj.title) && (this.body == obj.body) && (this.subsro.SequenceEqual(obj.subsro));
        }

        /// <summary>
        /// Serves as a hash function for <see cref="vCommands.Manuals.Section"/>.
        /// </summary>
        /// <returns>A hash code for the current <see cref="vCommands.Manuals.Section"/>.</returns>
        public override int GetHashCode()
        {
            base.GetHashCode();
            return (this.title == null ? -1337 : this.title.GetHashCode()) ^ 892625479 ^ (this.body == null ? -9001 : this.body.GetHashCode()) ^ this.subsro.GetHashCode();
            //  A random prime number.
        }

        /// <summary>
        /// Determines whether two sections are equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Section a, Section b)
        {
            //  Both null or identical pointers.
            if (ReferenceEquals(a, b))
                return true;

            //  One is null and the other is not.
            if ((object)a == null || (object)b == null)
                return false;

            return (a.title == b.title) && (a.body == b.body) && (a.subsro.SequenceEqual(b.subsro));
        }

        /// <summary>
        /// Determines whether two sections are unequal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Section a, Section b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Returns a string that represents the current section.
        /// </summary>
        /// <remarks>
        /// This is only for debugging purposes. The resulted string is not suitable for placing in a manual.
        /// </remarks>
        /// <returns>A string that represents the current section.</returns>
        public override string ToString()
        {
            return string.Format("[Section{0} {1} ({2} subsections) | {3}]", this.Sealed ? " (SEALED):" : ":", this.title ?? "-NULL T-", this.subsro.Count, this.body == null ? "-NULL B-" : (this.body.Length > 50 ? (this.body.Substring(0, 47) + "...") : this.body));
        }

        #endregion

        #region Lookup

        internal bool IsMatch(System.Text.RegularExpressions.Regex mask, ManualLookupLocation ll)
        {
            if ((ll & ManualLookupLocation.SectionTitles) != 0)
                if (mask.IsMatch(title))
                    return true;

            if ((ll & ManualLookupLocation.SectionBodies) != 0)
                if (mask.IsMatch(body))
                    return true;

            if (subs.Where(s => s.IsMatch(mask, ll)).Any())
                return true;

            return false;
        }

        #endregion
    }
}
