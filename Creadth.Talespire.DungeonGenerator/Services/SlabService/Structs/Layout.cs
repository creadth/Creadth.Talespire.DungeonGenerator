using System;
using System.Runtime.InteropServices;
using Creadth.Talespire.DungeonGenerator.Helpers;

namespace Creadth.Talespire.DungeonGenerator.Services.SlabService.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Layout
    {
        /// <summary>
        /// Asset id to be used
        /// </summary>
        public readonly Guid AssetId;

        /// <summary>
        /// Asset count
        /// </summary>
        public readonly ushort AssetCount;

        /// <summary>
        /// RESERVED
        /// </summary>
        public readonly ushort __RESERVED__;

        public Layout(Guid assetId, ushort assetCount)
        {
            AssetId = assetId;
            AssetCount = assetCount;
            __RESERVED__ = 0;
        }

        public static Layout Read(byte[] data)
        {
            return Serializer.Deserialize<Layout>(data);
        }

        public static byte[] Write(Layout l)
        {
            return Serializer.Serialize(l);
        }
    }
}
