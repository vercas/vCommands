using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace vCommands.Manuals
{
    /// <summary>
    /// Contains methods for parsing manuals.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Parses a <see cref="vCommands.Manuals.Manual"/> or a collection of <see cref="vCommands.Manuals.Manual"/>s from the given XML document string.
        /// </summary>
        /// <param name="xmlText"></param>
        /// <returns>An enumeration of manuals.</returns>
        public static IEnumerable<Manual> ParseXML(string xmlText)
        {
            XDocument x = XDocument.Parse(xmlText);

            if (x.FirstNode != x.LastNode)
                throw new FormatException("There is more than one node in the document?");
            
            var lst = x.FirstNode;

            if (lst.NodeType != XmlNodeType.Element)
                throw new FormatException("The only document node is not an element.");

            var lste = (XElement)lst;

            switch (lste.Name.LocalName)
            {
                case "manuals":
                    return from e in lste.Elements("manual") select ParseXElementManual(e);

                case "manual":
                    return new Manual[1] { ParseXElementManual(lste) };

                default:
                    throw new FormatException("Document root element must be either named 'manuals' and contain one or more proper 'manual' entries, or named 'manual' for be itself a single manual entry.");
            }
        }

        /// <summary>
        /// Parses a <see cref="vCommands.Manuals.Manual"/> from the given <see cref="System.Xml.Linq.XElement"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Manual ParseXElementManual(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (element.Name != "manual")
                throw new FormatException("Element must be named 'manual'.");

            var aTitle = element.Attribute("title");
            if (aTitle == null)
                throw new FormatException("Manual element must have a 'title' attribute.");

            var eAbstract = element.Element("abstract");
            if (eAbstract == null)
                throw new FormatException("Manual element must have an 'abstract' sub-element.");

            Manual m = new Manual();

            m.Title = aTitle.Value;
            m.Abstract = eAbstract.Value;

            m.AddSections(from e in element.Elements("section") select ParseXElementSection(e));

            m.Seal();
            return m;
        }

        /// <summary>
        /// Parses a <see cref="vCommands.Manuals.Section"/> from the given <see cref="System.Xml.Linq.XElement"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Section ParseXElementSection(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (element.Name != "section")
                throw new FormatException("Element must be named 'section'.");

            var aTitle = element.Attribute("title");
            if (aTitle == null)
                throw new FormatException("Section element must have a 'title' attribute.");

            var eBody = element.Element("body");
            if (eBody == null)
                throw new FormatException("Section element must have a 'body' sub-element.");

            Section s = new Section(aTitle.Value, eBody.Value);

            s.AddSubsections(from e in element.Elements("section") select ParseXElementSection(e));

            s.Seal();
            return s;
        }
    }
}
