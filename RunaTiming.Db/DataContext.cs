using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RunaTiming.Db.Models;

namespace RunaTiming.Db;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Race> Races => Set<Race>();
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Result> Results => Set<Result>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new RaceConfiguration());
        modelBuilder.ApplyConfiguration(new ResultConfiguration());

        modelBuilder.Entity<Result>()
            .HasKey(p => new { p.Bib, p.RaceId });
    }

    public static void CreateDbIfNotExists(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<DataContext>();
        context.Database.Migrate();
    }
}