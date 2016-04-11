using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Parsing
{
    /// <summary>
    /// Possible types of tokens in a console command string.
    /// </summary>
    public enum TokenTypes
        : int
    {
        /// <summary>
        /// Separates a sequence of commands.
        /// </summary>
        Separator = -1,
        /// <summary>
        /// Equivalent to 'then' in "if left-hand then right-hand".
        /// </summary>
        Include = -2,
        /// <summary>
        /// Equivalent to 'else' in "if left-hand then middle-hand else right-hand".
        /// </summary>
        Otherwise = -3,
        /// <summary>
        /// Equivalent to 'then' in "if not left-hand then right-hand".
        /// </summary>
        Exclude = -4,

        /// <summary>
        /// The beginning of a compound argument.
        /// </summary>
        CompoundArgumentStart = 64,
        /// <summary>
        /// The end of a compound argument.
        /// </summary>
        CompoundArgumentEnd = -127,

        /// <summary>
        /// A sign representing the toggle mode of a command.
        /// </summary>
        Toggler = 0,
        /// <summary>
        /// The name of a command.
        /// </summary>
        CommandName = 1,
        /// <summary>
        /// An argument for a command.
        /// </summary>
        Argument = 2,
    }

    /// <summary>
    /// A token in a console command string.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        public TokenTypes Type { get; internal set; }

        /// <summary>
        /// Gets the content of the token.
        /// </summary>
        public string Content { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Parsing.Token"/> class with the given type and content.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="content"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given content string is empty.</exception>
        public Token(TokenTypes type, string content)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            this.Type = type;
            this.Content = content;
        }
    }
}
