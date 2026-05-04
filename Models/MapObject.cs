using System.ComponentModel.DataAnnotations.Schema;

namespace StankinMaps.Models;

[Table("map_objects")]
public class MapObject
{
    [Column("id")]
    public int Id { get; set; }

    [Column("floor_map_id")]
    public int FloorMapId { get; set; }

    [Column("object_type_id")]
    public int ObjectTypeId { get; set; }

    [Column("number")]
    public string? Number { get; set; }

    [Column("title")]
    public string Title { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    //[Column("capacity")]
    //public int? Capacity { get; set; }

    //[Column("department")]
    //public string? Department { get; set; }

    [Column("is_clickable")]
    public bool IsClickable { get; set; }

    [Column("is_searchable")]
    public bool IsSearchable { get; set; }

    [Column("label_x")]
    public double? LabelX { get; set; }

    [Column("label_y")]
    public double? LabelY { get; set; }

    public FloorMap FloorMap { get; set; } = null!;

    public MapObjectType ObjectType { get; set; } = null!;

    public List<MapObjectSvgElement> SvgElements { get; set; } = new();
}