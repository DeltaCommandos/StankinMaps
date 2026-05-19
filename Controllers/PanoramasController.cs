using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StankinMaps.Data;

namespace StankinMaps.Controllers
{
    public class PanoramasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PanoramasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetByMapObject(string building, int floor, string svgObjectId)
        {
            var panorama = await _context.Panoramas
                .Where(p =>
                    p.Building.ToLower() == building.ToLower()
                    && p.Floor == floor
                    && p.SvgObjectId == svgObjectId
                    && p.IsPublished)
                .FirstOrDefaultAsync();

            if (panorama == null)
            {
                return NotFound();
            }

            var hotspots = await _context.PanoramaHotspots
                .Where(h => h.PanoramaId == panorama.Id)
                .Select(h => new
                {
                    h.Id,
                    h.Yaw,
                    h.Pitch,
                    h.Text,
                    h.Type,
                    h.TargetPanoramaId
                })
                .ToListAsync();

            return Json(new
            {
                panorama.Id,
                panorama.Title,
                panorama.Description,

                // Важно: отдаём браузеру URL, а не физический путь
                ImagePath = GetPanoramaUrl(panorama.ImagePath),

                panorama.DefaultYaw,
                panorama.DefaultPitch,
                panorama.DefaultHfov,
                Hotspots = hotspots
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var panorama = await _context.Panoramas
                .FirstOrDefaultAsync(p => p.Id == id && p.IsPublished);

            if (panorama == null)
            {
                return NotFound();
            }

            var hotspots = await _context.PanoramaHotspots
                .Where(h => h.PanoramaId == panorama.Id)
                .Select(h => new
                {
                    h.Id,
                    h.Yaw,
                    h.Pitch,
                    h.Text,
                    h.Type,
                    h.TargetPanoramaId
                })
                .ToListAsync();

            return Json(new
            {
                panorama.Id,
                panorama.Title,
                panorama.Description,

                // Важно: отдаём браузеру URL, а не физический путь
                ImagePath = GetPanoramaUrl(panorama.ImagePath),

                panorama.DefaultYaw,
                panorama.DefaultPitch,
                panorama.DefaultHfov,
                Hotspots = hotspots
            });
        }

        private static string GetPanoramaUrl(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return string.Empty;
            }

            imagePath = imagePath.Replace("\\", "/").TrimStart('/');

            return $"/Panoramas/{imagePath}";
        }
    }
}