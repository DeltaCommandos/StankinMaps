using System.ComponentModel.DataAnnotations.Schema;

namespace StankinMaps.Models;

[Table("floor_maps")]
public class FloorMap
{
    [Column("id")]
    public int Id { get; set; }

    [Column("building_id")]
    public int BuildingId { get; set; }

    [Column("floor_number")]
    public int FloorNumber { get; set; }

    [Column("svg_path")]
    public string SvgPath { get; set; } = null!;

    [Column("name")]
    public string? Name { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    public Building Building { get; set; } = null!;

    public List<MapObject> MapObjects { get; set; } = new();

    
}