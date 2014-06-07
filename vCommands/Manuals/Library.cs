using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace vCommands.Manuals
{
    /// <summary>
    /// Represents a collection of <see cref="vCommands.Manuals.Manual"/>s.
    /// </summary>
    public class Library
        : ICollection<Manual>
    {
        internal IDictionary<string, Manual> mans = new Dictionary<string, Manual>();

        #region ICollection<Manual> Members

        /// <summary>
        /// Adds the given manual to the library.
        /// </summary>
        /// <param name="manual"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given manual is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the library already contains a manual with the title of the given one -or- the given manual's title is null.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when the given manual is not sealed.</exception>
        /// <exception cref="System.NotSupportedException">Thrown when the library is read-only.</exception>
        public void Add(Manual manual)
        {
            if (manual == null)
                throw new ArgumentNullException("manual");

            if (manual.Title == null)
                throw new ArgumentException("The given manual's title is null.");

            if (!manual.Sealed)
                throw new InvalidOperationException("Added manual must be sealed.");

            if (mans.ContainsKey(manual.Title))
                throw new ArgumentException("The library already contains a manual with the same title.");

            mans.Add(manual.Title, manual);
        }

        /// <summary>
        /// Adds every manual in the given enumeration to the library.
        /// </summary>
        /// <param name="manuals"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given enumeration is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the library already contains a manual with the title of a given one -or- a given manual's title is null.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when a given manual is not sealed.</exception>
        /// <exception cref="System.NotSupportedException">Thrown when the library is read-only.</exception>
        public void Add(IEnumerable<Manual> manuals)
        {
            if (manuals == null)
                throw new ArgumentNullException("manuals");

            int i = 0;

            foreach (var item in manuals)
            {
                if (item.Title == null)
                    throw new ArgumentException(string.Format("Manual at index {0} has a null title.", i));

                if (!item.Sealed)
                    throw new InvalidOperationException(string.Format("Manual at index {0} must be sealed.", i));

                if (mans.ContainsKey(item.Title))
                    throw new ArgumentException(string.Format("Library already contains a manual with the title of that at index {0}.", i));

                mans.Add(item.Title, item);

                i++;
            }
        }

        /// <summary>
        /// Removes all manuals from the library.
        /// </summary>
        /// <exception cref="System.NotSupportedException">Thrown when the library is read-only.</exception>
        public void Clear()
        {
            mans.Clear();
        }

        /// <summary>
        /// Determines whether the library contains the given manual specifically.
        /// </summary>
        /// <param name="manual"></param>
        /// <returns>True if the specific manual is contained within the library; otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given manual is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given manual's title is null.</exception>
        public bool Contains(Manual manual)
        {
            if (manual == null)
                throw new ArgumentNullException("manual");

            if (manual.Title == null)
                throw new ArgumentException("The given manual's title is null.");

            return mans.Contains(new KeyValuePair<string, Manual>(manual.Title, manual));
        }

        /// <summary>
        /// Determines whether the library contains a manual with the given title.
        /// </summary>
        /// <param name="title"></param>
        /// <returns>True if a manual with the specific title is contained within the library; otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given title is null.</exception>
        public bool Contains(string title)
        {
            if (title == null)
                throw new ArgumentNullException("title");

            return mans.ContainsKey(title);
        }

        /// <summary>
        /// Copies the manuals of the current library to an <see cref="System.Array"/>, starting at a particular <see cref="System.Array"/> index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given array is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the given array index is less than 0.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the number of manuals in the library is greater than the available space from the given index to the end of the destination array.</exception>
        public void CopyTo(Manual[] array, int arrayIndex)
        {
            mans.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of manuals contained in the library.
        /// </summary>
        public int Count
        {
            get { return mans.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the library is read-only (no manuals can be added).
        /// </summary>
        public bool IsReadOnly
        {
            get { return mans.IsReadOnly; }
        }

        /// <summary>
        /// Removes the specific manual from the library.
        /// </summary>
        /// <param name="manual"></param>
        /// <returns>True if the specific manual was found and removed; false if not found.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given manual is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given manual's title is null.</exception>
        public bool Remove(Manual manual)
        {
            if (manual == null)
                throw new ArgumentNullException("manual");

            if (manual.Title == null)
                throw new ArgumentException("The given manual's title is null.");

            Manual temp = null;

            if (!mans.TryGetValue(manual.Title, out temp))
                return false;

            if (manual == temp)
            {
                mans.Remove(manual.Title);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the manual with the specified title from the library.
        /// </summary>
        /// <param name="title"></param>
        /// <returns>True if found and removed; false if not found</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given title string is null.</exception>
        public bool Remove(string title)
        {
            if (title == null)
                throw new ArgumentNullException("title");

            return mans.Remove(title);
        }

        #endregion

        #region IEnumerable<Manual> Members

        /// <summary>
        /// Returns an enumerator that iterates through the library.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Manual> GetEnumerator()
        {
            return mans.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mans.Values.GetEnumerator();
        }

        #endregion

        #region Querying

        /// <summary>
        /// Retrieves the manual with the given title.
        /// </summary>
        /// <param name="title"></param>
        /// <returns>A <see cref="vCommands.Manuals.Manual"/> object if found; otherwise null.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given title is null.</exception>
        public Manual this[string title]
        {
            get
            {
                if (title == null)
                    throw new ArgumentNullException("title");

                Manual res = null;
                
                if (mans.TryGetValue(title, out res))
                    return res;

                return null;
            }
        }

        /// <summary>
        /// Retrieves the section at the given index from the manual with the given title.
        /// </summary>
        /// <remarks>
        /// If the manual or any section is not found, it returns null.
        /// </remarks>
        /// <param name="title"></param>
        /// <param name="indexes">Sequential indexes to look up for in the manual.</param>
        /// <returns>A <see cref="vCommands.Manuals.Section"/> object if found; otherwise null.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given title or indexes array is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given indexes array does not contain at least one element.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the given indexes array contains a negative element.</exception>
        public Section this[string title, params int[] indexes]
        {
            get
            {
                if (title == null)
                    throw new ArgumentNullException("title");
                if (indexes == null)
                    throw new ArgumentNullException("indexes");

                if (indexes.Length < 1)
                    throw new ArgumentException("Given indexes array must have at least one element.");

                Manual res = null;

                if (!mans.TryGetValue(title, out res))
                    return null;

                return res[indexes];
            }
        }

        /// <summary>
        /// Finds a manual by matching specific elements against a regular expression.
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="lookupLocation">A set of flags containing elements to look up.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given regular expression is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given lookup location set does not contain any location (is 0).</exception>
        public IEnumerable<Manual> FindManual(Regex mask, ManualLookupLocation lookupLocation = ManualLookupLocation.ManualTitle)
        {
            if (mask == null)
                throw new ArgumentNullException("mask");

            if (lookupLocation == 0)
                throw new ArgumentException("There must be at least one lookup location.", "lookupLocation");

            return mans.Values.Where(m => m.IsMatch(mask, lookupLocation));
        }

        #endregion
    }
}
