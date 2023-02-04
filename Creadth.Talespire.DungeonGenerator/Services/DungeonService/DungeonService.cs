using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Creadth.Talespire.DungeonGenerator.Services.DungeonService.Models;
using Creadth.Talespire.DungeonGenerator.Services.SlabService.Models;
using Creadth.Talespire.DungeonGenerator.Services.SlabService.Structs;

namespace Creadth.Talespire.DungeonGenerator.Services.DungeonService
{
    public class DungeonService
    {
        private static readonly Vector3 FloorExtents = new Vector3(.5f, .25f, .5f);
        private const float PillarHeight = .5f;

        public SlabModel ConvertDungeonToSlab(DungeonData data)
        {
            var cells = GenerateCells(data);
            var assets = new List<AssetModel>();
            foreach (var cell in cells)
            {
                var cellAsset = new AssetModel
                {
                    Id = cell.Type.ToAssetId(),
                    Bounds = new Bounds(
                        new Vector3(cell.Pos.Y, 0, cell.Pos.X),
                        Vector3.Zero),
                    Rotation = cell.Direction.ToAssetDirection()
                };
                assets.Add(cellAsset);
                assets.AddRange(cell.Decorations.Select(decoration => new AssetModel
                {
                    Bounds = new Bounds(decoration.Center + cellAsset.Bounds.Center, Vector3.Zero),
                    Rotation = decoration.Direction.ToAssetDirection(),
                    Id = decoration.Type.ToAssetId()
                }));
            }

            return new SlabModel
            {
                Version = 2,
                Assets = assets
            };
        }

        private static IEnumerable<CellData> GenerateCells(DungeonData data)
        {
            //TODO: this is dumb. Undumb it
            var dictionary = new Dictionary<(int x, int y), CellData>();

            #region local helpers

            void PushData(CellData cellData)
            {
                foreach (var dir in Enum.GetValues(typeof(Direction)).Cast<Direction>())
                {
                    var cors = dir.ToPoint();
                    if (!dictionary.TryGetValue((cellData.Pos.X + cors.X, cellData.Pos.Y + cors.Y), out var neighbour)) continue;
                    AddCellNeighbour(cellData, neighbour, dir);
                    AddCellNeighbour(neighbour, cellData, dir.GetOppositeDirection());
                }
                dictionary[(cellData.Pos.X, cellData.Pos.Y)] = cellData;
            }

            #endregion


            foreach (var rect in data.Rects)
            {
                foreach (var cell in RenderRectRoom(rect))
                {
                    PushData(cell);
                }
            }

            //door generator
            foreach (var door in data.Doors)
            {
                dictionary.TryGetValue((door.X-Math.Abs(door.Dir.X), door.Y-Math.Abs(door.Dir.Y)), out var cell);
                var doorData = new DecorationData
                {
                    Type = DecorationType.SingleDoor,
                    Direction = door.Dir.Y == 0 ? Direction.Left : Direction.Up,
                    Center = new Vector3(FloorExtents.X * Math.Abs(door.Dir.Y), FloorExtents.Y,
                        FloorExtents.Z * Math.Abs(door.Dir.X))
                };
                if (cell == null) continue;
                cell.Decorations.Add(doorData);
                //propagate walls
                var propagationAxis = doorData.Direction.GetAdjacentClockwise();
                var leftCors = propagationAxis.ToPoint();
                var rightCors = propagationAxis.GetOppositeDirection().ToPoint();
                var left = dictionary[(cell.Pos.X + leftCors.X, cell.Pos.Y + leftCors.Y)];
                var right = dictionary[(cell.Pos.X + rightCors.X, cell.Pos.Y + rightCors.Y)];
                var stackPropagation = new Stack<(CellData, Point)>();
                //cross-doors
                if (left.Type == CellType.WallWithTorch)
                {
                    BuildDoorCorner(left, dictionary);
                }
                else
                {
                    left.Type = CellType.WallWithTorch;
                    stackPropagation.Push((left, propagationAxis.ToPoint()));
                }

                if (right.Type == CellType.WallWithTorch)
                {
                    BuildDoorCorner(right, dictionary);
                }
                else
                {
                    stackPropagation.Push((right, propagationAxis.GetOppositeDirection().ToPoint()));
                    right.Type = CellType.WallWithTorch;
                }

                while (stackPropagation.Count > 0)
                {
                    var (propCell, dir) = stackPropagation.Pop();
                    propCell.Decorations.Clear();
                    var nextCors = (propCell.Pos.X + dir.X, propCell.Pos.Y + dir.Y);
                    dictionary.TryGetValue(nextCors, out var nextCell);
                    if (propCell.Type != CellType.WallWithTorch && !propCell.Type.IsCornerWall())
                    {
                        propCell.Type = CellType.Wall;
                    }

                    if (nextCell != null && (nextCell.Type == CellType.Floor || nextCell.Direction != propagationAxis.GetAdjacentClockwise()))
                    {
                        propCell.Direction = propagationAxis.GetAdjacentClockwise();
                        stackPropagation.Push((nextCell, dir));
                    }
                    else
                    {
                        if (propCell.Type.IsWall() && nextCell == null)
                        {
                            propCell.Type = CellType.CornerWall;
                            propCell.Direction = propCell.Direction switch
                            {
                                Direction.Left => Direction.Down,
                                Direction.Up => Direction.Up,
                                Direction.Right => Direction.Right,
                                Direction.Down => Direction.Right,
                                _ => propCell.Direction
                            };
                        }
                        else
                        {
                            propCell.Direction = propagationAxis.GetAdjacentClockwise();
                        }
                    }
                }

            }

            //pillar placement
            foreach (var cell in dictionary.Values)
            {
                dictionary.TryGetValue((cell.Pos.X, cell.Pos.Y - 1), out var upper);
                dictionary.TryGetValue((cell.Pos.X, cell.Pos.Y + 1), out var bottom);
                dictionary.TryGetValue((cell.Pos.X - 1, cell.Pos.Y), out var left);
                dictionary.TryGetValue((cell.Pos.X + 1, cell.Pos.Y), out var right);
                //TODO: fix code dup.. somehow
                if (cell.Type != CellType.Floor) continue;
                if (left?.Type.IsWall() == true)
                {
                    if (upper.Type.IsWall() && left.Direction == Direction.Up && upper.Direction == Direction.Left)
                    {
                        cell.Decorations.Add(new DecorationData
                        {
                            Type = DecorationType.Pillar,
                            Center = new Vector3(0, PillarHeight, 0),
                        });
                    }

                    if (bottom.Type.IsWall() && bottom.Direction == Direction.Left &&
                        left.Direction == Direction.Down)
                    {
                        //bot-left pillar
                        cell.Decorations.Add(new DecorationData
                        {
                            Type = DecorationType.Pillar,
                            Center = new Vector3(FloorExtents.X, PillarHeight, 0),
                        });
                    }
                }

                if (right?.Type.IsWall() != true) continue;
                if (upper.Type.IsWall() && right.Direction == Direction.Up && upper.Direction == Direction.Right)
                {
                    //top-right pillar
                    cell.Decorations.Add(new DecorationData
                    {
                        Type = DecorationType.Pillar,
                        Center = new Vector3(0, PillarHeight,
                            FloorExtents.Z),
                    });
                }

                if (bottom.Type.IsWall() && bottom.Direction == Direction.Right &&
                    right.Direction == Direction.Down)
                {
                    //bot-left pillar
                    cell.Decorations.Add(new DecorationData
                    {
                        Type = DecorationType.Pillar,
                        Center = new Vector3(FloorExtents.X, PillarHeight,
                            FloorExtents.Z),
                    });
                }
            }


            return dictionary.Values;
        }

        private static void AddCellNeighbour(CellData cellData, CellData other, Direction neighbourDirection)
        {
            if (cellData.Type.IsFloor()) return;

            if (cellData.Type.IsWall() && cellData.Direction == neighbourDirection)
            {
                cellData.Type = CellType.Floor;
                return;
            }

            if (!cellData.Type.IsCornerWall() || cellData.Direction != neighbourDirection && cellData.Direction.GetAdjacentClockwise() != neighbourDirection) return;
            cellData.Type = CellType.Wall;
            if (neighbourDirection == cellData.Direction)
            {
                cellData.Direction = cellData.Direction.GetAdjacentClockwise();
            }
        }

        private static IEnumerable<CellData> RenderRectRoom(Rect rect)
        {
            for (var x = 0; x < rect.W; x++)
            for (var y = 0; y < rect.H; y++)
            {
                var (tx, ty) = (x + rect.X, y + rect.Y);
                var cell = new CellData {Pos = new Point(tx, ty)};

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (x == 0 && y == 0)
                {
                    cell.Type = CellType.CornerWall;
                    cell.Direction = Direction.Left;
                    yield return cell;
                    continue;
                }

                if (x == 0 && y == rect.H - 1)
                {
                    cell.Type = CellType.CornerWall;
                    cell.Direction = Direction.Down;
                    yield return cell;
                    continue;
                }

                if (x == rect.W - 1 && y == 0)
                {
                    cell.Type = CellType.CornerWall;
                    cell.Direction = Direction.Up;
                    yield return cell;
                    continue;
                }

                if (x == rect.W - 1 && y == rect.H - 1)
                {
                    cell.Type = CellType.CornerWall;
                    cell.Direction = Direction.Right;
                    yield return cell;
                    continue;
                }

                if (x == 0)
                {
                    cell.Type = CellType.Wall;
                    cell.Direction = Direction.Left;
                    yield return cell;
                    continue;
                }

                if (y == 0)
                {
                    cell.Type = CellType.Wall;
                    cell.Direction = Direction.Up;
                    yield return cell;
                    continue;
                }

                if (x == rect.W - 1)
                {
                    cell.Type = CellType.Wall;
                    cell.Direction = Direction.Right;
                    yield return cell;
                    continue;
                }

                if (y == rect.H - 1)
                {
                    cell.Type = CellType.Wall;
                    cell.Direction = Direction.Down;
                    yield return cell;
                    continue;
                }

                cell.Type = CellType.Floor;
                yield return cell;
            }
        }

        private static IEnumerable<CellData> RenderRotunda(Rect rect)
        {
            var radX = rect.W / 2;
            var radY = rect.H / 2;
            for (var x = 0; x < rect.W; x++)
            for (var y = 0; y < rect.H; y++)
            {
                var cx = x - radX;
                var cy = y - radY;
                if (cx * cx + cy * cy <= radX * radY)
                {
                    yield return new CellData {Type = CellType.Floor, Direction = Direction.Left, Pos = new Point(x + rect.X, y + rect.Y)};
                }
            }
        }

        private static void BuildDoorCorner(CellData cell, IDictionary<(int x, int y), CellData> dictionary)
        {
            dictionary.TryGetValue((cell.Pos.X, cell.Pos.Y - 1), out var upper);
            dictionary.TryGetValue((cell.Pos.X, cell.Pos.Y + 1), out var bottom);
            dictionary.TryGetValue((cell.Pos.X - 1, cell.Pos.Y), out var left);
            dictionary.TryGetValue((cell.Pos.X + 1, cell.Pos.Y), out var right);
            if (upper?.Decorations.Any(x => x.Type == DecorationType.SingleDoor) == true)
            {
                if (left?.Decorations.Any(x => x.Type == DecorationType.SingleDoor) == true)
                {
                    cell.Type = CellType.CornerWall;
                    cell.Direction = Direction.Right;
                    return;
                }
                if (right?.Decorations.Any(x => x.Type == DecorationType.SingleDoor) != true) return;
                cell.Type = CellType.CornerWall;
                cell.Direction = Direction.Down;
                return;
            }

            if (bottom?.Decorations.Any(x => x.Type == DecorationType.SingleDoor) != true) return;
            if (left?.Decorations.Any(x => x.Type == DecorationType.SingleDoor) == true)
            {
                cell.Type = CellType.CornerWall;
                cell.Direction = Direction.Up;
                return;
            }

            if (right?.Decorations.Any(x => x.Type == DecorationType.SingleDoor) != true) return;
            cell.Type = CellType.CornerWall;
            cell.Direction = Direction.Left;


        }

    }
}
