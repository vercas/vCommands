using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace vCommands.Manuals.Drivers
{
    /// <summary>
    /// Represents a collection of <see cref="vCommands.Manuals.Drivers.IDriver"/>s.
    /// </summary>
    public class DriverCollection
        : ICollection<IDriver>
    {
        internal IDictionary<string, IDriver> drvs = new Dictionary<string, IDriver>();

        #region ICollection<IDriver> Members

        /// <summary>
        /// Adds the given driver to the collection.
        /// </summary>
        /// <param name="driver"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given driver is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the collection already contains a driver with the name of the given one -or- the given driver's name is null.</exception>
        /// <exception cref="System.NotSupportedException">Thrown when the collection is read-only.</exception>
        public void Add(IDriver driver)
        {
            if (driver == null)
                throw new ArgumentNullException("driver");

            if (driver.Name == null)
                throw new ArgumentException("The given driver's name is null.");

            if (drvs.ContainsKey(driver.Name))
                throw new ArgumentException("The collection already contains a driver with the same name.");

            drvs.Add(driver.Name, driver);
        }

        /// <summary>
        /// Adds every driver in the given enumeration to the collection.
        /// </summary>
        /// <param name="drivers"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given enumeration is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the collection already contains a driver with the name of a given one -or- a given driver's name is null.</exception>
        /// <exception cref="System.NotSupportedException">Thrown when the collection is read-only.</exception>
        public void Add(IEnumerable<IDriver> drivers)
        {
            if (drivers == null)
                throw new ArgumentNullException("drivers");

            int i = 0;

            foreach (var item in drivers)
            {
                if (item.Name == null)
                    throw new ArgumentException(string.Format("Driver at index {0} has a null name.", i));

                if (drvs.ContainsKey(item.Name))
                    throw new ArgumentException(string.Format("Collection already contains a driver with the name of that at index {0}.", i));

                drvs.Add(item.Name, item);

                i++;
            }
        }

        /// <summary>
        /// Removes all drivers from the collection.
        /// </summary>
        /// <exception cref="System.NotSupportedException">Thrown when the collection is read-only.</exception>
        public void Clear()
        {
            drvs.Clear();

            DefaultDriver = null;
        }

        /// <summary>
        /// Determines whether the collection contains the given driver specifically.
        /// </summary>
        /// <param name="driver"></param>
        /// <returns>True if the specific driver is contained within the collection; otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given driver is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given driver's name is null.</exception>
        public bool Contains(IDriver driver)
        {
            if (driver == null)
                throw new ArgumentNullException("driver");

            if (driver.Name == null)
                throw new ArgumentException("The given driver's name is null.");

            return drvs.Contains(new KeyValuePair<string, IDriver>(driver.Name, driver));
        }

        /// <summary>
        /// Determines whether the collection contains a driver with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if a driver with the specific name is contained within the collection; otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        public bool Contains(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            return drvs.ContainsKey(name);
        }

        /// <summary>
        /// Copies the drivers of the current collection to an <see cref="System.Array"/>, starting at a particular <see cref="System.Array"/> index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given array is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the given array index is less than 0.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the number of drivers in the collection is greater than the available space from the given index to the end of the destination array.</exception>
        public void CopyTo(IDriver[] array, int arrayIndex)
        {
            drvs.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of drivers contained in the collection.
        /// </summary>
        public int Count
        {
            get { return drvs.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only (no drivers can be added).
        /// </summary>
        public bool IsReadOnly
        {
            get { return drvs.IsReadOnly; }
        }

        /// <summary>
        /// Removes the specific driver from the collection.
        /// </summary>
        /// <param name="driver"></param>
        /// <returns>True if the specific driver was found and removed; false if not found.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given driver is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given driver's name is null.</exception>
        public bool Remove(IDriver driver)
        {
            if (driver == null)
                throw new ArgumentNullException("driver");

            if (driver.Name == null)
                throw new ArgumentException("The given driver's name is null.");

            IDriver temp = null;

            if (!drvs.TryGetValue(driver.Name, out temp))
                return false;

            if (driver == temp)
            {
                drvs.Remove(driver.Name);

                if (DefaultDriver == temp)
                    DefaultDriver = null;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the driver with the specified name from the library.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if found and removed; false if not found</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name string is null.</exception>
        public bool Remove(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (DefaultDriver != null && DefaultDriver.Name == name)
                DefaultDriver = null;

            return drvs.Remove(name);
        }

        #endregion

        #region IEnumerable<IDriver> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IDriver> GetEnumerator()
        {
            return drvs.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return drvs.Values.GetEnumerator();
        }

        #endregion

        #region Querying

        /// <summary>
        /// Retrieves the driver with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A <see cref="vCommands.Manuals.Drivers.IDriver"/> object if found; otherwise null.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        public IDriver this[string name]
        {
            get
            {
                if (name == null)
                    throw new ArgumentNullException("name");

                IDriver res = null;

                if (drvs.TryGetValue(name, out res))
                    return res;

                return null;
            }
        }

        /// <summary>
        /// Finds a driver by matching the name against a regular expression.
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given regular expression is null.</exception>
        public IEnumerable<IDriver> FindDriver(Regex mask)
        {
            if (mask == null)
                throw new ArgumentNullException("mask");

            return drvs.Values.Where(m => mask.IsMatch(m.Name));
        }

        #endregion

        #region Default

        /// <summary>
        /// Gets the default driver, if any.
        /// </summary>
        public IDriver DefaultDriver { get; private set; }

        /// <summary>
        /// Sets the default driver to the one carrying the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if found and set; false if not found.</returns>
        public bool SetDefault(string name)
        {
            if (!drvs.ContainsKey(name))
                return false;

            DefaultDriver = drvs[name];

            return true;
        }

        /// <summary>
        /// Sets the default driver to the given driver.
        /// </summary>
        /// <param name="driver"></param>
        /// <returns>True if the driver is contained in the collection and set; false if not contained.</returns>
        public bool SetDefault(IDriver driver)
        {
            if (!drvs.Values.Contains(driver))
                return false;

            DefaultDriver = driver;

            return true;
        }

        #endregion
    }
}
