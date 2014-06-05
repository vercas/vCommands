using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using vCommands;
using vCommands.Commands;
using vCommands.Variables;

namespace Test_Application
{
    class Program
    {
        static decimal tester = 0m;

        static void Main(string[] args)
        {
            var host = new CommandHost();
            host.RegisterDefaultCommands();

            host.RegisterVariable(new Variable<decimal>("1", "A decimal test variable.", 0m));
            host.RegisterVariable(new DelegatedVariable<decimal>("2", () =>
            {
                Console.WriteLine("getting tester");
                return tester;
            }, (val) =>
            {
                Console.WriteLine("setting tester to {0}", val);
                tester = val;
            }, () =>
            {
                Console.WriteLine("getting tester string");
                return tester.ToString();
            }, (val) =>
            {
                Console.Write("setting tester string to {0}... ", val);

                decimal d;

                if (decimal.TryParse(val, out d))
                {
                    Console.WriteLine("success!");
                    tester = d;
                    return true;
                }
                else
                {
                    Console.WriteLine("fail.");
                    return false;
                }
            }, "A delegated decimal test variable."));

            host.GetCommand("cvar").Invocation += Program_Invocation;

            host.GetVariable("1").Change += Program_Change;

            string cmd = null;

            while (Read(out cmd))
            {
                EvaluationResult res;

                try
                {
                    res = host.Evaluate(cmd);

                    if (!string.IsNullOrWhiteSpace(res.Output))
                    {
                        if (!res.TruthValue)
                            Console.Write("{0}: ", res.Status);

                        Console.WriteLine(res.Output);
                    }
                    else
                    {
                        if (!res.TruthValue)
                            Console.Write("{0}: ", res.Status);

                        //  Eh, byobu bash-like.
                    }
                }
                catch (FormatException x)
                {
                    Console.WriteLine("Error: {0}", x.Message);
                }
            }

            Console.Write("Press any key to terminate... ");
            Console.ReadKey(true);
        }

        static void Program_Change(object sender, vCommands.EventArguments.VariableChangeEventArgs e)
        {
            e.Cancel = true;
            e.CancelReason = "u no set dis shiet mayun";
        }

        static void Program_Invocation(Command sender, vCommands.EventArguments.CommandInvocationEventArgs e)
        {
            //  Nuthin'.
        }

        static bool Read(out string cmd)
        {
            Console.Write("> ");

            cmd = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(cmd))
                return Read(out cmd);

            return cmd != "!Q";
        }
    }
}
