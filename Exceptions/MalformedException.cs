using System;
using Securify.ShellLink.Structures;

namespace Securify.ShellLink.Exceptions
{
    internal abstract class MalformedStructureException : Exception
    {
        public Structure Structure { get; set; }
        public string StructName { get; set; }

        public MalformedStructureException(Structure structure, string message):
            base(message)
        {
            Structure = structure;
            StructName = structure.GetType().Name;
        }

    }

    internal class MinimumStructureSizeViolationException : MalformedStructureException
    {
        public int BlockSize { get; set; }
        public int MinimumBlockSize { get; set; }

        public MinimumStructureSizeViolationException(Structure structure, int blockSize, int minimumBlockSize)
            : base(structure: structure, message: $"Size of the <{structure.GetType().Name}> is less than minimum expected block size <{minimumBlockSize}> (size: <{blockSize}>)")
        {
            BlockSize = blockSize;
            MinimumBlockSize = minimumBlockSize;
        }
    }

    internal class UnexpectedStructureSizeException : MalformedStructureException
    {
        public uint BlockSize { get; set; }
        public uint ExpectedBlockSize { get; set; }

        public UnexpectedStructureSizeException(Structure structure, uint blockSize, int expectedBlockSize)
            : base(structure: structure, message: $"Size of the <{structure.GetType().Name}> has unexpected size of <{blockSize}> (expected <{expectedBlockSize}>)")
        {
            BlockSize = blockSize;
            ExpectedBlockSize = (uint)expectedBlockSize;
        }

        public UnexpectedStructureSizeException(Structure structure, uint blockSize, uint expectedBlockSize)
    : base(structure: structure, message: $"Size of the <{structure.GetType().Name}> has unexpected size of <{blockSize}> (expected <{expectedBlockSize}>)")
        {
            BlockSize = blockSize;
            ExpectedBlockSize = expectedBlockSize;
        }
    }

    internal class UnexpectedStructureBlockSignatureException : MalformedStructureException
    {

        public BlockSignature BlockSignature { get; set; }
        public BlockSignature ExpectedBlockSignature { get; set; }

        public UnexpectedStructureBlockSignatureException(Structure structure, BlockSignature blockSignature, BlockSignature expectedBlockSignature)
            : base(structure: structure, message: $"BlockSignature for <{structure.GetType().Name}> is <{blockSignature}> is incorrect (expected <{expectedBlockSignature}>)")
        {
            BlockSignature = blockSignature;
            ExpectedBlockSignature = expectedBlockSignature;
        }
    }
}