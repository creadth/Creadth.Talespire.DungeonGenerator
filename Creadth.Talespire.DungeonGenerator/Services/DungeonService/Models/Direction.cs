using System;
using System.Drawing;
using System.Numerics;

namespace Creadth.Talespire.DungeonGenerator.Services.DungeonService.Models
{
    public enum Direction
    {
        Left = 0,
        Up = 4,
        Right = 8,
        Down = 12
    }

    public static class DirectionExtensions
    {
        public static byte ToAssetDirection(this Direction dir)
        {
            return dir switch
            {
                Direction.Left => 0,
                Direction.Up => 6,
                Direction.Right => 12,
                Direction.Down => 18,
                _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
            };
        }


        public static Direction GetOppositeDirection(this Direction dir)
        {
            return (Direction) ((8 + (int) dir) % 16);
        }

        public static Direction GetAdjacentClockwise(this Direction dir)
        {
            return (Direction)((4 + (int) dir) % 16);
        }

        public static Direction GetAdjacentClockCounterWise(this Direction dir)
        {
            var ret = -4 + (int) dir;
            return (Direction)(ret < 0 ? ret+16 : ret);
        }

        public static Point ToPoint(this Direction dir)
        {
            return dir switch
            {
                Direction.Down => new Point(0, 1),
                Direction.Left => new Point(-1, 0),
                Direction.Right => new Point(1, 0),
                Direction.Up => new Point(0, -1),
                _ => throw new ArgumentOutOfRangeException(nameof(dir))
            };
        }

        public static Direction FromPoint(Point p)
        {
            return p.X switch
            {
                0 when p.Y == -1 => Direction.Down,
                0 when p.Y == 1 => Direction.Up,
                1 when p.Y == 0 => Direction.Right,
                -1 when p.Y == 0 => Direction.Left,
                _ => throw new ArgumentOutOfRangeException(nameof(p))
            };
        }
    }
}
