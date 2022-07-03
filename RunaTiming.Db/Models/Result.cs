using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace RunaTiming.Db.Models;

public class Result
{
    public int Bib { get; set; }

    public int RaceId { get; set; }

    public Race Race { get; set; }
    public DateTime Modified { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Sex Sex { get; set; }
    public DateTime? BirthDate { get; set; }

    public DateTime? StartTime { get; set; }
    public double? ChipStartTime { get; set; }
    public double? FinishingTime { get; set; }
    public List<double> Splits { get; set; } = new();
}

public enum Sex
{
    Male = 0,
    Female = 1
}

public class ResultConfiguration : IEntityTypeConfiguration<Result>
{
    public void Configure(EntityTypeBuilder<Result> builder)
    {
        builder
            .Property(e => e.Splits)
            .HasConversion(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<List<double>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
    }
}