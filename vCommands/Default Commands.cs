using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace vCommands
{
    using Commands;
    using Variables;
    using Manuals;
    using Manuals.Drivers;
    using Parsing.Expressions;
    using Utilities;

    delegate T UnaryMathematicalFunction<T>(T arg);
    delegate T BinaryMathematicalFunction<T>(T arg1, T arg2);
    delegate bool BinaryMathematicalLogicFunction<T>(T arg1, T arg2);
    delegate T NaryMathematicalFunction<T>(T[] args);

    internal static class DefaultCommands
    {
        static MethodCommand[] Commands;
        const string AssemblyName = "vCommands";

        [ThreadStatic]
        static Random rnd = null;

        static DefaultCommands()
        {
            var written = CommandRegistration.FromType(typeof(DefaultCommands), true);

            var all = new List<MethodCommand>(written);
            
            //  Unary mathematical commands.

            var unaryMathematics = new Tuple<UnaryMathematicalFunction<double>, string>[]
            {
                new Tuple<UnaryMathematicalFunction<double>, string>(Math.Abs, "Computes the absolute value of the given number."),
                new Tuple<UnaryMathematicalFunction<double>, string>(Math.Sin, "Computes the sine of the given number."),
                new Tuple<UnaryMathematicalFunction<double>, string>(Math.Cos, "Computes the cosine of the given number."),
                new Tuple<UnaryMathematicalFunction<double>, string>(Math.Asin, "Computes the arcsine of the given number."),
                new Tuple<UnaryMathematicalFunction<double>, string>(Math.Acos, "Computes the arccosine of the given number."),
                new Tuple<UnaryMathematicalFunction<double>, string>(Math.Tan, "Computes the tangent of the given number."),
                new Tuple<UnaryMathematicalFunction<double>, string>(Math.Atan, "Computes the arctangent of the given number."),
                new Tuple<UnaryMathematicalFunction<double>, string>(Math.Sinh, "Computes the hyperbolic sine of the given number."),
                new Tuple<UnaryMathematicalFunction<double>, string>(Math.Cosh, "Computes the hyperbolic cosine of the given number."),
                new Tuple<UnaryMathematicalFunction<double>, string>(Math.Tanh, "Computes the hyperbolic tangent of the given number."),
                new Tuple<UnaryMathematicalFunction<double>, string>(Math.Exp, "Computes the exponential of the given number."),
                new Tuple<UnaryMathematicalFunction<double>, string>(Math.Floor, "Computes the floored value of the given number."),
                new Tuple<UnaryMathematicalFunction<double>, string>(Math.Ceiling, "Computes the ceiled value of the given number."),
                new Tuple<UnaryMathematicalFunction<double>, string>(Math.Log10, "Computes the base-1o logarithm of the given number."),
                new Tuple<UnaryMathematicalFunction<double>, string>(Math.Sqrt, "Computes the square root of the given number."),
                new Tuple<UnaryMathematicalFunction<double>, string>(Neg, "Negates the given number."),
                new Tuple<UnaryMathematicalFunction<double>, string>(Ln, "Computes the natural logarithm of the given number."),
                new Tuple<UnaryMathematicalFunction<double>, string>(D2R, "Turns the given number of degrees into radians."),
                new Tuple<UnaryMathematicalFunction<double>, string>(R2D, "Turns the given number of radians into degrees."),
            };

            foreach (var kv in unaryMathematics)
            {
                var mthd = kv.Item1;
                var mthdName = kv.Item1.Method.Name.ToLower();

                CommandMethod deleg = (toggle, context, args) =>
                {
                    if (toggle != Toggler.Neutral)
                        return new EvaluationResult(CommonStatusCodes.TogglerNotSupported, null, string.Format("'{0}' does not support toggler '{1}'.", mthdName, toggle == Toggler.On ? "+" : "-"));
                    if (args.Length != 1)
                        return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, string.Format("'{0}' must receive one argument: a number.", mthdName), args);

                    var evalRes = args[0].Evaluate(context);

                    var evalRes2 = evalRes.CheckTruthValue(0, mthdName, true);
                    if (evalRes2 != null) return evalRes2;

                    double d = 0d;

                    evalRes2 = evalRes.ExtractUniqueDatum<double>(0, mthdName, ref d);
                    if (evalRes2 != null) return evalRes2;

                    var d2 = mthd(d);

                    return new EvaluationResult(CommonStatusCodes.Success, null, d2.ToString(), d2, new double[1] { d });
                };

                var cmd = new MethodCommand(mthdName, "Mathematics", kv.Item2, deleg);

                all.Add(cmd);
            }

            //  Binary commands, which require two arguments/operands.

            var binaryMathematics = new Tuple<BinaryMathematicalFunction<double>, string>[]
            {
                new Tuple<BinaryMathematicalFunction<double>, string>(Math.Log, "Computes the logarithm of the first numbers in the base of the second number."),
                new Tuple<BinaryMathematicalFunction<double>, string>(Math.Pow, "Computes the first number raised to the power of the second number."),
                new Tuple<BinaryMathematicalFunction<double>, string>(Math.Atan2, "Computes the arctangent using the two given numbers."),
                new Tuple<BinaryMathematicalFunction<double>, string>(mod, "Computes the remainder of dividing the first number to the second."),
            };

            foreach (var kv in binaryMathematics)
            {
                var mthd = kv.Item1;
                var mthdName = kv.Item1.Method.Name.ToLower();

                CommandMethod deleg = (toggle, context, args) =>
                {
                    if (toggle != Toggler.Neutral)
                        return new EvaluationResult(CommonStatusCodes.TogglerNotSupported, null, string.Format("'{0}' does not support toggler '{1}'.", mthdName, toggle == Toggler.On ? "+" : "-"));
                    if (args.Length != 2)
                        return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, string.Format("'{0}' must receive two arguments, both numbers.", mthdName), args);

                    double d = 0d;
                    var numbers = new double[2];

                    for (int i = 0; i < 2; i++)
                    {
                        var evalRes = args[i].Evaluate(context);

                        var evalRes2 = evalRes.CheckTruthValue(i, mthdName, true);
                        if (evalRes2 != null) return evalRes2;
                        evalRes2 = evalRes.ExtractUniqueDatum<double>(i, mthdName, ref d);
                        if (evalRes2 != null) return evalRes2;

                        numbers[i] = d;
                    }

                    d = mthd(numbers[0], numbers[1]);

                    return new EvaluationResult(CommonStatusCodes.Success, null, d.ToString(), d, numbers);
                };

                var cmd = new MethodCommand(mthdName, "Mathematics", kv.Item2, deleg);

                all.Add(cmd);
            }

            //  Binary logic commands, which require two arguments/operands.

            var binaryLogicMathematics = new Tuple<BinaryMathematicalLogicFunction<double>, bool, char, char, string>[]
            {
                new Tuple<BinaryMathematicalLogicFunction<double>, bool, char, char, string>(lt, true, '<', '≮', "Determines whether the first number is less than the second."),
                new Tuple<BinaryMathematicalLogicFunction<double>, bool, char, char, string>(lteq, true, '≤', '≰', "Determines whether the first number is less than or equal to the second."),
                new Tuple<BinaryMathematicalLogicFunction<double>, bool, char, char, string>(gt, true, '>', '≯', "Determines whether the first number is greater than the second."),
                new Tuple<BinaryMathematicalLogicFunction<double>, bool, char, char, string>(gteq, true, '≥', '≱', "Determines whether the first number is greater than or equal the second."),
            };

            foreach (var kv in binaryLogicMathematics)
            {
                var mthd = kv.Item1;
                var retStat = kv.Item2;
                char sym1 = kv.Item3, sym2 = kv.Item4;
                var mthdName = kv.Item1.Method.Name.ToLower();

                CommandMethod deleg = (toggle, context, args) =>
                {
                    var willDoStatus = false;

                    if (retStat)
                    {
                        if (toggle == Toggler.On)
                            return new EvaluationResult(CommonStatusCodes.TogglerNotSupported, null, string.Format("'{0}' does not support toggler '+'.", mthdName));
                        else
                            willDoStatus = toggle != Toggler.Neutral; //  -command = will return status;
                    }
                    else if (toggle != Toggler.Neutral)
                        return new EvaluationResult(CommonStatusCodes.TogglerNotSupported, null, string.Format("'{0}' does not support toggler '{1}'.", mthdName, toggle == Toggler.On ? "+" : "-"));

                    if (args.Length != 2)
                        return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, string.Format("'{0}' must receive two arguments, both numbers.", mthdName));

                    double d = 0d;
                    var numbers = new double[2];

                    for (int i = 0; i < 2; i++)
                    {
                        var evalRes = args[i].Evaluate(context);

                        var evalRes2 = evalRes.CheckTruthValue(i, mthdName, true);
                        if (evalRes2 != null) return evalRes2;
                        evalRes2 = evalRes.ExtractUniqueDatum<double>(i, mthdName, ref d);
                        if (evalRes2 != null) return evalRes2;

                        numbers[i] = d;
                    }

                    var res = mthd(numbers[0], numbers[1]);

                    if (willDoStatus)
                        if (res)
                            return new EvaluationResult(CommonStatusCodes.Success, null, string.Format("{0} {1} {2}", numbers[0], sym1, numbers[1]), numbers);
                        else
                            return new EvaluationResult(CommonStatusCodes.MathematicalLogicFailure, null, string.Format("{0} {1} {2}", numbers[0], sym2, numbers[1]), numbers);
                    else
                        return new EvaluationResult(CommonStatusCodes.Success, null, res.ToString(), res, numbers);
                };

                var cmd = new MethodCommand(mthdName, "Comparison", kv.Item5, deleg);

                all.Add(cmd);
            }

            //  N-ary commands, they take at least 2 arguments.

            var naryMathematics = new Tuple<NaryMathematicalFunction<double>, string>[]
            {
                new Tuple<NaryMathematicalFunction<double>, string>(min, "Computes the minimum value of the given numbers."),
                new Tuple<NaryMathematicalFunction<double>, string>(max, "Computes the maximum value of the given numbers."),
                new Tuple<NaryMathematicalFunction<double>, string>(aravg, "Computes the arithmetic average/mean of the given numbers."),
                new Tuple<NaryMathematicalFunction<double>, string>(gmavg, "Computes the geometric average/mean of the given numbers."),
                new Tuple<NaryMathematicalFunction<double>, string>(hmavg, "Computes the harmonic average/mean of the given numbers."),
                new Tuple<NaryMathematicalFunction<double>, string>(rsavg, "Computes the root-mean-square (quadratic mean) of the given numbers."),
            };

            foreach (var kv in naryMathematics)
            {
                var mthd = kv.Item1;
                var mthdName = kv.Item1.Method.Name.ToLower();

                CommandMethod deleg = (toggle, context, args) =>
                {
                    if (toggle != Toggler.Neutral)
                        return new EvaluationResult(CommonStatusCodes.TogglerNotSupported, null, string.Format("'{0}' does not support toggler '{1}'.", mthdName, toggle == Toggler.On ? "+" : "-"));
                    if (args.Length < 2)
                        return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, string.Format("'{0}' must receive at least two arguments, all numbers.", mthdName));

                    double d = 0d;
                    var numbers = new double[args.Length];

                    for (int i = 0; i < args.Length; i++)
                    {
                        var evalRes = args[i].Evaluate(context);

                        var evalRes2 = evalRes.CheckTruthValue(i, mthdName, true);
                        if (evalRes2 != null) return evalRes2;
                        evalRes2 = evalRes.ExtractUniqueDatum<double>(i, mthdName, ref d);
                        if (evalRes2 != null) return evalRes2;

                        numbers[i] = d;
                    }

                    d = mthd(numbers);

                    return new EvaluationResult(CommonStatusCodes.Success, null, d.ToString(), d, numbers);
                };

                var cmd = new MethodCommand(mthdName, "Mathematics", kv.Item2, deleg);

                all.Add(cmd);
            }

            Commands = all.ToArray();
        }

        public static void Register(CommandHost host, bool includeManual, bool includeMath, bool shortHelp)
        {
            for (int i = 0; i < Commands.Length; i++)
            {
                var c = Commands[i];

                if (c.Name == "man" && !includeManual)
                    continue;

                if (!includeMath)
                {
                    if (c.Category == "Mathematics")
                        continue;

                    switch (c.Name)
                    {
                        case "lt":
                        case "lteq":
                        case "gt":
                        case "gteq":
                            continue;
                    }
                }

                host.RegisterCommand(c);
            }
        }

        //  And now, the commands.

        [MethodCommandData(Abstract = "Displays every available command and variable and their descriptions.")]
        static EvaluationResult help(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            IEnumerable<KeyValuePair<string, Command>> cmds = context.Host.cmds;
            IEnumerable<KeyValuePair<string, IVariable>> vars = context.Host.vars;
            var ob = new StringBuilder();
            int ccnt = 0, gcnt = 0, vcnt = 0;

            if (args.Length > 1)
            {
                //  For now, quit.

                return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "Must not provide more than one argument to 'help', and that argument may be a regular expression.");
            }
            else if (args.Length == 1)
            {
                var evalRes = args[0].Evaluate(context);

                var evalRes2 = evalRes.CheckTruthValue(0, "help", true);
                if (evalRes2 != null) return evalRes2;

                var regex = new Regex(evalRes.Output);

                if (toggle == Toggler.Neutral)
                {
                    cmds = cmds.Where(kv => regex.IsMatch(kv.Key));
                    vars = vars.Where(kv => regex.IsMatch(kv.Key));

                    if (!context.Host.ShortHelp)
                        ob.Append("Looking for regular expression in command/variable names: ");
                }
                else if (toggle == Toggler.On)
                {
                    if (context.Host.ShortHelp)
                    {
                        cmds = cmds.Where(kv => kv.Key == evalRes.Output);
                        vars = vars.Where(kv => kv.Key == evalRes.Output);
                    }
                    else
                    {
                        cmds = cmds.Where(kv => regex.IsMatch(kv.Key) || regex.IsMatch(kv.Value.Abstract));
                        vars = vars.Where(kv => regex.IsMatch(kv.Key) || regex.IsMatch(kv.Value.Abstract));

                        ob.Append("Looking for regular expression in command/variable names and descriptions: ");
                    }
                }
                else
                {
                    cmds = cmds.Where(kv => regex.IsMatch(kv.Value.Abstract));
                    vars = vars.Where(kv => regex.IsMatch(kv.Value.Abstract));

                    if (!context.Host.ShortHelp)
                        ob.Append("Looking for regular expression in command/variable descriptions: ");
                }

                if (!context.Host.ShortHelp)
                    ob.AppendLine(evalRes.Output);
            }
            else if (!context.Host.ShortHelp)
                ob.AppendLine("Listing all commands and variables with descriptions:");

            if (context.Host.ShortHelp)
            {
                foreach (var g in cmds.GroupBy(kv => kv.Value.Category).OrderBy(g => g.Key))
                {
                    if (gcnt > 0)
                        ob.AppendLine(".");

                    ob.Append(g.Key);

                    bool first = true;

                    foreach (var kv in g.OrderBy(kv => kv.Key))
                    {
                        ob.AppendFormat("{1} {0}", kv.Value.Name, first ? ":" : ",");

                        ccnt++;
                        first = false;
                    }
                    
                    gcnt++;
                }

                if (vars.Any())
                {
                    if (gcnt > 0)
                        ob.AppendLine(".");

                    ob.Append("Variables");

                    foreach (var kv in vars.OrderBy(kv => kv.Key))
                    {
                        ob.AppendFormat("{1} {0}", kv.Value.Name, vcnt == 0 ? ":" : ",");
                        vcnt++;
                    }
                }

                if (ccnt == 1 && vcnt == 0)
                    ob.AppendLine(" - " + cmds.First().Value.Abstract);
                else if (ccnt == 0 && vcnt == 1)
                    ob.AppendLine(" - " + vars.First().Value.Abstract);
                else
                {
                    if ((vcnt == 0 && gcnt > 0) || vcnt > 0)
                        ob.AppendLine(".");

                    ob.AppendFormat("{3} shows {0} command{4} under {1} categor{5} and {2} variable{6}."
                        , ccnt, gcnt, vcnt > 0 ? vcnt.ToString() : "no", AssemblyName
                        , ccnt == 1 ? "" : "s", gcnt == 1 ? "y" : "ies", vcnt == 1 ? "" : "s");
                    ob.AppendLine();
                }
            }
            else
            {
                foreach (var g in cmds.GroupBy(kv => kv.Value.Category).OrderBy(g => g.Key))
                {
                    ob.AppendLine();
                    ob.AppendFormat("\t{0}:", g.Key);
                    ob.AppendLine();
                    ob.AppendLine();

                    foreach (var kv in g.OrderBy(kv => kv.Key))
                    {
                        ob.AppendFormat("{1}\t- {2}{0}", Environment.NewLine, kv.Value.Name, kv.Value.Abstract);
                        ccnt++;
                    }

                    gcnt++;
                }

                if (vars.Any())
                {
                    ob.AppendLine();
                    ob.AppendLine("\tVariables:");
                    ob.AppendLine();

                    foreach (var kv in vars.OrderBy(kv => kv.Key))
                    {
                        ob.AppendFormat("{1}\t- {2}{0}", Environment.NewLine, kv.Value.Name, kv.Value.Abstract);
                        vcnt++;
                    }
                }

                ob.AppendLine();
                ob.AppendFormat("Shown {0} command{3} under {1} categor{4} and {2} variable{5}."
                    , ccnt, gcnt, vcnt > 0 ? vcnt.ToString() : "no"
                    , ccnt == 1 ? "" : "s", gcnt == 1 ? "y" : "ies", vcnt == 1 ? "" : "s");
                ob.AppendLine();
                ob.AppendFormat("Powered by {0}.", AssemblyName);
            }

            return new EvaluationResult(CommonStatusCodes.Success, null, ob.ToString());
        }

        [MethodCommandData(Abstract = "Returns the given input arguments, separated by a tabulator.")]
        static EvaluationResult echo(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            string output;

            if (toggle == Toggler.Neutral)
                output = string.Join("\t", from a in args select a.Evaluate(context).Output);
            else if (toggle == Toggler.On)
                output = string.Join(Environment.NewLine, from a in args select a.Evaluate(context).Output);
            else
                output = string.Concat(from a in args select a.Evaluate(context).Output);

            return new EvaluationResult(CommonStatusCodes.Success, null, output);
        }

        [MethodCommandData(Abstract = "Evaluates the given arguments sequentially until one returns non-zero status.")]
        static EvaluationResult eval(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            StringBuilder ob = new StringBuilder(4096);
            EvaluationResult[] reses = new EvaluationResult[args.Length];
            int i = 0;

            for (; i < args.Length; i++)
            {
                var res = args[i].Evaluate(context);
                reses[i] = res;

                if (i > 0)
                    ob.Append('\t');

                ob.Append(res.Output);

                if (res.Status != 0)
                    break;
            }

            return new EvaluationResult(CommonStatusCodes.SequentialEvaluationFailure, null, ob.ToString(), reses, i);
        }

        [MethodCommandData(Abstract = "Repeats a command for a number of times.")]
        static EvaluationResult repeat(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            if (args.Length != 2)
                return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'repeat' must receive two arguments: a count and an expression.", args);

            var evalRes = args[0].Evaluate(context);
            int i = -1;

            var evalRes2 = evalRes.CheckTruthValue(0, "repeat", true);
            if (evalRes2 != null) return evalRes2;
            evalRes2 = evalRes.ExtractUniqueDatum<int>(0, "for", ref i);
            if (evalRes2 != null) return evalRes2;
            if (i < 1)  //  We will use 1-based indexes here.
                return new EvaluationResult(CommonStatusCodes.LoopNegativeBound, null, "Evaluation of argument #1 to 'repeat' returned non-positive number.", i, evalRes);

            var exp = args[1];

            switch (toggle)
            {
                case Toggler.Neutral:
                    var ous1 = new string[i];

                    for (int j = 0; j < i; j++)
                    {
                        evalRes = exp.Evaluate(context);

                        if (!evalRes.TruthValue)
                            return new EvaluationResult(CommonStatusCodes.LoopExpressionFailure, null, string.Format("Evaluation #{0} of the 'repeat' loop expression (argument #2) failed with non-zero status: {1} ({2}); {3}", j + 1, evalRes.Status, evalRes.CommonStatus, evalRes.Output), exp, evalRes, j, ous1, 1);

                        ous1[j] = evalRes.Output;
                    }

                    return new EvaluationResult(CommonStatusCodes.Success, null, string.Concat(ous1), exp, ous1, i);

                case Toggler.On:
                    var ous2 = new string[i];

                    for (int j = 0; j < i; j++)
                        ous2[j] = exp.Evaluate(context).Output;

                    return new EvaluationResult(CommonStatusCodes.Success, null, string.Concat(ous2), exp, ous2, i);

                default:    //  false
                    for (int j = 0; j < i; j++)
                        exp.Evaluate(context);

                    return new EvaluationResult(CommonStatusCodes.Success, null, string.Format("Given expression has been indiscriminately evaluated {0} times.", i), exp, i);
            }
        }

        [MethodCommandData(Name = "for", Abstract = "Repeats a command for a number of times.")]
        static EvaluationResult for_loop(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            if (args.Length != 4 && args.Length != 5)
                return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'for' must receive 4 or 5 arguments: iterator name, initial value, end value, an expression and an optional increment amount that defaults to 1 or -1 depending on direction.", args);

            var evalRes = args[0].Evaluate(context);

            var evalRes2 = evalRes.CheckTruthValue(0, "for", true);
            if (evalRes2 != null) return evalRes2;

            var iteratorName = evalRes.Output;

            var bounds = new int[2];

            for (int j = 0; j < 2; j++)
            {
                evalRes = args[1 + j].Evaluate(context);
                int numba = -1;

                evalRes2 = evalRes.CheckTruthValue(1 + j, "for", true);
                if (evalRes2 != null) return evalRes2;
                evalRes2 = evalRes.ExtractUniqueDatum<int>(1 + j, "for", ref numba);
                if (evalRes2 != null) return evalRes2;
                if (numba < 1)  //  We will use 1-based indexes here.
                    return new EvaluationResult(CommonStatusCodes.LoopNegativeBound, null, string.Format("Evaluation of argument #{0} to 'repeat' returned non-positive number.", j + 2), j + 1, numba, evalRes, bounds);

                bounds[j] = numba;
            }

            bool backwards = bounds[0] > bounds[1];
            int incrementor = backwards ? -1 : 1;

            if (args.Length == 5)
            {
                evalRes = args[4].Evaluate(context);
                int numba = -1;

                evalRes2 = evalRes.CheckTruthValue(4, "for", true);
                if (evalRes2 != null) return evalRes2;
                evalRes2 = evalRes.ExtractUniqueDatum<int>(4, "for", ref numba);
                if (evalRes2 != null) return evalRes2;
                if (numba == 0 || (backwards && numba > 0) || (!backwards && numba < 0))
                    return new EvaluationResult(CommonStatusCodes.LoopIncrementorInvalid, null, 
                        backwards
                            ? "Incrementor of 'for' loop must be strictly negative when initial value is greater than end value."
                            : "Incrementor of 'for' loop must be strictly positive when initial value is lower than or equal to end value.", 
                        numba, backwards, evalRes);

                incrementor = numba;
            }

            var exp = args[3];
            int i = 0;

            switch (toggle)
            {
                case Toggler.Neutral:
                    var ous1 = new List<string>((int)Math.Ceiling(Math.Abs((double)(bounds[1] - bounds[0]) / incrementor)));

                    System.Diagnostics.Debug.WriteLine("Expected {0} for loop iterations; Start = {1}; End = {2}; Incrementor = {3}", ous1.Count, bounds[0], bounds[1], incrementor);

                    for (int j = bounds[0]; backwards ? (j >= bounds[1]) : (j <= bounds[1]); j += incrementor)
                    {
                        evalRes = exp.Evaluate(context.WithLocal(iteratorName, ConstantExpression.Fetch(j.ToString()).Evaluate(context)));

                        if (!evalRes.TruthValue)
                            return new EvaluationResult(CommonStatusCodes.LoopExpressionFailure, null, string.Format("Evaluation #{0} of the 'repeat' loop expression (argument #4) failed with non-zero status: {1} ({2}); {3}", i + 1, evalRes.Status, evalRes.CommonStatus, evalRes.Output), exp, evalRes, j, ous1, 1);
                        
                        ous1.Add(evalRes.Output);
                        i++;

                        System.Diagnostics.Debug.WriteLine("\tJ = {0}", j);
                    }

                    return new EvaluationResult(CommonStatusCodes.Success, null, string.Concat(ous1), exp, i, ous1);

                case Toggler.On:
                    var ous2 = new List<string>((int)Math.Ceiling(Math.Abs((double)(bounds[1] - bounds[0]) / incrementor)));

                    for (int j = bounds[0]; backwards ? (j >= bounds[1]) : (j <= bounds[1]); j += incrementor)
                    {
                        ous2.Add(exp.Evaluate(context.WithLocal(iteratorName, ConstantExpression.Fetch(j.ToString()).Evaluate(context))).Output);
                        i++;
                    }

                    return new EvaluationResult(CommonStatusCodes.Success, null, string.Concat(ous2), exp, i, ous2);

                default:
                    for (int j = bounds[0]; backwards ? (j >= bounds[1]) : (j <= bounds[1]); j += incrementor)
                    {
                        exp.Evaluate(context.WithLocal(iteratorName, ConstantExpression.Fetch(j.ToString()).Evaluate(context)));
                        i++;
                    }

                    return new EvaluationResult(CommonStatusCodes.Success, null, string.Format("Given expression has been indiscriminately evaluated {0} times.", i), exp, i);
            }
        }

        [MethodCommandData(Abstract = "Retrieves a local value from the evaluation context.")]
        static EvaluationResult local(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            switch (toggle)
            {
                case Toggler.Neutral:
                    if (args.Length != 1)
                        return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'local' must receive one argument: a name.", args);
                    break;

                case Toggler.Off:
                    if (args.Length != 1)
                        return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'-local' must receive one argument: a name.", args);
                    break;

                default:
                    if (args.Length != 2)
                        return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'+local' must receive two arguments: a name and a value.", args);
                    break;
            }

            var evalRes = args[0].Evaluate(context);

            var evalRes2 = evalRes.CheckTruthValue(0, "local", true);
            if (evalRes2 != null) return evalRes2;

            switch (toggle)
            {
                case Toggler.Neutral:
                    EvaluationResult res = null;

                    if (context.Locals.TryGetValue(evalRes.Output, out res))
                        return res;
                    else
                        return new EvaluationResult(CommonStatusCodes.LocalVariableNotFound, null, string.Format("No local variable named '{0}' exists in the context.", evalRes.Output), evalRes);

                case Toggler.Off:
                    if (context.Locals.Remove(evalRes.Output))
                        return new EvaluationResult(CommonStatusCodes.Success, null, string.Format("Local variable '{0}' is removed.", evalRes.Output)); //  Operation is most likely going to be part of something more complex, but that is fixable.
                    else
                        return new EvaluationResult(CommonStatusCodes.LocalVariableNotFound, null, string.Format("No local variable named '{0}' exists in the context.", evalRes.Output), evalRes);

                default:
                    var name = evalRes.Output;

                    evalRes = args[1].Evaluate(context);

                    evalRes2 = evalRes.CheckTruthValue(1, "local", true);
                    if (evalRes2 != null) return evalRes2;

                    context.Locals[name] = evalRes;

                    return evalRes; //  Has all we need.
            }
        }

        [MethodCommandData(Abstract = "Evaluates an expression with some local values.")]
        static EvaluationResult with(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            if (args.Length < 3 || args.Length % 2 != 1)
                return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'with' must receive at least one local definition(a name followed by a value) and an expression.");

            List<KeyValuePair<string, EvaluationResult>> locals = new List<KeyValuePair<string, EvaluationResult>>(args.Length / 2);

            EvaluationResult evalRes;

            for (int i = 0; i < args.Length - 1; i += 2)
            {
                string name = null;

                evalRes = args[i].Evaluate(context);

                var evalRes2 = evalRes.CheckTruthValue(i, "with", true);
                if (evalRes2 != null) return evalRes2;

                name = evalRes.Output;

                evalRes = args[i + 1].Evaluate(context);

                evalRes2 = evalRes.CheckTruthValue(i + 1, "with", true);
                if (evalRes2 != null) return evalRes2;

                locals.Add(new KeyValuePair<string, EvaluationResult>(name, evalRes));
            }

            return args[args.Length - 1].Evaluate(context.WithLocal(locals));
        }

        #region User-defined commands and aliases

        [MethodCommandData(Abstract = "Creates a named alias for an expression.")]
        static EvaluationResult alias(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            switch (toggle)
            {
                case Toggler.Neutral:
                    if (args.Length != 2)
                        return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'alias' must receive two arguments: a name and an expression.");
                    break;

                case Toggler.On:
                    if (args.Length != 2)
                        return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'+alias' must receive two arguments: a name and an expression.");
                    break;

                default:
                    if (args.Length != 1)
                        return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'-alias' must receive one argument: a name.");
                    break;
            }

            var evalRes = args[0].Evaluate(context);

            var evalRes2 = evalRes.CheckTruthValue(0, "alias", true);
            if (evalRes2 != null) return evalRes2;

            switch (toggle)
            {
                case Toggler.Neutral:
                    var als1 = new Alias(evalRes.Output, args[1].ToString());

                    if (context.Host.RegisterCommand(als1, false, false))
                        return EvaluationResult.NewEmptyPositive;
                    else
                        return new EvaluationResult(CommonStatusCodes.CommandAlreadyExists, null, string.Format("A command already exists with the given Name = {0}", evalRes.Output));

                case Toggler.On:
                    var als2 = new Alias(evalRes.Output, args[1].ToString());

                    if (context.Host.RegisterCommand(als2, true, true))
                        return EvaluationResult.NewEmptyPositive;
                    else
                        return new EvaluationResult(CommonStatusCodes.CommandAlreadyExists, null, string.Format("A command already exists with the given name, and it is not an alias: {0}", evalRes.Output));

                default:
                    if (context.Host.RemoveCommand(evalRes.Output))
                        return EvaluationResult.NewEmptyPositive;
                    else
                        return new EvaluationResult(CommonStatusCodes.CommandAlreadyExists, null, string.Format("A command already exists with the given Name = {0}", evalRes.Output));
            }
        }

        [MethodCommandData(Abstract = "Creates a named command which can accept arguments.")]
        static EvaluationResult define(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            switch (toggle)
            {
                case Toggler.Neutral:
                    if (args.Length != 2)
                        return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'define' must receive two arguments: a name and an expression.");
                    break;

                case Toggler.On:
                    if (args.Length != 2)
                        return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'+define' must receive two arguments: a name and an expression.");
                    break;

                default:
                    if (args.Length != 1)
                        return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'-define' must receive one argument: a name.");
                    break;
            }

            var evalRes = args[0].Evaluate(context);

            var evalRes2 = evalRes.CheckTruthValue(0, "define", true);
            if (evalRes2 != null) return evalRes2;
            
            switch (toggle)
            {
                case Toggler.Neutral:
                    var als1 = new UserCommand(evalRes.Output, args[1]);

                    if (context.Host.RegisterCommand(als1, false, false))
                        return EvaluationResult.EmptyPositive;
                    else
                        return new EvaluationResult(CommonStatusCodes.CommandAlreadyExists, null, string.Format("A command already exists with the given Name = {0}", evalRes.Output));

                case Toggler.On:
                    var als2 = new UserCommand(evalRes.Output, args[1]);

                    if (context.Host.RegisterCommand(als2, true, true))
                        return EvaluationResult.EmptyPositive;
                    else
                        return new EvaluationResult(CommonStatusCodes.CommandAlreadyExists, null, string.Format("A command already exists with the given Name = {0}", evalRes.Output));

                default:
                    if (context.Host.RemoveCommand(evalRes.Output))
                        return EvaluationResult.EmptyPositive;
                    else
                        return new EvaluationResult(CommonStatusCodes.CommandAlreadyExists, null, string.Format("A command already exists with the given Name = {0}", evalRes.Output));
            }

        }

        [MethodCommandData(Abstract = "Retrieves a user argument from the evaluation context for a user-defined function.")]
        static EvaluationResult arg(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            if (args.Length != 1)
                return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'arg' must receive one argument: an index.");
            if (context.UserArguments == null)
                return new EvaluationResult(CommonStatusCodes.UserArgumentsMissing, null, "Execution context lacks user arguments.");

            var evalRes = args[0].Evaluate(context);

            var evalRes2 = evalRes.CheckTruthValue(0, "arg", true);
            if (evalRes2 != null) return evalRes2;

            int i = -1;

            evalRes2 = evalRes.ExtractUniqueDatum<int>(0, "arg", ref i);
            if (evalRes2 != null) return evalRes2;

            if (i < 1)  //  We will use 1-based indexes here.
                return new EvaluationResult(CommonStatusCodes.UserArgumentIndexInvalid, null, string.Format("Argument index ({0}) must be strictly positive.", i), evalRes, i);
            if (i > context.UserArguments.Count)
                return new EvaluationResult(CommonStatusCodes.UserArgumentNotFound, null, "There is no user argument at index #" + i, evalRes, i);

            return context.UserArguments[i - 1].Evaluate(context);
        }

        [MethodCommandData(Abstract = "Retrieves the number of user arguments from the execution context for a user-defined function.")]
        static EvaluationResult argc(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            if (args.Length != 0)
                return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'argc' must receive no arguments.");
            if (context.UserArguments == null)
                return new EvaluationResult(CommonStatusCodes.UserArgumentsMissing, null, "There are no user arguments in the execution context.");

            return new EvaluationResult(CommonStatusCodes.Success, null, context.UserArguments.Count.ToString()
                , context.UserArguments.Count, (double)context.UserArguments.Count);
        }

        #endregion

        #region Mathematics

        //  Are added above.

        static double Neg(double d)
        {
            return -d;
        }

        static double Ln(double d)
        {
            return Math.Log(d);
        }

        static double DegToRad = Math.PI / 180d;
        static double RadToDeg = 180d / Math.PI;

        static double D2R(double d)
        {
            return d * DegToRad;
        }

        static double R2D(double d)
        {
            return d * RadToDeg;
        }

        static double mod(double a, double b)
        {
            return a % b;   //  Why does this work?
        }

        static double aravg(double[] args)
        {
            double up = 0d;

            for (int i = 0; i < args.Length; i++) up += args[i];

            return up / args.Length;
        }

        static double gmavg(double[] args)
        {
            double up = 1d;

            for (int i = 0; i < args.Length; i++) up *= args[i];

            return Math.Pow(up, 1d / args.Length);
        }

        static double rsavg(double[] args)
        {
            double up = 0d;

            for (int i = 0; i < args.Length; i++) up += (args[i] * args[i]);

            return Math.Sqrt(up / args.Length);
        }

        static double hmavg(double[] args)
        {
            double down = 0d;

            for (int i = 0; i < args.Length; i++) down += (1d / args[i]);

            return (double)args.Length / down;
        }

        static double min(double[] args)
        {
            double val = args[0];

            for (int i = 1; i < args.Length; i++)
                if (args[i] < val)
                    val = args[i];

            return val;
        }

        static double max(double[] args)
        {
            double val = args[0];

            for (int i = 1; i < args.Length; i++)
                if (args[i] > val)
                    val = args[i];

            return val;
        }

        static bool lt(double a, double b)
        {
            return a < b;
        }

        static bool lteq(double a, double b)
        {
            return a <= b;
        }

        static bool gt(double a, double b)
        {
            return a > b;
        }

        static bool gteq(double a, double b)
        {
            return a > b;
        }

        [MethodCommandData(Abstract = "Generates a random number.")]
        static EvaluationResult rand(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            if (rnd == null)
                rnd = new Random();

            EvaluationResult evalRes = null, evalRes2 = null;

            switch (args.Length)
            {
                case 0:
                    var r1 = rnd.NextDouble();
                    return new EvaluationResult(CommonStatusCodes.Success, null, r1.ToString(), r1);

                case 1:
                    int i = -1;
                    
                    evalRes = args[0].Evaluate(context);
                    evalRes2 = evalRes.CheckTruthValue(0, "rand", true);
                    if (evalRes2 != null) return evalRes2;
                    evalRes2 = evalRes.ExtractUniqueDatum<int>(0, "rand", ref i);
                    if (evalRes2 != null) return evalRes2;

                    if (int.TryParse(evalRes.Output, out i))
                        return new EvaluationResult(CommonStatusCodes.Success, null, rnd.Next(i).ToString());
                    else
                        return new EvaluationResult(CommonStatusCodes.ArgumentEvaluationFailure, null, string.Format("Failed to convert output of argument #{0} to an unsigned integer: {1}", i + 1, evalRes.Output));

                case 2:
                    var bounds = new int[2];

                    for (int j = 0; j < 2; j++)
                    {
                        evalRes = args[j].Evaluate(context);

                        if (!evalRes.TruthValue)
                            return new EvaluationResult(CommonStatusCodes.ArgumentEvaluationFailure, null, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", j + 1, evalRes.Status, evalRes.Output));

                        int b;

                        if (int.TryParse(evalRes.Output, out b))
                            bounds[j] = b;
                        else
                            return new EvaluationResult(CommonStatusCodes.ArgumentEvaluationFailure, null, string.Format("Failed to convert output of argument #{0} to an unsigned integer: {1}", j + 1, evalRes.Output));
                    }

                    return new EvaluationResult(CommonStatusCodes.Success, null, rnd.Next(bounds[0], bounds[1]).ToString());

                default:
                    return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "Too many arguments given to 'rand'.");
            }
        }

        [MethodCommandData(Category = "Mathematics", Abstract = "Adds the given arguments together, converting input to numbers.")]
        static EvaluationResult add(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            decimal res = 0m;

            if (args.Length < 2)
                return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "Please provide at least two arguments to 'add'.");

            for (int i = 0; i < args.Length; i++)
            {
                var evalRes = args[i].Evaluate(context);

                if (evalRes.Status != 0)
                    return new EvaluationResult(CommonStatusCodes.ArgumentEvaluationFailure, null, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                decimal d;

                if (decimal.TryParse(evalRes.Output, out d))
                    res += d;
                else
                    return new EvaluationResult(3, null, string.Format("Failed to convert output of argument #{0} to a number: {1}", i + 1, evalRes.Output));
            }

            return new EvaluationResult(CommonStatusCodes.Success, null, res.ToString());
        }

        [MethodCommandData(Category = "Mathematics", Abstract = "Subtracts from the first argument the values of the other arguments, converting input to numbers.")]
        static EvaluationResult sub(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            decimal res = 0m;

            if (args.Length < 2)
                return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "Please provide at least two arguments to 'sub'.");

            for (int i = 0; i < args.Length; i++)
            {
                var evalRes = args[i].Evaluate(context);

                if (evalRes.Status != 0)
                    return new EvaluationResult(CommonStatusCodes.ArgumentEvaluationFailure, null, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                decimal d;

                if (decimal.TryParse(evalRes.Output, out d))
                {
                    if (i == 0)
                        res = d;
                    else
                        res -= d;
                }
                else
                    return new EvaluationResult(3, null, string.Format("Failed to convert output of argument #{0} to a number: {1}", i + 1, evalRes.Output));
            }

            return new EvaluationResult(CommonStatusCodes.Success, null, res.ToString());
        }

        [MethodCommandData(Category = "Mathematics", Abstract = "Multiplies the given arguments together, converting input to numbers.")]
        static EvaluationResult mul(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            decimal res = 1m;

            if (args.Length < 2)
                return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "Please provide at least two arguments to 'mul'.");

            for (int i = 0; i < args.Length; i++)
            {
                var evalRes = args[i].Evaluate(context);

                if (evalRes.Status != 0)
                    return new EvaluationResult(CommonStatusCodes.ArgumentEvaluationFailure, null, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                decimal d;

                if (decimal.TryParse(evalRes.Output, out d))
                    res *= d;
                else
                    return new EvaluationResult(3, null, string.Format("Failed to convert output of argument #{0} to a number: {1}", i + 1, evalRes.Output));
            }

            return new EvaluationResult(CommonStatusCodes.Success, null, res.ToString());
        }

        [MethodCommandData(Category = "Mathematics", Abstract = "Divides the first argument by the values of the other arguments, converting input to numbers.")]
        static EvaluationResult div(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            decimal res = 1m;

            if (args.Length < 2)
                return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "Please provide at least two arguments to 'div'.");

            for (int i = 0; i < args.Length; i++)
            {
                var evalRes = args[i].Evaluate(context);

                if (evalRes.Status != 0)
                    return new EvaluationResult(CommonStatusCodes.ArgumentEvaluationFailure, null, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                decimal d;

                if (decimal.TryParse(evalRes.Output, out d))
                {
                    if (i == 0)
                        res = d;
                    else
                        res /= d;
                }
                else
                    return new EvaluationResult(3, null, string.Format("Failed to convert output of argument #{0} to a number: {1}", i + 1, evalRes.Output));
            }

            return new EvaluationResult(CommonStatusCodes.Success, null, res.ToString());
        }

        [MethodCommandData(Category = "Mathematics", Abstract = "Outputs the rounded value of the given input number..")]
        static EvaluationResult round(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            if (args.Length != 1 && args.Length != 2)
                return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'floor' must receive one or two argument, both numbers.");

            var evalRes = args[0].Evaluate(context);

            if (!evalRes.TruthValue)
                return new EvaluationResult(CommonStatusCodes.ArgumentEvaluationFailure, null, string.Format("Evaluation of argument #1 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

            int digits = 0;

            if (args.Length > 1)
            {
                evalRes = args[1].Evaluate(context);

                if (!evalRes.TruthValue)
                    return new EvaluationResult(3, null, string.Format("Evaluation of argument #2 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

                if (!int.TryParse(evalRes.Output, out digits))
                    return new EvaluationResult(4, null, "The given second argument is not a number.");
                if (digits < 0 || digits > 28)
                    return new EvaluationResult(5, null, "The number of digits (second argument) must be between 0 and 28 inclusively.");
            }

            decimal d;

            if (!decimal.TryParse(evalRes.Output, out d))
                return new EvaluationResult(6, null, "The given first argument is not a number.");

            return new EvaluationResult(CommonStatusCodes.Success, null, Math.Round(d, digits).ToString());
        }

        [MethodCommandData(Category = "Comparison", Abstract = "Determines whether all given arguments are equal.")]
        static EvaluationResult eq(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            bool restrictive = toggle != Toggler.Off;

            if (args.Length < 2)
                return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "Please provide at least two arguments to 'eq'.");

            switch (toggle)
            {
                case Toggler.Neutral:
                case Toggler.On:
                    string cur = null;

                    for (int i = 0; i < args.Length; i++)
                    {
                        var evalRes = args[i].Evaluate(context);

                        if (restrictive && evalRes.Status != 0)
                            return new EvaluationResult(CommonStatusCodes.ArgumentEvaluationFailure, null, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                        if (cur == null)
                            cur = evalRes.Output;
                        else
                            if (cur != evalRes.Output)
                                return new EvaluationResult(3, null, string.Format("Argument #{0}'s output is not equal to its predecessors'.", i + 1));
                    }

                    return new EvaluationResult(CommonStatusCodes.Success, null, cur);

                default:
                    bool? st = null;

                    for (int i = 0; i < args.Length; i++)
                    {
                        var evalRes = args[i].Evaluate(context);

                        if (st == null)
                            st = evalRes.TruthValue;
                        else
                            if (st != evalRes.TruthValue)
                                return new EvaluationResult(4, null, string.Format("Argument #{0}'s truth value is not equal to its predecessors'.", i + 1));
                    }

                    return new EvaluationResult(CommonStatusCodes.Success, null, "All arguments have identical truth value");
            }
        }

        [MethodCommandData(Category = "Comparison", Abstract = "Determines whether the two given arguments are equal.")]
        static EvaluationResult neq(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            bool restrictive = toggle != Toggler.Off;

            if (args.Length != 2)
                return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "Please provide two arguments to 'neq'.");

            switch (toggle)
            {
                case Toggler.Neutral:
                case Toggler.On:
                    string[] ou = new string[2];

                    for (int i = 0; i < 2; i++)
                    {
                        var evalRes = args[i].Evaluate(context);

                        if (restrictive && evalRes.Status != 0)
                            return new EvaluationResult(CommonStatusCodes.ArgumentEvaluationFailure, null, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                        ou[i] = args[i].Evaluate(context).Output;
                    }

                    if (ou[0] == ou[1])
                        return new EvaluationResult(4, null, string.Format("{0} = {1}", ou[0], ou[1]));
                    else
                        return new EvaluationResult(CommonStatusCodes.Success, null, string.Format("{0} ≠ {1}", ou[0], ou[1]));

                default:
                    bool[] st = new bool[2];

                    for (int i = 0; i < 2; i++)
                        st[i] = args[i].Evaluate(context).TruthValue;

                    if (st[0] == st[1])
                        return new EvaluationResult(4, null, string.Format("{0} = {1}", st[0], st[1]));
                    else
                        return new EvaluationResult(CommonStatusCodes.Success, null, string.Format("{0} ≠ {1}", st[0], st[1]));
            }
        }

        #endregion

        #region Strings

        [MethodCommandData(Abstract = "Extracts a substring from a string.")]
        static EvaluationResult subs(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            if (args.Length != 3)
                return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'subs' must receive 3 arguments: start index, length and a string.");

            EvaluationResult evalRes;

            var bounds = new int[2];

            for (int j = 0; j < 2; j++)
            {
                evalRes = args[j].Evaluate(context);

                if (!evalRes.TruthValue)
                    return new EvaluationResult(CommonStatusCodes.ArgumentEvaluationFailure, null, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", j + 1, evalRes.Status, evalRes.Output));

                int numba;

                if (!int.TryParse(evalRes.Output, out numba))
                    return new EvaluationResult(3, null, "The given argument is not an integer.");
                if (numba < 0)  //  We will use 1-based indexes here.
                    return new EvaluationResult(4, null, "The given argument must be greater than or equal to 0 (zero).");

                bounds[j] = numba;
            }

            evalRes = args[2].Evaluate(context);

            if (!evalRes.TruthValue)
                return new EvaluationResult(5, null, string.Format("Evaluation of argument #3 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

            if (bounds[0] > evalRes.Output.Length)
                return new EvaluationResult(6, null, string.Format("Start index is {0}, but string only contains {1} characters.", bounds[0], evalRes.Output.Length));

            if (bounds[0] + bounds[1] > evalRes.Output.Length)
                return new EvaluationResult(7, null, string.Format("Resulted end index is {0}, but string only contains {1} characters.", bounds[0] + bounds[1], evalRes.Output.Length));

            return new EvaluationResult(CommonStatusCodes.Success, null, evalRes.Output.Substring(bounds[0], bounds[1]));
        }

        [MethodCommandData(Abstract = "Formats the given first argument with the other arguments.")]
        static EvaluationResult format(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            if (args.Length < 1)
                return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'format' must be given at least one argument: a format string.");

            var evalRes = args[0].Evaluate(context);

            if (!evalRes.TruthValue)
                return new EvaluationResult(CommonStatusCodes.ArgumentEvaluationFailure, null, string.Format("Evaluation of argument #1 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

            var format = evalRes.Output;

            var frmargs = new object[args.Length - 1];

            for (int i = 1; i < args.Length; i++)
            {
                evalRes = args[i].Evaluate(context);

                if (!evalRes.TruthValue)
                    return new EvaluationResult(3, null, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                frmargs[i - 1] = evalRes.Output;
            }

            try
            {
                format = string.Format(format, frmargs);
            }
            catch (FormatException x)
            {
                return new EvaluationResult(4, null, string.Format("There is an issue with the format string: {0}", x.Message));
            }

            return new EvaluationResult(CommonStatusCodes.Success, null, format);
        }

        #endregion

        #region Variables

        [MethodCommandData(Abstract = "Retrieves the value of a variable from the host.")]
        static EvaluationResult cvar(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            switch (toggle)
            {
                case Toggler.Neutral:
                    if (args.Length != 1)
                        return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'cvar' must receive one argument: a name.");
                    break;

                case Toggler.Off:
                    if (args.Length != 2)
                        return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'-cvar' must receive two arguments: a name and a value.");
                    break;

                default:
                    if (args.Length != 2)
                        return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'+cvar' must receive two arguments: a name and a value.");
                    break;
            }

            var evalRes = args[0].Evaluate(context);

            if (!evalRes.TruthValue)
                return new EvaluationResult(CommonStatusCodes.ArgumentEvaluationFailure, null, string.Format("Evaluation of argument #1 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

            var variable = context.Host.GetVariable(evalRes.Output);

            if (variable == null)
                return new EvaluationResult(CommonStatusCodes.CvarNotFound, null, "There is no command variable with the specified name.");

            switch (toggle)
            {
                case Toggler.Neutral:
                    return new EvaluationResult(CommonStatusCodes.Success, null, variable.StringValue, variable);

                case Toggler.Off:
                    return variable.ChangeValue(context, args[1], ChangeType.FromDataOrOutput);

                default:
                    return variable.ChangeValue(context, args[1], ChangeType.FromOutput);
            }
        }

        #endregion

        [MethodCommandData(Abstract = "Searches for a manual in the library by title.")]
        static EvaluationResult man(Toggler toggle, EvaluationContext context, Expression[] args)
        {
            if (args.Length < 1)
                return new EvaluationResult(CommonStatusCodes.InvalidArgumentCount, null, "'man' must receive at least one argument: a regex or name and additional flags.");

            var evalRes = args[0].Evaluate(context);

            if (!evalRes.TruthValue)
                return new EvaluationResult(CommonStatusCodes.ArgumentEvaluationFailure, null, string.Format("Evaluation of argument #1 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

            var rgx = new Regex(evalRes.Output);

            //  Processing flags

            int displayLocation = 0;
            ManualLookupLocation ll = ManualLookupLocation.ManualTitle;
            IDriver drv = context.Host.ManualDrivers.DefaultDriver;
            int[] scsi = null;

            for (int i = 1; i < args.Length; i++)
            {
                evalRes = args[i].Evaluate(context);

                if (!evalRes.TruthValue)
                    return new EvaluationResult(CommonStatusCodes.ArgumentEvaluationFailure, null, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                switch (evalRes.Output)
                {
                    //  Lookup locations

                    case "nomt":
                        ll &= ~ManualLookupLocation.ManualTitle;
                        break;

                    case "ma":
                        ll |= ManualLookupLocation.ManualAbstract;
                        break;

                    case "st":
                        ll |= ManualLookupLocation.SectionTitles;
                        break;

                    case "sb":
                        ll |= ManualLookupLocation.SectionBodies;
                        break;

                    //  Display information

                    case "jt":
                        if (displayLocation != 0) return new EvaluationResult(4, null, string.Format("A display location was already specified before argument #{0}", i + 1));
                        displayLocation = 1;
                        break;

                    case "ja":
                        if (displayLocation != 0) return new EvaluationResult(4, null, string.Format("A display location was already specified before argument #{0}", i + 1));
                        displayLocation = 2;
                        break;

                    case "ji":
                        if (displayLocation != 0) return new EvaluationResult(4, null, string.Format("A display location was already specified before argument #{0}", i + 1));
                        displayLocation = 3;
                        break;

                    default:
                        var str = evalRes.Output;

                        if (str.StartsWith("driver="))
                        {
                            if (displayLocation != 0) return new EvaluationResult(4, null, string.Format("A display location was already specified before argument #{0}", i + 1));
                            displayLocation = -1;

                            var drvn = str.Substring(7);

                            if (drvn.Length == 0)
                                return new EvaluationResult(13, null, string.Format("Something must follow \"driver=\" at argument #{0}.", i + 1));

                            drv = context.Host.ManualDrivers[drvn];

                            if (drv == null)
                            {
                                var rgx2 = new Regex(drvn);

                                var drvs = context.Host.ManualDrivers.FindDriver(rgx2).ToArray();

                                switch (drvs.Length)
                                {
                                    case 0:
                                        return new EvaluationResult(11, null, "No driver found matching the given mask.");

                                    case 1:
                                        drv = drvs[0];
                                        break;

                                    case 2:
                                        var sb = new StringBuilder();

                                        sb.AppendFormat("Found {0} drivers matching given mask:");

                                        for (int j = 0; j < drvs.Length; j++)
                                        {
                                            sb.AppendLine();
                                            sb.Append('\t');
                                            sb.Append(j + 1);
                                            sb.Append(": ");
                                            sb.Append(drvs[i].Name);
                                        }

                                        return new EvaluationResult(12, null, sb.ToString());
                                }
                            }
                        }
                        else if (str.StartsWith("section="))
                        {
                            if (displayLocation != 0) return new EvaluationResult(4, null, string.Format("A display location was already specified before argument #{0}", i + 1));
                            displayLocation = 4;

                            var scss = str.Substring(8);

                            if (scss.Length == 0)
                                return new EvaluationResult(30, null, string.Format("Something must follow \"section=\" at argument #{0}", i + 1));

                            var scsp = scss.Split('.');
                            scsi = new int[scsp.Length];

                            for (int j = 0; j < scsi.Length; j++)
                            {
                                int tmp = -1;

                                if (!int.TryParse(scsp[j], out tmp))
                                    return new EvaluationResult(31, null, string.Format("Section #{0} in argument #{1} is not an integer.", j + 1, i + 1));

                                if (tmp < 1)
                                    return new EvaluationResult(32, null, string.Format("Section #{0} in argument #{1} must be strictly positive.", j + 1, i + 1));

                                scsi[j] = tmp - 1;  //  The command takes 1-based indexes; the inner representation is 0-based.
                            }
                        }

                        break;
                }
            }

            if (ll == 0)
                return new EvaluationResult(5, null, "Must have at least one specified lookup location.");

            //

            Manual man = null;

            if (ll == ManualLookupLocation.ManualTitle)
                man = context.Host.Library[evalRes.Output];

            if (man == null)
            {
                var mans = context.Host.Library.FindManual(rgx, ll).ToArray();

                switch (mans.Length)
                {
                    case 0:
                        return new EvaluationResult(20, null, "No manual(s) found matching the given arguments.");

                    case 1:
                        man = mans[0];
                        break;

                    default:
                        var sb = new StringBuilder();

                        sb.AppendFormat("Found {0} manuals matching given mask:", mans.Length);

                        for (int i = 0; i < mans.Length; i++)
                        {
                            sb.AppendLine();
                            sb.Append('\t');
                            sb.Append(i + 1);
                            sb.Append(": ");
                            sb.Append(mans[i].Title);
                        }

                        return new EvaluationResult(21, null, sb.ToString());
                }
            }

            switch (displayLocation)
            {
                case 1:
                    return new EvaluationResult(CommonStatusCodes.Success, null, man.Title);

                case 2:
                    return new EvaluationResult(CommonStatusCodes.Success, null, man.Abstract);

                case 3:
                    var b = new StringBuilder();

                    int i = 0;
                    foreach (var s in man.Sections)
                        ManualSections.indexSection(b, s, ++i, string.Empty, null, " ", ".");

                    return new EvaluationResult(CommonStatusCodes.Success, null, b.ToString());

                case 4:
                    var sc = man[scsi];

                    if (sc == null)
                        return new EvaluationResult(33, null, string.Format("Could not find the specified section in the manual."));

                    return new EvaluationResult(CommonStatusCodes.Success, null, sc.Body);

                default:
                    if (drv == null)
                        return new EvaluationResult(10, null, "There is no driver to display the manual with.");

                    return drv.Display(context, man);
            }
        }
    }
}
