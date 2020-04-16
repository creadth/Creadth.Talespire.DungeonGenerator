using System;
using System.Runtime.InteropServices;
using Creadth.Talespire.DungeonGenerator.Helpers;

namespace Creadth.Talespire.DungeonGenerator.Services.SlabService.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Layout
    {
        public readonly Guid AssetId;
        public readonly ushort AssetCount;

        public Layout(Guid assetId, ushort assetCount)
        {
            AssetId = assetId;
            AssetCount = assetCount;
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
