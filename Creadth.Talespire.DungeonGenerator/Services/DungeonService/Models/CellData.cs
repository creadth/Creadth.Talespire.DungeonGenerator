using System.Collections.Generic;
using System.Drawing;

namespace Creadth.Talespire.DungeonGenerator.Services.DungeonService.Models
{
    public class CellData
    {
        public Point Pos { get; set; }
        public CellType Type { get; set; }
        public Direction Direction { get; set; }
        public IList<DecorationData> Decorations { get; set; }

        public CellData()
        {
            Decorations = new List<DecorationData>();
        }
    }
}
