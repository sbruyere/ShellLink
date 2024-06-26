﻿using System;
using System.Text;
using Securify.ShellLink.Exceptions;
using Securify.ShellLink.Internal;

namespace Securify.ShellLink.Structures
{
    /// <summary>
    /// TerminalBlock (4 bytes): A 32-bit, unsigned integer that indicates the end of 
    /// the extra data section. This value MUST be less than 0x00000004.
    /// </summary>
    public class TerminalBlock : Structure
    {
        public override uint MinimumBlockSize => 4;
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public TerminalBlock() : base() { }
        #endregion // Constructor

        #region GetBytes
        /// <inheritdoc />
        public override byte[] GetBytes() => new byte[] { 0x00, 0x00, 0x00, 0x00 };
        #endregion // GetBytes

        #region ToString
        /// <inheritdoc />
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.ToString());
            builder.AppendFormat("TerminalBlock: {0}", BitConverter.ToString(GetBytes()).Replace("-", " "));
            builder.AppendLine();
            return builder.ToString();
        }
        #endregion // ToString

        #region FromByteArray
        /// <summary>
        /// Create a TerminalBlock from a given byte array
        /// </summary>
        /// <param name="ba">The byte array</param>
        /// <returns>A TerminalBlock object</returns>
        public static TerminalBlock FromByteArray(byte[] ba)
        {
            TerminalBlock terminalBlock = new TerminalBlock();

            terminalBlock.ValidateInputDataSize(ba.Length);

            return terminalBlock;
        }
        #endregion // FromByteArray
    }
}
