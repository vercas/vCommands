using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using vCommands;
using vCommands.Parsing;
using vCommands.Parsing.Expressions;

namespace UnitTestProject1
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void Simple_Command()
        {
            var exp = Parser.Parse(Parser.Tokenize("asd"));

            Assert.IsInstanceOfType(exp, typeof(CommandInvocationExpression));

            var cmd = exp as CommandInvocationExpression;

            Assert.IsNull(cmd.Toggle);
            Assert.AreEqual("asd", cmd.CommandName);
            Assert.AreEqual(0, cmd.Arguments.Count);
        }

        [TestMethod]
        public void Simple_Command_With_Arguments()
        {
            var exp = Parser.Parse(Parser.Tokenize("asd f g h"));

            Assert.IsInstanceOfType(exp, typeof(CommandInvocationExpression));

            var cmd = exp as CommandInvocationExpression;

            Assert.IsNull(cmd.Toggle);
            Assert.AreEqual("asd", cmd.CommandName);

            Assert.AreEqual(3, cmd.Arguments.Count);

            var vals = new string[] { "f", "g", "h" };

            for(int i = 0; i < vals.Length; i++)
            {
                Assert.IsInstanceOfType(cmd.Arguments[i], typeof(ConstantExpression));
                Assert.AreEqual(vals[i], ((ConstantExpression)cmd.Arguments[i]).Value);
            }
        }

        [TestMethod]
        public void Toggled_Command()
        {
            var exp = Parser.Parse(Parser.Tokenize("-asd"));

            Assert.IsInstanceOfType(exp, typeof(CommandInvocationExpression));

            var cmd = exp as CommandInvocationExpression;

            Assert.AreEqual(false, cmd.Toggle);
            Assert.AreEqual("asd", cmd.CommandName);

            Assert.AreEqual(0, cmd.Arguments.Count);
        }

        [TestMethod]
        public void Toggled_Command_With_Arguments()
        {
            var exp = Parser.Parse(Parser.Tokenize("+asd f g h"));

            Assert.IsInstanceOfType(exp, typeof(CommandInvocationExpression));

            var cmd = exp as CommandInvocationExpression;

            Assert.AreEqual(true, cmd.Toggle);
            Assert.AreEqual("asd", cmd.CommandName);

            Assert.AreEqual(3, cmd.Arguments.Count);

            var vals = new string[] { "f", "g", "h" };

            for (int i = 0; i < vals.Length; i++)
            {
                Assert.IsInstanceOfType(cmd.Arguments[i], typeof(ConstantExpression));
                Assert.AreEqual(vals[i], ((ConstantExpression)cmd.Arguments[i]).Value);
            }
        }

        [TestMethod]
        public void Toggled_Command_Series()
        {
            var exp2 = Parser.Parse(Parser.Tokenize("-asd; +asd; -asd; +asd; -asd; +asd; -asd; +asd; -asd; +asd; -asd; +asd"));

            Assert.IsInstanceOfType(exp2, typeof(SeriesExpression));

            var srs = exp2 as SeriesExpression;

            Assert.AreEqual(12, srs.Subexpressions.Count);

            for (int i = 0; i < srs.Subexpressions.Count; i++)
            {
                var exp = srs.Subexpressions[i];

                Assert.IsInstanceOfType(exp, typeof(CommandInvocationExpression));

                var cmd = exp as CommandInvocationExpression;

                Assert.AreEqual(i % 2 == 1, cmd.Toggle);
                Assert.AreEqual("asd", cmd.CommandName);

                Assert.AreEqual(0, cmd.Arguments.Count);
            }
        }

        [TestMethod]
        public void Toggled_Command_With_Arguments_Series()
        {
            var vals = new string[] { "f", "g", "h" };

            var exp2 = Parser.Parse(Parser.Tokenize("-asd f g h; +asd \\f \\g \\h; -asd f g h; +asd \\f \\g \\h; -asd f g h; +asd \\f \\g \\h; -asd f g h; +asd \\f \\g \\h; -asd f g h; +asd \\f \\g \\h; -asd f g h; +asd \\f \\g \\h"));

            Assert.IsInstanceOfType(exp2, typeof(SeriesExpression));

            var srs = exp2 as SeriesExpression;

            Assert.AreEqual(12, srs.Subexpressions.Count);

            for (int i = 0; i < srs.Subexpressions.Count; i++)
            {
                var exp = srs.Subexpressions[i];

                Assert.IsInstanceOfType(exp, typeof(CommandInvocationExpression));

                var cmd = exp as CommandInvocationExpression;

                Assert.AreEqual(i % 2 == 1, cmd.Toggle);
                Assert.AreEqual("asd", cmd.CommandName);

                Assert.AreEqual(3, cmd.Arguments.Count);

                for (int j = 0; i < vals.Length; i++)
                {
                    Assert.IsInstanceOfType(cmd.Arguments[j], typeof(ConstantExpression));
                    Assert.AreEqual(vals[j], ((ConstantExpression)cmd.Arguments[j]).Value);
                }
            }
        }

        [TestMethod]
        public void Simple_Conditional()
        {
            //  Checking conditional;

            var exp2 = Parser.Parse(Parser.Tokenize("blah ? asd f g h"));

            Assert.IsInstanceOfType(exp2, typeof(ConditionalExpression));

            var cond = exp2 as ConditionalExpression;

            Assert.AreEqual(true, cond.TruthValue);
            Assert.IsNotNull(cond.PrimaryAction);
            Assert.IsNull(cond.SecondaryAction);
            
            //  Checking condition;

            var exp1 = cond.Condition;

            Assert.IsInstanceOfType(exp1, typeof(CommandInvocationExpression));

            var cmd = exp1 as CommandInvocationExpression;

            Assert.IsNull(cmd.Toggle);
            Assert.AreEqual("blah", cmd.CommandName);
            Assert.AreEqual(0, cmd.Arguments.Count);

            //  Checking primary action.

            var exp = cond.PrimaryAction;

            Assert.IsInstanceOfType(exp, typeof(CommandInvocationExpression));

            cmd = exp as CommandInvocationExpression;

            Assert.IsNull(cmd.Toggle);
            Assert.AreEqual("asd", cmd.CommandName);

            Assert.AreEqual(3, cmd.Arguments.Count);

            var vals = new string[] { "f", "g", "h" };

            for (int i = 0; i < vals.Length; i++)
            {
                Assert.IsInstanceOfType(cmd.Arguments[i], typeof(ConstantExpression));
                Assert.AreEqual(vals[i], ((ConstantExpression)cmd.Arguments[i]).Value);
            }
        }

        [TestMethod]
        public void Chained_Conditional()
        {
            Assert.AreEqual("blah ? blah ? blah ? blah ? blah ? blah ? blah ? blah", Parser.Parse(Parser.Tokenize("blah ? blah ? blah ? blah ? blah ? blah ? blah ? blah")).ToString());
        }

        [TestMethod]
        public void Series_Conditional()
        {
            Assert.AreEqual("blah ? asd; asd ? blah; asd ? blah; asd ? blah; asd ? blah", Parser.Parse(Parser.Tokenize("blah ? asd; asd ? blah; asd ? blah; asd ? blah; asd ? blah")).ToString());
        }

        [TestMethod]
        public void Series_Chained_Conditional()
        {
            Assert.AreEqual("blah ? blah ? blah ? blah ? blah ? blah ? blah ? blah; blah ? blah ? blah ? blah ? blah ? blah ? blah ? blah; blah ? blah ? blah ? blah ? blah ? blah ? blah ? blah; blah ? blah ? blah ? blah ? blah ? blah ? blah ? blah",
                Parser.Parse(Parser.Tokenize("blah ? blah ? blah ? blah ? blah ? blah ? blah ? blah; blah ? blah ? blah ? blah ? blah ? blah ? blah ? blah; blah ? blah ? blah ? blah ? blah ? blah ? blah ? blah; blah ? blah ? blah ? blah ? blah ? blah ? blah ? blah")).ToString());
        }

        [TestMethod]
        public void Full_Conditional()
        {
            Assert.AreEqual("a ? b : c", Parser.Parse(Parser.Tokenize("a ? b : c")).ToString());
        }

        [TestMethod]
        public void Series_Full_Conditionals()
        {
            Assert.AreEqual("blah ? asd : fgh; asd ? blah : fgh; asd ? blah : fgh; asd ? blah : fgh; asd ? blah : fgh", Parser.Parse(Parser.Tokenize("blah ? asd : fgh; asd ? blah : fgh; asd ? blah : fgh; asd ? blah : fgh; asd ? blah : fgh")).ToString());
        }

        [TestMethod]
        public void Simple_Compound_Argument()
        {
            Assert.AreEqual("a [b]", Parser.Parse(Parser.Tokenize("a [b]")).ToString());
        }

        [TestMethod]
        public void Simple_Compound_Arguments()
        {
            Assert.AreEqual("a [b] [c] [d]", Parser.Parse(Parser.Tokenize("a [b] [c] [d]")).ToString());
        }

        [TestMethod]
        public void Cascaded_Compound_Argument()
        {
            Assert.AreEqual("a [b [c [d [e [f [g [h [i]]]]]]]]", Parser.Parse(Parser.Tokenize("a [b [c [d [e [f [g [h [i]]]]]]]]")).ToString());
        }

        [TestMethod]
        public void Cascaded_Compound_Arguments()
        {
            Assert.AreEqual("a [b [c [d [e [f [g [h [i]]]] [f [g [h [i]]]]]]]]", Parser.Parse(Parser.Tokenize("a [b [c [d [e [f [g [h [i]]]] [f [g [h [i]]]]]]]]")).ToString());
        }

        [TestMethod]
        public void Almost_Badass()
        {
            Assert.AreEqual("+\"A B\" \"rataC Dtouille\" \"\" E; -\"lol maoyaong \" brah; yet \"more\\\"s;u;f\"; even [more [fricking]] [arguments] ! \"mara are mere\"",
                Parser.Parse(Parser.Tokenize("+A\\ B rataC\\ Dtouille \"\" E;-\\lol\\ mao\"yaong \" brah;yet more\\\"s\\;u\\;f ; even [more [fricking] ] [arguments] ! mara\\ are\\ mere")).ToString());
        }

        [TestMethod]
        public void Parsing_Errors()
        {
            TestForError("a; b;;c");
            TestForError("a ? b : c : d");
            TestForError("a ]");
            TestForError("a b]");
            TestForError("a [b] ]");
            TestForError("a [");
            TestForError("a [b");
            TestForError("a [ ]");
            TestForError("a [b [] ]");
        }

        #region Utilitary Methods

        private void TestForError(string command)
        {
            try
            {
                Parser.Parse(Parser.Tokenize(command));

                Assert.Fail("The following command should've errored, but it did not: {0}", command);
            }
            catch (FormatException x)
            {
                Console.WriteLine("Successfully caught format exception: {0}", x);
            }
        }

        #endregion
    }
}
