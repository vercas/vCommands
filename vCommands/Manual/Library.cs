using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace vCommands.Manual
{
    /// <summary>
    /// Represents a collection of <see cref="vCommands.Manual.Manual"/>s.
    /// </summary>
    public class Library
        : ICollection<Manual>
    {
        internal IDictionary<string, Manual> mans = new Dictionary<string, Manual>();

        #region ICollection<Manual> Members

        /// <summary>
        /// Adds the given manual to the library.
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given manual is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the library already contains a manual with the title of the given one -or- the given manual's title is null.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when the given manual is not sealed.</exception>
        /// <exception cref="System.NotSupportedException">Thrown when the library is read-only.</exception>
        public void Add(Manual item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (item.Title == null)
                throw new ArgumentException("The given manual's title is null.");

            if (!item.Sealed)
                throw new InvalidOperationException("Added manual must be sealed.");

            if (mans.ContainsKey(item.Title))
                throw new ArgumentException("The library already contains a manual with the same title.");

            mans.Add(item.Title, item);
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
        /// <param name="item"></param>
        /// <returns>True if the specific manual is contained within the library; otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given manual is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given manual's title is null.</exception>
        public bool Contains(Manual item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (item.Title == null)
                throw new ArgumentException("The given manual's title is null.");

            return mans.Contains(new KeyValuePair<string, Manual>(item.Title, item));
        }

        /// <summary>
        /// Determines whether the library contains a manual with the given title.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if a manual with the specific title is contained within the library; otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given title is null.</exception>
        public bool Contains(string item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            return mans.ContainsKey(item);
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
        /// <param name="item"></param>
        /// <returns>True if the specific manual was found and removed; false if not found.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given manual is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given manual's title is null.</exception>
        public bool Remove(Manual item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (item.Title == null)
                throw new ArgumentException("The given manual's title is null.");

            Manual temp = null;

            if (!mans.TryGetValue(item.Title, out temp))
                return false;

            if (item == temp)
            {
                mans.Remove(item.Title);

                return true;
            }

            return false;
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
        /// <returns>A <see cref="vCommands.Manual.Manual"/> object if found; otherwise null.</returns>
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
        /// <returns>A <see cref="vCommands.Manual.Section"/> object if found; otherwise null.</returns>
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

                Section sec = res.Sections[indexes[0]];

                for (int i = 1; i < indexes.Length; i++)
                {
                    if (indexes[i] < 0)
                        throw new ArgumentOutOfRangeException("Every index in the indexes array must be greated than or equal to 0.");

                    if (indexes[i] >= sec.Subsections.Count)
                        return null;

                    sec = sec.Subsections[indexes[i]];
                }

                return sec;
            }
        }

        #endregion
    }
}
