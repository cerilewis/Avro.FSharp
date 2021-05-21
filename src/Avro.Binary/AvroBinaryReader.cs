namespace Avro.Binary
{
    using System;
    using System.IO;
    using System.Text;

    public class AvroBinaryReader : BinaryReader
    {
        public AvroBinaryReader(Stream input) : base(input)
        {
        }

        public AvroBinaryReader(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        public AvroBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        public override int ReadInt32()
        {
            var value = (int) this.ReadInt64();
            return value;
        }

        public override long ReadInt64()
        {
            var b = this.ReadByte();
            var n = b & 0x7FUL;
            var shift = 7;
            while ((b & 0x80) != 0)
            {
                b = this.ReadByte();
                n |= (b & 0x7FUL) << shift;
                shift += 7;
            }

            var value = (long) n;
            var result = (-(value & 0x01L)) ^ ((value >> 1) & 0x7fffffffffffffffL);
            return result;
        }
    }
}