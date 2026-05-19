using System.ComponentModel.DataAnnotations;

namespace StankinMaps.Models
{
    public class Panorama
    {
        public int Id { get; set; }

        [Required]
        public string Building { get; set; } = "new";

        public int Floor { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        // id объекта в SVG, например pano_hall_1 или room_0123
        public string? SvgObjectId { get; set; }

        // если панорама относится к аудитории
        public int? RoomId { get; set; }

        // путь к файлу
        [Required]
        public string ImagePath { get; set; } = string.Empty;

        // начальное направление камеры
        public double DefaultYaw { get; set; } = 0;
        public double DefaultPitch { get; set; } = 0;
        public double DefaultHfov { get; set; } = 100;

        public bool IsPublished { get; set; } = true;
    }
}