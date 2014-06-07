using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands
{
    using Manuals;
    using Manuals.Drivers;

    internal class DefaultManuals
    {
        internal static Manual[] mans = null;

        static DefaultManuals()
        {
            mans = Manuals.Parser.ParseXML(Properties.Resources.default_manual).ToArray();
        }

        public static void Register(Library l, DriverCollection d)
        {
            l.Add(mans);

            d.Add(OutputDriver.Instance);
            d.SetDefault(OutputDriver.Instance);
        }
    }
}
