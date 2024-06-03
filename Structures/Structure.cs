using Securify.ShellLink.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Securify.ShellLink.Structures
{
    /// <summary>
    /// Abstract Structure class
    /// </summary>
    public abstract class Structure
    {
        /// <summary>
        /// Convert the Structure to a byte array
        /// </summary>
        /// <returns>Byte array representation of the Structure</returns>
        public abstract byte[] GetBytes();

        public List<Exception> Errors { get; private set; } = new List<Exception>();


        /// <summary>
        /// headerBlockSize (4 bytes): A 32-bit, unsigned integer that specifies the size of the ExtraDataBlock structure.
        /// </summary>
        public abstract UInt32 MinimumBlockSize { get; }

        public virtual void ValidateInputDataSize(int inputDataSize)
        {
            if (MinimumBlockSize > inputDataSize)
                throw new MinimumStructureSizeViolationException(this, (int)inputDataSize, (int)MinimumBlockSize);
        }


        public virtual uint ValidateHeaderBlockSize(ref byte[] input, bool headerSize16b = false)
        {
            uint headerBlockSize = headerSize16b ? BitConverter.ToUInt16(input, 0) : BitConverter.ToUInt32(input, 0);

            if (headerBlockSize > input.Length)
                throw new UnexpectedStructureSizeException(this, headerBlockSize, input.Length);

            return headerBlockSize;
        }

        public virtual uint Validate(ref byte[] input, bool headerSize16b = false)
        {
            ValidateInputDataSize(input.Length);
            uint headerBlockSize = ValidateHeaderBlockSize(ref input, headerSize16b);
            return headerBlockSize;
        }

        /// <inheritdoc />
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0}:", this.GetType().Name);
            builder.AppendLine();
            return builder.ToString();
        }
    }
}
