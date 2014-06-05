using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Variables
{
    using EventArguments;
    using Parsing.Expressions;
    using Utilities;

    /// <summary>
    /// Represents a method which can attempt to turn a string into another value type.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="str">String to turn.</param>
    /// <param name="val"></param>
    /// <returns></returns>
    public delegate bool Setter<T>(string str, out T val);

    /// <summary>
    /// Represents a variable that can be used in the console.
    /// </summary>
    public class Variable<T>
        : IVariable
    {
        /// <summary>
        /// The default abstract of a <see cref="vCommands.Variables.Variable{T}"/>.
        /// </summary>
        public static readonly string DefaultAbstract = "Value-backed command variable.";

        /// <summary>
        /// Gets the method which would convert the string to the current type.
        /// </summary>
        static protected Setter<T> Setter { get; private set; }

        static Variable()
        {
            var tot = typeof(T);

            if (tot == typeof(string))
                Setter = (string str, out T val) =>
                {
                    val = (T)Convert.ChangeType(str, tot, null); return true;
                };
            else if (tot == typeof(int))
                Setter = (string str, out T val) =>
                {
                    int res;

                    if (!int.TryParse(str, out res))
                    {
                        val = default(T);
                        return false;
                    }

                    val = (T)Convert.ChangeType(res, tot, null);

                    return true;
                };
            else if (tot == typeof(uint))
                Setter = (string str, out T val) =>
                {
                    uint res;

                    if (!uint.TryParse(str, out res))
                    {
                        val = default(T);
                        return false;
                    }

                    val = (T)Convert.ChangeType(res, tot, null);

                    return true;
                };
            else if (tot == typeof(long))
                Setter = (string str, out T val) =>
                {
                    long res;

                    if (!long.TryParse(str, out res))
                    {
                        val = default(T);
                        return false;
                    }

                    val = (T)Convert.ChangeType(res, tot, null);

                    return true;
                };
            else if (tot == typeof(ulong))
                Setter = (string str, out T val) =>
                {
                    ulong res;

                    if (!ulong.TryParse(str, out res))
                    {
                        val = default(T);
                        return false;
                    }

                    val = (T)Convert.ChangeType(res, tot, null);

                    return true;
                };
            else if (tot == typeof(short))
                Setter = (string str, out T val) =>
                {
                    short res;

                    if (!short.TryParse(str, out res))
                    {
                        val = default(T);
                        return false;
                    }

                    val = (T)Convert.ChangeType(res, tot, null);

                    return true;
                };
            else if (tot == typeof(ushort))
                Setter = (string str, out T val) =>
                {
                    ushort res;

                    if (!ushort.TryParse(str, out res))
                    {
                        val = default(T);
                        return false;
                    }

                    val = (T)Convert.ChangeType(res, tot, null);

                    return true;
                };
            else if (tot == typeof(byte))
                Setter = (string str, out T val) =>
                {
                    byte res;

                    if (!byte.TryParse(str, out res))
                    {
                        val = default(T);
                        return false;
                    }

                    val = (T)Convert.ChangeType(res, tot, null);

                    return true;
                };
            else if (tot == typeof(sbyte))
                Setter = (string str, out T val) =>
                {
                    sbyte res;

                    if (!sbyte.TryParse(str, out res))
                    {
                        val = default(T);
                        return false;
                    }

                    val = (T)Convert.ChangeType(res, tot, null);

                    return true;
                };
            else if (tot == typeof(bool))
                Setter = (string str, out T val) =>
                {
                    bool res;

                    if (!bool.TryParse(str, out res))
                    {
                        val = default(T);
                        return false;
                    }

                    val = (T)Convert.ChangeType(res, tot, null);

                    return true;
                };
            else if (tot == typeof(float))
                Setter = (string str, out T val) =>
                {
                    float res;

                    if (!float.TryParse(str, out res))
                    {
                        val = default(T);
                        return false;
                    }

                    val = (T)Convert.ChangeType(res, tot, null);

                    return true;
                };
            else if (tot == typeof(double))
                Setter = (string str, out T val) =>
                {
                    double res;

                    if (!double.TryParse(str, out res))
                    {
                        val = default(T);
                        return false;
                    }

                    val = (T)Convert.ChangeType(res, tot, null);

                    return true;
                };
            else if (tot == typeof(decimal))
                Setter = (string str, out T val) =>
                {
                    decimal res;

                    if (!decimal.TryParse(str, out res))
                    {
                        val = default(T);
                        return false;
                    }

                    val = (T)Convert.ChangeType(res, tot, null);

                    return true;
                };
            else
                throw new TypeInitializationException("Cannot initialize over type " + tot, null);
        }

        #region Properties

        /// <summary>
        /// The underlying value.
        /// </summary>
        internal protected T val;

        /// <summary>
        /// Gets or sets the value of the variable.
        /// </summary>
        public virtual T Value
        {
            get
            {
                return val;
            }
            set
            {
                val = value;
            }
        }

        /// <summary>
        /// Gets or sets the string representing the value of the variable.
        /// </summary>
        public virtual String StringValue
        {
            get
            {
                return val.ToString();
            }
            set
            {
                T temp;

                if (!Setter(value, out temp))
                    throw new FormatException(string.Format("Given data is not of the correct format. It should match a {0}."));

                val = temp;
            }
        }

        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        public String Name { get; internal set; }

        /// <summary>
        /// Gets a brief description of the variable.
        /// </summary>
        public String Abstract { get; internal set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Variables.Variable{T}"/> class with the specified name and, optionally, an initial value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="abstr">optional; A brief description of the variable.</param>
        /// <param name="value">The initial value.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        public Variable(string name, string abstr = null, T value = default(T))
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.Name = name;
            this.Abstract = abstr ?? DefaultAbstract;
            this.val = value;
        }

        #region Value and Conversion

        /// <summary>
        /// Gets the underlying value of the variable.
        /// </summary>
        /// <typeparam name="TVar">The type of data to attempt to get.</typeparam>
        /// <returns></returns>
        public TVar GetValue<TVar>()
        {
            return (TVar)Convert.ChangeType(Value, typeof(TVar), null);
        }

        /// <summary>
        /// Changes the underlying value of the variable under the given context and according to the given value expression.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public EvaluationResult ChangeValue(EvaluationContext context, Expression value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var e1 = new VariableChangeEventArgs(context, this.StringValue, value);

            OnChange(e1);

            if (e1.Cancel)
                return new EvaluationResult(-2, e1.CancelReason ?? "Change has stopped.");

#if HVCE
            var e2 = new HostVariableChangeEventArgs(this, context, this.StringValue, value);

            context.Host.OnChange(e2);

            if (e2.Cancel)
                return new EvaluationResult(-2, e2.CancelReason ?? "Change has stopped.");
#endif

            var evalRes = value.Evaluate(context);

            if (!evalRes.TruthValue)
                return new EvaluationResult(-1, string.Format("Evaluation of variable value expression returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

            this.StringValue = evalRes.Output;

            return new EvaluationResult(0, evalRes.Output);
        }

        /// <summary>
        /// Converts the variable to its value.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static implicit operator T(Variable<T> v)
        {
            return v.Value;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns a string that represents the current <see cref="vCommands.Variables.Variable{T}"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[Command Variable: {0} | {1}]", Name, val);
        }

        /// <summary>
        /// Servers as a hash function for <see cref="vCommands.Variables.Variable{T}"/>.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ 0x5BBADB99 ^ val.GetHashCode() ^ 0x5BBADBD5;
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised before the value of the variable is changed.
        /// </summary>
        public event TypedEventHandler<IVariable, VariableChangeEventArgs> Change;

        /// <summary>
        /// Raisese the <see cref="vCommands.Variables.Variable{T}.Change"/> event.
        /// </summary>
        /// <param name="e">A <see cref="vCommands.EventArguments.VariableChangeEventArgs"/> that contains event data.</param>
        protected virtual void OnChange(VariableChangeEventArgs e)
        {
            var ev = Change;

            if (ev != null)
            {
                ev(this, e);
            }
        }

        #endregion
    }
}
