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

namespace MergeJPEGExtendedXMP
{
    internal sealed class JpegXmpPackets
    {
        public JpegXmpPackets(XmpPacket main, XmpPacket extended, XmpPacket merged)
        {
            Main = main;
            Extended = extended;
            Merged = merged;
        }

        public XmpPacket Main { get; }

        public XmpPacket Extended { get; }

        public XmpPacket Merged { get; }
    }
}
