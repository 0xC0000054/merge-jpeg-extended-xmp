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

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MergeJPEGExtendedXMP
{
    internal sealed class XmpPacket
    {
        public XmpPacket(byte[] xmpPacketBytes)
        {
            if (xmpPacketBytes is null)
            {
                throw new ArgumentNullException(nameof(xmpPacketBytes));
            }

            Document = CreateDocument(xmpPacketBytes);
            LengthInBytes = xmpPacketBytes.Length;
        }

        public XmpPacket(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            Document = CreateDocument(stream);
            LengthInBytes = stream.Length;
        }

        public XmpPacket(XDocument document)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            Document = document;
            LengthInBytes = GetDocumentLengthInBytes(document);
        }

        private XmpPacket(XmpPacket cloneMe)
        {
            Document = new XDocument(cloneMe.Document);
            LengthInBytes = cloneMe.LengthInBytes;
        }

        public XDocument Document { get; }

        public long LengthInBytes { get; }

        public XmpPacket Clone()
        {
            return new XmpPacket(this);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true, OmitXmlDeclaration = true };

            using (XmlWriter xmlWriter = XmlWriter.Create(builder, settings))
            {
                Document.Save(xmlWriter);
            }

            return builder.ToString();
        }

        private static XDocument CreateDocument(byte[] xmpPacketBytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(xmpPacketBytes, false))
            {
                return CreateDocument(memoryStream);
            }
        }

        private static XDocument CreateDocument(Stream stream)
        {
            return XDocument.Load(stream);
        }

        private static long GetDocumentLengthInBytes(XDocument document)
        {
            long length;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlWriterSettings settings = new XmlWriterSettings { Indent = false, OmitXmlDeclaration = true };

                using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, settings))
                {
                    document.Save(xmlWriter);
                }

                length = memoryStream.Length;
            }

            return length;
        }
    }
}
