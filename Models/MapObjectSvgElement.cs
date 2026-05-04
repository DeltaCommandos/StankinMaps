using System.ComponentModel.DataAnnotations.Schema;

namespace StankinMaps.Models;

[Table("map_object_svg_elements")]
public class MapObjectSvgElement
{
    [Column("id")]
    public int Id { get; set; }

    [Column("map_object_id")]
    public int MapObjectId { get; set; }

    [Column("svg_element_id")]
    public string? SvgElementId { get; set; }

    [Column("svg_label")]
    public string? SvgLabel { get; set; }

    [Column("css_class")]
    public string? CssClass { get; set; }

    public MapObject MapObject { get; set; } = null!;
}