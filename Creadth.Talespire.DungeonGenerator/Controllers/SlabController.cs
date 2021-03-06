using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;
using Creadth.Talespire.DungeonGenerator.Services;
using Creadth.Talespire.DungeonGenerator.Services.DungeonService;
using Creadth.Talespire.DungeonGenerator.Services.DungeonService.Models;
using Creadth.Talespire.DungeonGenerator.Services.SlabService;
using Creadth.Talespire.DungeonGenerator.Services.SlabService.Models;
using Creadth.Talespire.DungeonGenerator.Services.SlabService.Structs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Creadth.Talespire.DungeonGenerator.Controllers
{
    [ApiController]
    [Route("slab")]
    public class SlabController
    : ControllerBase
    {
        private readonly DungeonService _dungeonService;
        private readonly SlabService _slabService;

        public SlabController(DungeonService dungeonService, SlabService slabService)
        {
            _dungeonService = dungeonService;
            _slabService = slabService;
        }

        /// <summary>
        /// Import dungeon from file
        /// </summary>
        /// <param name="file">File with JSON data</param>
        /// <param name="scale">Scale the dungeon. Default 2 is recommended to correctly generate walls</param>
        /// <returns></returns>
        [HttpPost("import/dungeon")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(IFormFile file, [FromQuery] int scale)
        {
            var dungeonData = await JsonSerializer.DeserializeAsync<DungeonData>(file.OpenReadStream(), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            if (dungeonData == null) return BadRequest();
            if (dungeonData?.Rects?.Any() != true) return BadRequest();
            foreach (var rect in dungeonData.Rects)
            {
                rect.H *= scale;
                rect.W *= scale;
                rect.X *= scale;
                rect.Y *= scale;
            }

            foreach (var door in dungeonData.Doors)
            {
                door.X *= scale;
                door.Y *= scale;
                if (door.Dir.Y == 0)
                {
                    door.Y += scale / 2;
                }

                if (door.Dir.X == 0)
                {
                    door.X += scale / 2;
                }
            }
            return Ok(_slabService.GenerateSlab(_dungeonService.ConvertDungeonToSlab(dungeonData)));
        }

        /// <summary>
        /// Convert Slab to Json
        /// </summary>
        /// <param name="data">Slab in TS</param>
        /// <returns></returns>
        [HttpPost, Route("export/json")]
        [ProducesResponseType(typeof(SlabModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public IActionResult GetData([FromBody] string data)
        {
            var slab = _slabService.ReadSlab(data);
            if (slab == null) return BadRequest();
            return Ok(slab);
        }

        /// <summary>
        /// Export JSON data to slab string
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("import/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public IActionResult GetSlab([FromBody] SlabModel model)
        {
            var slabString = _slabService.GenerateSlab(model);
            if (string.IsNullOrEmpty(slabString)) return BadRequest();
            return Ok(slabString);
        }

    }
}
