using System.ComponentModel.DataAnnotations;

namespace StankinMaps.Models
{
    public class PanoramaHotspot
    {
        public int Id { get; set; }

        public int PanoramaId { get; set; }
        public Panorama? Panorama { get; set; }

        // точка на сфере
        public double Yaw { get; set; }
        public double Pitch { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        // если это переход в другую панораму
        public int? TargetPanoramaId { get; set; }

        // например: "scene", "info"
        public string Type { get; set; } = "scene";
    }
}