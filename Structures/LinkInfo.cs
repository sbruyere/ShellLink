﻿using System;
using System.Text;
using System.Linq;
using Securify.ShellLink.Flags;
using Securify.ShellLink.Exceptions;

namespace Securify.ShellLink.Structures
{
    /// <summary>
    /// The LinkInfo structure specifies information necessary to resolve a link target if it is not found in its 
    /// original location. This includes information about the volume that the target was stored on, the mapped 
    /// drive letter, and a Universal Naming Convention (UNC) form of the path if one existed when the link was 
    /// created.
    /// </summary>
    public class LinkInfo : Structure
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public LinkInfo() : base()
        {
            LocalBasePath = "";
            CommonPathSuffix = "";
        }
        #endregion // Constructor

        #region LinkInfoSize
        /// <summary>
        /// LinkInfoSize (4 bytes): A 32-bit, unsigned integer that specifies the size, in bytes, of the LinkInfo 
        /// structure. All offsets specified in this structure MUST be less than this value, and all strings 
        /// contained in this structure MUST fit within the extent defined by this size.
        /// </summary>
        public UInt32 LinkInfoSize
        {
            get
            {
                UInt32 Size = LinkInfoHeaderSize;

                if ((LinkInfoFlags & LinkInfoFlags.VolumeIDAndLocalBasePath) != 0)
                {
                    Size += VolumeID.VolumeIDSize;
                    Size += (UInt32)LocalBasePath.Length + 1;
                    if (LocalBasePathUnicode != null)
                    {
                        Size += (UInt32)LocalBasePathUnicode.Length * 2 + 2;
                    }
                }

                if ((LinkInfoFlags & LinkInfoFlags.CommonNetworkRelativeLinkAndPathSuffix) != 0)
                {
                    Size += CommonNetworkRelativeLink.CommonNetworkRelativeLinkSize;
                    Size += (UInt32)CommonPathSuffix.Length + 1;
                    if (CommonPathSuffixUnicode != null)
                    {
                        Size += (UInt32)CommonPathSuffixUnicode.Length * 2 + 2;
                    }
                }

                return Size;
            }
        }
        #endregion // LinkInfoSize

        #region LinkInfoHeaderSize
        /// <summary>
        /// LinkInfoHeaderSize (4 bytes): A 32-bit, unsigned integer that specifies the size, in bytes, of the LinkInfo 
        /// header section, which is composed of the LinkInfoSize, LinkInfoHeaderSize, LinkInfoFlags, VolumeIDOffset, 
        /// LocalBasePathOffset, CommonNetworkRelativeLinkOffset, CommonPathSuffixOffset fields, and, if included, the 
        /// LocalBasePathOffsetUnicode and CommonPathSuffixOffsetUnicode fields.
        /// </summary>
        public UInt32 LinkInfoHeaderSize
        {
            get
            {
                UInt32 Size = 0x1C;
                if ((LinkInfoFlags & LinkInfoFlags.VolumeIDAndLocalBasePath) != 0 && LocalBasePathUnicode != null)
                {
                    Size += 4;
                }
                if ((LinkInfoFlags & LinkInfoFlags.CommonNetworkRelativeLinkAndPathSuffix) != 0 && CommonPathSuffixUnicode != null)
                {
                    Size += 4;
                }
                return Size;
            }
        }
        #endregion // LinkInfoHeaderSize

        public override uint MinimumBlockSize => 0x1C;

        /// <summary>
        /// LinkInfoFlags (4 bytes): Flags that specify whether the VolumeID, LocalBasePath, LocalBasePathUnicode, 
        /// and CommonNetworkRelativeLink fields are present in this structure.
        /// </summary>
        public LinkInfoFlags LinkInfoFlags => (VolumeID != null ? LinkInfoFlags.VolumeIDAndLocalBasePath : 0) | (CommonNetworkRelativeLink != null ? LinkInfoFlags.CommonNetworkRelativeLinkAndPathSuffix : 0);

        /// <summary>
        /// VolumeIDOffset (4 bytes): A 32-bit, unsigned integer that specifies the location of the VolumeID field. 
        /// If the VolumeIDAndLocalBasePath flag is set, this value is an offset, in bytes, from the start of the 
        /// LinkInfo structure; otherwise, this value MUST be zero.
        /// </summary>
        public UInt32 VolumeIDOffset => (VolumeID != null ? LinkInfoHeaderSize : 0);

        /// <summary>
        /// LocalBasePathOffset (4 bytes): A 32-bit, unsigned integer that specifies the location of the LocalBasePath 
        /// field. If the VolumeIDAndLocalBasePath flag is set, this value is an offset, in bytes, from the start of the 
        /// LinkInfo structure; otherwise, this value MUST be zero.
        /// </summary>
        public UInt32 LocalBasePathOffset => (VolumeID != null ? LinkInfoHeaderSize + VolumeID.VolumeIDSize : 0);

        #region CommonNetworkRelativeLinkOffset
        /// <summary>
        /// CommonNetworkRelativeLinkOffset (4 bytes): A 32-bit, unsigned integer that specifies the location of the 
        /// CommonNetworkRelativeLink field. If the CommonNetworkRelativeLinkAndPathSuffix flag is set, this value is an 
        /// offset, in bytes, from the start of the LinkInfo structure; otherwise, this value MUST be zero.
        /// </summary>
        public UInt32 CommonNetworkRelativeLinkOffset
        {
            get
            {
                if (CommonNetworkRelativeLink != null)
                {
                    if (VolumeID != null)
                    {
                        return LocalBasePathOffset + (UInt32)LocalBasePath.Length + 1;
                    }
                    return LinkInfoHeaderSize;
                }
                return 0;
            }
        }
        #endregion // CommonNetworkRelativeLinkOffset

        /// <summary>
        /// CommonPathSuffixOffset (4 bytes): A 32-bit, unsigned integer that specifies the location of the CommonPathSuffix 
        /// field. This value is an offset, in bytes, from the start of the LinkInfo structure.
        /// </summary>
        public UInt32 CommonPathSuffixOffset => (CommonNetworkRelativeLink != null ? CommonNetworkRelativeLinkOffset + CommonNetworkRelativeLink.CommonNetworkRelativeLinkSize : 0);

        #region LocalBasePathOffsetUnicode
        /// <summary>
        /// LocalBasePathOffsetUnicode (4 bytes): An optional, 32-bit, unsigned integer that specifies the location of the 
        /// LocalBasePathUnicode field. If the VolumeIDAndLocalBasePath flag is set, this value is an offset, in bytes, from 
        /// the start of the LinkInfo structure; otherwise, this value MUST be zero. This field can be present only if the 
        /// value of the LinkInfoHeaderSize field is greater than or equal to 0x00000024.
        /// </summary>
        public UInt32 LocalBasePathOffsetUnicode
        {
            get
            {
                if (LocalBasePathUnicode != null)
                {
                    if (VolumeID != null)
                    {
                        if(CommonNetworkRelativeLink != null)
                        {
                            return CommonPathSuffixOffset + (UInt32)CommonPathSuffix.Length + 2;
                        }

                        return LocalBasePathOffset + (UInt32)LocalBasePath.Length + 1;
                    }
                }

                return 0;
            }
        }
        #endregion // LocalBasePathOffsetUnicode

        #region CommonPathSuffixOffsetUnicode
        /// <summary>
        /// CommonPathSuffixOffsetUnicode (4 bytes): An optional, 32-bit, unsigned integer that specifies the location of the 
        /// CommonPathSuffixUnicode field. This value is an offset, in bytes, from the start of the LinkInfo structure. This 
        /// field can be present only if the value of the LinkInfoHeaderSize field is greater than or equal to 0x00000024.
        /// </summary>
        public UInt32 CommonPathSuffixOffsetUnicode
        {
            get
            {
                if(CommonPathSuffixUnicode != null && CommonNetworkRelativeLink != null)
                {
                    if (VolumeID != null)
                    {
                        if(LocalBasePathUnicode != null)
                        {
                            return LocalBasePathOffsetUnicode + (UInt32)LocalBasePathUnicode.Length + 2;
                        }
                        return CommonPathSuffixOffset + (UInt32)CommonPathSuffix.Length + 1;
                    }
                    return CommonPathSuffixOffset + (UInt32)CommonPathSuffix.Length + 1;
                }
                return 0;
            }
        }
        #endregion // CommonPathSuffixOffsetUnicode

        /// <summary>
        /// VolumeID (variable): An optional VolumeID structure (section 2.3.1) that specifies information about the volume that 
        /// the link target was on when the link was created. This field is present if the VolumeIDAndLocalBasePath flag is set.
        /// </summary>
        public VolumeID VolumeID { get; set; }

        /// <summary>
        /// LocalBasePath (variable): An optional, NULL–terminated string, defined by the system default code page, which is 
        /// used to construct the full path to the link item or link target by appending the string in the CommonPathSuffix 
        /// field. This field is present if the VolumeIDAndLocalBasePath flag is set.
        /// </summary>
        public String LocalBasePath { get; set; }

        /// <summary>
        /// CommonNetworkRelativeLink (variable): An optional CommonNetworkRelativeLink structure that specifies information 
        /// about the network location where the link target is stored.
        /// </summary>
        public CommonNetworkRelativeLink CommonNetworkRelativeLink { get; set; }

        /// <summary>
        /// CommonPathSuffix (variable): A NULL–terminated string, defined by the system default code page, which is used to 
        /// construct the full path to the link item or link target by being appended to the string in the LocalBasePath field.
        /// </summary>
        public string CommonPathSuffix { get; set; }

        /// <summary>
        /// LocalBasePathUnicode (variable): An optional, NULL–terminated, Unicode string that is used to construct the full 
        /// path to the link item or link target by appending the string in the CommonPathSuffixUnicode field. This field can 
        /// be present only if the VolumeIDAndLocalBasePath flag is set and the value of the LinkInfoHeaderSize field is greater 
        /// than or equal to 0x00000024.
        /// </summary>
        public string LocalBasePathUnicode { get; set; }

        /// <summary>
        /// CommonPathSuffixUnicode (variable): An optional, NULL–terminated, Unicode string that is used to construct the
        /// full path to the link item or link target by being appended to the string in the LocalBasePathUnicode field. This
        /// field can be present only if the value of the LinkInfoHeaderSize field is greater than or equal to 0x00000024.
        /// </summary>
        public string CommonPathSuffixUnicode { get; set; }

        #region GetBytes
        /// <inheritdoc />
        public override byte[] GetBytes()
        {
            byte[] LinkInfo = new byte[LinkInfoSize];
            Buffer.BlockCopy(BitConverter.GetBytes(LinkInfoSize), 0, LinkInfo, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(LinkInfoHeaderSize), 0, LinkInfo, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((UInt32)LinkInfoFlags), 0, LinkInfo, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(VolumeIDOffset), 0, LinkInfo, 12, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(LocalBasePathOffset), 0, LinkInfo, 16, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(CommonNetworkRelativeLinkOffset), 0, LinkInfo, 20, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(CommonPathSuffixOffset), 0, LinkInfo, 24, 4);

            if (LinkInfoHeaderSize > 0x1C)
            {
                if ((LinkInfoFlags & LinkInfoFlags.VolumeIDAndLocalBasePath) != 0)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(LocalBasePathOffsetUnicode), 0, LinkInfo, 28, 4);
                    Buffer.BlockCopy(Encoding.Unicode.GetBytes(LocalBasePathUnicode), 0, LinkInfo, (int)LocalBasePathOffsetUnicode, LocalBasePathUnicode.Length * 2);
                }

                if ((LinkInfoFlags & LinkInfoFlags.CommonNetworkRelativeLinkAndPathSuffix) != 0)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(CommonPathSuffixOffsetUnicode), 0, LinkInfo, 32, 4);
                    Buffer.BlockCopy(Encoding.Unicode.GetBytes(CommonPathSuffixUnicode), 0, LinkInfo, (int)CommonPathSuffixOffsetUnicode, CommonPathSuffixUnicode.Length * 2);
                }
            }

            if ((LinkInfoFlags & LinkInfoFlags.VolumeIDAndLocalBasePath) != 0)
            {
                Buffer.BlockCopy(VolumeID.GetBytes(), 0, LinkInfo, (int)VolumeIDOffset, (int)VolumeID.VolumeIDSize);
                Buffer.BlockCopy(Encoding.Default.GetBytes(LocalBasePath), 0, LinkInfo, (int)LocalBasePathOffset, LocalBasePath.Length);
            }

            if ((LinkInfoFlags & LinkInfoFlags.CommonNetworkRelativeLinkAndPathSuffix) != 0)
            {
                Buffer.BlockCopy(CommonNetworkRelativeLink.GetBytes(), 0, LinkInfo, (int)CommonNetworkRelativeLinkOffset, (int)CommonNetworkRelativeLink.CommonNetworkRelativeLinkSize);
                Buffer.BlockCopy(Encoding.Default.GetBytes(CommonPathSuffix), 0, LinkInfo, (int)CommonPathSuffixOffset, CommonPathSuffix.Length);
            }
            return LinkInfo;
        }
        #endregion // GetBytes

        #region ToString
        /// <inheritdoc />
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.ToString());
            builder.AppendFormat("LinkInfoSize: {0} (0x{0:X})", LinkInfoSize);
            builder.AppendLine();
            builder.AppendFormat("LinkInfoHeaderSize: {0} (0x{0:X})", LinkInfoHeaderSize);
            builder.AppendLine();
            builder.AppendFormat("LinkInfoFlags: {0}", LinkInfoFlags);
            builder.AppendLine();
            builder.AppendFormat("VolumeIDOffset: {0} (0x{0:X})", VolumeIDOffset);
            builder.AppendLine();
            builder.AppendFormat("LocalBasePathOffset: {0} (0x{0:X})", LocalBasePathOffset);
            builder.AppendLine();
            builder.AppendFormat("CommonNetworkRelativeLinkOffset: {0} (0x{0:X})", CommonNetworkRelativeLinkOffset);
            builder.AppendLine();
            builder.AppendFormat("CommonPathSuffixOffset: {0} (0x{0:X})", CommonPathSuffixOffset);
            builder.AppendLine();
            if (LinkInfoHeaderSize > 0x1C)
            {
                if ((LinkInfoFlags & LinkInfoFlags.VolumeIDAndLocalBasePath) != 0)
                {
                    builder.AppendFormat("LocalBasePathOffsetUnicode: {0} (0x{0:X})", LocalBasePathOffsetUnicode);
                    builder.AppendLine();
                }
                if ((LinkInfoFlags & LinkInfoFlags.CommonNetworkRelativeLinkAndPathSuffix) != 0)
                {
                    builder.AppendFormat("CommonPathSuffixOffsetUnicode: {0} (0x{0:X})", CommonPathSuffixOffsetUnicode);
                    builder.AppendLine();
                }
            }
            if ((LinkInfoFlags & LinkInfoFlags.VolumeIDAndLocalBasePath) != 0)
            {
                builder.Append(VolumeID.ToString());
                builder.AppendFormat("LocalBasePath: {0}", LocalBasePath);
                builder.AppendLine();
            }
            if ((LinkInfoFlags & LinkInfoFlags.CommonNetworkRelativeLinkAndPathSuffix) != 0)
            {
                builder.Append(CommonNetworkRelativeLink.ToString());
            }
            builder.AppendFormat("CommonPathSuffix: {0}", CommonPathSuffix);
            builder.AppendLine();
            if (LinkInfoHeaderSize > 0x1C)
            {
                if ((LinkInfoFlags & LinkInfoFlags.VolumeIDAndLocalBasePath) != 0)
                {
                    builder.AppendFormat("LocalBasePathUnicode: {0}", LocalBasePathUnicode);
                    builder.AppendLine();
                }
                if ((LinkInfoFlags & LinkInfoFlags.CommonNetworkRelativeLinkAndPathSuffix) != 0)
                {
                    builder.AppendFormat("CommonPathSuffixUnicode: {0}", CommonPathSuffixUnicode);
                    builder.AppendLine();
                }
            }
            return builder.ToString();
        }
        #endregion // ToString

        #region FromByteArray
        /// <summary>
        /// Create a LinkInfo from a given byte array
        /// </summary>
        /// <param name="ba">The byte array</param>
        /// <returns>A LinkInfo object</returns>
        public static LinkInfo FromByteArray(byte[] ba)
        {
            LinkInfo LinkInfo = new LinkInfo();
            LinkInfo.Validate(ref ba);

            UInt32 LinkInfoHeaderSize = BitConverter.ToUInt32(ba, 4);
            if (LinkInfoHeaderSize < LinkInfo.MinimumBlockSize)
            {
                throw new ArgumentException(String.Format("The LinkInfoHeaderSize is {0} is incorrect)", LinkInfoHeaderSize));
            }

            LinkInfoFlags LinkInfoFlags = (LinkInfoFlags)BitConverter.ToUInt32(ba, 8);

            // TODO: check offsets
            UInt32 VolumeIDOffset = BitConverter.ToUInt32(ba, 12);
            UInt32 LocalBasePathOffset = BitConverter.ToUInt32(ba, 16);
            UInt32 CommonNetworkRelativeLinkOffset = BitConverter.ToUInt32(ba, 20);
            UInt32 CommonPathSuffixOffset = BitConverter.ToUInt32(ba, 24);
            UInt32 LocalBasePathOffsetUnicode = 0;
            UInt32 CommonPathSuffixOffsetUnicode = 0;

            if (LinkInfoHeaderSize > LinkInfo.MinimumBlockSize)
            {
                if ((LinkInfoFlags & LinkInfoFlags.VolumeIDAndLocalBasePath) != 0)
                {
                    LocalBasePathOffsetUnicode = BitConverter.ToUInt32(ba, 28);
                }
                if ((LinkInfoFlags & LinkInfoFlags.CommonNetworkRelativeLinkAndPathSuffix) != 0)
                {
                    CommonPathSuffixOffsetUnicode = BitConverter.ToUInt32(ba, 32);
                }
            }

            byte[] tmp;
            if ((LinkInfoFlags & LinkInfoFlags.VolumeIDAndLocalBasePath) != 0)
            {
                tmp = ba.Skip((int)VolumeIDOffset).ToArray();
                LinkInfo.VolumeID = VolumeID.FromByteArray(tmp);
                tmp = ba.Skip((int)LocalBasePathOffset).ToArray();
                LinkInfo.LocalBasePath = Encoding.Default.GetString(tmp.Take(Array.IndexOf(tmp, (byte)0x00) + 1).ToArray()).TrimEnd(new char[] { (char)0 });
            }

            if ((LinkInfoFlags & LinkInfoFlags.CommonNetworkRelativeLinkAndPathSuffix) != 0)
            {
                tmp = ba.Skip((int)CommonNetworkRelativeLinkOffset).ToArray();
                LinkInfo.CommonNetworkRelativeLink = CommonNetworkRelativeLink.FromByteArray(tmp);
                tmp = ba.Skip((int)CommonPathSuffixOffset).ToArray();
                LinkInfo.CommonPathSuffix = Encoding.Default.GetString(tmp.Take(Array.IndexOf(tmp, (byte)0x00) + 1).ToArray()).TrimEnd(new char[] { (char)0 });
            }

            if (LinkInfoHeaderSize >= 0x24)
            {
                if ((LinkInfoFlags & LinkInfoFlags.VolumeIDAndLocalBasePath) != 0)
                {
                    int Index = 0;
                    tmp = ba.Skip((int)LocalBasePathOffsetUnicode).ToArray();
                    for (int i = 0; i < tmp.Length - 1; i++)
                    {
                        if (tmp[i] == 0x00 && tmp[i + 1] == 0x00)
                        {
                            Index = i;
                            break;
                        }
                    }

                    LinkInfo.LocalBasePathUnicode = Encoding.Unicode.GetString(tmp.Take(Index + 1).ToArray()).TrimEnd(new char[] { (char)0 });
                }

                if ((LinkInfoFlags & LinkInfoFlags.CommonNetworkRelativeLinkAndPathSuffix) != 0)
                {
                    int Index = 0;
                    tmp = ba.Skip((int)CommonPathSuffixOffsetUnicode).ToArray();
                    for (int i = 0; i < tmp.Length - 1; i++)
                    {
                        if (tmp[i] == 0x00 && tmp[i + 1] == 0x00)
                        {
                            Index = i;
                            break;
                        }
                    }
                    LinkInfo.CommonPathSuffixUnicode = Encoding.Unicode.GetString(tmp.Take(Index + 1).ToArray()).TrimEnd(new char[] { (char)0 });
                }
            }

            return LinkInfo;
        }
        #endregion // FromByteArray
    }
}
