﻿using Securify.ShellLink.Exceptions;
using System;
using System.Text;

namespace Securify.ShellLink.Structures
{
    /// <summary>
    /// The TrackerDataBlock structure specifies data that can be used to resolve a link target 
    /// if it is not found in its original location when the link is resolved. This data is 
    /// passed to the Link Tracking service o find the link target.
    /// </summary>
    public class TrackerDataBlock : ExtraDataBlock
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public TrackerDataBlock() : base()
        {
            MachineID = "";
            Droid = new Guid[2];
            DroidBirth = new Guid[2];
        }
        #endregion // Constructor

        /// <summary>
        /// MinimumBlockSize (4 bytes): A 32-bit, unsigned integer that specifies the minimum size of the 
        /// TrackerDataBlock structure. This value MUST be 0x00000060
        /// </summary>
        public override UInt32 MinimumBlockSize => 0x00000060;


        /// <summary>
        /// BlockSize (4 bytes): A 32-bit, unsigned integer that specifies the size of the 
        /// TrackerDataBlock structure. This value MUST be 0x00000060.
        /// 
        /// BUGBUG: Can BlockSize be > 0x60 if Length > 0x58???
        /// </summary>
        public override UInt32 BlockSize => Length + 8;

        /// <summary>
        /// BlockSignature (4 bytes): A 32-bit, unsigned integer that specifies the signature of 
        /// the TrackerDataBlock extra data section. This value MUST be 0xA0000003.
        /// </summary>
        public override BlockSignature BlockSignature => BlockSignature.TRACKER_PROPS;

        /// <summary>
        /// Length (4 bytes): A 32-bit, unsigned integer. This value MUST be greater than or equal 
        /// to 0x0000058.
        /// </summary>
        public UInt32 Length => (UInt32)(MachineID.Length < 16 ? 0x58 : MachineID.Length + 0x48);

        /// <summary>
        /// Version (4 bytes): A 32-bit, unsigned integer. This value MUST be 0x00000000.
        /// </summary>
        public UInt32 Version { get; set; }

        /// <summary>
        /// MachineID (variable): A character string, as defined by the system default code page, 
        /// which specifies the NetBIOS name of the machine where the link target was last known to 
        /// reside.
        /// </summary>
        public String MachineID { get; set; }

        /// <summary>
        /// Droid (32 bytes): Two values in GUID packet representation that are used to find the 
        /// link target with the Link Tracking service
        /// </summary>
        public Guid[] Droid { get; set; }

        /// <summary>
        /// DroidBirth (32 bytes): Two values in GUID packet representation that are used to find 
        /// the link target with the Link Tracking service
        /// </summary>
        public Guid[] DroidBirth { get; set; }
        
        #region GetBytes
        /// <inheritdoc />
        public override byte[] GetBytes()
        {
            byte[] TrackerDataBlock = new byte[BlockSize];
            Buffer.BlockCopy(BitConverter.GetBytes(BlockSize), 0, TrackerDataBlock, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((UInt32)BlockSignature), 0, TrackerDataBlock, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Length), 0, TrackerDataBlock, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Version), 0, TrackerDataBlock, 12, 4);
            Buffer.BlockCopy(Encoding.Default.GetBytes(MachineID), 0, TrackerDataBlock, 16, (int)MachineID.Length);
            Buffer.BlockCopy(Droid[0].ToByteArray(), 0, TrackerDataBlock, (int)BlockSize - 64, 16);
            Buffer.BlockCopy(Droid[1].ToByteArray(), 0, TrackerDataBlock, (int)BlockSize - 48, 16);
            Buffer.BlockCopy(DroidBirth[0].ToByteArray(), 0, TrackerDataBlock, (int)BlockSize - 32, 16);
            Buffer.BlockCopy(DroidBirth[1].ToByteArray(), 0, TrackerDataBlock, (int)BlockSize - 16, 16);
            return TrackerDataBlock;
        }
        #endregion // GetBytes

        #region ToString
        /// <inheritdoc />
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.ToString());
            builder.AppendFormat("Version: {0}", Version);
            builder.AppendLine();
            builder.AppendFormat("MachineID: {0}", MachineID);
            builder.AppendLine();
            builder.AppendFormat("Droid: {0} {1}", Droid[0], Droid[1]);
            builder.AppendLine();
            builder.AppendFormat("DroidBirth: {0} {1}", DroidBirth[0], DroidBirth[1]);
            builder.AppendLine();
            return builder.ToString();
        }
        #endregion // ToString

        #region FromByteArray
        /// <summary>
        /// Create a TrackerDataBlock from a given byte array
        /// </summary>
        /// <param name="ba">The byte array</param>
        /// <returns>A TrackerDataBlock object</returns>
        public static TrackerDataBlock FromByteArray(byte[] ba)
        {
            TrackerDataBlock TrackerDataBlock = new TrackerDataBlock();
            TrackerDataBlock.Validate(ref ba);

            UInt32 Length = BitConverter.ToUInt32(ba, 8);
            if (Length < 0x58)
            {
                throw new ArgumentException(String.Format("Length is {0} is incorrect (expected 88)", Length));
            }

            TrackerDataBlock.Version = BitConverter.ToUInt32(ba, 12);

            byte[] MachineID = new byte[Length - 0x48];
            Buffer.BlockCopy(ba, 16, MachineID, 0, MachineID.Length);
            TrackerDataBlock.MachineID = Encoding.Default.GetString(MachineID).TrimEnd(new char[] { (char)0 });

            byte[] Guid = new byte[16];
            Buffer.BlockCopy(ba, 16 + MachineID.Length, Guid, 0, Guid.Length);
            TrackerDataBlock.Droid = new Guid[2];
            TrackerDataBlock.Droid[0] = new Guid(Guid);
            Buffer.BlockCopy(ba, 32 + MachineID.Length, Guid, 0, Guid.Length);
            TrackerDataBlock.Droid[1] = new Guid(Guid);

            Buffer.BlockCopy(ba, 48 + MachineID.Length, Guid, 0, Guid.Length);
            TrackerDataBlock.DroidBirth = new Guid[2];
            TrackerDataBlock.DroidBirth[0] = new Guid(Guid);
            Buffer.BlockCopy(ba, 64 + MachineID.Length, Guid, 0, Guid.Length);
            TrackerDataBlock.DroidBirth[1] = new Guid(Guid);

            return TrackerDataBlock;
        }
        #endregion // FromByteArray
    }
}
