using System.Numerics;

namespace Creadth.Talespire.DungeonGenerator.Services.DungeonService.Models
{
    public class DecorationData
    {
        public DecorationType Type { get; set; }
        public Vector3 Center { get; set; }
        public Direction Direction { get; set; }
    }
}
