﻿using System;
using System.Text;
using Securify.ShellLink.Internal;
using System.Runtime.InteropServices;
using Securify.ShellLink.Exceptions;

namespace Securify.ShellLink.Structures
{
    /// <summary>
    /// An ItemID is an element in an IDList structure. The data stored in a given ItemID is defined 
    /// by the source that corresponds to the location in the target namespace of the preceding ItemIDs. 
    /// This data uniquely identifies the items in that part of the namespace.
    /// </summary>
    public class ItemID : Structure
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ItemID() : this(new byte[0]) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemID">An ItemID value</param>
        public ItemID(byte[] itemID) { Data = itemID; }
        #endregion // Constructor

        #region GetBytes
        /// <inheritdoc />
        public override byte[] GetBytes()
        {
            byte[] ItemID = new byte[ItemIDSize];
            Buffer.BlockCopy(BitConverter.GetBytes(ItemIDSize), 0, ItemID, 0, 2);
            Buffer.BlockCopy(Data, 0, ItemID, 2, Data.Length);
            return ItemID;
        }
        #endregion // GetBytes

        public override uint MinimumBlockSize => 2;

        /// <summary>
        /// ItemIDSize (2 bytes): A 16-bit, unsigned integer that specifies the size, in bytes, of the 
        /// ItemID structure, including the ItemIDSize field.
        /// </summary>
        public UInt16 ItemIDSize => (UInt16)(Data.Length + MinimumBlockSize);

        /// <summary>
        /// Data (variable): The shell data source-defined data that specifies an item.
        /// </summary>
        public byte[] Data { get; set; }

        #region DisplayName
        /// <summary>
        /// Retrieves the display name of an item identified by its IDList.
        /// </summary>
        public String DisplayName
        {
            get
            {
                IntPtr pszName;
                if (Win32.SHGetNameFromIDList(GetBytes(), SIGDN.SIGDN_PARENTRELATIVE, out pszName) == 0)
                {
                    try
                    {
                        return Marshal.PtrToStringAuto(pszName);
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                    finally
                    {
                        Win32.CoTaskMemFree(pszName);
                    }
                }
                return "";
            }
        }
        #endregion // DisplayName

        #region ToString
        /// <inheritdoc />
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.ToString());
            builder.AppendFormat("ItemIDSize: {0}", ItemIDSize);
            builder.AppendLine();
            builder.AppendFormat("DisplayName: {0}", DisplayName);
            builder.AppendLine();
            builder.AppendFormat("Data: {0}", BitConverter.ToString(Data).Replace("-", " "));
            builder.AppendLine();
            return builder.ToString();
        }
        #endregion // ToString

        #region FromByteArray
        /// <summary>
        /// Create an ItemID from a given byte array
        /// </summary>
        /// <param name="ba">The byte array</param>
        /// <returns>An ItemID object</returns>
        public static ItemID FromByteArray(byte[] ba)
        {
            ItemID ItemId = new ItemID();

            UInt16 ItemIDSize = (ushort)ItemId.Validate(ref ba, headerSize16b: true);
            
            ItemId.Data = new byte[ItemIDSize - ItemId.MinimumBlockSize];
            Buffer.BlockCopy(ba, (int)ItemId.MinimumBlockSize, ItemId.Data, 0, ItemId.Data.Length);

            return ItemId;
        }
        #endregion // FromByteArray
    }
}
