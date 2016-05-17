using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace vCommands.Parsing.Expressions
{
    /// <summary>
    /// Represents a constant expression - its result is always positive and the value never changes.
    /// </summary>
    public sealed class ConstantExpression
        : Expression
    {
        #region Static

        static ConstantExpression()
        {
            bit32 = IntPtr.Size == 4;
            bit64 = IntPtr.Size == 8;
        }

        private static bool bit32, bit64;

        #endregion

        #region Cache

        static Dictionary<string, ConstantExpression> dict = new Dictionary<string, ConstantExpression>();
        static object locker = new object();

        /// <summary>
        /// Obtains a <see cref="vCommands.Parsing.Expressions.ConstantExpression"/> with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ConstantExpression Fetch(string value)
        {
            ConstantExpression res = null;

            lock (locker)
                if (!dict.TryGetValue(value, out res))
                    dict.Add(value, res = new ConstantExpression(value));

            return res;
        }

        #endregion

        /// <summary>
        /// Gets the constant value of the expression.
        /// </summary>
        public String Value { get; private set; }

        private object[] datas; //  Various types of data contained in the expression.

        /// <summary>
        /// Evaluates the current expression.
        /// </summary>
        /// <param name="context">The context of the evaluation.</param>
        /// <param name="res">The variable which will contain the result of the evaluation.</param>
        protected override void Evaluate(EvaluationContext context, out EvaluationResult res)
        {
            res = new EvaluationResult(CommonStatusCodes.Success, this, Value, datas);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Parsing.Expressions.ConstantExpression"/> class with the specified value.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given value is null.</exception>
        private ConstantExpression(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            this.Value = value;
            ExtractDatas();

            Seal();
        }

        /// <summary>
        /// Returns a string that represents the current constant expression.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Value) || Value.ToCharArray().Intersect(Parser.MustEscape).Any())
                return string.Format(CultureInfo.InvariantCulture
                    , "\"{0}\""
                    , Value.Replace("\\", "\\\\").Replace("\"", "\\\""));
            else
                return Value;
        }

        private void ExtractDatas()
        {
            List<object> list = new List<object>();

            decimal tempM = 0m; double tempD = 0d; float tempF = 0f; long tempL = 0L; ulong tempUL = 0UL; int tempI = 0; uint tempUI = 0U; short tempS = 0; ushort tempUS = 0; byte tempB = 0; sbyte tempSB = 0;
            bool tempBOOL = false; char tempC = char.MaxValue; DateTime tempDT = DateTime.MaxValue; TimeSpan tempTS = TimeSpan.MaxValue; Uri tempUri = null;

            if (decimal.TryParse(this.Value, out tempM)) list.Add(tempM);
            if (double.TryParse(this.Value, out tempD)) list.Add(tempD);
            if (float.TryParse(this.Value, out tempF)) list.Add(tempF);
            if (long.TryParse(this.Value, out tempL)) { list.Add(tempL); if (bit64) list.Add(new IntPtr(tempL)); }
            if (ulong.TryParse(this.Value, out tempUL)) { list.Add(tempUL); if (bit64) list.Add(new UIntPtr(tempUL)); }
            if (int.TryParse(this.Value, out tempI)) { list.Add(tempI); if (bit32) list.Add(new IntPtr(tempI)); }
            if (uint.TryParse(this.Value, out tempUI)) { list.Add(tempUI); if (bit32) list.Add(new UIntPtr(tempUI)); }
            if (short.TryParse(this.Value, out tempS)) list.Add(tempS);
            if (ushort.TryParse(this.Value, out tempUS)) list.Add(tempUS);
            if (byte.TryParse(this.Value, out tempB)) list.Add(tempB);
            if (sbyte.TryParse(this.Value, out tempSB)) list.Add(tempSB);
            if (bool.TryParse(this.Value, out tempBOOL)) list.Add(tempBOOL);
            if (char.TryParse(this.Value, out tempC)) list.Add(tempC);
            if (DateTime.TryParse(this.Value, out tempDT)) list.Add(tempDT);
            if (TimeSpan.TryParse(this.Value, out tempTS)) list.Add(tempTS);
            if (Uri.TryCreate(this.Value, UriKind.RelativeOrAbsolute, out tempUri)) list.Add(tempUri);

            datas = list.ToArray();
        }
    }
}
