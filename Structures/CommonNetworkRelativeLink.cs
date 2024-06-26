﻿using System;
using System.Text;
using System.Linq;
using Securify.ShellLink.Flags;
using Securify.ShellLink.Exceptions;

namespace Securify.ShellLink.Structures
{
    /// <summary>
    /// The CommonNetworkRelativeLink structure specifies information about the network location where a 
    /// link target is stored, including the mapped drive letter and the UNC path prefix.
    /// </summary>
    public class CommonNetworkRelativeLink : Structure
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CommonNetworkRelativeLink() : base() { NetName = "";  }
        #endregion // Constructor

        /// <summary>
        /// headerBlockSize (4 bytes): A 32-bit, unsigned integer that specifies the size of the ExtraDataBlock structure.
        /// </summary>
        public override UInt32 MinimumBlockSize => 0x14;

        /// <summary>
        /// When set to true, wide chars will be used
        /// </summary>
        public Boolean IsUnicode => NetNameOffset > MinimumBlockSize;

        #region CommonNetworkRelativeLinkSize
        /// <summary>
        /// CommonNetworkRelativeLinkSize (4 bytes): A 32-bit, unsigned integer that specifies the size, 
        /// in bytes, of the CommonNetworkRelativeLink structure. This value MUST be greater than or equal 
        /// to 0x00000014. All offsets specified in this structure MUST be less than this value, and all 
        /// strings contained in this structure MUST fit within the extent defined by this size.
        /// </summary>
        public UInt32 CommonNetworkRelativeLinkSize
        {
            get
            {
                UInt32 Size = 0x14;

                Size += (UInt32)NetName.Length + 1;
                if ((CommonNetworkRelativeLinkFlags & CommonNetworkRelativeLinkFlags.ValidDevice) != 0)
                {
                    Size += (UInt32)DeviceName.Length + 1;
                }

                if (IsUnicode)
                {
                    Size += 8;
                    if (DeviceNameUnicode == null)
                    {
                        DeviceNameUnicode = DeviceName;
                    }
                    if (NetNameUnicode == null)
                    {
                        NetNameUnicode = NetName;
                    }
                    Size += ((UInt32)NetNameUnicode.Length + 1) * 2;
                    if ((CommonNetworkRelativeLinkFlags & CommonNetworkRelativeLinkFlags.ValidDevice) != 0)
                    {
                        Size += ((UInt32)DeviceNameUnicode.Length + 1) * 2;
                    }
                }

                return Size;
            }
        }
        #endregion // CommonNetworkRelativeLinkSize

        /// <summary>
        /// CommonNetworkRelativeLinkFlags (4 bytes): Flags that specify the contents of the 
        /// DeviceNameOffset and NetProviderType fields.
        /// </summary>
        public CommonNetworkRelativeLinkFlags CommonNetworkRelativeLinkFlags => (NetworkProviderType != 0 ? CommonNetworkRelativeLinkFlags.ValidNetType : 0) | (DeviceName != null ? CommonNetworkRelativeLinkFlags.ValidDevice : 0);

        /// <summary>
        /// NetNameOffset (4 bytes): A 32-bit, unsigned integer that specifies the location of the 
        /// NetName field. This value is an offset, in bytes, from the start of the 
        /// CommonNetworkRelativeLink structure.
        /// </summary>
        public UInt32 NetNameOffset => (UInt32)(NetNameUnicode != null ? 0x1C : 0x14);

        /// <summary>
        /// DeviceNameOffset (4 bytes): A 32-bit, unsigned integer that specifies the location of 
        /// the DeviceName field. If the ValidDevice flag is set, this value is an offset, in bytes, 
        /// from the start of the CommonNetworkRelativeLink structure; otherwise, this value MUST be 
        /// zero.
        /// </summary>
        public UInt32 DeviceNameOffset => (UInt32)((CommonNetworkRelativeLinkFlags & CommonNetworkRelativeLinkFlags.ValidDevice) != 0 ? NetNameOffset + NetName.Length + 1 : 0);

        /// <summary>
        /// NetworkProviderType (4 bytes): A 32-bit, unsigned integer that specifies the type of 
        /// network provider. If the ValidNetType flag is set, this value MUST be one of the following; 
        /// otherwise, this value MUST be ignored.
        /// </summary>
        public NetworkProviderType NetworkProviderType { get; set; }

        #region NetNameOffsetUnicode
        /// <summary>
        /// NetNameOffsetUnicode (4 bytes): An optional, 32-bit, unsigned integer that specifies the 
        /// location of the NetNameUnicode field. This value is an offset, in bytes, from the start of 
        /// the CommonNetworkRelativeLink structure. This field MUST be present if the value of the 
        /// NetNameOffset field is greater than 0x00000014; otherwise, this field MUST NOT be present.
        /// </summary>
        public UInt32 NetNameOffsetUnicode
        {
            get
            {
                if(IsUnicode)
                {
                    if((CommonNetworkRelativeLinkFlags & CommonNetworkRelativeLinkFlags.ValidDevice) != 0)
                    {
                        return (UInt32)(DeviceNameOffset + DeviceName.Length + 1);
                    }

                    return (UInt32)(NetNameOffset + NetName.Length + 1);
                }

                return 0;
            }
        }
        #endregion // NetNameOffsetUnicode

        /// <summary>
        /// DeviceNameOffsetUnicode (4 bytes): An optional, 32-bit, unsigned integer that specifies 
        /// the location of the DeviceNameUnicode field. This value is an offset, in bytes, from the 
        /// start of the CommonNetworkRelativeLink structure. This field MUST be present if the value 
        /// of the NetNameOffset field is greater than 0x00000014; otherwise, this field MUST NOT be 
        /// present.
        /// </summary>
        public UInt32 DeviceNameOffsetUnicode => (UInt32)(IsUnicode && (CommonNetworkRelativeLinkFlags & CommonNetworkRelativeLinkFlags.ValidDevice) != 0 ? NetNameOffsetUnicode + (NetNameUnicode.Length + 1) * 2 : 0);

        /// <summary>
        /// NetName (variable): A NULL–terminated string, as defined by the system default code page, 
        /// which specifies a server share path; for example, "\\server\share".
        /// </summary>
        public String NetName { get; set; }

        /// <summary>
        /// DeviceName (variable): A NULL–terminated string, as defined by the system default code 
        /// page, which specifies a device; for example, the drive letter "D:".
        /// </summary>
        public String DeviceName { get; set; }

        /// <summary>
        /// NetNameUnicode (variable): An optional, NULL–terminated, Unicode string that is the 
        /// Unicode version of the NetName string. This field MUST be present if the value of the 
        /// NetNameOffset field is greater than 0x00000014; otherwise, this field MUST NOT be present.
        /// </summary>
        public String NetNameUnicode { get; set; }

        /// <summary>
        /// DeviceNameUnicode (variable): An optional, NULL–terminated, Unicode string that is the
        /// Unicode version of the DeviceName string. This field MUST be present if the value of the 
        /// NetNameOffset field is greater than 0x00000014; otherwise, this field MUST NOT be present.
        /// </summary>
        public String DeviceNameUnicode { get; set; }

        #region GetBytes
        /// <inheritdoc />
        public override byte[] GetBytes()
        {
            byte[] CommonNetworkRelativeLink = new byte[CommonNetworkRelativeLinkSize];
            Buffer.BlockCopy(BitConverter.GetBytes(CommonNetworkRelativeLinkSize), 0, CommonNetworkRelativeLink, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((UInt32)CommonNetworkRelativeLinkFlags), 0, CommonNetworkRelativeLink, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(NetNameOffset), 0, CommonNetworkRelativeLink, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(DeviceNameOffset), 0, CommonNetworkRelativeLink, 12, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((UInt32)NetworkProviderType), 0, CommonNetworkRelativeLink, 16, 4);
            if (IsUnicode)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(NetNameOffsetUnicode), 0, CommonNetworkRelativeLink, 20, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(DeviceNameOffsetUnicode), 0, CommonNetworkRelativeLink, 24, 4);
                Buffer.BlockCopy(Encoding.Unicode.GetBytes(NetNameUnicode), 0, CommonNetworkRelativeLink, (int)NetNameOffsetUnicode, NetNameUnicode.Length * 2);
                if ((CommonNetworkRelativeLinkFlags & CommonNetworkRelativeLinkFlags.ValidDevice) != 0)
                {
                    Buffer.BlockCopy(Encoding.Unicode.GetBytes(DeviceNameUnicode), 0, CommonNetworkRelativeLink, (int)DeviceNameOffsetUnicode, DeviceNameUnicode.Length * 2);
                }
            }
            Buffer.BlockCopy(Encoding.Default.GetBytes(NetName), 0, CommonNetworkRelativeLink, (int)NetNameOffset, NetName.Length);
            if ((CommonNetworkRelativeLinkFlags & CommonNetworkRelativeLinkFlags.ValidDevice) != 0)
            {
                Buffer.BlockCopy(Encoding.Default.GetBytes(DeviceName), 0, CommonNetworkRelativeLink, (int)DeviceNameOffset, DeviceName.Length);

            }
            return CommonNetworkRelativeLink;
        }
        #endregion // GetBytes

        #region ToString
        /// <inheritdoc />
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.ToString());
            builder.AppendFormat("CommonNetworkRelativeLinkSize: {0} (0x{0:X})", CommonNetworkRelativeLinkSize);
            builder.AppendLine();
            builder.AppendFormat("CommonNetworkRelativeLinkFlags: {0}", CommonNetworkRelativeLinkFlags);
            builder.AppendLine();
            builder.AppendFormat("NetNameOffset: {0} (0x{0:X})", NetNameOffset);
            builder.AppendLine();
            builder.AppendFormat("DeviceNameOffset: {0} (0x{0:X})", DeviceNameOffset);
            builder.AppendLine();
            builder.AppendFormat("NetworkProviderType: {0}", NetworkProviderType);
            builder.AppendLine();
            if (NetNameOffset > 0x14)
            {
                builder.AppendFormat("NetNameOffsetUnicode: {0} (0x{0:X})", NetNameOffsetUnicode);
                builder.AppendLine();
                builder.AppendFormat("DeviceNameOffsetUnicode: {0} (0x{0:X})", DeviceNameOffsetUnicode);
                builder.AppendLine();

            }
            builder.AppendFormat("NetName: {0}", NetName);
            builder.AppendLine();
            if ((CommonNetworkRelativeLinkFlags & CommonNetworkRelativeLinkFlags.ValidDevice) != 0)
            {
                builder.AppendFormat("DeviceName: {0}", DeviceName);
                builder.AppendLine();
            }
            if (NetNameOffset > 0x14)
            {
                builder.AppendFormat("NetNameUnicode: {0}", NetNameUnicode);
                builder.AppendLine();
                if ((CommonNetworkRelativeLinkFlags & CommonNetworkRelativeLinkFlags.ValidDevice) != 0)
                {
                    builder.AppendFormat("DeviceNameUnicode: {0}", DeviceNameUnicode);
                    builder.AppendLine();
                }
            }
            return builder.ToString();
        }
        #endregion // ToString

        #region FromByteArray
        /// <summary>
        /// Create a CommonNetworkRelativeLink from a given byte array
        /// </summary>
        /// <param name="ba">The byte array</param>
        /// <returns>A CommonNetworkRelativeLink object</returns>

        public static CommonNetworkRelativeLink FromByteArray(byte[] ba)
        {
            CommonNetworkRelativeLink CommonNetworkRelativeLink = new CommonNetworkRelativeLink();

            CommonNetworkRelativeLink.Validate(ref ba);

            CommonNetworkRelativeLinkFlags CommonNetworkRelativeLinkFlags = (CommonNetworkRelativeLinkFlags)BitConverter.ToUInt32(ba, 4);
            UInt32 NetNameOffset = BitConverter.ToUInt32(ba, 8);
            UInt32 DeviceNameOffset = BitConverter.ToUInt32(ba, 12);
            UInt32 NetNameOffsetUnicode = 0;
            UInt32 DeviceNameOffsetUnicode = 0;
            CommonNetworkRelativeLink.NetworkProviderType = (NetworkProviderType)BitConverter.ToUInt32(ba, 16);

            if (NetNameOffset > 0x14)
            {
                NetNameOffsetUnicode = BitConverter.ToUInt32(ba, 20);
                DeviceNameOffsetUnicode = BitConverter.ToUInt32(ba, 24);
            }

            byte[] tmp = ba.Skip((int)NetNameOffset).ToArray();
            CommonNetworkRelativeLink.NetName = Encoding.Default.GetString(tmp.Take(Array.IndexOf(tmp, (byte)0x00) + 1).ToArray()).TrimEnd(new char[] { (char)0 });

            if ((CommonNetworkRelativeLinkFlags & CommonNetworkRelativeLinkFlags.ValidDevice) != 0)
            {
                tmp = ba.Skip((int)DeviceNameOffset).ToArray();
                CommonNetworkRelativeLink.DeviceName = Encoding.Default.GetString(tmp.Take(Array.IndexOf(tmp, (byte)0x00) + 1).ToArray()).TrimEnd(new char[] { (char)0 });
            }

            if (NetNameOffset > 0x14)
            {
                int Index = 0;
                tmp = ba.Skip((int)NetNameOffsetUnicode).ToArray();
                for (int i = 0; i < tmp.Length - 1; i++)
                {
                    if (tmp[i] == 0x00 && tmp[i + 1] == 0x00)
                    {
                        Index = i;
                        break;
                    }
                }
                CommonNetworkRelativeLink.NetNameUnicode = Encoding.Unicode.GetString(tmp.Take(Index + 1).ToArray()).TrimEnd(new char[] { (char)0 });

                if ((CommonNetworkRelativeLinkFlags & CommonNetworkRelativeLinkFlags.ValidDevice) != 0)
                {
                    tmp = ba.Skip((int)DeviceNameOffsetUnicode).ToArray();
                    for (int i = 0; i < tmp.Length - 1; i++)
                    {
                        if (tmp[i] == 0x00 && tmp[i + 1] == 0x00)
                        {
                            Index = i;
                            break;
                        }
                    }
                    CommonNetworkRelativeLink.DeviceNameUnicode = Encoding.Unicode.GetString(tmp.Take(Index + 1).ToArray()).TrimEnd(new char[] { (char)0 });
                }
            }

            return CommonNetworkRelativeLink;
        }
        #endregion // FromByteArray
    }
}