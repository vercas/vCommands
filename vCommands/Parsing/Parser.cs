using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Parsing
{
    using Commands;
    using Expressions;

    /// <summary>
    /// Contains methods for parsing commands.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Turns the given command string into a series of tokens.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>An enumeration of tokens extracted from the string.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given command string is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given command string is empty or consists only of white-space characters.</exception>
        public static IEnumerable<Token> Tokenize(string command)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException("Command string may not be empty or consist only of white-space characters.");

            command += "\n";    //  Just to help parsing a bit.

            bool inString = false, underEscape = false, expectingCommand = true, gotWhitespace = true, gotNonSpaceSeparator = false;
            int compoundArgumentCount = 0;  //  A state of the tokenizer; the number of compound arguments pseudo-stacked at the current character position.
            StringBuilder current = new StringBuilder(command.Length);

            for (int i = 0; i < command.Length; i++)
            {
                var c = command[i];

                bool preceededByWhitespace = gotWhitespace, preceededByNonSpaceSeparator = gotNonSpaceSeparator;
                gotWhitespace = false; gotNonSpaceSeparator = false;

                bool canFinishToken = !(preceededByWhitespace || preceededByNonSpaceSeparator); //  Empty tokens should not be created after a whitespace or some separators.

                if (underEscape)
                {
                    if (i == command.Length - 1)
                        throw new FormatException("Command string ends in a backslash.");

                    current.Append(c);

                    underEscape = false;    //  No escape!
                }
                else if (inString)
                {
                    switch (c)
                    {
                        //  \ is used to escape whatever character follows after it.

                        case '\\':
                            underEscape = true;
                            break;

                        //  " marks the start and end of a string, unless escaped, of course.

                        case '"':
                            inString = false;
                            break;

                        //  Anything else is just content.

                        default:
                            current.Append(c);
                            break;
                    }

                    continue;
                }
                else
                {
                    switch (c)
                    {
                        //  + and - are toggling flags for a command.

                        case '+':
                        case '-':
                            if (expectingCommand)
                                yield return new Token(TokenTypes.Toggler, c.ToString());
                            else
                                throw new FormatException("Toggling character '" + c + "' (position " + i + ") appears in an unusual place; maybe it should be escaped?");

                            gotNonSpaceSeparator = true;

                            break;

                        //  Whitespaces (spaces, tabs and newlines) separate commands, arguments and operators.

                        case ' ':
                        case '\t':
                        case '\n':
                            if (i == command.Length - 1)
                            {
                                //System.Diagnostics.Debug.WriteLine("End character reached: {0} | {1}", compoundArgumentCount, current.Length);

                                if (current.Length == 0 && expectingCommand)
                                    throw new FormatException("Command string does not contain a full command name!");

                                if (compoundArgumentCount > 0)
                                    throw new FormatException("Missing " + compoundArgumentCount + " compound argument ending(s) in command string.");
                                //  Too many endings are reported when an ending is detected.
                            }

                            if (current.Length > 0 || canFinishToken)
                            {
                                yield return new Token(expectingCommand ? TokenTypes.CommandName : TokenTypes.Argument, current.ToString());

                                current.Clear();
                                expectingCommand = false;
                            }

                            gotWhitespace = true;

                            break;

                        //  " marks the start and end of a string, unless escaped, of course.

                        case '"':
                            inString = true;

                            break;

                        //  \ is used to escape whatever character follows after it.

                        case '\\':
                            /*if (expectingCommand)
                                throw new FormatException("Character '\\' (position " + i + ") cannot appear where a command name is expected.");
                            else*/

                            underEscape = true;

                            break;

                        //  ; separates a series of commands.
                        // a ? b : c means if a then b else c
                        // a ! b : c means if not a then b else c
                        // The : is optional.

                        case ';':
                        case '?':
                        case ':':
                        case '!':
                            if (expectingCommand && current.Length == 0)
                                throw new FormatException("Separator '" + c + "' at position " + i + " is preceeded by an empty command.");

                            if (current.Length > 0 || canFinishToken)
                            {
                                yield return new Token(expectingCommand ? TokenTypes.CommandName : TokenTypes.Argument, current.ToString());

                                current.Clear();
                            }

                            expectingCommand = true;
                            gotWhitespace = true;

                            yield return new Token(SeparatorTypes[c], c.ToString());

                            break;

                        //  [ is the start of a compound argument, which is just another command.
                        //  ] is the end of a compoound argument.

                        case '[':
                            if (expectingCommand)
                                throw new FormatException("Command name is expected where compoound argument starts at position " + i + ".");

                            if (!preceededByWhitespace)
                                throw new FormatException("Compound argument start is not preceeded by a space at position " + i + ".");

                            if (current.Length > 0)
                            {
                                yield return new Token(TokenTypes.Argument, current.ToString());

                                current.Clear();
                            }

                            expectingCommand = true;
                            gotWhitespace = true;
                            compoundArgumentCount++;

                            yield return new Token(TokenTypes.CompoundArgumentStart, c.ToString());

                            break;

                        case ']':
                            if (expectingCommand && current.Length == 0)
                                throw new FormatException("Empty command name is found where compoound argument ends at position " + i + ".");

                            if (current.Length > 0 || canFinishToken)
                            {
                                yield return new Token(expectingCommand ? TokenTypes.CommandName : TokenTypes.Argument, current.ToString());

                                current.Clear();
                            }

                            expectingCommand = false;
                            gotWhitespace = false;
                            gotNonSpaceSeparator = true;
                            compoundArgumentCount--;

                            if (compoundArgumentCount < 0)
                                throw new FormatException("Excess of " + compoundArgumentCount + " compound argument ending(s) in command string.");

                            yield return new Token(TokenTypes.CompoundArgumentEnd, c.ToString());

                            break;

                        //  Anything else is just content.

                        default:
                            current.Append(c);

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Turns the given string into an expression.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given command string is null.</exception>
        public static Expression Parse(string command)
        {
            return Parse(Tokenize(command));
        }

        /// <summary>
        /// Turns the given enumeration of tokens into an expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given enumeration of tokens is null.</exception>
        public static Expression Parse(IEnumerable<Token> tokens)
        {
            using (var enumerator = tokens.GetEnumerator())
                return Parse(enumerator, false);
        }

        /// <summary>
        /// Builds an expression from the tokens given by the enumerator.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given enumerator of tokens is null.</exception>
        public static Expression Parse(IEnumerator<Token> tokens)
        {
            return Parse(tokens, false);
        }

        private static Expression Parse(IEnumerator<Token> tokens, bool isCompoundArgument)
        {
            Stack<Expression> stack = new Stack<Expression>();

            CommandInvocationExpression lastCommand = null;
            ConditionalExpression lastCondition = null;
            SeriesExpression lastSeries = null;

            Expression temp = null;

            while (tokens.MoveNext())
            {
                var tok = tokens.Current;
                
                /*System.Diagnostics.Debug.WriteLine("BEFORE:");
                System.Diagnostics.Debug.WriteLine("Token: {0} | \"{1}\" | {2} | {3} | {4}", tok.Type, tok.Content, lastCommand, lastSeries, lastCondition);

                System.Diagnostics.Debug.WriteLine("Stack dump:");

                foreach (var stuff in stack)
                    System.Diagnostics.Debug.WriteLine("\t<{0}> ({1})", stuff, stuff.GetType());//*/

                switch (tok.Type)
                {
                    case TokenTypes.Toggler:
                        if (tok.Content != "+" && tok.Content != "-")
                            throw new FormatException("Invalid toggler token!");

                        lastCommand = new CommandInvocationExpression(toggle: tok.Content == "+" ? Toggler.On : Toggler.Off);

                        //  Conditionals have higher priority than series.

                        if (lastCondition != null)
                        {
                            if (lastCondition.PrimaryAction == null)
                                lastCondition.PrimaryAction = lastCommand;
                            else if (lastCondition.SecondaryAction == null)
                                lastCondition.SecondaryAction = lastCommand;
                            else
                                throw new ArgumentException("No series and a full conditional preceeds a toggler.");
                        }
                        else if (lastSeries != null)
                            lastSeries.AddExpression(lastCommand);
                        else if (stack.Count > 0)
                            throw new FormatException("Toggler follows in a non-empty stack, but there are no preceeding series or conditions.");

                        stack.Push(lastCommand);

                        break;

                    case TokenTypes.CommandName:
                        if (lastCommand == null)
                            stack.Push(lastCommand = new CommandInvocationExpression(commandName: tok.Content));
                        else if (lastCommand.CommandName == null)
                        {
                            //  This means there was a toggler before.

                            lastCommand.CommandName = tok.Content;

                            break;  //  So the code below already executed.
                        }
                        else
                            throw new FormatException("Last command execution expression already contains a command name.");

                        if (lastCondition != null)
                        {
                            if (lastCondition.PrimaryAction == null)
                                lastCondition.PrimaryAction = lastCommand;
                            else if (lastCondition.SecondaryAction == null)
                                lastCondition.SecondaryAction = lastCommand;
                            else
                                throw new ArgumentException("No series and a full conditional preceeds a command name.");
                        }
                        else if (lastSeries != null)
                            lastSeries.AddExpression(lastCommand);

                        break;

                    case TokenTypes.Argument:
                        //if (lastCommand == null)
                        //    lastCommand = stack.Peek() as CommandExecutionExpression;

                        if (lastCommand == null)
                            throw new FormatException("An argument seems to not be preceeded by a command.");

                        lastCommand.AddArgument(ConstantExpression.Fetch(tok.Content));

                        break;

                    case TokenTypes.Separator:
                        var didNothing = true;

                        if (lastCommand != null)
                        {
                            lastCommand.Seal();
                            lastCommand = null;
                            didNothing = false;
                            temp = stack.Pop();
                        }

                        if (lastCondition != null)
                        {
                            lastCondition.Seal();
                            lastCondition = null;
                            didNothing = false;
                            temp = stack.Pop();
                        }

                        if (didNothing)
                            throw new FormatException("Separator does not follow anything.");

                        if (lastSeries == null)
                        {
                            if (temp != null)
                            {
                                //temp = stack.Pop();

                                lastSeries = new SeriesExpression(new Expression[] { temp });

                                stack.Push(lastSeries);
                            }
                            else
                                throw new FormatException("A separator does not have any commands to separate.");
                        }
                        //else
                        //    stack.Pop();    //  Make it ready to receive more commands.

                        break;

                    case TokenTypes.Include:
                    case TokenTypes.Exclude:
                        if (lastCondition != null)
                        {
                            if (lastCommand != null)
                                stack.Pop().Seal();

                            stack.Pop().Seal();

                            lastCondition = new ConditionalExpression(tok.Type == TokenTypes.Include, lastCondition);

                            stack.Push(lastCondition);
                            lastCommand = null;
                        }
                        else if (lastCommand != null)
                        {
                            stack.Pop().Seal();

                            lastCondition = new ConditionalExpression(tok.Type == TokenTypes.Include, lastCommand);

                            stack.Push(lastCondition);
                            lastCommand = null;
                        }
                        else
                            throw new FormatException("Inclusion does not follow a command.");

                        if (lastSeries != null)
                        {
                            lastSeries.exprs[lastSeries.exprs.Count - 1] = lastCondition;
                        }

                        break;

                    case TokenTypes.Otherwise:
                        if (lastCondition == null)
                            throw new FormatException("Otherwise token does not follow a conditional.");
                        else if (lastCondition.PrimaryAction == null)
                            throw new FormatException("Otherwise follows a conditional with no primary action.");
                        else if (lastCondition.SecondaryAction != null)
                            throw new FormatException("Otherwise follows a conditional which already contains a secondary action.");

                        if (lastCommand == null)
                            throw new FormatException("Otherwise token does not follow a command (which would be the primary action).");

                        lastCommand = null;
                        stack.Pop().Seal();    //  Pop the primary action.

                        break;

                    case TokenTypes.CompoundArgumentStart:
                        if (lastCommand == null)
                            throw new FormatException("A compound argument seems to not be preceeded by a command.");

                        lastCommand.AddArgument(Parse(tokens, true));

                        break;

                    case TokenTypes.CompoundArgumentEnd:
                        if (lastCommand == null)
                            throw new FormatException("A compound argument seems to end with no command.");

                        if (isCompoundArgument)
                            return SealAllAndReturnLast(stack);
                        else
                            throw new FormatException("A compound argument end token doesn't seem to end any argument.");
                }
                
                /*System.Diagnostics.Debug.WriteLine("AFTER:");
                System.Diagnostics.Debug.WriteLine("Token: {0} | \"{1}\" | {2} | {3} | {4}", tok.Type, tok.Content, lastCommand, lastSeries, lastCondition);

                System.Diagnostics.Debug.WriteLine("Stack dump:");

                foreach (var stuff in stack)
                    System.Diagnostics.Debug.WriteLine("\t<{0}> ({1})", stuff, stuff.GetType());

                System.Diagnostics.Debug.WriteLine("");//*/
            }

            /*System.Diagnostics.Debug.WriteLine("END Stack dump:");

            foreach (var stuff in stack)
                System.Diagnostics.Debug.WriteLine("\t<{0}> ({1})", stuff, stuff.GetType());//*/

            return SealAllAndReturnLast(stack);
        }

        #region Utilities

        static Dictionary<char, TokenTypes> SeparatorTypes = new Dictionary<char, TokenTypes>() {
            { ';', TokenTypes.Separator },
            { '?', TokenTypes.Include },
            { ':', TokenTypes.Otherwise },
            { '!', TokenTypes.Exclude },
        };

        internal static char[] MustEscape = new char[] { '+', '-', ' ', '\t', '\n', '"', '[', ']', '?', '!', ':', ';', '\\' };

        static Expression SealAllAndReturnLast(IEnumerable<Expression> exprs)
        {
            using (var en = exprs.GetEnumerator())
            {
                Expression last = null;

                while (en.MoveNext())
                {
                    var cur = en.Current;
                    cur.Seal();
                    last = cur;
                }

                return last;
            }
        }

        #endregion
    }
}
