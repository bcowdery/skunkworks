using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortAuthority.Data.Entities;
using PortAuthority.Data.Json;

namespace PortAuthority.Data.Configuration
{
    public class SubtaskEntityConfiguration : IEntityTypeConfiguration<Subtask>
    {
        public void Configure(EntityTypeBuilder<Subtask> builder)
        {  
            // Indexes
            builder.HasIndex(t => t.TaskId);
            builder.HasIndex(t => new { t.JobId, t.Name });

            // Metadata as a JSON serialized column
            builder
                .Property(j => j.Meta)
                .IsRequired()
                .HasConversion(JsonDictionary.ValueConverter)
                .Metadata.SetValueComparer(JsonDictionary.ValueComparer);
        }
    }
}
