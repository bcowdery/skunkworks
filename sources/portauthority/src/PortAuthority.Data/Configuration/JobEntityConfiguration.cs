using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortAuthority.Data.Entities;
using PortAuthority.Data.Json;

namespace PortAuthority.Data.Configuration
{
    public class JobEntityConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {          
            // Indexes
            builder.HasIndex(j => j.JobId);
            builder.HasIndex(j => new { j.Type, j.Namespace });

            // One-to-many
            builder
                .HasMany(j => j.Tasks)
                .WithOne(t => t.Job);

            // Metadata as a JSON serialized column
            builder
                .Property(j => j.Meta)
                .IsRequired()
                .HasConversion(JsonDictionary.ValueConverter)
                .Metadata.SetValueComparer(JsonDictionary.ValueComparer);
        }
    }
}
