using Securify.ShellLink.Exceptions;
using System;
using System.Text;

namespace Securify.ShellLink.Structures
{
    /// <summary>
    /// The ConsoleFEDataBlock structure specifies the code page to use for displaying text when a 
    /// link target specifies an application that is run in a console window.
    /// </summary>
    public class ConsoleFEDataBlock : ExtraDataBlock
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ConsoleFEDataBlock() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CodePage">Unsigned integer that specifies a code page language code identifier</param>
        public ConsoleFEDataBlock(UInt32 CodePage) : base()
        {
            this.CodePage = CodePage;
        }
        #endregion // Constructor

        /// <summary>
        /// MinimumBlockSize (4 bytes): A 32-bit, unsigned integer that specifies the minimum size of the 
        /// ConsoleFEDataBlock structure. This value MUST be 0x0000000C
        /// </summary>
        public override UInt32 MinimumBlockSize => 0x0000000C;

        /// <summary>
        /// BlockSize (4 bytes): A 32-bit, unsigned integer that specifies the size of the 
        /// ConsoleFEDataBlock structure. This value MUST be 0x0000000C.
        /// </summary>
        public override UInt32 BlockSize => MinimumBlockSize;

        /// <summary>
        /// BlockSignature (4 bytes): A 32-bit, unsigned integer that specifies the signature 
        /// of the ConsoleFEDataBlock extra data section. This value MUST be 0xA0000004.
        /// </summary>
        public override BlockSignature BlockSignature => BlockSignature.CONSOLE_FE_PROPS;

        /// <summary>
        /// CodePage (4 bytes): A 32-bit, unsigned integer that specifies a code page language code 
        /// identifier. For details concerning the structure and meaning of language code identifiers
        /// </summary>
        public UInt32 CodePage { get; set; }

        #region GetBytes
        /// <inheritdoc />
        public override byte[] GetBytes()
        {
            byte[] ConsoleFEDataBlock = new byte[BlockSize];
            Buffer.BlockCopy(BitConverter.GetBytes(BlockSize), 0, ConsoleFEDataBlock, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((UInt32)BlockSignature), 0, ConsoleFEDataBlock, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(CodePage), 0, ConsoleFEDataBlock, 8, 4);
            return ConsoleFEDataBlock;
        }
        #endregion // GetBytes

        #region ToString
        /// <inheritdoc />
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.ToString());
            builder.AppendFormat("CodePage: {0} (0x{0:X})", CodePage);
            builder.AppendLine();
            return builder.ToString();
        }
        #endregion // ToString

        #region FromByteArray
        /// <summary>
        /// Create a ConsoleFEDataBlock from a given byte array
        /// </summary>
        /// <param name="ba">The byte array</param>
        /// <returns>A ConsoleFEDataBlock object</returns>
        public static ConsoleFEDataBlock FromByteArray(byte[] ba)
        {
            ConsoleFEDataBlock ConsoleFEDataBlock = new ConsoleFEDataBlock();

            ConsoleFEDataBlock.Validate(ref ba);

            ConsoleFEDataBlock.CodePage = BitConverter.ToUInt32(ba, 8);

            return ConsoleFEDataBlock;
        }
        #endregion // FromByteArray
    }
}
