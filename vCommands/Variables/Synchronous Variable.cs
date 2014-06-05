using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Variables
{
    /// <summary>
    /// Represents a variable that can be used in the console and accessed from any number of places.
    /// </summary>
    public class SynchronousVariable<T>
        : Variable<T>
    {
        /// <summary>
        /// The default abstract of a <see cref="vCommands.Variables.SynchronousVariable{T}"/>.
        /// </summary>
        public static new string DefaultAbstract = "Synchronous value-backed command variable.";

        object locker = new object();

        /// <summary>
        /// Gets or sets the value of the variable.
        /// </summary>
        public override T Value
        {
            get
            {
                lock (locker)
                    return val;
            }
            set
            {
                lock (locker)
                    val = value;
            }
        }

        /// <summary>
        /// Gets or sets the string representing the value of the variable.
        /// </summary>
        public override String StringValue
        {
            get
            {
                lock (locker)
                    return val.ToString();
            }
            set
            {
                T temp;

                if (!Setter(value, out temp))
                    throw new FormatException(string.Format("Given data is not of the correct format. It should match a {0}."));

                lock (locker)
                    val = temp;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Variables.SynchronousVariable{T}"/> class with the specified name and, optionally, an initial value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="abstr">optional; A brief description of the variable.</param>
        /// <param name="value">The initial value.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        public SynchronousVariable(string name, string abstr = null, T value = default(T))
            : base(name, abstr ?? DefaultAbstract, value)
        {
            
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="vCommands.Variables.SynchronousVariable{T}"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[Command Synchronous Variable: {0} | {1}]", Name, val);
        }
    }
}
