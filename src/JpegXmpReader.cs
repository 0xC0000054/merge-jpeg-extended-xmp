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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MergeJPEGExtendedXMP
{
    internal sealed class JpegXmpReader
    {
        private readonly string path;
        private readonly Dictionary<string, ExtendedXmp> extendedXmpChunks;
        private string extendedXmpGuid;
        private XmpPacket mainXmpPacket;
        private XmpPacket extendedXmpPacket;
        private XmpPacket mergedXmpPacket;
        private bool fileParsed;

        private const string MainXmpSignature = "http://ns.adobe.com/xap/1.0/\0";
        private const int MainXmpSignatureLength = 29;

        private const string ExtendedXmpSignature = "http://ns.adobe.com/xmp/extension/\0";
        private const int ExtendedXmpSignatureLength = 35;
        private const int ExtendedXmpGUIDLength = 32;
        private const int ExtendedXmpPrefixLength = ExtendedXmpSignatureLength + ExtendedXmpGUIDLength + sizeof(uint) + sizeof(uint);

        public JpegXmpReader(string path)
        {
            this.path = path;
            extendedXmpChunks = new Dictionary<string, ExtendedXmp>(StringComparer.OrdinalIgnoreCase);
            fileParsed = false;
        }

        public JpegXmpPackets GetXmpPackets()
        {
            if (!fileParsed)
            {
                ParseFile();
                fileParsed = true;
            }

            if (mainXmpPacket == null)
            {
                // The file does not contain any XMP data.
                return null;
            }

            if (mergedXmpPacket == null)
            {
                if (extendedXmpPacket != null)
                {
                    mergedXmpPacket = XmpUtil.MergeExtendedXmp(mainXmpPacket, extendedXmpPacket);
                }
                else
                {
                    mergedXmpPacket = mainXmpPacket.Clone();
                }
            }

            return new JpegXmpPackets(mainXmpPacket, extendedXmpPacket, mergedXmpPacket);
        }

        private void AssembleExtendedXmpChunks()
        {
            if (extendedXmpChunks.TryGetValue(extendedXmpGuid, out ExtendedXmp value))
            {
                if (value.TotalLength > int.MaxValue)
                {
                    ExceptionUtil.ThrowInvalidOperationException("The extended XMP packet is larger than 2GB.");
                }

                using (MemoryStream memoryStream = new MemoryStream((int)value.TotalLength))
                {
                    IReadOnlyList<ExtendedXmpContent> chunks = value.GetChunks();

                    uint dstOffset = 0;

                    foreach (ExtendedXmpContent item in chunks.OrderBy(i => i.Offset))
                    {
                        if (item.Length != value.TotalLength)
                        {
                            ExceptionUtil.ThrowInvalidOperationException("The extended XMP chunk length does not equal the total packet length.");
                        }

                        if (item.Offset == dstOffset)
                        {
                            byte[] data = item.GetDataReadOnly();

                            memoryStream.Write(data, 0, data.Length);

                            dstOffset += (uint)data.Length;
                        }
                        else
                        {
                            ExceptionUtil.ThrowInvalidOperationException($"Unexpected offset: { item.Offset }, expected { dstOffset }");
                        }
                    }

                    memoryStream.Position = 0;
                    extendedXmpPacket = new XmpPacket(memoryStream);
                }
            }
        }

        private void ParseFile()
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read);

                using (EndianBinaryReader reader = new EndianBinaryReader(stream, Endianess.Big))
                {
                    stream = null;

                    GatherXmpDataFromFile(reader);
                }
            }
            finally
            {
                stream?.Dispose();
            }

            if (extendedXmpGuid != null)
            {
                AssembleExtendedXmpChunks();
            }
        }

        private void GatherXmpDataFromFile(EndianBinaryReader reader)
        {
            ushort marker = reader.ReadUInt16();

            // Check the file signature.
            if (marker == JpegMarkers.StartOfImage)
            {
                while (reader.Position < reader.Length)
                {
                    marker = reader.ReadUInt16();
                    if (marker == 0xFFFF)
                    {
                        // Skip the first padding byte and read the marker again.
                        reader.Position++;
                        continue;
                    }

                    if (marker == JpegMarkers.StartOfScan || marker == JpegMarkers.EndOfImage)
                    {
                        // The application data segments always come before these markers.
                        break;
                    }

                    ushort segmentLength = reader.ReadUInt16();

                    if (segmentLength < 2)
                    {
                        ExceptionUtil.ThrowFormatException($"JPEG segment length must be in the range of [2, 65535], actual value: { segmentLength }");
                    }

                    // The segment length field includes its own length in the total.
                    segmentLength -= sizeof(ushort);

                    if (marker == JpegMarkers.App1)
                    {
                        if (mainXmpPacket == null && segmentLength >= MainXmpSignatureLength)
                        {
                            string sig = reader.ReadAsciiString(MainXmpSignatureLength);
                            if (sig.Equals(MainXmpSignature, StringComparison.Ordinal))
                            {
                                byte[] xmpPacketBytes = reader.ReadBytes(segmentLength - MainXmpSignatureLength);

                                mainXmpPacket = new XmpPacket(xmpPacketBytes);

                                extendedXmpGuid = XmpUtil.GetExtendedXmpGuid(mainXmpPacket);
                                continue;
                            }
                            else
                            {
                                reader.Position -= MainXmpSignatureLength;
                            }
                        }

                        if (segmentLength >= ExtendedXmpPrefixLength)
                        {
                            string sig = reader.ReadAsciiString(ExtendedXmpSignatureLength);
                            if (sig.Equals(ExtendedXmpSignature, StringComparison.Ordinal))
                            {
                                string guid = reader.ReadAsciiString(ExtendedXmpGUIDLength);
                                uint length = reader.ReadUInt32();
                                uint offset = reader.ReadUInt32();

                                byte[] data = reader.ReadBytes(segmentLength - ExtendedXmpPrefixLength);

                                if (extendedXmpChunks.TryGetValue(guid, out ExtendedXmp value))
                                {
                                    value.AddChunk(data, offset, length);
                                }
                                else
                                {
                                    extendedXmpChunks.Add(guid, new ExtendedXmp(data, offset, length));
                                }

                                continue;
                            }
                            else
                            {
                                segmentLength -= ExtendedXmpSignatureLength;
                            }
                        }
                    }

                    reader.Position += segmentLength;
                }
            }
        }

        private sealed class ExtendedXmp
        {
            private readonly List<ExtendedXmpContent> chunks;

            public ExtendedXmp(byte[] data, uint offset, uint totalLength)
            {
                TotalLength = totalLength;
                chunks = new List<ExtendedXmpContent>
                {
                    new ExtendedXmpContent(data, offset, totalLength)
                };
            }

            public uint TotalLength { get; }

            public void AddChunk(byte[] data, uint offset, uint length)
            {
                chunks.Add(new ExtendedXmpContent(data, offset, length));
            }

            public IReadOnlyList<ExtendedXmpContent> GetChunks()
            {
                return chunks;
            }
        }

        private sealed class ExtendedXmpContent
        {
            private readonly byte[] data;

            public ExtendedXmpContent(byte[] data, uint offset, uint length)
            {
                if (data is null)
                {
                    throw new ArgumentNullException(nameof(data));
                }

                this.data = data;
                Offset = offset;
                Length = length;
            }

            public uint Offset { get; }

            public uint Length { get; }

            public byte[] GetDataReadOnly()
            {
                return data;
            }
        }

        private static class JpegMarkers
        {
            internal const ushort StartOfImage = 0xFFD8;
            internal const ushort EndOfImage = 0xFFD9;
            internal const ushort StartOfScan = 0xFFDA;
            internal const ushort App1 = 0xFFE1;
        }
    }
}
