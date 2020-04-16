using System.Runtime.InteropServices;
using Creadth.Talespire.DungeonGenerator.Helpers;

namespace Creadth.Talespire.DungeonGenerator.Services.SlabService.Structs
{

    [StructLayout(LayoutKind.Sequential, Pack=4)]
    public struct AssetCopyData
    {
        public readonly Bounds SelectionBounds;
        public readonly byte Rotation; // Value range is 0-15. Indicates 16 possible rotation positions. Only multiples of 4 are valid for tiles.

        public AssetCopyData(Bounds selectionBounds, byte rotation)
        {
            SelectionBounds = selectionBounds;
            Rotation = rotation;
        }

        public static AssetCopyData Read(byte[] data)
        {
            return Serializer.Deserialize<AssetCopyData>(data);
        }

        public static byte[] Write(AssetCopyData l)
        {
            return Serializer.Serialize(l);
        }



    }
}
