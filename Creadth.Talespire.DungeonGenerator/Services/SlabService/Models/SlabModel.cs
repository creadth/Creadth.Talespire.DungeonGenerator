using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Creadth.Talespire.DungeonGenerator.Services.SlabService.Models
{
    public class SlabModel
    {
        public int Version { get; set; }
        public IEnumerable<AssetModel> Assets { get; set; }
    }
}
