using System;
using Creadth.Talespire.DungeonGenerator.Services.SlabService.Structs;

namespace Creadth.Talespire.DungeonGenerator.Services.SlabService.Models
{
    public class AssetModel
    {
        public Guid Id { get; set; }
        public Bounds Bounds { get; set; }
        public byte Rotation { get; set; }
    }
}
