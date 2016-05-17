using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace vCommands
{
    using Parsing.Expressions;

    /// <summary>
    /// Represents the result of evaluating a <see cref="vCommands.Parsing.Expressions.Expression"/>.
    /// </summary>
    public sealed class EvaluationResult
    {
        /// <summary>
        /// A result whic contains no output text and status zero.
        /// </summary>
        public static readonly EvaluationResult EmptyPositive = new EvaluationResult(CommonStatusCodes.Success, null, string.Empty);

        /// <summary>
        /// Gets a new evaluation result with a successful return code and empty output.
        /// </summary>
        public static EvaluationResult NewEmptyPositive { get { return new EvaluationResult(CommonStatusCodes.Success, null, string.Empty); } }

        /// <summary>
        /// Gets the numerical status of the result.
        /// </summary>
        /// <remarks>
        /// 0 means True/positive, anything else is False/negative.
        /// </remarks>
        public int Status { get; private set; }

        /// <summary>
        /// Gets the <see cref="vCommands.CommonStatusCodes"/> of the evaluation result.
        /// </summary>
        public CommonStatusCodes CommonStatus { get; private set; }

        /// <summary>
        /// Gets the truth value interpretede from the status.
        /// </summary>
        public Boolean TruthValue { get; private set; }

        /// <summary>
        /// Gets the resulted text output of the expression.
        /// </summary>
        public String Output { get; private set; }

        /// <summary>
        /// Gets the data in this evaluation result.
        /// </summary>
        public object[] Data { get; private set; }

        /// <summary>
        /// Gets the expression which evaluated to the current result.
        /// </summary>
        public Expression Expression { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.EvaluationResult"/> class with the specified status code, originating expression, text output and data.
        /// </summary>
        /// <param name="status">Numerical status indicating the success of the evaluation.</param>
        /// <param name="exp">The expression which evaluated to the current result.</param>
        /// <param name="output">The text output of the evaluation.</param>
        /// <param name="data"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="output"/> or <paramref name="data"/> are null.</exception>
        public EvaluationResult(int status, Expression exp, string output, params object[] data)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            this.Status = status;
            this.CommonStatus = (CommonStatusCodes)status;
            this.TruthValue = status == 0;
            this.Output = output;
            this.Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.EvaluationResult"/> class with the specified known status code, originating expression, text output and data.
        /// </summary>
        /// <param name="status">Known status code indicating the success of the evaluation.</param>
        /// <param name="exp">The expression which evaluated to the current result.</param>
        /// <param name="output">The text output of the evaluation.</param>
        /// <param name="data"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="output"/> or <paramref name="data"/> are null.</exception>
        public EvaluationResult(CommonStatusCodes status, Expression exp, string output, params object[] data)
            : this((int)status, exp, output, data)
        {

        }

        #region Utilities

        /// <summary>
        /// Makes sure that the evaluation result has the specified truth value and returns null, otherwise returns an appropriate error result.
        /// </summary>
        /// <param name="argumentNumber">The index of the argument represented by the current evaluation result.</param>
        /// <param name="commandName">The name of the command (or variable) to which this evaluation result is an argument.</param>
        /// <param name="expected">The expected truth value.</param>
        /// <returns>Null on success; otherwise a result representing the appropriate error.</returns>
        public EvaluationResult CheckTruthValue(int argumentNumber, string commandName, bool expected = true)
        {
            if (this.TruthValue != expected)
            {
                if (expected)
                    return new EvaluationResult(argumentNumber >= 0 ? CommonStatusCodes.ArgumentEvaluationFailure : CommonStatusCodes.CvarValueEvaluationFailure, this.Expression
                        , argumentNumber >= 0
                            ? string.Format(CultureInfo.InvariantCulture
                                , "Evaluation of argument #{0} to '{1}' returned non-zero status: {2} ({3}); {4}"
                                , argumentNumber + 1, commandName, this.Status, this.CommonStatus, this.Output)
                            : string.Format(CultureInfo.InvariantCulture
                                , "Evaluation of value for variable '{0}' returned non-zero status: {1} ({2}); {3}"
                                , commandName, this.Status, this.CommonStatus, this.Output)
                        , commandName, argumentNumber, this);
                else
                    return new EvaluationResult(argumentNumber >= 0 ? CommonStatusCodes.ArgumentEvaluationFailure : CommonStatusCodes.CvarValueEvaluationFailure, this.Expression
                        , argumentNumber >= 0
                            ? string.Format(CultureInfo.InvariantCulture
                                , "Evaluation of argument #{0} to '{1}' returned zero status; {2}"
                                , argumentNumber + 1, commandName, this.Output)
                            : string.Format(CultureInfo.InvariantCulture
                                , "Evaluation of value for variable '{0}' returned zero status; {1}"
                                , commandName, this.Output)
                        , commandName, argumentNumber, this);
            }

            return null;
        }

        /// <summary>
        /// Makes sure that the evaluation result contains only one of the specified data type and returns null, otherwise returns an appropriate error result.
        /// </summary>
        /// <typeparam name="TData">The type of unique data to look for.</typeparam>
        /// <param name="argumentNumber">The index of the argument represented by the current evaluation result.</param>
        /// <param name="commandName">The name of the command (or variable) to which this evaluation result is an argument.</param>
        /// <param name="res">The variable which will hold the data.</param>
        /// <returns>Null on success; otherwise a result representing the appropriate error.</returns>
        public EvaluationResult ExtractUniqueDatum<TData>(int argumentNumber, string commandName, ref TData res)
        {
            bool found = false;

            for (int i = 0; i < this.Data.Length; i++)
                if (this.Data[i] is TData)
                {
                    if (found)
                    {
                        return new EvaluationResult(CommonStatusCodes.TypedDataDuplicate, this.Expression
                            , argumentNumber >= 0
                                ? string.Format(CultureInfo.InvariantCulture
                                    , "Evaluation of argument #{0} to '{1}' contains more than one datum of type {2}."
                                    , argumentNumber + 1, commandName, typeof(TData))
                                : string.Format(CultureInfo.InvariantCulture
                                    , "Evaluation of value for variable '{0}' contains more than one datum of type {1}."
                                    , commandName, typeof(TData))
                            , typeof(TData), commandName, argumentNumber, this);
                    }
                    else
                    {
                        res = (TData)this.Data[i];

                        found = true;
                    }
                }

            if (found)
                return null;
            else
                return new EvaluationResult(CommonStatusCodes.TypedDataNotFound, this.Expression
                    , argumentNumber >= 0
                        ? string.Format(CultureInfo.InvariantCulture
                            , "Evaluation of argument #{0} to '{1}' expected to contain data of type {2}."
                            , argumentNumber + 1, commandName, typeof(TData))
                        : string.Format(CultureInfo.InvariantCulture
                            , "Evaluation of value for variable '{0}' expected to contain data of type {1}."
                            , commandName, typeof(TData))
                    , typeof(TData), commandName, argumentNumber, this);
        }

        /// <summary>
        /// Makes sure that the evaluation result contains at least one of the specified data type and returns null, otherwise returns an appropriate error result.
        /// </summary>
        /// <typeparam name="TData">The type of data to look for.</typeparam>
        /// <param name="argumentNumber">The index of the argument represented by the current evaluation result.</param>
        /// <param name="commandName">The name of the command (or variable) to which this evaluation result is an argument.</param>
        /// <param name="res">The variable which will hold the data.</param>
        /// <returns>Null on success; otherwise a result representing the appropriate error.</returns>
        public EvaluationResult ExtractFirstDatum<TData>(int argumentNumber, string commandName, ref TData res)
        {
            for (int i = 0; i < this.Data.Length; i++)
                if (this.Data[i] is TData)
                {
                    res = (TData)this.Data[i];

                    return null;
                }

            return new EvaluationResult(CommonStatusCodes.TypedDataNotFound, this.Expression
                , argumentNumber >= 0
                    ? string.Format(CultureInfo.InvariantCulture
                        , "Evaluation of argument #{0} to '{1}' expected to contain data of type {2}."
                        , argumentNumber + 1, commandName, typeof(TData))
                    : string.Format(CultureInfo.InvariantCulture
                        , "Evaluation of value for variable '{0}' expected to contain data of type {1}."
                        , commandName, typeof(TData))
                , typeof(TData), commandName, argumentNumber, this);
        }

        /// <summary>
        /// Makes sure that the evaluation result contains only one double and returns null, otherwise attempts to parse one from the output. If that too fails, returns an appropriate error result.
        /// </summary>
        /// <param name="argumentNumber">The index of the argument represented by the current evaluation result.</param>
        /// <param name="commandName">The name of the command (or variable) to which this evaluation result is an argument.</param>
        /// <param name="res">The variable which will hold the number.</param>
        /// <returns>Null on success; otherwise a result representing the appropriate error.</returns>
        public EvaluationResult ExtractNumber(int argumentNumber, string commandName, ref double res)
        {
            if (this.ExtractUniqueDatum<double>(argumentNumber, commandName, ref res) == null)
                return null;
            else
            {
                int tmp = 0;

                if (this.ExtractUniqueDatum<int>(argumentNumber, commandName, ref tmp) == null)
                {
                    res = tmp;

                    return null;
                }
            }

            if (double.TryParse(this.Output, out res))
                return null;
            else
                return new EvaluationResult(CommonStatusCodes.ArgumentEvaluationFailure, this.Expression
                    , argumentNumber >= 0
                        ? string.Format(CultureInfo.InvariantCulture
                            , "Evaluation of argument #{0} to '{1}' expected to contain data of type Double or a valid number in the output: {2}"
                            , argumentNumber + 1, commandName, this.Output)
                        : string.Format(CultureInfo.InvariantCulture
                            , "Evaluation of value for variable '{0}' expected to contain data of type Double or a valid number in the output: {1}"
                            , commandName, this.Output)
                    , typeof(double), commandName, argumentNumber, this, this.Output);
        }

        #endregion

        private void ToString(StringBuilder b, string indent, string indentStr)
        {
            b.Append(indent);
            b.Append("Expression = ");
            if (this.Expression != null)
                b.AppendLine(this.Expression.ToString());
            else
                b.AppendLine("-- NULL --");

            b.Append(indent);
            b.Append("Status = ");
            b.Append(this.Status);
            b.Append(" (");
            b.Append(this.CommonStatus);
            b.Append(")");
            b.AppendLine();

            b.Append(indent);
            b.Append("Output = ");
            b.AppendLine(this.Output ?? "-- NULL --");

            b.Append(indent);
            b.Append("Data:");
            b.Append(indentStr);
            b.Append(this.Data.Length);
            b.AppendLine("entries:");
            b.AppendLine();

            ToStringEnumerable(b, indent + indentStr, indentStr, this.Data);
        }

        private void ToStringEnumerable(StringBuilder b, string indent, string indentStr, IEnumerable enumerable)
        {
            foreach (var dat in enumerable)
            {
                b.Append(indent);
                b.Append(dat.GetType().FullName);
                b.Append(":");
                b.Append(indentStr);

                if (dat is EvaluationResult)
                    ((EvaluationResult)dat).ToString(b, indent + indentStr, indentStr);
                else if (dat is string)
                    b.AppendLine(dat.ToString());   //  System.String implements the interfaces handled specifically below...
                else if (dat is IList)
                {
                    var lst = (IList)dat;

                    b.Append(lst.Count);
                    b.AppendLine(" entries:");

                    ToStringEnumerable(b, indent + indentStr, indentStr, lst);
                }
                else if (dat is IEnumerable)
                    ToStringEnumerable(b, indent + indentStr, indentStr, (IEnumerable)dat);
                else
                    b.AppendLine(dat.ToString());
            }
        }

        /// <summary>
        /// Converts the result into a human-readable form.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder b = new StringBuilder(8192);
            this.ToString(b, string.Empty, "    ");
            return b.ToString();
        }
    }
}
