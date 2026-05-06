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

        public IActionResult Maps(string building = "New", int floor = 1)
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
            building = (building ?? string.Empty).Trim();
            svgLabel = svgLabel?.Trim();
            svgElementId = svgElementId?.Trim();

            if (string.IsNullOrWhiteSpace(building) ||
                (string.IsNullOrWhiteSpace(svgLabel) && string.IsNullOrWhiteSpace(svgElementId)))
            {
                return BadRequest("building and svgLabel/svgElementId are required");
            }

            var buildingLower = building.ToLower();

            var mapObject = await _context.MapObjects
                .Where(x =>
                    x.FloorMap.Building.Code.ToLower() == buildingLower &&
                    x.FloorMap.FloorNumber == floor &&
                    (
                        (!string.IsNullOrEmpty(svgLabel) && x.Number == svgLabel) ||
                        (!string.IsNullOrEmpty(svgElementId) && x.Number == svgElementId) ||
                        x.SvgElements.Any(svg =>
                            (!string.IsNullOrEmpty(svgLabel) &&
                                (svg.SvgLabel == svgLabel || svg.SvgElementId == svgLabel)) ||
                            (!string.IsNullOrEmpty(svgElementId) &&
                                (svg.SvgLabel == svgElementId || svg.SvgElementId == svgElementId))
                        )
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
    }
}
