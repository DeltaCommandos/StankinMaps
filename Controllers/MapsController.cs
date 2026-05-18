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

        public IActionResult Maps(string building = "new", int floor = 1)
        {
            building = building?.ToLower() ?? "new";

            int maxFloor = building == "old"
                ? 5
                : 9;

            if (floor < 1)
            {
                floor = 1;
            }

            if (floor > maxFloor)
            {
                floor = maxFloor;
            }

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
            building = building?.Trim().ToLower() ?? "";

            svgLabel = string.IsNullOrWhiteSpace(svgLabel)
                ? null
                : svgLabel.Trim();

            svgElementId = string.IsNullOrWhiteSpace(svgElementId)
                ? null
                : svgElementId.Trim();

            var mapObject = await _context.MapObjects
                .Where(x =>
                    EF.Functions.ILike(x.FloorMap.Building.Code, building) &&
                    x.FloorMap.FloorNumber == floor &&
                    x.IsClickable &&
                    (
                        x.SvgElements.Any(s =>
                            (
                                svgLabel != null &&
                                s.SvgLabel != null &&
                                EF.Functions.ILike(s.SvgLabel, svgLabel)
                            ) ||
                            (
                                svgElementId != null &&
                                s.SvgElementId != null &&
                                EF.Functions.ILike(s.SvgElementId, svgElementId)
                            )
                        ) ||

                        (
                            svgLabel != null &&
                            x.Number != null &&
                            EF.Functions.ILike(x.Number, svgLabel)
                        ) ||

                        (
                            svgElementId != null &&
                            x.Number != null &&
                            EF.Functions.ILike(x.Number, svgElementId)
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

        [HttpGet]
        public async Task<IActionResult> SearchMapObject(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest();
            }

            query = query.Trim();
            var pattern = $"%{query}%";

            string? targetBuilding = null;

            // Если запрос начинается с цифры,
            // считаем, что пользователь ищет аудиторию по номеру.
            // 0... = новый корпус
            // не 0... = старый корпус
            if (char.IsDigit(query[0]))
            {
                targetBuilding = query.StartsWith("0")
                    ? "new"
                    : "old";
            }

            var result = await _context.MapObjects
                .Where(x =>
                    x.IsSearchable &&

                    // Если это номер аудитории — ищем только в нужном корпусе
                    (targetBuilding == null || EF.Functions.ILike(x.FloorMap.Building.Code, targetBuilding)) &&

                    (
                        (x.Number != null && EF.Functions.ILike(x.Number, pattern)) ||

                        (x.Title != null && EF.Functions.ILike(x.Title, pattern)) ||

                        (x.Description != null && EF.Functions.ILike(x.Description, pattern)) ||

                        x.SvgElements.Any(s =>
                            (s.SvgLabel != null && EF.Functions.ILike(s.SvgLabel, pattern)) ||
                            (s.SvgElementId != null && EF.Functions.ILike(s.SvgElementId, pattern))
                        )
                    )
                )
                .OrderByDescending(x => x.Number == query)
                .ThenByDescending(x => x.Number != null && x.Number.StartsWith(query))
                .ThenBy(x => x.Number)
                .Select(x => new
                {
                    id = x.Id,
                    number = x.Number,
                    title = x.Title,
                    description = x.Description,
                    type = x.ObjectType.Name,

                    // Возвращаем корпус:
                    // если искали по номеру — по правилу 0 / не 0,
                    // иначе берём корпус из базы
                    building = targetBuilding ?? x.FloorMap.Building.Code,

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