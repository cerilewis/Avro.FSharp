namespace Avro.Binary
{
    using System.IO;
    using System.Text;

    public class AvroBinaryWriter : BinaryWriter
    {
        public AvroBinaryWriter(Stream output) : base(output)
        {
        }

        public AvroBinaryWriter(Stream output, Encoding encoding) : base(output, encoding)
        {
        }

        public AvroBinaryWriter(Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen)
        {
        }

        public override void Write(int value)
        {
            this.Write((long) value);
        }

        public override void Write(long value)
        {
            ulong n = (ulong) ((value << 1) ^ (value >> 63));
            while ((n & ~0x7FUL) != 0)
            {
                this.Write((byte) ((n & 0x7f) | 0x80));
                n >>= 7;
            }

            this.Write((byte) n);
        }
    }
}