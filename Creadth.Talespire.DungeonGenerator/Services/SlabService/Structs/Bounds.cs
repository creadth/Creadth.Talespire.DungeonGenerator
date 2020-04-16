using System.Numerics;
using System.Runtime.InteropServices;
using Creadth.Talespire.DungeonGenerator.Helpers;

namespace Creadth.Talespire.DungeonGenerator.Services.SlabService.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack=4)]
    public struct Bounds
    {
        private  Vector3 _center;
        private  Vector3 _extents;
        public Vector3 Center { get => _center;
            set => _center = value;
        }

        public Vector3 Extents { get => _extents;
            set => _extents = value;
        }

        public Bounds(Vector3 center, Vector3 extents)
        {
            _center = center;
            _extents = extents;
        }

        public static Bounds Read(byte[] data)
        {
            return Serializer.Deserialize<Bounds>(data);
        }

        public static byte[] Write(Bounds l)
        {
            return Serializer.Serialize(l);
        }


    }
}
