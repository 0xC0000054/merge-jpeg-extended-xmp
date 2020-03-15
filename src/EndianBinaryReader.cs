﻿////////////////////////////////////////////////////////////////////////
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

namespace MergeJPEGExtendedXMP
{
    // Adapted from 'Problem and Solution: The Terrible Inefficiency of FileStream and BinaryReader'
    // https://jacksondunstan.com/articles/3568

    internal sealed class EndianBinaryReader : IDisposable
    {
#pragma warning disable IDE0032 // Use auto property
        private Stream stream;
        private int readOffset;
        private int readLength;

        private readonly byte[] buffer;
        private readonly int bufferSize;
        private readonly Endianess byteOrder;
        private readonly bool leaveOpen;
#pragma warning restore IDE0032 // Use auto property

        private const int MaxBufferSize = 4096;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndianBinaryReader"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="byteOrder">The byte order of the stream.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="byteOrder"/> is not a valid <see cref="Endianess"/> value.</exception>
        public EndianBinaryReader(Stream stream, Endianess byteOrder) : this(stream, byteOrder, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndianBinaryReader"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="byteOrder">The byte order of the stream.</param>
        /// <param name="leaveOpen">
        /// <see langword="true"/> to leave the stream open after the EndianBinaryReader is disposed; otherwise, <see langword="false"/>
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="byteOrder"/> is not a valid <see cref="Endianess"/> value.</exception>
        public EndianBinaryReader(Stream stream, Endianess byteOrder, bool leaveOpen)
        {
            if (stream == null)
            {
                ExceptionUtil.ThrowArgumentNullException(nameof(stream));
            }

            if (byteOrder < Endianess.Big || byteOrder > Endianess.Little)
            {
                ExceptionUtil.ThrowArgumentOutOfRangeException(nameof(EndianBinaryReader.byteOrder));
            }

            this.stream = stream;
            this.byteOrder = byteOrder;
            this.leaveOpen = leaveOpen;

            bufferSize = (int)Math.Min(stream.Length, MaxBufferSize);
            buffer = new byte[bufferSize];

            readOffset = 0;
            readLength = 0;
        }

        public Endianess ByteOrder => byteOrder;

        /// <summary>
        /// Gets the length of the stream.
        /// </summary>
        /// <value>
        /// The length of the stream.
        /// </value>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public long Length
        {
            get
            {
                VerifyNotDisposed();

                return stream.Length;
            }
        }

        /// <summary>
        /// Gets or sets the position in the stream.
        /// </summary>
        /// <value>
        /// The position in the stream.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">value is negative.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public long Position
        {
            get
            {
                VerifyNotDisposed();

                return stream.Position - readLength + readOffset;
            }
            set
            {
                if (value < 0)
                {
                    ExceptionUtil.ThrowArgumentOutOfRangeException("value");
                }

                VerifyNotDisposed();

                long current = Position;

                if (value != current)
                {
                    long bufferStartOffset = current - readOffset;
                    long bufferEndOffset = bufferStartOffset + readLength;

                    // Avoid reading from the stream if the offset is within the current buffer.
                    if (value >= bufferStartOffset && value <= bufferEndOffset)
                    {
                        readOffset = (int)(value - bufferStartOffset);
                    }
                    else
                    {
                        // Invalidate the existing buffer.
                        readOffset = 0;
                        readLength = 0;
                        stream.Seek(value, SeekOrigin.Begin);
                    }
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (stream != null)
            {
                if (!leaveOpen)
                {
                    stream.Dispose();
                }
                stream = null;
            }
        }

        /// <summary>
        /// Reads the specified number of bytes from the stream, starting from a specified point in the byte array.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="offset">The starting offset in the array.</param>
        /// <param name="count">The count.</param>
        /// <returns>The number of bytes read from the stream.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        /// <exception cref="EndOfStreamException">The end of the stream has been reached.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public int Read(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
            {
                ExceptionUtil.ThrowArgumentNullException(nameof(bytes));
            }
            if (count < 0)
            {
                ExceptionUtil.ThrowArgumentOutOfRangeException(nameof(count));
            }

            VerifyNotDisposed();

            if (count == 0)
            {
                return 0;
            }

            if ((readOffset + count) <= readLength)
            {
                Buffer.BlockCopy(buffer, readOffset, bytes, offset, count);
                readOffset += count;

                return count;
            }
            else
            {
                // Ensure that any bytes at the end of the current buffer are included.
                int bytesUnread = readLength - readOffset;

                if (bytesUnread > 0)
                {
                    Buffer.BlockCopy(buffer, readOffset, bytes, offset, bytesUnread);
                }

                // Invalidate the existing buffer.
                readOffset = 0;
                readLength = 0;

                int totalBytesRead = bytesUnread;

                totalBytesRead += stream.Read(bytes, offset + bytesUnread, count - bytesUnread);

                return totalBytesRead;
            }
        }

        /// <summary>
        /// Reads the next byte from the current stream.
        /// </summary>
        /// <returns>The next byte read from the current stream.</returns>
        /// <exception cref="EndOfStreamException">The end of the stream has been reached.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public byte ReadByte()
        {
            VerifyNotDisposed();

            EnsureBuffer(sizeof(byte));

            byte val = buffer[readOffset];
            readOffset += sizeof(byte);

            return val;
        }

        /// <summary>
        /// Reads the specified number of bytes from the stream.
        /// </summary>
        /// <param name="count">The number of bytes to read..</param>
        /// <returns>An array containing the specified bytes.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        /// <exception cref="EndOfStreamException">The end of the stream has been reached.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public byte[] ReadBytes(int count)
        {
            if (count < 0)
            {
                ExceptionUtil.ThrowArgumentOutOfRangeException(nameof(count));
            }

            VerifyNotDisposed();

            if (count == 0)
            {
                return Array.Empty<byte>();
            }

            byte[] bytes = new byte[count];

            if ((readOffset + count) <= readLength)
            {
                Buffer.BlockCopy(buffer, readOffset, bytes, 0, count);
                readOffset += count;
            }
            else
            {
                // Ensure that any bytes at the end of the current buffer are included.
                int bytesUnread = readLength - readOffset;

                if (bytesUnread > 0)
                {
                    Buffer.BlockCopy(buffer, readOffset, bytes, 0, bytesUnread);
                }

                int numBytesToRead = count - bytesUnread;
                int numBytesRead = bytesUnread;
                do
                {
                    int n = stream.Read(bytes, numBytesRead, numBytesToRead);

                    if (n == 0)
                    {
                        ExceptionUtil.ThrowEndOfStreamException();
                    }

                    numBytesRead += n;
                    numBytesToRead -= n;

                } while (numBytesToRead > 0);

                // Invalidate the existing buffer.
                readOffset = 0;
                readLength = 0;
            }

            return bytes;
        }

        /// <summary>
        /// Reads a 8-byte floating point value.
        /// </summary>
        /// <returns>The 8-byte floating point value.</returns>
        /// <exception cref="EndOfStreamException">The end of the stream has been reached.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public unsafe double ReadDouble()
        {
            ulong temp = ReadUInt64();

            return *(double*)&temp;
        }

        /// <summary>
        /// Reads a 2-byte signed integer.
        /// </summary>
        /// <returns>The 2-byte signed integer.</returns>
        /// <exception cref="EndOfStreamException">The end of the stream has been reached.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public short ReadInt16()
        {
            return unchecked((short)ReadUInt16());
        }

        /// <summary>
        /// Reads a 2-byte unsigned integer.
        /// </summary>
        /// <returns>The 2-byte unsigned integer.</returns>
        /// <exception cref="EndOfStreamException">The end of the stream has been reached.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public ushort ReadUInt16()
        {
            VerifyNotDisposed();

            EnsureBuffer(sizeof(ushort));

            ushort val;

            switch (byteOrder)
            {
                case Endianess.Big:
                    val = (ushort)((buffer[readOffset] << 8) | buffer[readOffset + 1]);
                    break;
                case Endianess.Little:
                    val = (ushort)(buffer[readOffset] | (buffer[readOffset + 1] << 8));
                    break;
                default:
                    throw new InvalidOperationException("Unsupported byte order: " + byteOrder.ToString());
            }

            readOffset += sizeof(ushort);

            return val;
        }

        /// <summary>
        /// Reads a 4-byte signed integer.
        /// </summary>
        /// <returns>The 4-byte signed integer.</returns>
        /// <exception cref="EndOfStreamException">The end of the stream has been reached.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public int ReadInt32()
        {
            return unchecked((int)ReadUInt32());
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer.
        /// </summary>
        /// <returns>The 4-byte unsigned integer.</returns>
        /// <exception cref="EndOfStreamException">The end of the stream has been reached.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public uint ReadUInt32()
        {
            VerifyNotDisposed();

            EnsureBuffer(sizeof(uint));

            uint val;

            switch (byteOrder)
            {
                case Endianess.Big:
                    val = unchecked((uint)((buffer[readOffset] << 24)
                                            | (buffer[readOffset + 1] << 16)
                                            | (buffer[readOffset + 2] << 8)
                                            | buffer[readOffset + 3]));
                    break;
                case Endianess.Little:
                    val = unchecked((uint)(buffer[readOffset]
                                           | (buffer[readOffset + 1] << 8)
                                           | (buffer[readOffset + 2] << 16)
                                           | (buffer[readOffset + 3] << 24)));
                    break;
                default:
                    throw new InvalidOperationException("Unsupported byte order: " + byteOrder.ToString());
            }

            readOffset += sizeof(uint);

            return val;
        }

        /// <summary>
        /// Reads a 4-byte floating point value.
        /// </summary>
        /// <returns>The 4-byte floating point value.</returns>
        /// <exception cref="EndOfStreamException">The end of the stream has been reached.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public unsafe float ReadSingle()
        {
            uint temp = ReadUInt32();

            return *(float*)&temp;
        }

        /// <summary>
        /// Reads a 8-byte signed integer.
        /// </summary>
        /// <returns>The 8-byte signed integer.</returns>
        /// <exception cref="EndOfStreamException">The end of the stream has been reached.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public long ReadInt64()
        {
            return (long)ReadUInt64();
        }

        /// <summary>
        /// Reads a 8-byte unsigned integer.
        /// </summary>
        /// <returns>The 8-byte unsigned integer.</returns>
        /// <exception cref="EndOfStreamException">The end of the stream has been reached.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public ulong ReadUInt64()
        {
            VerifyNotDisposed();

            EnsureBuffer(sizeof(ulong));

            uint hi;
            uint lo;

            switch (byteOrder)
            {
                case Endianess.Big:
                    hi = unchecked((uint)((buffer[readOffset] << 24)
                                           | (buffer[readOffset + 1] << 16)
                                           | (buffer[readOffset + 2] << 8)
                                           | buffer[readOffset + 3]));
                    lo = unchecked((uint)((buffer[readOffset + 4] << 24)
                                           | (buffer[readOffset + 5] << 16)
                                           | (buffer[readOffset + 6] << 8)
                                           | buffer[readOffset + 7]));
                    break;
                case Endianess.Little:
                    lo = unchecked((uint)(buffer[readOffset]
                                          | (buffer[readOffset + 1] << 8)
                                          | (buffer[readOffset + 2] << 16)
                                          | (buffer[readOffset + 3] << 24)));
                    hi = unchecked((uint)(buffer[readOffset + 4]
                                          | (buffer[readOffset + 5] << 8)
                                          | (buffer[readOffset + 6] << 16)
                                          | (buffer[readOffset + 7] << 24)));
                    break;
                default:
                    throw new InvalidOperationException("Unsupported byte order: " + byteOrder.ToString());
            }

            readOffset += sizeof(ulong);

            return (((ulong)hi) << 32) | lo;
        }

        /// <summary>
        /// Reads an ASCII string from the stream.
        /// </summary>
        /// <param name="length">The length of the string.</param>
        /// <returns>The string.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is negative.</exception>
        /// <exception cref="EndOfStreamException">The end of the stream has been reached.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public string ReadAsciiString(int length)
        {
            if (length < 0)
            {
                ExceptionUtil.ThrowArgumentOutOfRangeException(nameof(length));
            }

            VerifyNotDisposed();

            if (length == 0)
            {
                return string.Empty;
            }

            EnsureBuffer(length);

            string value = Encoding.ASCII.GetString(buffer, readOffset, length);

            readOffset += length;

            return value;
        }


        /// <summary>
        /// Reads an UTF-16 string from the stream.
        /// </summary>
        /// <param name="lengthInBytes">The length of the string in bytes.</param>
        /// <returns>The string.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="lengthInBytes"/> is negative.</exception>
        /// <exception cref="EndOfStreamException">The end of the stream has been reached.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public string ReadUnicodeString(int lengthInBytes)
        {
            if (lengthInBytes < 0)
            {
                ExceptionUtil.ThrowArgumentOutOfRangeException(nameof(lengthInBytes));
            }

            VerifyNotDisposed();

            if (lengthInBytes == 0)
            {
                return string.Empty;
            }

            EnsureBuffer(lengthInBytes);

            string value;

            switch (byteOrder)
            {
                case Endianess.Big:
                    value = Encoding.BigEndianUnicode.GetString(buffer, readOffset, lengthInBytes);
                    break;
                case Endianess.Little:
                    value = Encoding.Unicode.GetString(buffer, readOffset, lengthInBytes);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported byte order: " + byteOrder.ToString());
            }

            readOffset += lengthInBytes;

            return value;
        }

        /// <summary>
        /// Ensures that the buffer contains at least the number of bytes requested.
        /// </summary>
        /// <param name="count">The minimum number of bytes the buffer should contain.</param>
        /// <exception cref="EndOfStreamException">The end of the stream has been reached.</exception>
        private void EnsureBuffer(int count)
        {
            if ((readOffset + count) > readLength)
            {
                FillBuffer(count);
            }
        }

        /// <summary>
        /// Fills the buffer with at least the number of bytes requested.
        /// </summary>
        /// <param name="minBytes">The minimum number of bytes to place in the buffer.</param>
        /// <exception cref="EndOfStreamException">The end of the stream has been reached.</exception>
        private void FillBuffer(int minBytes)
        {
            int bytesUnread = readLength - readOffset;

            if (bytesUnread > 0)
            {
                Buffer.BlockCopy(buffer, readOffset, buffer, 0, bytesUnread);
            }

            int numBytesToRead = bufferSize - bytesUnread;
            int numBytesRead = bytesUnread;
            do
            {
                int n = stream.Read(buffer, numBytesRead, numBytesToRead);

                if (n == 0)
                {
                    ExceptionUtil.ThrowEndOfStreamException();
                }

                numBytesRead += n;
                numBytesToRead -= n;

            } while (numBytesRead < minBytes);

            readOffset = 0;
            readLength = numBytesRead;
        }

        /// <summary>
        /// Verifies that the <see cref="EndianBinaryReader"/> has not been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        private void VerifyNotDisposed()
        {
            if (stream == null)
            {
                ExceptionUtil.ThrowObjectDisposedException(nameof(EndianBinaryReader));
            }
        }
    }
}
