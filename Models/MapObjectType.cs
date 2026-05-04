using System.ComponentModel.DataAnnotations.Schema;

namespace StankinMaps.Models;

[Table("map_object_types")]
public class MapObjectType
{
    [Column("id")]
    public int Id { get; set; }

    [Column("code")]
    public string Code { get; set; } = null!;

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("default_clickable")]
    public bool DefaultClickable { get; set; }

    public List<MapObject> MapObjects { get; set; } = new();
}