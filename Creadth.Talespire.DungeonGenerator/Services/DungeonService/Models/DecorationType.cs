using System;

namespace Creadth.Talespire.DungeonGenerator.Services.DungeonService.Models
{
    public enum DecorationType
    {
        Pillar = 0,
        SingleDoor = 1
    }

    public static class DecorationTypeExtensions
    {
        private static readonly Guid Pillar = Guid.Parse("cec14f2e-faf2-4a9a-bd96-909c16b92197");
        private static readonly Guid SingleDoor = Guid.Parse("a9759b32-9ad0-4157-ba8f-7d7e73a1f00c");
        public static Guid ToAssetId(this DecorationType type)
        {
            return type switch
            {
                DecorationType.Pillar => Pillar,
                DecorationType.SingleDoor => SingleDoor,
                _ => throw new ArgumentOutOfRangeException()
            };

        }
    }
}
