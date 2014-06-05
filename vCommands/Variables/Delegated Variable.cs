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
    /// Represents a variable that can be used in the console.
    /// </summary>
    public class DelegatedVariable<T>
        : IVariable
    {
        /// <summary>
        /// The default abstract of a <see cref="vCommands.Variables.DelegatedVariable{T}"/>.
        /// </summary>
        public static readonly string DefaultAbstract = "Delegated command variable.";

        #region Properties and Fields

        internal Func<T> vgetter;
        internal Func<string> sgetter;
        internal Action<T> vsetter;
        internal Func<string, bool> ssetter;

        /// <summary>
        /// Gets or sets the value of the variable.
        /// </summary>
        public virtual T Value
        {
            get
            {
                if (vgetter == null)
                    throw new InvalidOperationException("Variable cannot be read.");

                return vgetter();
            }
            set
            {
                if (vsetter == null)
                    throw new InvalidOperationException("Variable cannot be written.");

                vsetter(value);
            }
        }

        /// <summary>
        /// Gets or sets the string representing the value of the variable.
        /// </summary>
        public virtual String StringValue
        {
            get
            {
                if (sgetter != null)
                    return sgetter();

                if (vgetter != null)
                    return vgetter().ToString();

                throw new InvalidOperationException("Variable cannot be read.");
            }
            set
            {
                if (ssetter == null)
                {
                    if (vsetter == null)
                        throw new InvalidOperationException("Variable cannot be written.");

                    vsetter((T)Convert.ChangeType(value, typeof(T), null));
                }
                else
                    if (!ssetter(value))
                        throw new FormatException("The given value is not valid for this type.");
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
        /// Initializes a new instance of the <see cref="vCommands.Variables.DelegatedVariable{T}"/> class with the specified name and, optionally, an initial value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="valueGetter">The method used to retrieve the underlying value of the variable.</param>
        /// <param name="valueSetter">optional; The method used to set the underlying value of the variable.</param>
        /// <param name="stringGetter">optional; The method used to retrieve the underlying value of the variable as a string. If null, the value getter's result will be turned into a string.</param>
        /// <param name="stringSetter">optional; The method used to set the underlying value of the variable as a string. If null and a value setter is present, an automatic conversion will be attempted.</param>
        /// <param name="abstr">optional; A brief description of the variable.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given name is null.</exception>
        public DelegatedVariable(string name, Func<T> valueGetter, Action<T> valueSetter = null, Func<string> stringGetter = null, Func<string, bool> stringSetter = null, string abstr = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.Name = name;
            this.Abstract = abstr ?? DefaultAbstract;

            this.vgetter = valueGetter;
            this.vsetter = valueSetter;
            this.sgetter = stringGetter;
            this.ssetter = stringSetter;
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

            string sval = null;

            if (sgetter != null)
                sval = sgetter();
            else if (vgetter != null)
                sval = vgetter().ToString();

            var e1 = new VariableChangeEventArgs(context, sval, value);

            OnChange(e1);

            if (e1.Cancel)
                return new EvaluationResult(-2, e1.CancelReason ?? "Change has stopped.");

#if HVCE
            var e2 = new HostVariableChangeEventArgs(this, context, sval, value);

            context.Host.OnChange(e2);

            if (e2.Cancel)
                return new EvaluationResult(-2, e2.CancelReason ?? "Change has stopped.");
#endif

            var evalRes = value.Evaluate(context);

            if (!evalRes.TruthValue)
                return new EvaluationResult(-1, string.Format("Evaluation of variable value expression returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

            if (ssetter == null)
            {
                if (vsetter == null)
                    return new EvaluationResult(-3, "Variable cannot be written.");

                vsetter((T)Convert.ChangeType(evalRes.Output, typeof(T), null));
            }
            else
                if (!ssetter(evalRes.Output))
                    return new EvaluationResult(-4, "The given value is not of the correct type.");

            return new EvaluationResult(0, evalRes.Output);
        }

        /// <summary>
        /// Converts the variable to its value.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static implicit operator T(DelegatedVariable<T> v)
        {
            return v.Value;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns a string that represents the current <see cref="vCommands.Variables.DelegatedVariable{T}"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[Command Delegated Variable: {0}]", Name);
        }

        /// <summary>
        /// Servers as a hash function for <see cref="vCommands.Variables.DelegatedVariable{T}"/>.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ 0x5BBADBD5;
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised before the value of the variable is changed.
        /// </summary>
        public event TypedEventHandler<IVariable, VariableChangeEventArgs> Change;

        /// <summary>
        /// Raisese the <see cref="vCommands.Variables.DelegatedVariable{T}.Change"/> event.
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
