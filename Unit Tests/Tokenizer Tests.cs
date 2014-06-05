using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using vCommands;
using vCommands.Parsing;

namespace UnitTestProject1
{
    [TestClass]
    public class TokenizerTests
    {
        [TestMethod]
        public void A_B_C()
        {
            TestTokens(new string[]
            {
                "A B C",
                " A B C ",
                "A    B     C",
                "   A     B    C  ",
            },
            new object[]
            {
                TokenTypes.CommandName, "A",
                TokenTypes.Argument, "B",
                TokenTypes.Argument, "C",
            });
        }

        [TestMethod]
        public void Toggler_blah()
        {
            TestTokens(new string[]
            {
                "+blah",
                " +blah",
                "+blah ",
                " +blah ",
                " + blah ",
            },
            new object[]
            {
                TokenTypes.Toggler, "+",
                TokenTypes.CommandName, "blah",
            });
        }

        [TestMethod]
        public void Toggler_A_B_C()
        {
            TestTokens(new string[]
            {
                "-A B C",
                " -A B C ",
                "-A    B     C",
                "   -A     B    C  ",
                "-   A     B    C  ",
            },
            new object[]
            {
                TokenTypes.Toggler, "-",
                TokenTypes.CommandName, "A",
                TokenTypes.Argument, "B",
                TokenTypes.Argument, "C",
            });
        }

        [TestMethod]
        public void Toggler_Escaped_Command_Name()
        {
            TestTokens(new string[]
            {
                "+blah",
                "+\\blah",
                "+b\\lah",
                "+bl\\ah",
                "+bla\\h",
                "+\\b\\lah",
                "+\\bl\\ah",
                "+\\bla\\h",
                "+\\b\\l\\ah",
                "+\\b\\la\\h",
                "+\\bl\\a\\h",
                "+\\b\\l\\a\\h",
            },
            new object[]
            {
                TokenTypes.Toggler, "+",
                TokenTypes.CommandName, "blah",
            });
        }

        [TestMethod]
        public void Escaped_Toggler()
        {
            TestTokens(new string[]
            {
                "\\+blah",
                " \\+blah",
                "\\+blah ",
                " \\+blah ",
            },
            new object[]
            {
                TokenTypes.CommandName, "+blah",
            });
        }

        [TestMethod]
        public void Toggler_Escaped_Toggler()
        {
            TestTokens(new string[]
            {
                "-\\+blah",
                "- \\+blah",
                "-\\+blah ",
                "- \\+blah ",
                " -\\+blah",
                " -\\+blah ",
            },
            new object[]
            {
                TokenTypes.Toggler, "-",
                TokenTypes.CommandName, "+blah",
            });
        }

        [TestMethod]
        public void Toggler_Escaped_Toggler_Twice_Separated()
        {
            TestTokens(new string[]
            {
                "-\\+asd;+\\-fgh m\\ e\\ h",
                " -\\+asd;+\\-fgh m\\ e\\ h",
                "- \\+asd;+\\-fgh m\\ e\\ h",
                " -\\+asd;+\\-fgh m\\ e\\ h ",
                "- \\+asd;+\\-fgh m\\ e\\ h ",
                
                "-\\+asd ;+\\-fgh m\\ e\\ h",
                " -\\+asd ;+\\-fgh m\\ e\\ h",
                "- \\+asd ;+\\-fgh m\\ e\\ h",
                " -\\+asd ;+\\-fgh m\\ e\\ h ",
                "- \\+asd ;+\\-fgh m\\ e\\ h ",
                
                "-\\+asd; +\\-fgh m\\ e\\ h",
                " -\\+asd; +\\-fgh m\\ e\\ h",
                "- \\+asd; +\\-fgh m\\ e\\ h",
                " -\\+asd; +\\-fgh m\\ e\\ h ",
                "- \\+asd; +\\-fgh m\\ e\\ h ",
                
                "-\\+asd ; +\\-fgh m\\ e\\ h",
                " -\\+asd ; +\\-fgh m\\ e\\ h",
                "- \\+asd ; +\\-fgh m\\ e\\ h",
                " -\\+asd ; +\\-fgh m\\ e\\ h ",
                "- \\+asd ; +\\-fgh m\\ e\\ h ",
                
                "-\\+asd;+ \\-fgh m\\ e\\ h",
                " -\\+asd;+ \\-fgh m\\ e\\ h",
                "- \\+asd;+ \\-fgh m\\ e\\ h",
                " -\\+asd;+ \\-fgh m\\ e\\ h ",
                "- \\+asd;+ \\-fgh m\\ e\\ h ",
                
                "-\\+asd ;+ \\-fgh m\\ e\\ h",
                " -\\+asd ;+ \\-fgh m\\ e\\ h",
                "- \\+asd ;+ \\-fgh m\\ e\\ h",
                " -\\+asd ;+ \\-fgh m\\ e\\ h ",
                "- \\+asd ;+ \\-fgh m\\ e\\ h ",
                
                "-\\+asd; + \\-fgh m\\ e\\ h",
                " -\\+asd; + \\-fgh m\\ e\\ h",
                "- \\+asd; + \\-fgh m\\ e\\ h",
                " -\\+asd; + \\-fgh m\\ e\\ h ",
                "- \\+asd; + \\-fgh m\\ e\\ h ",
                
                "-\\+asd ; + \\-fgh m\\ e\\ h",
                " -\\+asd ; + \\-fgh m\\ e\\ h",
                "- \\+asd ; + \\-fgh m\\ e\\ h",
                " -\\+asd ; + \\-fgh m\\ e\\ h ",
                "- \\+asd ; + \\-fgh m\\ e\\ h ",
            },
            new object[]
            {
                TokenTypes.Toggler, "-",
                TokenTypes.CommandName, "+asd",
                TokenTypes.Separator, ";",
                TokenTypes.Toggler, "+",
                TokenTypes.CommandName, "-fgh",
                TokenTypes.Argument, "m e h",
            });
        }

        [TestMethod]
        public void Simple_String_Argument()
        {
            TestTokens(new string[]
            {
                "A \"B C\"",
                " A \"B C\"",
                "A \"B C\" ",
                " A \"B C\" ",
                "      A        \"B C\"        ",
            },
            new object[]
            {
                TokenTypes.CommandName, "A",
                TokenTypes.Argument, "B C",
            });
        }

        [TestMethod]
        public void Simple_String_Command()
        {
            TestTokens(new string[]
            {
                "\"A B\" C",
                " \"A B\" C",
                "\"A B\" C ",
                " \"A B\" C ",
                "      \"A B\"      C       ",
            },
            new object[]
            {
                TokenTypes.CommandName, "A B",
                TokenTypes.Argument, "C",
            });
        }

        [TestMethod]
        public void Toggler_String_Command()
        {
            TestTokens(new string[]
            {
                "+\"A B\" C",
                " +\"A B\" C",
                "+\"A B\" C ",
                " +\"A B\" C ",
                "      +\"A B\"      C       ",
                "+ \"A B\" C",
                "+\"A B\" C ",
                "+      \"A B\"      C       ",
            },
               new object[]
            {
                TokenTypes.Toggler, "+",
                TokenTypes.CommandName, "A B",
                TokenTypes.Argument, "C",
            });
        }

        [TestMethod]
        public void Toggler_String_Command_String_Arguments()
        {
            TestTokens(new string[]
            {
                "+\"A B\" \"C D\" \"\" E",
                " +\"A B\" \"C D\" \"\" E",
                "+\"A B\" \"C D\" \"\" E ",
                " +\"A B\" \"C D\" \"\" E ",
                "      +\"A B\"        \"C D\"      \"\"    E    ",
            },
               new object[]
            {
                TokenTypes.Toggler, "+",
                TokenTypes.CommandName, "A B",
                TokenTypes.Argument, "C D",
                TokenTypes.Argument, "",
                TokenTypes.Argument, "E",
            });
        }

        [TestMethod]
        public void Toggler_String_n_Escaped_Command_n_Arguments()
        {
            TestTokens(new string[]
            {
                "+\"A B\" rata\"C D\"touille \"\" E",
                " +\"A B\" rata\"C D\"touille \"\" E",
                "+\"A B\" rata\"C D\"touille \"\" E ",
                " +\"A B\" rata\"C D\"touille \"\" E ",
                "     +  \"A B\"     rata\"C D\"touille      \"\"       E        ",
                
                "+A\\ B rataC\\ Dtouille \"\" E",
                " +A\\ B rataC\\ Dtouille \"\" E",
                "+A\\ B rataC\\ Dtouille \"\" E ",
                " +A\\ B rataC\\ Dtouille \"\" E ",
                "    +   A\\ B     rataC\\ Dtouille      \"\"    E    ",
            },
               new object[]
            {
                TokenTypes.Toggler, "+",
                TokenTypes.CommandName, "A B",
                TokenTypes.Argument, "rataC Dtouille",
                TokenTypes.Argument, "",
                TokenTypes.Argument, "E",
            });
        }

        [TestMethod]
        public void Trailing_Backslash()
        {
            try
            {
                Parser.Tokenize("+blah \\").ToArray();

                Assert.Fail("Command ending in backslash slipped!");
            }
            catch (FormatException x)
            {
                Console.WriteLine("Successfully caught format exception on trailing backslash: {0}", x);
            }
        }

        [TestMethod]
        public void Whitespace_Arguments()
        {
            TestTokens(new string[]
            {
                "+yada \\ \\  \\  \\ ",
                " +yada \\ \\  \\  \\ ",
                "+yada \\ \\  \\  \\  ",
                " +yada \\ \\  \\  \\  ",
                "   +   yada    \\ \\     \\     \\     ",
            },
               new object[]
            {
                TokenTypes.Toggler, "+",
                TokenTypes.CommandName, "yada",
                TokenTypes.Argument, "  ",
                TokenTypes.Argument, " ",
                TokenTypes.Argument, " ",
            });
        }

        [TestMethod]
        public void Basic_Separation()
        {
            foreach (var sep in new char[] { ';', '?', ':', '!' })
                TestTokens(new string[]
                {
                    "a" + sep + "b",
                    "a " + sep + "b",
                    "a" + sep + " b",
                    "a " + sep + " b",
                    " a                  " + sep + "                          b ",
                },
                new object[]
                {
                    TokenTypes.CommandName, "a",
                    SeparatorTypes[sep], sep.ToString(),
                    TokenTypes.CommandName, "b",
                });

            //  20 actual tests in one test method...
        }

        [TestMethod]
        public void Compound_Argument_Single()
        {
            TestTokens(new string[]
            {
                "a [b]",
                " a [b]",
                "a [b] ",
                " a [b] ",
                
                "a [ b]",
                " a [ b]",
                "a [ b] ",
                " a [ b] ",

                "a [b ]",
                " a [b ]",
                "a [b ] ",
                " a [b ] ",

                "a [ b ]",
                " a [ b ]",
                "a [ b ] ",
                " a [ b ] ",
            },
            new object[]
            {
                TokenTypes.CommandName, "a",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "b",
                TokenTypes.CompoundArgumentEnd, "]",
            });
        }

        [TestMethod]
        public void Compound_Argument_Single_Series()
        {
            TestTokens(new string[]
            {
                "a [b] [c] [d] [e] [f] [g] [h] [i]",
                " a   [ b ]   [ c ]   [ d ]   [ e ]   [ f ]   [ g ]   [ h ]   [ i ] ",
            },
            new object[]
            {
                TokenTypes.CommandName, "a",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "b",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "c",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "d",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "e",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "f",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "g",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "h",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "i",
                TokenTypes.CompoundArgumentEnd, "]",
            });
        }

        [TestMethod]
        public void Compound_Argument_Many()
        {
            TestTokens(new string[]
            {
                "a [b [c [d [e [f [g [h [i]]]]]]]]",
                " a [ b [ c [ d [ e [ f [ g [ h [ i ] ] ] ] ] ] ] ] ",
            },
            new object[]
            {
                TokenTypes.CommandName, "a",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "b",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "c",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "d",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "e",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "f",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "g",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "h",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "i",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentEnd, "]",
            });
        }

        [TestMethod]
        public void Compound_Argument_Many_Series()
        {
            TestTokens(new string[]
            {
                "a [b [c [d [e [f [g [h [i]]]] [f [g [h [i]]]]]]]]",
                " a [ b [ c [ d [ e [ f [ g [ h [ i ] ] ] ] [ f [ g [ h [ i ] ] ] ] ] ] ] ] ",
            },
            new object[]
            {
                TokenTypes.CommandName, "a",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "b",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "c",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "d",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "e",

                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "f",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "g",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "h",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "i",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentEnd, "]",

                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "f",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "g",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "h",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "i",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentEnd, "]",

                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentEnd, "]",
            });
        }

        [TestMethod]
        public void Compound_Argument_Errors()
        {
            try
            {
                Parser.Tokenize("a [b").ToArray();

                Assert.Fail("Lacking compound argument end slipped!");
            }
            catch (FormatException x)
            {
                Console.WriteLine("Successfully caught format exception on lacking compound argument end: {0}", x);
            }

            try
            {
                Parser.Tokenize("a b]").ToArray();

                Assert.Fail("Excess of compound argument end slipped!");
            }
            catch (FormatException x)
            {
                Console.WriteLine("Successfully caught format exception on excess of compound argument end: {0}", x);
            }

            try
            {
                Parser.Tokenize("a [b [c]").ToArray();

                Assert.Fail("Lacking compound argument end slipped!");
            }
            catch (FormatException x)
            {
                Console.WriteLine("Successfully caught format exception on lacking compound argument end: {0}", x);
            }

            try
            {
                Parser.Tokenize("a [b] c]").ToArray();

                Assert.Fail("Excess of compound argument end slipped!");
            }
            catch (FormatException x)
            {
                Console.WriteLine("Successfully caught format exception on excess of compound argument end: {0}", x);
            }
        }

        [TestMethod]
        public void Almost_Badass()
        {
            TestTokens("+A\\ B rataC\\ Dtouille \"\" E;-\\lol\\ mao\"yaong \" brah;yet more\\\"s\\;u\\;f ; even [more [fricking] ] [arguments] ! mara\\ are\\ mere", new object[]
            {
                TokenTypes.Toggler, "+",
                TokenTypes.CommandName, "A B",
                TokenTypes.Argument, "rataC Dtouille",
                TokenTypes.Argument, "",
                TokenTypes.Argument, "E",

                TokenTypes.Separator, ";",

                TokenTypes.Toggler, "-",
                TokenTypes.CommandName, "lol maoyaong ",
                TokenTypes.Argument, "brah",

                TokenTypes.Separator, ";",

                TokenTypes.CommandName, "yet",
                TokenTypes.Argument, "more\"s;u;f",

                TokenTypes.Separator, ";",

                TokenTypes.CommandName, "even",
                
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "more",
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "fricking",
                TokenTypes.CompoundArgumentEnd, "]",
                TokenTypes.CompoundArgumentEnd, "]",
                
                TokenTypes.CompoundArgumentStart, "[",
                TokenTypes.CommandName, "arguments",
                TokenTypes.CompoundArgumentEnd, "]",

                TokenTypes.Exclude, "!",

                TokenTypes.CommandName, "mara are mere",
            });
        }

        [TestMethod]
        public void Empty_or_Lacking_Command()
        {
            try
            {
                Parser.Tokenize("+").ToArray();

                Assert.Fail("Lacking command slipped!");
            }
            catch (FormatException x)
            {
                Console.WriteLine("Successfully caught format exception on lacking/empty command: {0}", x);
            }
        }

        #region Utilitary Methods

        private Token[] TokenizeAndPrint(string command)
        {
            var tokens = Parser.Tokenize(command).ToArray();

            Console.WriteLine("Found {0} tokens in command: {1}", tokens.Length, command);

            foreach (var t in tokens)
                Console.WriteLine("\t{0}: \"{1}\"", t.Type, t.Content);

            return tokens;
        }

        private void TestTokens(string command, object[] shiz)
        {
            var tokens = TokenizeAndPrint(command);

            Assert.AreEqual(shiz.Length / 2, tokens.Length, "Tokens length.");

            for (int i = 0; i < tokens.Length; i++)
            {
                Assert.AreEqual((TokenTypes)shiz[2 * i], tokens[i].Type, "Token #" + i + " type.");
                Assert.AreEqual((string)shiz[2 * i + 1], tokens[i].Content, "Token #" + i + " content.");
            }
        }

        private void TestTokens(string[] commands, object[] shiz)
        {
            foreach (var c in commands)
                TestTokens(c, shiz);
        }

        static System.Collections.Generic.Dictionary<char, TokenTypes> SeparatorTypes = new System.Collections.Generic.Dictionary<char, TokenTypes>() {
            { ';', TokenTypes.Separator },
            { '?', TokenTypes.Include },
            { ':', TokenTypes.Otherwise },
            { '!', TokenTypes.Exclude },
        };

        #endregion
    }
}
