using Microsoft.EntityFrameworkCore;
using StankinMaps.Models;

namespace StankinMaps.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Panorama> Panoramas { get; set; }
    public DbSet<PanoramaHotspot> PanoramaHotspots { get; set; }
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<FloorMap> FloorMaps => Set<FloorMap>();
    public DbSet<MapObjectType> MapObjectTypes => Set<MapObjectType>();
    public DbSet<MapObject> MapObjects => Set<MapObject>();
    public DbSet<MapObjectSvgElement> MapObjectSvgElements => Set<MapObjectSvgElement>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Building>()
            .HasMany(x => x.FloorMaps)
            .WithOne(x => x.Building)
            .HasForeignKey(x => x.BuildingId);

        modelBuilder.Entity<FloorMap>()
            .HasMany(x => x.MapObjects)
            .WithOne(x => x.FloorMap)
            .HasForeignKey(x => x.FloorMapId);

        modelBuilder.Entity<MapObjectType>()
            .HasMany(x => x.MapObjects)
            .WithOne(x => x.ObjectType)
            .HasForeignKey(x => x.ObjectTypeId);

        modelBuilder.Entity<MapObject>()
            .HasMany(x => x.SvgElements)
            .WithOne(x => x.MapObject)
            .HasForeignKey(x => x.MapObjectId);
    }
}