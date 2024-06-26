﻿using System;
using System.Text;
using Securify.ShellLink.Exceptions;
using Securify.ShellLink.Flags;

namespace Securify.ShellLink.Structures
{
    /// <summary>
    /// The SpecialFolderDataBlock structure specifies the location of a special folder. This data can be 
    /// used when a link target is a special folder to keep track of the folder, so that the link target 
    /// IDList can be translated when the link is loaded.
    /// </summary>
    public class SpecialFolderDataBlock : ExtraDataBlock
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public SpecialFolderDataBlock() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="SpecialFolderID">nsigned integer that specifies the folder integer ID.</param>
        /// <param name="Offset">The offset, in bytes, into the link target IDList.</param>
        public SpecialFolderDataBlock(CSIDL SpecialFolderID, UInt32 Offset) : base()
        {
            this.SpecialFolderID = SpecialFolderID;
            this.Offset = Offset;
        }
        #endregion // Constructor

        /// <summary>
        /// MinimumBlockSize (4 bytes): A 32-bit, unsigned integer that specifies the minimum size of the 
        /// SpecialFolderDataBlock structure. This value MUST be 0x00000010
        /// </summary>
        public override UInt32 MinimumBlockSize => 0x00000010;

        /// <summary>
        /// BlockSize (4 bytes): A 32-bit, unsigned integer that specifies the size of the 
        /// SpecialFolderDataBlock structure. This value MUST be 0x00000010.
        /// </summary>
        public override UInt32 BlockSize => MinimumBlockSize;

        /// <summary>
        /// BlockSignature (4 bytes): A 32-bit, unsigned integer that specifies the signature 
        /// of the SpecialFolderDataBlock extra data section. This value MUST be 0xA0000005.
        /// </summary>
        public override BlockSignature BlockSignature => BlockSignature.SPECIAL_FOLDER_PROPS;

        /// <summary>
        /// SpecialFolderID (4 bytes): A 32-bit, unsigned integer that specifies the folder 
        /// integer ID.
        /// </summary>
        public CSIDL SpecialFolderID { get; set; }

        /// <summary>
        /// Offset (4 bytes): A 32-bit, unsigned integer that specifies the location of the 
        /// ItemID of the first child segment of the IDList specified by SpecialFolderID. 
        /// This value is the offset, in bytes, into the link target IDList.
        /// </summary>
        public UInt32 Offset { get; set; }

        #region GetBytes
        /// <inheritdoc />
        public override byte[] GetBytes()
        {
            byte[] SpecialFolderDataBlock = new byte[BlockSize];
            Buffer.BlockCopy(BitConverter.GetBytes(BlockSize), 0, SpecialFolderDataBlock, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((UInt32)BlockSignature), 0, SpecialFolderDataBlock, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((UInt32)SpecialFolderID), 0, SpecialFolderDataBlock, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Offset), 0, SpecialFolderDataBlock, 12, 4);
            return SpecialFolderDataBlock;
        }
        #endregion // GetBytes

        #region ToString
        /// <inheritdoc />
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.ToString());
            builder.AppendFormat("SpecialFolderID: {0}", SpecialFolderID);
            builder.AppendLine();
            builder.AppendFormat("Offset: {0} (0x{0:X})", Offset);
            builder.AppendLine();
            return builder.ToString();
        }
        #endregion // ToString

        #region FromByteArray
        /// <summary>
        /// Create a SpecialFolderDataBlock from a given byte array
        /// </summary>
        /// <param name="ba">The byte array</param>
        /// <returns>A SpecialFolderDataBlock object</returns>
        public static SpecialFolderDataBlock FromByteArray(byte[] ba)
        {
            SpecialFolderDataBlock SpecialFolderDataBlock = new SpecialFolderDataBlock();
            SpecialFolderDataBlock.Validate(ref ba);

            SpecialFolderDataBlock.SpecialFolderID = (CSIDL)BitConverter.ToUInt32(ba, 8);
            SpecialFolderDataBlock.Offset = BitConverter.ToUInt32(ba, 12);

            return SpecialFolderDataBlock;
        }
        #endregion // FromByteArray
    }
}
