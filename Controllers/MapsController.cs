using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StankinMaps.Data;

namespace StankinMaps.Controllers
{
    public class MapsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MapsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Maps(string building = "main", int floor = 1)
        {
            ViewBag.Building = building;
            ViewBag.Floor = floor;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMapObject(
            string building,
            int floor,
            string? svgLabel,
            string? svgElementId)
        {
            var mapObject = await _context.MapObjects
                .Where(x =>
                    x.FloorMap.Building.Code == building &&
                    x.FloorMap.FloorNumber == floor &&
                    x.IsClickable &&
                    (
                        x.SvgElements.Any(s =>
                            !string.IsNullOrEmpty(svgLabel) && s.SvgLabel == svgLabel ||
                            !string.IsNullOrEmpty(svgElementId) && s.SvgElementId == svgElementId
                        ) ||
                        !string.IsNullOrEmpty(svgLabel) && x.Number == svgLabel ||
                        !string.IsNullOrEmpty(svgElementId) && x.Number == svgElementId
                    ))
                .Select(x => new
                {
                    id = x.Id,
                    number = x.Number,
                    title = x.Title,
                    description = x.Description,
                    type = x.ObjectType.Name
                })
                .FirstOrDefaultAsync();

            if (mapObject == null)
            {
                return NotFound();
            }

            return Json(mapObject);
        }

        [HttpGet]
        public async Task<IActionResult> SearchMapObject(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest();
            }

            query = query.Trim();
            var pattern = $"%{query}%";

            var result = await _context.MapObjects
                .Where(x =>
                    x.IsSearchable &&
                    (
                        x.Number != null && EF.Functions.ILike(x.Number, pattern) ||
                        EF.Functions.ILike(x.Title, pattern) ||
                        x.Description != null && EF.Functions.ILike(x.Description, pattern) ||
                        x.SvgElements.Any(s =>
                            s.SvgLabel != null && EF.Functions.ILike(s.SvgLabel, pattern) ||
                            s.SvgElementId != null && EF.Functions.ILike(s.SvgElementId, pattern)
                        )
                    ))
                .OrderByDescending(x => x.Number == query)
                .ThenBy(x => x.Number)
                .Select(x => new
                {
                    id = x.Id,
                    number = x.Number,
                    title = x.Title,
                    description = x.Description,
                    type = x.ObjectType.Name,

                    building = x.FloorMap.Building.Code,
                    floor = x.FloorMap.FloorNumber,

                    svgLabel = x.SvgElements
                        .Where(s => s.SvgLabel != null)
                        .Select(s => s.SvgLabel)
                        .FirstOrDefault(),

                    svgElementId = x.SvgElements
                        .Where(s => s.SvgElementId != null)
                        .Select(s => s.SvgElementId)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound();
            }

            return Json(result);
        }
    }
}