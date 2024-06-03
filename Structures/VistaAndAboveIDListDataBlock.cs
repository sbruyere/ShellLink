using Securify.ShellLink.Exceptions;
using System;
using System.Linq;
using System.Text;

namespace Securify.ShellLink.Structures
{
    /// <summary>
    /// The VistaAndAboveIDListDataBlock structure specifies an alternate IDList that can 
    /// be used instead of the LinkTargetIDList structure (section 2.2) on platforms that 
    /// support it
    /// </summary>
    public class VistaAndAboveIDListDataBlock : ExtraDataBlock
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public VistaAndAboveIDListDataBlock() : base()
        {
            IDList = new IDList();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="IDList">An IDList structure</param>
        public VistaAndAboveIDListDataBlock(IDList IDList) : base()
        {
            this.IDList = IDList;
        }
        #endregion // Constructor

        /// <summary>
        /// MinimumBlockSize (4 bytes): A 32-bit, unsigned integer that specifies the minimum size of the 
        /// VistaAndAboveIDListDataBlock structure. This value MUST be 0x0000000A
        /// </summary>
        public override UInt32 MinimumBlockSize => 0x0000000A;

        /// <summary>
        /// BlockSize (4 bytes): A 32-bit, unsigned integer that specifies the size of the 
        /// VistaAndAboveIDListDataBlock structure. This value MUST be greater than or equal 
        /// to 0x0000000A.
        /// </summary>
        public override UInt32 BlockSize => 8 + (UInt32)IDList.IDListSize;

        /// <summary>
        /// BlockSignature (4 bytes): A 32-bit, unsigned integer that specifies the signature of 
        /// the VistaAndAboveIDListDataBlock extra data section. This value MUST be 0xA000000C.
        /// </summary>
        public override BlockSignature BlockSignature => BlockSignature.VISTA_AND_ABOVE_IDLIST_PROPS;

        /// <summary>
        /// IDList (variable): An IDList structure.
        /// </summary>
        public IDList IDList { get; set; }

        #region GetBytes
        /// <inheritdoc />
        public override byte[] GetBytes()
        {
            byte[] VistaAndAboveIDListDataBlock = new byte[BlockSize];
            Buffer.BlockCopy(BitConverter.GetBytes(BlockSize), 0, VistaAndAboveIDListDataBlock, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((UInt32)BlockSignature), 0, VistaAndAboveIDListDataBlock, 4, 4);
            Buffer.BlockCopy(IDList.GetBytes(), 0, VistaAndAboveIDListDataBlock, 8, (int)BlockSize - 8);
            return VistaAndAboveIDListDataBlock;
        }
        #endregion // GetBytes

        #region ToString
        /// <inheritdoc />
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.ToString());
            builder.Append(IDList.ToString());
            return builder.ToString();
        }
        #endregion // ToString

        #region FromByteArray
        /// <summary>
        /// Create a VistaAndAboveIDListDataBlock from a given byte array
        /// </summary>
        /// <param name="ba">The byte array</param>
        /// <returns>A VistaAndAboveIDListDataBlock object</returns>
        public static VistaAndAboveIDListDataBlock FromByteArray(byte[] ba)
        {
            VistaAndAboveIDListDataBlock VistaAndAboveIDListDataBlock = new VistaAndAboveIDListDataBlock();

            uint hBlockSize = VistaAndAboveIDListDataBlock.Validate(ref ba);

            ba = ba.Skip(8).ToArray();
            UInt32 Count = (uint)hBlockSize - 8;
            while (Count > 0)
            {
                UInt16 ItemIDSize = BitConverter.ToUInt16(ba, 0);
                if (ItemIDSize != 0)
                {
                    byte[] itemID = new byte[ItemIDSize - 2];
                    Buffer.BlockCopy(ba, 2, itemID, 0, itemID.Length);
                    Count -= ItemIDSize;
                    VistaAndAboveIDListDataBlock.IDList.ItemIDList.Add(new ItemID(itemID));
                    ba = ba.Skip(ItemIDSize).ToArray();
                }
                else
                {
                    break;
                }
            }

            return VistaAndAboveIDListDataBlock;
        }
        #endregion // FromByteArray
    }
}
