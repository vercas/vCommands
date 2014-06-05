using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Utilities
{
    /// <summary>
    /// Represents an object which can be sealed. After that, it cannot be changed.
    /// </summary>
    public class Sealable
    {
        internal volatile bool @sealed = false;

        /// <summary>
        /// Gets a value indicating whether the object is sealed or not.
        /// </summary>
        public Boolean Sealed
        {
            get
            {
                return @sealed;
            }
            internal set
            {
                @sealed = value;
            }
        }

        /// <summary>
        /// Seals the object, making it unchangeable.
        /// </summary>
        public virtual void Seal()
        {
            this.Sealed = true;
        }

        /// <summary>
        /// Checks if the object is sealed. If it is, an <see cref="System.InvalidOperationException"/> is thrown.
        /// </summary>
        protected void CheckSeal()
        {
            if (@sealed)
                throw new InvalidOperationException("Object is sealed! Its properties may no longer be changed.");
        }
    }
}
