using System.ComponentModel.DataAnnotations.Schema;

namespace StankinMaps.Models;

[Table("buildings")]
public class Building
{
    [Column("id")]
    public int Id { get; set; }

    [Column("code")]
    public string Code { get; set; } = null!;

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    public List<FloorMap> FloorMaps { get; set; } = new();
}