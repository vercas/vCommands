using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace vCommands.Manual
{
    using Utilities;

    /// <summary>
    /// Represents a virtual set of instructions for using a command or variable or for learning a subject.
    /// </summary>
    public class Manual
        : Sealable
    {
        #region Properties and Fields

        string title = null, abstr = null;
        List<Section> subs = null; List<Keyword> keys = null; List<string> refs = null;
        ReadOnlyCollection<Section> subsro = null; ReadOnlyCollection<Keyword> keysro = null; ReadOnlyCollection<string> refsro = null;

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
        /// Gets a brief description of the subject of the manual.
        /// </summary>
        public String Abstract
        {
            get
            {
                return abstr;
            }
            set
            {
                CheckSeal();

                abstr = value;
            }
        }

        /// <summary>
        /// Gets a read only sequential collection of sections of the current manual.
        /// </summary>
        public ReadOnlyCollection<Section> Sections
        {
            get
            {
                return subsro;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Manual.Manual"/> class.
        /// </summary>
        public Manual()
        {
            this.subs = new List<Section>();
            this.subsro = new ReadOnlyCollection<Section>(subs);

            this.keys = new List<Keyword>();
            this.keysro = new ReadOnlyCollection<Keyword>(keys);

            this.refs = new List<string>();
            this.refsro = new ReadOnlyCollection<string>(refs);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Manual.Manual"/> class with the specified title and abstract.
        /// </summary>
        /// <param name="title">A suitable title to represent the subject of the manual.</param>
        /// <param name="abstr">A brief description of the subject of the manual.</param>
        public Manual(string title, string abstr)
            : this()
        {
            this.title = title;
            this.abstr = abstr;
        }

        #region Sections

        /// <summary>
        /// Adds the given section to the manual.
        /// </summary>
        /// <param name="sub"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given section is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given section is contained within the hierarcy of the manual -or- the manual already contains a section with the title of the given section.</exception>
        public void AddSection(Section sub)
        {
            if (sub == null)
                throw new ArgumentNullException("sub");

            if (SectionRecursionChecking.Check(this.subs, sub))
                throw new ArgumentException("The given section is already found within the hierarchy of the manual.");

            if (subs.Where(s => s.Title == sub.Title).Any())
                throw new ArgumentException("There already is a section with that title.");

            CheckSeal();

            subs.Add(sub);
        }

        /// <summary>
        /// Adds all sections from the given enumeration to the current manual.
        /// </summary>
        /// <param name="subsenum"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given enumeration is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the enumeration contains a null section -or- an enumerated section is contained within the hierarcy of the manual -or- the manual already contains a section with the title of an enumerated section.</exception>
        public void AddSections(IEnumerable<Section> subsenum)
        {
            if (subsenum == null)
                throw new ArgumentNullException("subsenum");

            try
            {
                foreach (var sub in subsenum)
                    AddSection(sub);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentException("The enumeration contains a null section.");
            }
        }

        /// <summary>
        /// Removes the given subection from the manual, if found.
        /// </summary>
        /// <param name="sub"></param>
        /// <returns>True if found and removed; false if not found.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given section is null.</exception>
        public bool RemoveSection(Section sub)
        {
            if (sub == null)
                throw new ArgumentNullException("sub");

            return subs.Remove(sub);
        }

        /// <summary>
        /// Removes the section with the given title from the manual, if found.
        /// </summary>
        /// <param name="title"></param>
        /// <returns>True if found and removed; false if not found.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given title is null.</exception>
        public bool RemoveSection(string title)
        {
            if (title == null)
                throw new ArgumentNullException("title");

            Section sub = null;

            sub = subs.Where(s => s.Title == title).FirstOrDefault();

            if (sub == null)
                return false;

            return subs.Remove(sub);
        }

        #endregion

        #region Keywords

        /// <summary>
        /// Adds the given keyword to the manual.
        /// </summary>
        /// <remarks>
        /// There can be duplicate words.
        /// </remarks>
        /// <param name="keyword"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given keyword is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the manual already contains the given keyword (identical word AND definition).</exception>
        public void AddKeyword(Keyword keyword)
        {
            if (keyword == null)
                throw new ArgumentNullException("keyword");

            if (keys.Contains(keyword))
                throw new ArgumentException("The manual already contains the given keyword instance.");

            CheckSeal();

            keys.Add(keyword);
        }

        /// <summary>
        /// Adds all keywords from the given enumeration to the current manual.
        /// </summary>
        /// <param name="keysenum"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given enumeration is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the enumeration contains a null keyword -or- the manual already contains an enumerated keyword (identical word AND definition).</exception>
        public void AddKeywords(IEnumerable<Keyword> keysenum)
        {
            if (keysenum == null)
                throw new ArgumentNullException("keysenum");

            try
            {
                foreach (var key in keysenum)
                    AddKeyword(key);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentException("The enumeration contains a null keyword.");
            }
        }

        /// <summary>
        /// Removes the given keyword from the manual, if found.
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns>True if found and removed; false if not found.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given keyword is null.</exception>
        public bool RemoveKeyword(Keyword keyword)
        {
            if (keyword == null)
                throw new ArgumentNullException("keyword");

            return keys.Remove(keyword);
        }

        /// <summary>
        /// Removes the subsection with the given title from the current section, if found.
        /// </summary>
        /// <param name="word"></param>
        /// <returns>True if found and removed; false if not found.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given title is null.</exception>
        public bool RemoveKeyword(string word)
        {
            if (word == null)
                throw new ArgumentNullException("word");

            Section sub = null;

            sub = subs.Where(s => s.Title == word).FirstOrDefault();

            if (sub == null)
                return false;

            return subs.Remove(sub);
        }

        #endregion

        #region References

        /// <summary>
        /// Adds the given reference string to the manual.
        /// </summary>
        /// <remarks>
        /// There can be duplicate words.
        /// </remarks>
        /// <param name="reff"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given string is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the manual already contains the given reference string.</exception>
        public void AddReference(string reff)
        {
            if (reff == null)
                throw new ArgumentNullException("reff");

            if (refs.Contains(reff))
                throw new ArgumentException("The manual already contains the given reference string.");

            CheckSeal();

            refs.Add(reff);
        }

        /// <summary>
        /// Adds all reference strings from the given enumeration to the current manual.
        /// </summary>
        /// <param name="refsenum"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given enumeration is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the enumeration contains a null keyword -or- the manual already contains an enumerated reference string.</exception>
        public void AddReferences(IEnumerable<string> refsenum)
        {
            if (refsenum == null)
                throw new ArgumentNullException("refsenum");

            try
            {
                foreach (var reff in refsenum)
                    AddReference(reff);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentException("The enumeration contains a null string.");
            }
        }

        /// <summary>
        /// Removes the given reference string from the manual, if found.
        /// </summary>
        /// <param name="reff"></param>
        /// <returns>True if found and removed; false if not found.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given reference string is null.</exception>
        public bool RemoveReference(string reff)
        {
            if (reff == null)
                throw new ArgumentNullException("reff");

            return refs.Remove(reff);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Seals the manual, making it unchangeable and refuse to add new sections, keywords or references.
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
        /// Determines whether the given <see cref="System.Object"/> is equal to the current <see cref="vCommands.Manual.Manual"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current manual.</param>
        /// <returns>true if the specified object is equal to the current manual; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            //  If the object is not a section, it will become null.

            return this.Equals(obj as Manual);
        }

        /// <summary>
        /// Determines whether the given <see cref="vCommands.Manual.Manual"/> is equal to the current <see cref="vCommands.Manual.Manual"/>.
        /// </summary>
        /// <param name="obj">The manual to compare with the current manual.</param>
        /// <returns>true if the specified manual is equal to the current manual; otherwise, false.</returns>
        public bool Equals(Manual obj)
        {
            if ((object)obj == null)    //  Casting to object avoids usage of the overloaded operator.
                return false;

            return (this.title == obj.title) && (this.abstr == obj.abstr) && this.subsro.SequenceEqual(obj.subsro) && this.keysro.SequenceEqual(obj.keysro) && this.refsro.SequenceEqual(obj.refsro);
        }

        /// <summary>
        /// Serves as a hash function for <see cref="vCommands.Manual.Manual"/>.
        /// </summary>
        /// <returns>A hash code for the current <see cref="vCommands.Manual.Manual"/>.</returns>
        public override int GetHashCode()
        {
            base.GetHashCode();
            return (this.title == null ? -1337 : this.title.GetHashCode()) ^ 892625479 ^ (this.abstr == null ? -9001 : this.abstr.GetHashCode()) ^ this.subsro.GetHashCode() ^ this.keysro.GetHashCode() ^ this.refsro.GetHashCode();
            //  A random prime number.
        }

        /// <summary>
        /// Determines whether two manuals are equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Manual a, Manual b)
        {
            //  Both null or identical pointers.
            if (ReferenceEquals(a, b))
                return true;

            //  One is null and the other is not.
            if ((object)a == null || (object)b == null)
                return false;

            return (a.title == b.title) && (a.abstr == b.abstr) && a.subsro.SequenceEqual(b.subsro) && a.keysro.SequenceEqual(b.keysro) && a.refsro.SequenceEqual(b.refsro);
        }

        /// <summary>
        /// Determines whether two manuals are unequal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Manual a, Manual b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Returns a string that represents the current manual.
        /// </summary>
        /// <remarks>
        /// This is only for debugging purposes. The resulted string is not suitable for placing in a manual.
        /// </remarks>
        /// <returns>A string that represents the current manual.</returns>
        public override string ToString()
        {
            return string.Format("[Manual{0} {1} ({2} sections) | {3}]", this.Sealed ? " (SEALED):" : ":", this.title ?? "-NULL T-", this.subsro.Count, this.abstr == null ? "-NULL A-" : (this.abstr.Length > 50 ? (this.abstr.Substring(0, 47) + "...") : this.abstr));
        }

        #endregion
    }
}
