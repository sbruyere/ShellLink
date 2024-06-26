﻿using Securify.ShellLink.Exceptions;
using System;
using System.Text;

namespace Securify.ShellLink.Structures
{
    /// <summary>
    /// The ShimDataBlock structure specifies the name of a shim that can be applied when 
    /// activating a link target.
    /// </summary>
    public class ShimDataBlock : ExtraDataBlock
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ShimDataBlock() : base()
        {
            LayerName = "";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="LayerName">A Unicode string that specifies the name of a shim layer to apply to a link target when it is being activated</param>
        public ShimDataBlock(String LayerName) : base()
        {
            this.LayerName = LayerName;
        }
        #endregion // Constructor

        /// <summary>
        /// MinimumBlockSize (4 bytes): A 32-bit, unsigned integer that specifies the minimum size of the 
        /// ShimDataBlock structure. This value MUST be 0x00000088
        /// </summary>
        public override UInt32 MinimumBlockSize => 0x00000088;

        /// <summary>
        /// BlockSize (4 bytes): A 32-bit, unsigned integer that specifies the size of the 
        /// ShimDataBlock structure. This value MUST be greater than or equal to 0x00000088.
        /// </summary>
        public override UInt32 BlockSize => LayerName.Length < 0x40 ? MinimumBlockSize : (uint)(LayerName.Length + 1) * 2 + 8;

        /// <summary>
        /// BlockSignature (4 bytes): A 32-bit, unsigned integer that specifies the signature 
        /// of the ShimDataBlock extra data section. This value MUST be 0xA0000008.
        /// </summary>
        public override BlockSignature BlockSignature => BlockSignature.SHIM_PROPS;

        /// <summary>
        /// LayerName (variable): A Unicode string that specifies the name of a shim layer to 
        /// apply to a link target when it is being activated.
        /// </summary>
        public String LayerName { get; set; }

        #region GetBytes
        /// <inheritdoc />
        public override byte[] GetBytes()
        {
            byte[] ShimDataBlock = new byte[BlockSize];
            Buffer.BlockCopy(BitConverter.GetBytes(BlockSize), 0, ShimDataBlock, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((UInt32)BlockSignature), 0, ShimDataBlock, 4, 4);
            Buffer.BlockCopy(Encoding.Unicode.GetBytes(LayerName), 0, ShimDataBlock, 8, LayerName.Length * 2);
            return ShimDataBlock;
        }
        #endregion // GetBytes

        #region ToString
        /// <inheritdoc />
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.ToString());
            builder.AppendFormat("LayerName: {0}", LayerName);
            builder.AppendLine();
            return builder.ToString();
        }
        #endregion // ToString

        #region FromByteArray
        /// <summary>
        /// Create a ShimDataBlock from a given byte array
        /// </summary>
        /// <param name="ba">The byte array</param>
        /// <returns>A ShimDataBlock object</returns>
        public static ShimDataBlock FromByteArray(byte[] ba)
        {
            ShimDataBlock ShimDataBlock = new ShimDataBlock();
            uint hBlockSize = ShimDataBlock.Validate(ref ba);

            byte[] LayerName = new byte[hBlockSize - 8];
            Buffer.BlockCopy(ba, 8, LayerName, 0, (int)hBlockSize - 8);
            ShimDataBlock.LayerName = Encoding.Unicode.GetString(LayerName).TrimEnd(new char[] { (char)0 });

            return ShimDataBlock;
        }
        #endregion // FromByteArray
    }
}
