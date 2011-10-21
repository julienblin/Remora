#region Licence

// The MIT License
// 
// Copyright (c) 2011 Julien Blin, julien.blin@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

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

        #region Nested type: Xsi

        private static class Xsi
        {
            public static readonly XNamespace XsiNamespace = "http://www.w3.org/2001/XMLSchema-instance/";

            public static readonly XName SchemaLocation = XsiNamespace + "schemaLocation";
            public static readonly XName NoNamespaceSchemaLocation = XsiNamespace + "noNamespaceSchemaLocation";
        }

        #endregion
    }
}