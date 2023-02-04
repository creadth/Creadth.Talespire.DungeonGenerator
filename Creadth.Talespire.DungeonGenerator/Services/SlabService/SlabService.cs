using System;
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

                // Write Magic Number
                binaryWrite.Write(MagicNumber);

                // Write version
                binaryWrite.Write((ushort) model.Version);

                // We have to convert assets to proper layouts

                var orderedAssets = model.Assets.OrderBy(x => x.Id).ToArray();
                // group all assets by id
                var groups = orderedAssets.GroupBy(x => x.Id).ToArray();

                // Write asset count
                binaryWrite.Write((ushort) groups.Length);
                // Write creature count. Always zero for v2 slab
                binaryWrite.Write((ushort)0);

                // Write layout data
                foreach (var g in groups)
                {
                    var assetId = g.Key;
                    var layout = new Layout(assetId, (ushort)g.Count());
                    binaryWrite.Write(Layout.Write(layout));
                }


                // Only positive coordinates allowed in v2 slab format, translate everything
                var minX = -Math.Min(orderedAssets.Min(x => x.Bounds.Center.X), 0);
                var minY = -Math.Min(orderedAssets.Min(x => x.Bounds.Center.Y), 0);
                var minZ = -Math.Min(orderedAssets.Min(x => x.Bounds.Center.Z), 0);
                foreach (var asset in orderedAssets)
                {
                    ulong assetData = 0;
                    //from MS to LS bits:
                    // 18 bits - scaledX (x*100)
                    assetData |= (ulong)((minX + asset.Bounds.Center.X) * 100);
                    // 18 bits - scaledY (y*100)
                    assetData |= (ulong)((minY + asset.Bounds.Center.Y) * 100) << 18;
                    // 18 bits - scaledZ (z*100)
                    assetData |= (ulong)((minZ + asset.Bounds.Center.Z) * 100) << 36;
                    // 5 bits - rot
                    assetData |= (ulong)asset.Rotation << 54;
                    // 5 bits - reserved
                    binaryWrite.Write(assetData);
                }
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
    }
}
