using System;
using System.Text;
using Securify.ShellLink.Const;
using Securify.ShellLink.Exceptions;

namespace Securify.ShellLink.Structures
{
    /// <summary>
    /// The KnownFolderDataBlock structure specifies the location of a known folder. This data can be used when a 
    /// link target is a known folder to keep track of the folder so that the link target IDList can be translated 
    /// when the link is loaded.
    /// </summary>
    public class KnownFolderDataBlock : ExtraDataBlock
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public KnownFolderDataBlock() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="KnownFolderID">A value in GUID packet representation that specifies the folder GUID ID</param>
        /// <param name="Offset">A 32-bit, unsigned integer that specifies the location of the ItemID of the 
        /// first child segment of the IDList specified by KnownFolderID</param>
        public KnownFolderDataBlock(Guid KnownFolderID, UInt32 Offset) : base()
        {
            this.KnownFolderID = KnownFolderID;
            this.Offset = Offset;
        }
        #endregion // Constructor

        /// <summary>
        /// MinimumBlockSize (4 bytes): A 32-bit, unsigned integer that specifies the minimum size of the 
        /// KnownFolderDataBlock structure. This value MUST be 0x0000001C
        /// </summary>
        public override UInt32 MinimumBlockSize => 0x0000001C;

        /// <summary>
        /// BlockSize (4 bytes): A 32-bit, unsigned integer that specifies the size of the KnownFolderDataBlock 
        /// structure. This value MUST be 0x0000001C.
        /// </summary>
        public override UInt32 BlockSize => MinimumBlockSize;

        /// <summary>
        /// BlockSignature (4 bytes): A 32-bit, unsigned integer that specifies the signature of the 
        /// KnownFolderDataBlock extra data section. This value MUST be 0xA000000B.
        /// </summary>
        public override BlockSignature BlockSignature => BlockSignature.KNOWN_FOLDER_PROPS;

        /// <summary>
        /// KnownFolderID (16 bytes): A value in GUID packet representation that specifies the folder GUID ID.
        /// </summary>
        public Guid KnownFolderID { get; set; }

        /// <summary>
        /// Offset (4 bytes): A 32-bit, unsigned integer that specifies the location of the ItemID of the 
        /// first child segment of the IDList specified by KnownFolderID. This value is the offset, in bytes, 
        /// into the link target IDList.
        /// </summary>
        public UInt32 Offset { get; set; }

        #region GetBytes
        /// <inheritdoc />
        public override byte[] GetBytes()
        {
            byte[] KnownFolderDataBlock = new byte[BlockSize];
            Buffer.BlockCopy(BitConverter.GetBytes(BlockSize), 0, KnownFolderDataBlock, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((UInt32)BlockSignature), 0, KnownFolderDataBlock, 4, 4);
            Buffer.BlockCopy(KnownFolderID.ToByteArray(), 0, KnownFolderDataBlock, 8, 16);
            Buffer.BlockCopy(BitConverter.GetBytes(Offset), 0, KnownFolderDataBlock, 24, 4);
            return KnownFolderDataBlock;
        }
        #endregion // GetBytes

        #region ToString
        /// <inheritdoc />
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.ToString());
            builder.AppendFormat("DisplayName: {0}", KNOWNFOLDERID.GetDisplayName(KnownFolderID));
            builder.AppendLine();
            builder.AppendFormat("KnownFolderID: {0}", KnownFolderID);
            builder.AppendLine();
            builder.AppendFormat("Offset: {0} (0x{0:X})", Offset);
            builder.AppendLine();
            return builder.ToString();
        }
        #endregion // ToString

        #region FromByteArray
        /// <summary>
        /// Create a KnownFolderDataBlock from a given byte array
        /// </summary>
        /// <param name="ba">The byte array</param>
        /// <returns>A KnownFolderDataBlock object</returns>
        public static KnownFolderDataBlock FromByteArray(byte[] ba)
        {
            KnownFolderDataBlock KnownFolderDataBlock = new KnownFolderDataBlock();

            KnownFolderDataBlock.Validate(ref ba);

            byte[] KnownFolderID = new byte[16];
            Buffer.BlockCopy(ba, 8, KnownFolderID, 0, KnownFolderID.Length);
            KnownFolderDataBlock.KnownFolderID = new Guid(KnownFolderID);
            KnownFolderDataBlock.Offset = BitConverter.ToUInt32(ba, 24);

            return KnownFolderDataBlock;
        }
        #endregion // FromByteArray
    }
}
