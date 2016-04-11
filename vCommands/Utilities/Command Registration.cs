using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace vCommands.Utilities
{
    using Commands;
    using Parsing.Expressions;
    
    /// <summary>
    /// Utilitary methods for registering commands.
    /// </summary>
    public static class CommandRegistration
    {
        static readonly Type DelegateType = typeof(CommandMethod);

        /// <summary>
        /// Attempts to turn all suitable static methods from the given type into commands.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includePrivate">optional; True to include private methods in the search; otherwise false.</param>
        /// <param name="prefix">optional; A prefix to add to all command names.</param>
        /// <param name="forcedCategory">optional; The category in which to place the commands, regardless of their setting. Null means the method's attributed category is used.</param>
        /// <returns></returns>
        public static MethodCommand[] FromType<T>(bool includePrivate = false, string prefix = "", string forcedCategory = null)
        {
            return FromType(typeof(T), includePrivate, prefix, forcedCategory);
        }

        /// <summary>
        /// Attempts to turn all suitable static methods from the given type into commands.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="includePrivate">optional; True to include private methods in the search; otherwise false.</param>
        /// <param name="prefix">optional; A prefix to add to all command names.</param>
        /// <param name="forcedCategory">optional; The category in which to place the commands, regardless of their setting. Null means the method's attributed category is used.</param>
        /// <returns></returns>
        public static MethodCommand[] FromType(Type t, bool includePrivate = false, string prefix = "", string forcedCategory = null)
        {
            //System.Diagnostics.Debug.WriteLine("Looking up commands in {0}", t.FullName);

            var bFlags = BindingFlags.Static | BindingFlags.Public;

            if (includePrivate) bFlags |= BindingFlags.NonPublic;

            /*var matchingMethods = o.GetType().GetMethods()
                .Where(mi => mi.ReturnType == gs.Method.ReturnType && mi.GetParameters()
                    .Select(pi => pi.ParameterType)
                        .SequenceEqual(gs.Method.GetParameters()
                            .Select(pi => pi.ParameterType)));*/

            var methods = t.GetMethods(bFlags);
            var cmds = new List<MethodCommand>(methods.Length);

            foreach (var m in methods)
            {
                var c = FromMethod(m, prefix, forcedCategory);

                //System.Diagnostics.Debug.WriteLine("\tChecking method {0}: {1}", m.Name, c != null);

                if (c != null)
                    cmds.Add(c);
            }

            return cmds.ToArray();
        }

        /// <summary>
        /// Attempts to turn the given method into a command, if the method matches the signature.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="prefix">optional; A prefix to add to the command's name.</param>
        /// <param name="forcedCategory">optional; The category in which to place the command, regardless of the method's setting. Null means the method's attributed category is used.</param>
        /// <returns>A <see cref="vCommands.Commands.MethodCommand"/> if successful; otherwise nil.</returns>
        public static MethodCommand FromMethod(MethodInfo m, string prefix = "", string forcedCategory = null)
        {
            if (m.ReturnType != typeof(EvaluationResult))
                return null;

            var parameters = m.GetParameters();

            if (parameters.Length != 3)
                return null;

            if (parameters[0].ParameterType != typeof(Toggler))
                return null;

            if (parameters[1].ParameterType != typeof(EvaluationContext))
                return null;

            if (parameters[2].ParameterType != typeof(Expression[]))
                return null;

            string name = m.Name;
            string category = null;
            string abstr = ".NET method";

            var att = m.GetCustomAttributes(typeof(MethodCommandDataAttribute), false);

            if (att.Length > 0)
            {
                var lastAtt = (MethodCommandDataAttribute)att[att.Length - 1];

                if (lastAtt.Name != null)
                    name = lastAtt.Name;

                category = lastAtt.Category;

                if (lastAtt.Abstract != null)
                    abstr = lastAtt.Abstract;
            }

            return new MethodCommand(prefix + name, forcedCategory ?? category, abstr, (CommandMethod)Delegate.CreateDelegate(typeof(CommandMethod), m));
        }
    }

    /// <summary>
    /// Specifies information about a method which would be a command. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class MethodCommandDataAttribute
        : Attribute
    {
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the category of the command.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets the description of the command.
        /// </summary>
        public string Abstract { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Utilities.MethodCommandDataAttribute"/> class.
        /// </summary>
        public MethodCommandDataAttribute()
        {
            
        }
    }
}
