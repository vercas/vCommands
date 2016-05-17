using System;
using System.Globalization;

namespace vCommands.Manuals
{
    using Utilities;

    /// <summary>
    /// Represents a word or a group of words of importance, accompanied by a definition.
    /// </summary>
    public class Keyword
        : Sealable
    {
        #region Properties and Fields

        string word = null, definition = null;

        /// <summary>
        /// Gets the word defined by the keyword.
        /// </summary>
        public String Word
        {
            get
            {
                return word;
            }
            set
            {
                CheckSeal();

                word = value;
            }
        }

        /// <summary>
        /// Gets the body of the section.
        /// </summary>
        public String Definition
        {
            get
            {
                return definition;
            }
            set
            {
                CheckSeal();

                definition = value;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Manuals.Keyword"/> class.
        /// </summary>
        public Keyword()
        {

        }

        #region Overrides

        /// <summary>
        /// Determines whether the given <see cref="System.Object"/> is equal to the current <see cref="vCommands.Manuals.Keyword"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current keyword.</param>
        /// <returns>true if the specified object is equal to the current keyword; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            //  If the object is not a keyword, it will become null.

            return this.Equals(obj as Keyword);
        }

        /// <summary>
        /// Determines whether the given <see cref="vCommands.Manuals.Keyword"/> is equal to the current <see cref="vCommands.Manuals.Keyword"/>.
        /// </summary>
        /// <param name="obj">The keyword to compare with the current keyword.</param>
        /// <returns>true if the specified keyword is equal to the current keyword; otherwise, false.</returns>
        public bool Equals(Keyword obj)
        {
            if ((object)obj == null)    //  Casting to object avoids usage of the overloaded operator.
                return false;

            return (this.word == obj.word) && (this.definition == obj.definition);
        }

        /// <summary>
        /// Serves as a hash function for <see cref="vCommands.Manuals.Keyword"/>.
        /// </summary>
        /// <returns>A hash code for the current <see cref="vCommands.Manuals.Keyword"/>.</returns>
        public override int GetHashCode()
        {
            base.GetHashCode();
            return (this.word == null ? -1337 : this.word.GetHashCode()) ^ 892625479 ^ (this.definition == null ? -9001 : this.definition.GetHashCode());
            //  A random prime number.
        }

        /// <summary>
        /// Determines whether two keywords are equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Keyword a, Keyword b)
        {
            //  Both null or identical pointers.
            if (ReferenceEquals(a, b))
                return true;

            //  One is null and the other is not.
            if ((object)a == null || (object)b == null)
                return false;

            return (a.word == b.word) && (a.definition == b.definition);
        }

        /// <summary>
        /// Determines whether two keywords are unequal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Keyword a, Keyword b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Returns a string that represents the current keyword.
        /// </summary>
        /// <remarks>
        /// This is only for debugging purposes. The resulted string is not suitable for placing in a manual.
        /// </remarks>
        /// <returns>A string that represents the current keyword.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture
                , "[Keyword{0} {1} | {2}]"
                , this.Sealed ? " (SEALED):" : ":", this.word ?? "-NULL W-"
                , this.definition == null ? "-NULL D-" : (this.definition.Length > 50 ? (this.definition.Substring(0, 47) + "...") : this.definition));
        }

        #endregion
    }
}
