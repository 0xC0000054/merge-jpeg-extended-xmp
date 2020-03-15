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

namespace MergeJPEGExtendedXMP
{
    internal static class ExceptionUtil
    {
        public static void ThrowArgumentNullException(string paramName)
        {
            throw new ArgumentNullException(paramName);
        }

        public static void ThrowArgumentOutOfRangeException(string paramName)
        {
            throw new ArgumentOutOfRangeException(paramName);
        }

        public static void ThrowEndOfStreamException()
        {
            throw new EndOfStreamException();
        }

        public static void ThrowFormatException(string message)
        {
            throw new FormatException(message);
        }

        public static void ThrowInvalidOperationException(string message)
        {
            throw new InvalidOperationException(message);
        }

        public static void ThrowObjectDisposedException(string objectName)
        {
            throw new ObjectDisposedException(objectName);
        }
    }
}
