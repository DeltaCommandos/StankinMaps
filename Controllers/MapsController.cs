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
            var mapObject = await _context.MapObjectSvgElements
                .Where(x =>
                    x.MapObject.FloorMap.Building.Code == building &&
                    x.MapObject.FloorMap.FloorNumber == floor &&
                    (
                        (!string.IsNullOrEmpty(svgLabel) && x.SvgLabel == svgLabel) ||
                        (!string.IsNullOrEmpty(svgElementId) && x.SvgElementId == svgElementId)
                    ))
                .Select(x => new
                {
                    id = x.MapObject.Id,
                    number = x.MapObject.Number,
                    title = x.MapObject.Title,
                    description = x.MapObject.Description,
                    type = x.MapObject.ObjectType.Name
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