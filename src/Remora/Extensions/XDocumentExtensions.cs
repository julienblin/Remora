using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Remora.Extensions
{
    public static class XDocumentExtensions
    {
        public static XDocument Normalize(this XDocument source)
        {
            return new XDocument(
                source.Declaration,
                source.Nodes().Select(n =>
                {
                    // Remove comments, processing instructions, and text nodes that are
                    // children of XDocument.  Only white space text nodes are allowed as
                    // children of a document, so we can remove all text nodes.
                    if (n is XComment || n is XProcessingInstruction || n is XText)
                        return null;
                    var e = n as XElement;
                    return e != null ? NormalizeElement(e) : n;
                })
            );
        }

        private static XElement NormalizeElement(XElement element)
        {
            return new XElement(element.Name, NormalizeAttributes(element), element.Nodes().Select(NormalizeNode));
        }

        private static XNode NormalizeNode(XNode node)
        {
            // trim comments and processing instructions from normalized tree
            if (node is XComment || node is XProcessingInstruction)
                return null;
            var e = node as XElement;
            return e != null ? NormalizeElement(e) : node;
            // Only thing left is XCData and XText, so clone them
        }

        private static IEnumerable<XAttribute> NormalizeAttributes(XElement element)
        {
            return element.Attributes()
                    .Where(a => !a.IsNamespaceDeclaration &&
                        a.Name != Xsi.SchemaLocation &&
                        a.Name != Xsi.NoNamespaceSchemaLocation)
                    .OrderBy(a => a.Name.NamespaceName)
                    .ThenBy(a => a.Name.LocalName);
        }

        private static class Xsi
        {
            public static readonly XNamespace XsiNamespace = "http://www.w3.org/2001/XMLSchema-instance/";

            public static readonly XName SchemaLocation = XsiNamespace + "schemaLocation";
            public static readonly XName NoNamespaceSchemaLocation = XsiNamespace + "noNamespaceSchemaLocation";
        }
    }
}
