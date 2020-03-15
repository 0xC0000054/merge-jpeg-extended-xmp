////////////////////////////////////////////////////////////////////////
//
// This file is part of merge-jpeg-extended-xmp, a utility that
// demonstrates merging the JPEG Extended XMP packet with the main
// XMP packet.
//
// Copyright (c) 2020 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MergeJPEGExtendedXMP
{
    internal static class XmpUtil
    {
        private static readonly XName RdfContainerElementName;
        private static readonly XName RdfDescriptionElementName;
        private static readonly XName RdfAboutAttributeName;
        private static readonly XName XmpNoteNamespaceAttributeName;
        private static readonly XNamespace XmpNoteNamespace;
        private static readonly XName HasExtendedXmpObjectName;

        static XmpUtil()
        {
            XNamespace rdfNamespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
            RdfContainerElementName = rdfNamespace + "RDF";
            RdfDescriptionElementName = rdfNamespace + "Description";
            RdfAboutAttributeName = rdfNamespace + "about";

            XmpNoteNamespaceAttributeName = XNamespace.Xmlns + "xmpNote";
            XmpNoteNamespace = "http://ns.adobe.com/xmp/note/";
            HasExtendedXmpObjectName = XmpNoteNamespace + "HasExtendedXMP";
        }

        public static string GetExtendedXmpGuid(XmpPacket xmpPacket)
        {
            if (xmpPacket is null)
            {
                ExceptionUtil.ThrowArgumentNullException(nameof(xmpPacket));
            }

            string extendedXmpGuid = null;

            XElement rdfElement = xmpPacket.Document.Descendants(RdfContainerElementName).First();

            XObject hasExtendedXmpObject = FindHasExtendedXmpObject(rdfElement);

            if (hasExtendedXmpObject != null)
            {
                if (hasExtendedXmpObject is XAttribute attribute)
                {
                    extendedXmpGuid = attribute.Value;
                }
                else if (hasExtendedXmpObject is XElement element)
                {
                    extendedXmpGuid = element.Value;
                }
            }

            return extendedXmpGuid;
        }

        public static XmpPacket MergeExtendedXmp(XmpPacket mainXmp, XmpPacket extendedXmp)
        {
            if (mainXmp is null)
            {
                ExceptionUtil.ThrowArgumentNullException(nameof(mainXmp));
            }

            if (extendedXmp is null)
            {
                ExceptionUtil.ThrowArgumentNullException(nameof(extendedXmp));
            }

            XDocument mergedDocument = new XDocument(mainXmp.Document);
            XElement mergedRdfElement = mergedDocument.Descendants(RdfContainerElementName).First();

            // Remove the xmpNote:HasExtendedXMP element.
            XObject hasExtendedXmpObject = FindHasExtendedXmpObject(mergedRdfElement);
            if (hasExtendedXmpObject != null)
            {
                if (ShouldRemoveExtendedXmpParent(hasExtendedXmpObject))
                {
                    hasExtendedXmpObject.Parent.Remove();
                }
                else
                {
                    MaybeRemoveXmpNoteNamespace(hasExtendedXmpObject.Parent);
                    if (hasExtendedXmpObject is XAttribute attribute)
                    {
                        attribute.Remove();
                    }
                    else if (hasExtendedXmpObject is XElement element)
                    {
                        element.Remove();
                    }
                }
            }

            // Add all of the rdf:Description elements in the extended XMP packet to the merged packet.
            //
            // Section 7.4 of the XMP Specification Part 1 allows a single rdf:RDF element to contain multiple
            // rdf:Description elements, with the XMP properties arbitrarily split between them.
            //
            // The XMP Specification Part 1 can be found at:
            // https://wwwimages2.adobe.com/content/dam/acom/en/devnet/xmp/pdfs/XMP%20SDK%20Release%20cc-2016-08/XMPSpecificationPart1.pdf
            //
            // The relevant portion of section 7.4 is quoted below:
            //
            // A single XMP packet shall be serialized using a single rdf:RDF XML element.
            // The rdf:RDF element content shall consist of only zero or more rdf:Description elements.
            //
            // The element content of top-level rdf:Description elements shall consist of zero or more XML elements for XMP properties.
            // XMP properties may be arbitrarily apportioned among the rdf:Description elements.

            XElement extendedXmpRdfElement = extendedXmp.Document.Descendants(RdfContainerElementName).First();

            foreach (XElement item in extendedXmpRdfElement.Elements(RdfDescriptionElementName))
            {
                mergedRdfElement.Add(item);
            }

            return new XmpPacket(mergedDocument);
        }

        private static XObject FindHasExtendedXmpObject(XElement rdfElement)
        {
            // The xmpNote:HasExtendedXMP element may be located in any top-level rdf:Description element, not just the first one.

            foreach (XElement rdfDescription in rdfElement.Elements(RdfDescriptionElementName))
            {
                XAttribute xmpNoteNamespaceAttribute = rdfDescription.Attribute(XmpNoteNamespaceAttributeName);

                if (xmpNoteNamespaceAttribute != null && xmpNoteNamespaceAttribute.Value == XmpNoteNamespace.NamespaceName)
                {
                    XAttribute hasExtendedXmpAttribute = rdfDescription.Attribute(HasExtendedXmpObjectName);

                    if (hasExtendedXmpAttribute != null)
                    {
                        return hasExtendedXmpAttribute;
                    }
                    else if (rdfDescription.HasElements)
                    {
                        XElement hasExtendedXmpElement = rdfDescription.Element(HasExtendedXmpObjectName);

                        if (hasExtendedXmpElement != null)
                        {
                            return hasExtendedXmpElement;
                        }
                    }
                }
            }

            return null;
        }

        private static void MaybeRemoveXmpNoteNamespace(XElement hasExtendedXmpParent)
        {
            // The xmpNote XML name-space declaration will be removed if the
            // xmpNote:HasExtendedXMP object is the only thing that uses it.

            bool removeXmpNote = true;

            if (hasExtendedXmpParent.HasElements)
            {
                foreach (XElement element in hasExtendedXmpParent.Elements())
                {
                    if (element.Name.Namespace == XmpNoteNamespace &&
                        element.Name.LocalName != HasExtendedXmpObjectName.LocalName)
                    {
                        removeXmpNote = false;
                        break;
                    }
                }
            }

            XAttribute xmpNoteNamespaceAttribute = null;

            foreach (XAttribute attribute in hasExtendedXmpParent.Attributes())
            {
                if (attribute.IsNamespaceDeclaration)
                {
                    if (attribute.Name == XmpNoteNamespaceAttributeName &&
                        attribute.Value == XmpNoteNamespace.NamespaceName)
                    {
                        xmpNoteNamespaceAttribute = attribute;
                    }
                }
                else if (attribute.Name.Namespace == XmpNoteNamespace &&
                         attribute.Name.LocalName != HasExtendedXmpObjectName.LocalName)
                {
                    removeXmpNote = false;
                    break;
                }
            }

            if (removeXmpNote && xmpNoteNamespaceAttribute != null)
            {
                xmpNoteNamespaceAttribute.Remove();
            }
        }

        private static bool ShouldRemoveExtendedXmpParent(XObject extendedXmp)
        {
            // The node will be removed if xmpNote:HasExtendedXMP is the only XMP attribute or child element it contains.

            XElement parent = extendedXmp.Parent;

            foreach (XAttribute attribute in parent.Attributes())
            {
                if (attribute.IsNamespaceDeclaration)
                {
                    if (attribute.Name != XmpNoteNamespaceAttributeName ||
                        attribute.Value != XmpNoteNamespace.NamespaceName)
                    {
                        return false;
                    }
                }
                else
                {
                    XName attributeName = attribute.Name;

                    if (attributeName != RdfAboutAttributeName && attributeName != HasExtendedXmpObjectName)
                    {
                        return false;
                    }
                }
            }

            if (extendedXmp is XElement)
            {
                if (parent.Elements().Count() > 1)
                {
                    return false;
                }
            }
            else
            {
                if (parent.HasElements)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
