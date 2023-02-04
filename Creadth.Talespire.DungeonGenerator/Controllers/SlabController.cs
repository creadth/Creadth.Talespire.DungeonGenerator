using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Creadth.Talespire.DungeonGenerator.Services.DungeonService;
using Creadth.Talespire.DungeonGenerator.Services.DungeonService.Models;
using Creadth.Talespire.DungeonGenerator.Services.SlabService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
