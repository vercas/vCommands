using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace vCommands
{
    using Commands;
    using Variables;
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
                    if (args.Length != 1)
                        return new EvaluationResult(1, string.Format("'{0}' must receive one argument: a number.", mthdName));

                    var evalRes = args[0].Evaluate(context);

                    if (!evalRes.TruthValue)
                        return new EvaluationResult(2, string.Format("Evaluation of argument #1 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

                    double d;

                    if (!double.TryParse(evalRes.Output, out d))
                        return new EvaluationResult(3, "The given argument is not a number.");

                    return new EvaluationResult(0, mthd(d).ToString());
                };

                var cmd = new MethodCommand(mthdName, "Mathematics", kv.Item2, deleg);

                all.Add(cmd);
            }

            //  Binary commands, which require two arguments/operands.

            var binaryMathematics = new Tuple<BinaryMathematicalFunction<double>, string>[]
            {
                new Tuple<BinaryMathematicalFunction<double>, string>(Math.Log, "Computes the logarithm of the first numbers in the base of the second number."),
                new Tuple<BinaryMathematicalFunction<double>, string>(Math.Pow, "Computes the first number raised to the power of the second number."),
                new Tuple<BinaryMathematicalFunction<double>, string>(mod, "Computes the remainder of dividing the first number to the second."),
            };

            foreach (var kv in binaryMathematics)
            {
                var mthd = kv.Item1;
                var mthdName = kv.Item1.Method.Name.ToLower();

                CommandMethod deleg = (toggle, context, args) =>
                {
                    if (args.Length != 2)
                        return new EvaluationResult(1, string.Format("'{0}' must receive two arguments, both numbers.", mthdName));

                    var numbers = new double[2];

                    for (int i = 0; i < 2; i++)
                    {
                        var evalRes = args[i].Evaluate(context);

                        if (!evalRes.TruthValue)
                            return new EvaluationResult(2, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                        double d;

                        if (!double.TryParse(evalRes.Output, out d))
                            return new EvaluationResult(3, string.Format("Argument #{0} is not a number.", i + 1));

                        numbers[i] = d;
                    }

                    return new EvaluationResult(0, mthd(numbers[0], numbers[1]).ToString());
                };

                var cmd = new MethodCommand(mthdName, "Mathematics", kv.Item2, deleg);

                all.Add(cmd);
            }

            //  Binary logic commands, which require two arguments/operands.

            var binaryLogicMathematics = new Tuple<BinaryMathematicalLogicFunction<double>, char, char, string>[]
            {
                new Tuple<BinaryMathematicalLogicFunction<double>, char, char, string>(lt, '<', '≮', "Determines whether the first number is less than the second."),
                new Tuple<BinaryMathematicalLogicFunction<double>, char, char, string>(lteq, '≤', '≰', "Determines whether the first number is less than or equal to the second."),
                new Tuple<BinaryMathematicalLogicFunction<double>, char, char, string>(gt, '>', '≯', "Determines whether the first number is greater than the second."),
                new Tuple<BinaryMathematicalLogicFunction<double>, char, char, string>(gteq, '≥', '≱', "Determines whether the first number is greater than or equal the second."),
            };

            foreach (var kv in binaryLogicMathematics)
            {
                var mthd = kv.Item1;
                char sym1 = kv.Item2, sym2 = kv.Item3;
                var mthdName = kv.Item1.Method.Name.ToLower();

                CommandMethod deleg = (toggle, context, args) =>
                {
                    if (args.Length != 2)
                        return new EvaluationResult(1, string.Format("'{0}' must receive two arguments, both numbers.", mthdName));

                    var numbers = new double[2];

                    for (int i = 0; i < 2; i++)
                    {
                        var evalRes = args[i].Evaluate(context);

                        if (!evalRes.TruthValue)
                            return new EvaluationResult(2, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                        double d;

                        if (!double.TryParse(evalRes.Output, out d))
                            return new EvaluationResult(3, string.Format("Argument #{0} is not a number.", i + 1));

                        numbers[i] = d;
                    }

                    var res = mthd(numbers[0], numbers[1]);

                    if (res)
                        return new EvaluationResult(0, string.Format("{0} {1} {2}", numbers[0], sym1, numbers[1]));
                    else
                        return new EvaluationResult(4, string.Format("{0} {1} {2}", numbers[0], sym2, numbers[1]));
                };

                var cmd = new MethodCommand(mthdName, "Comparison", kv.Item4, deleg);

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
                    if (args.Length < 2)
                        return new EvaluationResult(1, string.Format("'{0}' must receive at least two arguments, all numbers.", mthdName));

                    var numbers = new double[args.Length];

                    for (int i = 0; i < args.Length; i++)
                    {
                        var evalRes = args[i].Evaluate(context);

                        if (!evalRes.TruthValue)
                            return new EvaluationResult(2, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                        double d;

                        if (!double.TryParse(evalRes.Output, out d))
                            return new EvaluationResult(3, string.Format("Argument #{0} is not a number.", i + 1));

                        numbers[i] = d;
                    }

                    return new EvaluationResult(0, mthd(numbers).ToString());
                };

                var cmd = new MethodCommand(mthdName, "Mathematics", kv.Item2, deleg);

                all.Add(cmd);
            }

            Commands = all.ToArray();
        }

        public static void Register(CommandHost host)
        {
            for (int i = 0; i < Commands.Length; i++)
                host.RegisterCommand(Commands[i]);
        }

        //  And now, the commands.

        [MethodCommandData(abstr: "Displays every available command and its description.")]
        static EvaluationResult help(bool? toggle, EvaluationContext context, Expression[] args)
        {
            IEnumerable<KeyValuePair<string, Command>> cmds = context.Host.cmds;
            IEnumerable<KeyValuePair<string, IVariable>> vars = context.Host.vars;
            var ob = new StringBuilder();
            int ccnt = 0, gcnt = 0, vcnt = 0;

            if (args.Length > 1)
            {
                //  For now, quit.

                return new EvaluationResult(1, "Must not provide more than one argument to 'help', and that argument may be a regular expression.");
            }
            else
            {
                if (args.Length == 1)
                {
                    var evalRes = args[0].Evaluate(context);

                    if (!evalRes.TruthValue)
                        return new EvaluationResult(2, string.Format("Argument evaluation resulted in non-zer: {0} ({1})", evalRes.Status, evalRes.Output));

                    var regex = new Regex(evalRes.Output);

                    if (toggle == null)
                    {
                        cmds = cmds.Where(kv => regex.IsMatch(kv.Key));

                        ob.Append("Looking for regular expression in command names: ");
                    }
                    else if (toggle.Value)
                    {
                        cmds = cmds.Where(kv => regex.IsMatch(kv.Key) || regex.IsMatch(kv.Value.Abstract));

                        ob.Append("Looking for regular expression in command names and descriptions: ");
                    }
                    else
                    {
                        cmds = cmds.Where(kv => regex.IsMatch(kv.Value.Abstract));

                        ob.Append("Looking for regular expression in command descriptions: ");
                    }

                    ob.AppendLine(evalRes.Output);
                }
                else
                {
                    ob.AppendLine("Listing all commands with descriptions:");
                }

                //ob.AppendLine();
            }

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
            ob.AppendFormat("Shown {0} commands under {1} categories and {2} variables.", ccnt, gcnt, vcnt > 0 ? vcnt.ToString() : "no");
            ob.AppendLine();
            ob.AppendFormat("Powered by {0}.", AssemblyName);

            return new EvaluationResult(0, ob.ToString());
        }

        [MethodCommandData(abstr: "Returns the given input arguments, separated by a tabulator.")]
        static EvaluationResult echo(bool? toggle, EvaluationContext context, Expression[] args)
        {
            string output;

            if (toggle == null)
                output = string.Join("\t", from a in args select a.Evaluate(context).Output);
            else if (toggle.Value)
                output = string.Join(Environment.NewLine, from a in args select a.Evaluate(context).Output);
            else
                output = string.Concat(from a in args select a.Evaluate(context).Output);

            return new EvaluationResult(0, output);
        }

        [MethodCommandData(abstr: "Evaluates the given arguments to the last or until one returns non-zero status.")]
        static EvaluationResult eval(bool? toggle, EvaluationContext context, Expression[] args)
        {
            StringBuilder ob = new StringBuilder(4096);
            int status = 0;

            for (int i = 0; i < args.Length; i++)
            {
                var res = args[i].Evaluate(context);

                if (i > 0)
                    ob.Append('\t');

                ob.Append(res.Output);

                status = res.Status;

                if (status != 0)
                    break;
            }

            return new EvaluationResult(status, ob.ToString());
        }

        [MethodCommandData(abstr: "Repeats a command for a number of times.")]
        static EvaluationResult repeat(bool? toggle, EvaluationContext context, Expression[] args)
        {
            if (args.Length != 2)
                return new EvaluationResult(1, "'repeat' must receive two arguments: a count and an expression.");

            var evalRes = args[0].Evaluate(context);

            if (!evalRes.TruthValue)
                return new EvaluationResult(2, string.Format("Evaluation of argument #1 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

            int i;

            if (!int.TryParse(evalRes.Output, out i))
                return new EvaluationResult(4, "The given argument is not an integer.");
            if (i < 1)  //  We will use 1-based indexes here.
                return new EvaluationResult(5, "The given argument must be greater than 0 (zero).");

            switch (toggle)
            {
                case null:
                    var ous1 = new string[i];

                    for (int j = 0; j < i; j++)
                    {
                        evalRes = args[1].Evaluate(context);

                        if (!evalRes.TruthValue)
                            return new EvaluationResult(6, string.Format("Evaluation #{0} of the given expression (argument #2) failed with non-zero status: {0} {1}", j + 1, evalRes.Status, evalRes.Output));

                        ous1[j] = evalRes.Output;
                    }

                    return new EvaluationResult(0, string.Concat(ous1));

                case true:
                    var ous2 = new string[i];

                    for (int j = 0; j < i; j++)
                        ous2[j] = args[1].Evaluate(context).Output;

                    return new EvaluationResult(0, string.Concat(ous2));

                default:
                    for (int j = 0; j < i; j++)
                        args[1].Evaluate(context);

                    return new EvaluationResult(0, string.Format("Given expression has been indiscriminately evaluated {0} times.", i));
            }
        }

        [MethodCommandData(name: "for", abstr: "Repeats a command for a number of times.")]
        static EvaluationResult for_loop(bool? toggle, EvaluationContext context, Expression[] args)
        {
            if (args.Length != 4 && args.Length != 5)
                return new EvaluationResult(1, "'for' must receive 4 or 5 arguments: iterator name, initial value, end value, an expression and an optional increment amount that defaults to 1 or -1 depending on direction.");

            var evalRes = args[0].Evaluate(context);

            if (!evalRes.TruthValue)
                return new EvaluationResult(2, string.Format("Evaluation of argument #1 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

            var iteratorName = evalRes.Output;

            var bounds = new int[2];

            for (int j = 0; j < 2; j++)
            {
                evalRes = args[1 + j].Evaluate(context);

                if (!evalRes.TruthValue)
                    return new EvaluationResult(3, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", 1 + j + 1, evalRes.Status, evalRes.Output));

                int numba;

                if (!int.TryParse(evalRes.Output, out numba))
                    return new EvaluationResult(4, "The given argument is not an integer.");
                if (numba < 1)  //  We will use 1-based indexes here.
                    return new EvaluationResult(5, "The given argument must be greater than 0 (zero).");

                bounds[j] = numba;
            }

            bool backwards = bounds[0] > bounds[1];
            int incrementor = backwards ? -1 : 1;

            if (args.Length == 5)
            {
                evalRes = args[4].Evaluate(context);

                if (!evalRes.TruthValue)
                    return new EvaluationResult(6, string.Format("Evaluation of argument #5 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

                int numba;

                if (!int.TryParse(evalRes.Output, out numba))
                    return new EvaluationResult(7, "The given argument is not an integer.");
                if (numba < 1)  //  We will use 1-based indexes here.
                    return new EvaluationResult(8, "The given argument must be greater than 0 (zero).");

                incrementor = numba;
            }

            int i = bounds[1] - bounds[0];

            switch (toggle)
            {
                case null:
                    var ous1 = new List<string>((int)Math.Ceiling(Math.Abs((double)(bounds[1] - bounds[0]) / incrementor)));

                    for (int j = bounds[0]; backwards ? (j >= bounds[1]) : (j <= bounds[1]); j += incrementor)
                    {
                        evalRes = args[3].Evaluate(context.WithLocal(iteratorName, j.ToString()));

                        if (!evalRes.TruthValue)
                            return new EvaluationResult(9, string.Format("Evaluation #{0} of the given expression (argument #2) failed with non-zero status: {0} {1}", j + 1, evalRes.Status, evalRes.Output));

                        ous1.Add(evalRes.Output);
                    }

                    return new EvaluationResult(0, string.Concat(ous1));

                case true:
                    var ous2 = new List<string>((int)Math.Ceiling(Math.Abs((double)(bounds[1] - bounds[0]) / incrementor)));

                    for (int j = bounds[0]; backwards ? (j >= bounds[1]) : (j <= bounds[1]); j += incrementor)
                        ous2.Add(args[3].Evaluate(context.WithLocal(iteratorName, j.ToString())).Output);

                    return new EvaluationResult(0, string.Concat(ous2));

                default:
                    for (int j = bounds[0]; backwards ? (j >= bounds[1]) : (j <= bounds[1]); j += incrementor)
                        args[3].Evaluate(context.WithLocal(iteratorName, j.ToString()));

                    return new EvaluationResult(0, string.Format("Given expression has been indiscriminately evaluated {0} times.", i));
            }
        }

        [MethodCommandData(abstr: "Retrieves a local value from the evaluation context.")]
        static EvaluationResult local(bool? toggle, EvaluationContext context, Expression[] args)
        {
            switch (toggle)
            {
                case null:
                    if (args.Length != 1)
                        return new EvaluationResult(1, "'local' must receive one argument: a name.");
                    break;

                case false:
                    if (args.Length != 1)
                        return new EvaluationResult(1, "'-local' must receive one argument: a name.");
                    break;

                default:
                    if (args.Length != 2)
                        return new EvaluationResult(1, "'+local' must receive two arguments: a name and a value.");
                    break;
            }

            var evalRes = args[0].Evaluate(context);

            if (!evalRes.TruthValue)
                return new EvaluationResult(2, string.Format("Evaluation of argument #1 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));
            
            switch (toggle)
            {
                case null:
                    if (context.Locals.Count == 0)
                        return new EvaluationResult(3, "There are no local variables in the execution context.");

                    string res = null;

                    if (context.Locals.TryGetValue(evalRes.Output, out res))
                        return new EvaluationResult(0, res);
                    else
                        return new EvaluationResult(4, "There is no local variable with the specified name.");

                case false:
                    if (context.Locals.Count == 0)
                        return new EvaluationResult(3, "There are no local variables in the execution context.");

                    if (context.Locals.ContainsKey(evalRes.Output))
                    {
                        context.Locals.Remove(evalRes.Output);

                        return new EvaluationResult(0, "Local removed.");
                    }
                    else
                        return new EvaluationResult(4, "There is no local variable with the specified name.");

                default:
                    var name = evalRes.Output;

                    evalRes = args[1].Evaluate(context);

                    if (!evalRes.TruthValue)
                        return new EvaluationResult(5, string.Format("Evaluation of argument #2 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

                    context.Locals[name] = evalRes.Output;

                    return new EvaluationResult(0, string.Format("{0} = {1}", name, evalRes.Output));
            }
        }

        [MethodCommandData(abstr: "Evaluates an expression with some local values.")]
        static EvaluationResult with(bool? toggle, EvaluationContext context, Expression[] args)
        {
            if (args.Length < 3 || args.Length % 2 != 1)
                return new EvaluationResult(1, "'with' must receive at least one local definition(a name followed by a value) and an expression.");

            List<KeyValuePair<string, string>> locals = new List<KeyValuePair<string, string>>(args.Length / 2);

            EvaluationResult evalRes;

            for (int i = 0; i < args.Length - 1; i += 2)
            {
                string name = null, value = null;

                evalRes = args[i].Evaluate(context);

                if (!evalRes.TruthValue)
                    return new EvaluationResult(2, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                name = evalRes.Output;

                evalRes = args[i + 1].Evaluate(context);

                if (!evalRes.TruthValue)
                    return new EvaluationResult(3, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 2, evalRes.Status, evalRes.Output));

                value = evalRes.Output;

                locals.Add(new KeyValuePair<string, string>(name, value));
            }

            return args[args.Length - 1].Evaluate(context.WithLocal(locals));
        }

        #region User-defined commands and aliases

        [MethodCommandData(abstr: "Creates a named alias for an expression.")]
        static EvaluationResult alias(bool? toggle, EvaluationContext context, Expression[] args)
        {
            switch (toggle)
            {
                case null:
                    if (args.Length != 2)
                        return new EvaluationResult(1, "'alias' must receive two arguments: a name and an expression.");
                    break;

                case true:
                    if (args.Length != 2)
                        return new EvaluationResult(1, "'+alias' must receive two arguments: a name and an expression.");
                    break;

                default:
                    if (args.Length != 1)
                        return new EvaluationResult(1, "'-alias' must receive one argument: a name.");
                    break;
            }

            var evalRes = args[0].Evaluate(context);

            if (!evalRes.TruthValue)
                return new EvaluationResult(2, string.Format("Evaluation of argument #1 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

            switch (toggle)
            {
                case null:
                    var als1 = new Alias(evalRes.Output, args[1].ToString());

                    if (context.Host.RegisterCommand(als1, false, false))
                        return EvaluationResult.EmptyPositive;
                    else
                        return new EvaluationResult(3, string.Format("A command already exists with the given name: {0}", evalRes.Output));

                case true:
                    var als2 = new Alias(evalRes.Output, args[1].ToString());

                    if (context.Host.RegisterCommand(als2, true, true))
                        return EvaluationResult.EmptyPositive;
                    else
                        return new EvaluationResult(3, string.Format("A command already exists with the given name, and it is not an alias: {0}", evalRes.Output));

                default:
                    if (context.Host.RemoveCommand(evalRes.Output))
                        return EvaluationResult.EmptyPositive;
                    else
                        return new EvaluationResult(3, string.Format("A command already exists with the given name: {0}", evalRes.Output));
            }
        }

        [MethodCommandData(abstr: "Creates a named command which can accept arguments.")]
        static EvaluationResult define(bool? toggle, EvaluationContext context, Expression[] args)
        {
            switch (toggle)
            {
                case null:
                    if (args.Length != 2)
                        return new EvaluationResult(1, "'define' must receive two arguments: a name and an expression.");
                    break;

                case true:
                    if (args.Length != 2)
                        return new EvaluationResult(1, "'+define' must receive two arguments: a name and an expression.");
                    break;

                default:
                    if (args.Length != 1)
                        return new EvaluationResult(1, "'-define' must receive one argument: a name.");
                    break;
            }

            var evalRes = args[0].Evaluate(context);

            if (!evalRes.TruthValue)
                return new EvaluationResult(2, string.Format("Evaluation of argument #1 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

            switch (toggle)
            {
                case null:
                    var als1 = new UserCommand(evalRes.Output, args[1]);

                    if (context.Host.RegisterCommand(als1, false, false))
                        return EvaluationResult.EmptyPositive;
                    else
                        return new EvaluationResult(3, string.Format("A command already exists with the given name: {0}", evalRes.Output));

                case true:
                    var als2 = new UserCommand(evalRes.Output, args[1]);

                    if (context.Host.RegisterCommand(als2, true, true))
                        return EvaluationResult.EmptyPositive;
                    else
                        return new EvaluationResult(3, string.Format("A command already exists with the given name: {0}", evalRes.Output));

                default:
                    if (context.Host.RemoveCommand(evalRes.Output))
                        return EvaluationResult.EmptyPositive;
                    else
                        return new EvaluationResult(3, string.Format("A command already exists with the given name: {0}", evalRes.Output));
            }

        }

        [MethodCommandData(abstr: "Retrieves a user argument from the evaluation context for a user-defined function.")]
        static EvaluationResult arg(bool? toggle, EvaluationContext context, Expression[] args)
        {
            if (args.Length != 1)
                return new EvaluationResult(1, "'arg' must receive one argument: an index.");

            var evalRes = args[0].Evaluate(context);

            if (!evalRes.TruthValue)
                return new EvaluationResult(2, string.Format("Evaluation of argument #1 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

            if (context.UserArguments == null || context.UserArguments.Count == 0)
                return new EvaluationResult(3, "There are no user arguments in the execution context.");

            int i;

            if (!int.TryParse(evalRes.Output, out i))
                return new EvaluationResult(4, "The given argument is not an integer.");
            if (i < 1)  //  We will use 1-based indexes here.
                return new EvaluationResult(5, "The given argument must be greater than 0 (zero).");
            if (i > context.UserArguments.Count)
                return new EvaluationResult(6, "There is no user argument at index #" + i);

            return context.UserArguments[i - 1].Evaluate(context);
        }

        [MethodCommandData(abstr: "Retrieves the number of user arguments from the execution context for a user-defined function.")]
        static EvaluationResult argc(bool? toggle, EvaluationContext context, Expression[] args)
        {
            if (args.Length != 0)
                return new EvaluationResult(1, "'argc' must receive no arguments.");

            if (context.UserArguments == null)
                return new EvaluationResult(2, "There are no user arguments in the execution context.");

            return new EvaluationResult(0, context.UserArguments.Count.ToString());
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

        [MethodCommandData(abstr: "Generates a random number.")]
        static EvaluationResult rand(bool? toggle, EvaluationContext context, Expression[] args)
        {
            if (rnd == null)
                rnd = new Random();

            EvaluationResult evalRes = null;

            switch (args.Length)
            {
                case 0:
                    return new EvaluationResult(0, rnd.NextDouble().ToString());

                case 1:
                    evalRes = args[0].Evaluate(context);

                    if (!evalRes.TruthValue)
                        return new EvaluationResult(2, string.Format("Evaluation of argument #1 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

                    uint i;

                    if (uint.TryParse(evalRes.Output, out i))
                        return new EvaluationResult(0, rnd.Next((int)i).ToString());
                    else
                        return new EvaluationResult(2, string.Format("Failed to convert output of argument #{0} to an unsigned integer: {1}", i + 1, evalRes.Output));

                case 2:
                    var bounds = new int[2];

                    for (int j = 0; j < 2; j++)
                    {
                        evalRes = args[j].Evaluate(context);

                        if (!evalRes.TruthValue)
                            return new EvaluationResult(2, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", j + 1, evalRes.Status, evalRes.Output));

                        int b;

                        if (int.TryParse(evalRes.Output, out b))
                            bounds[j] = b;
                        else
                            return new EvaluationResult(2, string.Format("Failed to convert output of argument #{0} to an unsigned integer: {1}", j + 1, evalRes.Output));
                    }

                    return new EvaluationResult(0, rnd.Next(bounds[0], bounds[1]).ToString());

                default:
                    return new EvaluationResult(1, "Too many arguments given to 'rand'.");
            }
        }

        [MethodCommandData(category: "Mathematics", abstr: "Adds the given arguments together, converting input to numbers.")]
        static EvaluationResult add(bool? toggle, EvaluationContext context, Expression[] args)
        {
            decimal res = 0m;

            if (args.Length < 2)
                return new EvaluationResult(1, "Please provide at least two arguments to 'add'.");

            for (int i = 0; i < args.Length; i++)
            {
                var evalRes = args[i].Evaluate(context);

                if (evalRes.Status != 0)
                    return new EvaluationResult(2, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                decimal d;

                if (decimal.TryParse(evalRes.Output, out d))
                    res += d;
                else
                    return new EvaluationResult(3, string.Format("Failed to convert output of argument #{0} to a number: {1}", i + 1, evalRes.Output));
            }

            return new EvaluationResult(0, res.ToString());
        }

        [MethodCommandData(category: "Mathematics", abstr: "Subtracts from the first argument the values of the other arguments, converting input to numbers.")]
        static EvaluationResult sub(bool? toggle, EvaluationContext context, Expression[] args)
        {
            decimal res = 0m;

            if (args.Length < 2)
                return new EvaluationResult(1, "Please provide at least two arguments to 'sub'.");

            for (int i = 0; i < args.Length; i++)
            {
                var evalRes = args[i].Evaluate(context);

                if (evalRes.Status != 0)
                    return new EvaluationResult(2, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                decimal d;

                if (decimal.TryParse(evalRes.Output, out d))
                {
                    if (i == 0)
                        res = d;
                    else
                        res -= d;
                }
                else
                    return new EvaluationResult(3, string.Format("Failed to convert output of argument #{0} to a number: {1}", i + 1, evalRes.Output));
            }

            return new EvaluationResult(0, res.ToString());
        }

        [MethodCommandData(category: "Mathematics", abstr: "Multiplies the given arguments together, converting input to numbers.")]
        static EvaluationResult mul(bool? toggle, EvaluationContext context, Expression[] args)
        {
            decimal res = 1m;

            if (args.Length < 2)
                return new EvaluationResult(1, "Please provide at least two arguments to 'mul'.");

            for (int i = 0; i < args.Length; i++)
            {
                var evalRes = args[i].Evaluate(context);

                if (evalRes.Status != 0)
                    return new EvaluationResult(2, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                decimal d;

                if (decimal.TryParse(evalRes.Output, out d))
                    res *= d;
                else
                    return new EvaluationResult(3, string.Format("Failed to convert output of argument #{0} to a number: {1}", i + 1, evalRes.Output));
            }

            return new EvaluationResult(0, res.ToString());
        }

        [MethodCommandData(category: "Mathematics", abstr: "Divides the first argument by the values of the other arguments, converting input to numbers.")]
        static EvaluationResult div(bool? toggle, EvaluationContext context, Expression[] args)
        {
            decimal res = 1m;

            if (args.Length < 2)
                return new EvaluationResult(1, "Please provide at least two arguments to 'div'.");

            for (int i = 0; i < args.Length; i++)
            {
                var evalRes = args[i].Evaluate(context);

                if (evalRes.Status != 0)
                    return new EvaluationResult(2, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                decimal d;

                if (decimal.TryParse(evalRes.Output, out d))
                {
                    if (i == 0)
                        res = d;
                    else
                        res /= d;
                }
                else
                    return new EvaluationResult(3, string.Format("Failed to convert output of argument #{0} to a number: {1}", i + 1, evalRes.Output));
            }
            
            return new EvaluationResult(0, res.ToString());
        }

        [MethodCommandData(category: "Mathematics", abstr: "Outputs the rounded value of the given input number..")]
        static EvaluationResult round(bool? toggle, EvaluationContext context, Expression[] args)
        {
            if (args.Length != 1 && args.Length != 2)
                return new EvaluationResult(1, "'floor' must receive one or two argument, both numbers.");

            var evalRes = args[0].Evaluate(context);

            if (!evalRes.TruthValue)
                return new EvaluationResult(2, string.Format("Evaluation of argument #1 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

            int digits = 0;

            if (args.Length > 1)
            {
                evalRes = args[1].Evaluate(context);

                if (!evalRes.TruthValue)
                    return new EvaluationResult(3, string.Format("Evaluation of argument #2 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

                if (!int.TryParse(evalRes.Output, out digits))
                    return new EvaluationResult(4, "The given second argument is not a number.");
                if (digits < 0 || digits > 28)
                    return new EvaluationResult(5, "The number of digits (second argument) must be between 0 and 28 inclusively.");
            }

            decimal d;

            if (!decimal.TryParse(evalRes.Output, out d))
                return new EvaluationResult(6, "The given first argument is not a number.");

            return new EvaluationResult(0, Math.Round(d, digits).ToString());
        }

        [MethodCommandData(category: "Comparison", abstr: "Determines whether all given arguments are equal.")]
        static EvaluationResult eq(bool? toggle, EvaluationContext context, Expression[] args)
        {
            bool restrictive = !toggle.HasValue || !toggle.Value;

            if (args.Length < 2)
                return new EvaluationResult(1, "Please provide at least two arguments to 'eq'.");

            switch (toggle)
            {
                case null:
                case true:
                    string cur = null;

                    for (int i = 0; i < args.Length; i++)
                    {
                        var evalRes = args[i].Evaluate(context);

                        if (restrictive && evalRes.Status != 0)
                            return new EvaluationResult(2, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                        if (cur == null)
                            cur = evalRes.Output;
                        else
                            if (cur != evalRes.Output)
                                return new EvaluationResult(3, string.Format("Argument #{0}'s output is not equal to its predecessors'.", i + 1));
                    }

                    return new EvaluationResult(0, cur);

                default:
                    bool? st = null;

                    for (int i = 0; i < args.Length; i++)
                    {
                        var evalRes = args[i].Evaluate(context);

                        if (st == null)
                            st = evalRes.TruthValue;
                        else
                            if (st != evalRes.TruthValue)
                                return new EvaluationResult(4, string.Format("Argument #{0}'s truth value is not equal to its predecessors'.", i + 1));
                    }

                    return new EvaluationResult(0, "All arguments have identical truth value");
            }
        }

        [MethodCommandData(category: "Comparison", abstr: "Determines whether the two given arguments.")]
        static EvaluationResult neq(bool? toggle, EvaluationContext context, Expression[] args)
        {
            bool restrictive = !toggle.HasValue || !toggle.Value;

            if (args.Length != 2)
                return new EvaluationResult(1, "Please provide two arguments to 'neq'.");

            switch (toggle)
            {
                case null:
                case true:
                    string[] ou = new string[2];

                    for (int i = 0; i < 2; i++)
                    {
                        var evalRes = args[i].Evaluate(context);

                        if (restrictive && evalRes.Status != 0)
                            return new EvaluationResult(2, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", i + 1, evalRes.Status, evalRes.Output));

                        ou[i] = args[i].Evaluate(context).Output;
                    }
                    
                    if (ou[0] == ou[1])
                        return new EvaluationResult(4, string.Format("{0} = {1}", ou[0], ou[1]));
                    else
                        return new EvaluationResult(0, string.Format("{0} ≠ {1}", ou[0], ou[1]));

                default:
                    bool[] st = new bool[2];

                    for (int i = 0; i < 2; i++)
                        st[i] = args[i].Evaluate(context).TruthValue;

                    if (st[0] == st[1])
                        return new EvaluationResult(4, string.Format("{0} = {1}", st[0], st[1]));
                    else
                        return new EvaluationResult(0, string.Format("{0} ≠ {1}", st[0], st[1]));
            }
        }

        #endregion

        #region Strings

        [MethodCommandData(abstr: "Extracts a substring from a string.")]
        static EvaluationResult subs(bool? toggle, EvaluationContext context, Expression[] args)
        {
            if (args.Length != 3)
                return new EvaluationResult(1, "'subs' must receive 3 arguments: start index, length and a string.");

            EvaluationResult evalRes;

            var bounds = new int[2];

            for (int j = 0; j < 2; j++)
            {
                evalRes = args[j].Evaluate(context);

                if (!evalRes.TruthValue)
                    return new EvaluationResult(2, string.Format("Evaluation of argument #{0} returned non-zero status: {1} ({2})", j + 1, evalRes.Status, evalRes.Output));

                int numba;

                if (!int.TryParse(evalRes.Output, out numba))
                    return new EvaluationResult(3, "The given argument is not an integer.");
                if (numba < 0)  //  We will use 1-based indexes here.
                    return new EvaluationResult(4, "The given argument must be greater than or equal to 0 (zero).");

                bounds[j] = numba;
            }

            evalRes = args[2].Evaluate(context);

            if (!evalRes.TruthValue)
                return new EvaluationResult(5, string.Format("Evaluation of argument #3 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

            if (bounds[0] > evalRes.Output.Length)
                return new EvaluationResult(6, string.Format("Start index is {0}, but string only contains {1} characters.", bounds[0], evalRes.Output.Length));

            if (bounds[0] + bounds[1] > evalRes.Output.Length)
                return new EvaluationResult(7, string.Format("Resulted end index is {0}, but string only contains {1} characters.", bounds[0] + bounds[1], evalRes.Output.Length));

            return new EvaluationResult(0, evalRes.Output.Substring(bounds[0], bounds[1]));
        }

        #endregion

        #region Variables

        [MethodCommandData(abstr: "Retrieves the value of a variable from the host.")]
        static EvaluationResult cvar(bool? toggle, EvaluationContext context, Expression[] args)
        {
            switch (toggle)
            {
                case null:
                    if (args.Length != 1)
                        return new EvaluationResult(1, "'cvar' must receive one argument: a name.");
                    break;

                case false:
                    return new EvaluationResult(4, "Operation not supported.");

                    /*if (args.Length != 1)
                        return new EvaluationResult(1, "'-cvar' must receive one argument: a name.");
                    break;*/

                default:
                    if (args.Length != 2)
                        return new EvaluationResult(1, "'+cvar' must receive two arguments: a name and a value.");
                    break;
            }

            var evalRes = args[0].Evaluate(context);

            if (!evalRes.TruthValue)
                return new EvaluationResult(2, string.Format("Evaluation of argument #1 returned non-zero status: {0} ({1})", evalRes.Status, evalRes.Output));

            switch (toggle)
            {
                case null:
                    string res1 = context.Host.GetVariable(evalRes.Output).StringValue;

                    if (res1 != null)
                        return new EvaluationResult(0, res1);
                    else
                        return new EvaluationResult(4, "There is no command variable with the specified name.");

                /*case false:
                    if (context.Locals.ContainsKey(evalRes.Output))
                    {
                        context.Locals.Remove(evalRes.Output);

                        return new EvaluationResult(0, "Local removed.");
                    }
                    else
                        return new EvaluationResult(4, "There is no local variable with the specified name.");*/

                default:
                    var variable = context.Host.GetVariable(evalRes.Output);

                    if (variable != null)
                    {
                        return variable.ChangeValue(context, args[1]);
                    }
                    else
                        return new EvaluationResult(4, "There is no command variable with the specified name.");
            }
        }

        #endregion
    }
}
