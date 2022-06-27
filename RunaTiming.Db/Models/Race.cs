using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace RunaTiming.Db.Models;

public class Race
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool IsOngoing { get; set; }
    public bool IsHidden { get; set; }

    public double Distance { get; set; }
    public List<RaceSplit> Splits { get; set; } = new();

    public ICollection<Participant> Participants { get; set; } = new List<Participant>();

    public ICollection<Result> Results { get; set; } = new List<Result>();
}

public class RaceSplit
{
    public double Distance { get; set; }
}

public class RaceConfiguration : IEntityTypeConfiguration<Race>
{
    public void Configure(EntityTypeBuilder<Race> builder)
    {
        builder
            .Property(e => e.Splits)
            .HasConversion(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<List<RaceSplit>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
    }
}