using System.Collections.Generic;
using System.Drawing;

namespace Creadth.Talespire.DungeonGenerator.Services.DungeonService.Models
{

    public class DungeonData
    {
        public IEnumerable<Rect> Rects { get; set; }
        public IEnumerable<Door> Doors { get; set; }
    }

    public class Rect
    {
        public bool Rotunda { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
    }

    public class Door
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point Dir { get; set; }
    }
}
