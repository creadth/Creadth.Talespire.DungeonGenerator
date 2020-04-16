using System;

namespace Creadth.Talespire.DungeonGenerator.Services.DungeonService.Models
{

    public enum CellType
    {
        Floor = 0,
        Wall = 2,
        RoundWall = 3,
        CornerWall = 4,
        RoundCornerWall = 5,
        WallWithTorch = 6
    }

    public static class CellTypeExtensions
    {

        private static Guid Floor = Guid.Parse("d5900784-9510-4cf7-b017-f369448d0d52");
        private static Guid CornerWall = Guid.Parse("fdd3c8dc-9c94-4a63-a7d9-10ae36d07fe7");
        private static Guid Wall = Guid.Parse("ed0ad169-3248-411a-9eb0-4aada656fb61");
        private static Guid WallWithTorch = Guid.Parse("7e51a34b-b25f-485c-b1c0-a937a936e4c2");

        public static bool IsWall(this CellType type)
        {
            return type == CellType.Wall || type == CellType.RoundWall || type == CellType.WallWithTorch;
        }

        public static bool IsCornerWall(this CellType type)
        {
            return type == CellType.CornerWall || type == CellType.RoundCornerWall;
        }

        public static bool IsFloor(this CellType type)
        {
            return type == CellType.Floor;
        }

        public static Guid ToAssetId(this CellType type)
        {
            return type switch
            {
                CellType.Floor => Floor,
                CellType.Wall => Wall,
                CellType.RoundWall => Wall,
                CellType.CornerWall => CornerWall,
                CellType.RoundCornerWall => CornerWall,
                CellType.WallWithTorch => WallWithTorch,
                _ => throw new ArgumentOutOfRangeException()
            };

        }
    }

}
