using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Creadth.Talespire.DungeonGenerator.Services.SlabService.Models;
using Creadth.Talespire.DungeonGenerator.Services.SlabService.Structs;

namespace Creadth.Talespire.DungeonGenerator.Services.SlabService
{
    public class SlabService
    {
        private const uint MagicNumber = 0xD1CEFACE;

        public string GenerateSlab(SlabModel model)
        {
            if (!model.Assets.Any()) return null;
            try
            {
                using var data = new MemoryStream();
                using var deflateStream = new GZipStream(data, CompressionLevel.Optimal);
                using var binaryWrite = new BinaryWriter(deflateStream);
                binaryWrite.Write(MagicNumber);
                binaryWrite.Write((ushort) model.Version);
                var orderedAssets = model.Assets.OrderBy(x => x.Id);
                var groups = orderedAssets.GroupBy(x => x.Id);

                binaryWrite.Write((ushort) groups.Count());
                var firstAsset = model.Assets.First();
                var unionMin = firstAsset.Bounds.Center - firstAsset.Bounds.Extents;
                var unionMax = firstAsset.Bounds.Center + firstAsset.Bounds.Extents;
                foreach (var g in groups)
                {
                    var assetId = g.Key;
                    var layout = new Layout(assetId, (ushort) g.Count());
                    binaryWrite.Write(Layout.Write(layout));
                }

                foreach (var asset in orderedAssets)
                {
                    binaryWrite.Write(AssetCopyData.Write(new AssetCopyData(asset.Bounds, asset.Rotation)));
                    var min = asset.Bounds.Center - asset.Bounds.Extents;
                    var max = asset.Bounds.Center + asset.Bounds.Extents;
                    unionMin.X = Math.Min(min.X, unionMin.X);
                    unionMin.Y = Math.Min(min.Y, unionMin.Y);
                    unionMin.Z = Math.Min(min.Z, unionMin.Z);
                    unionMax.X = Math.Max(max.X, unionMax.X);
                    unionMax.Y = Math.Max(max.Y, unionMax.Y);
                    unionMax.Z = Math.Max(max.Z, unionMax.Z);
                }

                var union = new Bounds(.5f * (unionMin + unionMax), .5f * (unionMax - unionMin));
                binaryWrite.Write(Bounds.Write(union));
                binaryWrite.Close();
                deflateStream.Close();
                data.Flush();
                var buff = data.ToArray();
                return $"```{Convert.ToBase64String(buff)}```";
            }
            catch
            {
                //TODO: proper logging
                return null;
            }
        }

        public SlabModel ReadSlab(string data)
        {
            try
            {
                var b64Data = Convert.FromBase64String(data.Trim('`'));
                using var stream = new MemoryStream(b64Data);
                using var deflateStream = new GZipStream(stream, CompressionMode.Decompress);
                using var memStream = new MemoryStream();
                deflateStream.CopyTo(memStream);
                memStream.Flush();
                var buff = memStream.ToArray();
                using var reader = new BinaryReader(new MemoryStream(buff));
                unsafe
                {
                    var slabModel = new SlabModel();
                    var check = reader.ReadUInt32();
                    if (check != MagicNumber) return null;
                    slabModel.Version = reader.ReadUInt16();
                    var layoutCnt = reader.ReadUInt16();
                    var layouts = new Layout[layoutCnt];
                    for (var i = 0; i < layoutCnt; i++)
                    {
                        layouts[i] = Layout.Read(reader.ReadBytes(sizeof(Layout)));
                    }

                    var lst = new List<AssetModel>();
                    foreach (var layout in layouts)
                    {
                        for (var i = 0; i < layout.AssetCount; i++)
                        {
                            var asset = AssetCopyData.Read(reader.ReadBytes(sizeof(AssetCopyData)));
                            lst.Add(new AssetModel
                            {
                                Id = layout.AssetId,
                                Bounds = asset.SelectionBounds,
                                Rotation = asset.Rotation
                            });
                        }
                    }

                    slabModel.Assets = lst;
                    return slabModel;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
