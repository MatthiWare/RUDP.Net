﻿/*  Copyright (C) 2017 MatthiWare
 *  Copyright (C) 2017 Matthias Beerens
 *  For the full notice see <https://github.com/MatthiWare/RUDP.Net/blob/master/LICENSE>. 
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MatthiWare.Net.Sockets.Base
{
    [DebuggerDisplay("Capacity: {Capacity}, Position: {position}")]
    public struct RawPacket
    {
        private byte[] buffer;
        private int position;
        private int Capacity => buffer.Length;

        public RawPacket(int initialSize)
        {
            buffer = new byte[initialSize];
            position = 0;
        }

        public RawPacket(byte[] buffer)
        {
            this.buffer = buffer;
            position = 0;
        }

        private void EnsureCapacity(int cap)
        {
            if (cap < Capacity) return;

            int newCap = cap;

            if (newCap < 256) newCap = 256;
            if (newCap < Capacity * 2) newCap = Capacity * 2;

            byte[] newBuffer = new byte[newCap];
            Buffer.BlockCopy(buffer, 0, newBuffer, 0, position);

            buffer = newBuffer;
        }

        public void Write(byte[] data, int offset, int count)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            int i = position + count;
            EnsureCapacity(i);

            if (count <= 8)
            {
                int byteCount = count;

                while (--byteCount >= 0)
                    buffer[position + byteCount] = data[offset + byteCount];
            }
            else
                Buffer.BlockCopy(data, offset, buffer, position, count);

            position = i;
        }

        public void Read(byte[] data, int offset, int count) => Buffer.BlockCopy(buffer , position, data, offset, count);

        public int ReadByte()
        {
            if (position >= Capacity)
                return -1;

            return buffer[position++];
        }

        public void WriteByte(byte value)
        {
            EnsureCapacity(position + 1);

            buffer[position++] = value;
        }

        public byte ReadUInt8()
        {
            int val = ReadByte();
            if (val == -1) throw new EndOfStreamException();
            return (byte)val;
        }

        public void WriteUInt8(byte value) => WriteByte(value);

        public sbyte ReadInt8() => (sbyte)ReadUInt8();

        public void WriteInt8(sbyte value) => WriteUInt8((byte)value);

        public ushort ReadUInt16()
        {
            return (ushort)(
                (ReadUInt8() << 8) |
                ReadUInt8());

        }

        public void WriteUInt16(ushort value)
        {
            Write(new[]
            {
                (byte)((value & 0xFF00) >> 8),
                (byte)(value & 0xFF)
            }, 0, 2);
        }

        public short ReadInt16() => (short)ReadUInt16();

        public void WriteInt16(short value) => WriteUInt16((ushort)value);

        public uint ReadUInt32()
        {
            return (uint)(
                (ReadUInt8() << 24) |
                (ReadUInt8() << 16) |
                (ReadUInt8() << 8) |
                ReadUInt8());
        }

        public void WriteUInt32(uint value)
        {
            Write(new[]
            {
                (byte)((value & 0xFF000000) >> 24),
                (byte)((value & 0xFF0000) >> 16),
                (byte)((value & 0xFF00) >> 8),
                (byte)(value & 0xFF),
            }, 0, 4);
        }

        public int ReadInt32() => (int)ReadUInt32();

        public void WriteInt32(int value) => WriteUInt32((uint)value);

        public ulong ReadUInt64()
        {
            return unchecked(
                ((ulong)ReadUInt8() << 56) |
                ((ulong)ReadUInt8() << 48) |
                ((ulong)ReadUInt8() << 40) |
                ((ulong)ReadUInt8() << 32) |
                ((ulong)ReadUInt8() << 24) |
                ((ulong)ReadUInt8() << 16) |
                ((ulong)ReadUInt8() << 8) |
                ReadUInt8());
        }

        public void WriteUInt64(ulong value)
        {
            Write(new[]
            {
                (byte)((value & 0xFF00000000000000) >> 56),
                (byte)((value & 0xFF000000000000) >> 48),
                (byte)((value & 0xFF0000000000) >> 40),
                (byte)((value & 0xFF00000000) >> 32),
                (byte)((value & 0xFF000000) >> 24),
                (byte)((value & 0xFF0000) >> 16),
                (byte)((value & 0xFF00) >> 8),
                (byte)(value & 0xFF),
            }, 0, 8);
        }

        public long ReadInt64() => (long)ReadUInt64();

        public void WriteInt64(long value) => WriteUInt64((ulong)value);

        public byte[] ReadUInt8Array(int length)
        {
            var result = new byte[length];
            if (length == 0) return result;
            Read(result, 0, length);
            return result;
        }

        public void WriteUInt8Array(byte[] value) => Write(value, 0, value.Length);

        public sbyte[] ReadInt8Array(int length) => (sbyte[])(Array)ReadUInt8Array(length);

        public void WriteInt8Array(sbyte[] value) => Write((byte[])(Array)value, 0, value.Length);

        public ushort[] ReadUInt16Array(int length)
        {
            var result = new ushort[length];
            if (length == 0) return result;
            for (int i = 0; i < length; i++)
                result[i] = ReadUInt16();
            return result;
        }

        public void WriteUInt16Array(ushort[] value)
        {
            for (int i = 0; i < value.Length; i++)
                WriteUInt16(value[i]);
        }

        public short[] ReadInt16Array(int length) => (short[])(Array)ReadUInt16Array(length);

        public void WriteInt16Array(short[] value) => WriteUInt16Array((ushort[])(Array)value);

        public uint[] ReadUInt32Array(int length)
        {
            var result = new uint[length];
            if (length == 0) return result;
            for (int i = 0; i < length; i++)
                result[i] = ReadUInt32();
            return result;
        }

        public void WriteUInt32Array(uint[] value)
        {
            for (int i = 0; i < value.Length; i++)
                WriteUInt32(value[i]);
        }

        public int[] ReadInt32Array(int length) => (int[])(Array)ReadUInt32Array(length);

        public void WriteInt32Array(int[] value) => WriteUInt32Array((uint[])(Array)value);

        public ulong[] ReadUInt64Array(int length)
        {
            var result = new ulong[length];
            if (length == 0) return result;
            for (int i = 0; i < length; i++)
                result[i] = ReadUInt64();
            return result;
        }

        public void WriteUInt64Array(ulong[] value)
        {
            for (int i = 0; i < value.Length; i++)
                WriteUInt64(value[i]);
        }

        public long[] ReadInt64Array(int length) => (long[])(Array)ReadUInt64Array(length);

        public void WriteInt64Array(long[] value) => WriteUInt64Array((ulong[])(Array)value);

        public unsafe float ReadSingle()
        {
            uint value = ReadUInt32();
            return *(float*)&value;
        }

        public unsafe void WriteSingle(float value)
        {
            WriteUInt32(*(uint*)&value);
        }

        public unsafe double ReadDouble()
        {
            ulong value = ReadUInt64();
            return *(double*)&value;
        }

        public unsafe void WriteDouble(double value)
        {
            WriteUInt64(*(ulong*)&value);
        }

        public bool ReadBool() => ReadUInt8() != 0;

        public void WriteBool(bool value) => WriteUInt8(value ? (byte)1 : (byte)0);

        public string ReadString()
        {
            ushort length = ReadUInt16();
            var data = ReadUInt8Array(length);
            return Encoding.UTF8.GetString(data);
        }

        public void WriteString(string value)
        {
            WriteUInt16((ushort)value.Length);
            WriteUInt8Array(Encoding.UTF8.GetBytes(value));
        }

        public byte[] ToBuffer()
        {
            byte[] buf = new byte[position];
            Buffer.BlockCopy(buffer, 0, buf, 0, position);
            return buf;
        }
    }
}
