using System;
using System.Collections.Generic;
using System.Text;
using Bogus;
using MassTransit;
using PortAuthority.Data.Entities;

namespace PortAuthority.Test.Fakes
{
    /// <summary>
    /// Generates fake Subtask objects
    /// </summary>
    public sealed class SubtaskFaker : Faker<Subtask>
    {
        private long? _jobId;
        private Dictionary<string, object> _metadata;
        
        public SubtaskFaker()
        {
            RuleFor(j => j.JobId, f => _jobId ?? 0L);
            RuleFor(j => j.TaskId, f => NewId.NextGuid());
            RuleFor(j => j.Name, f => f.Lorem.Word());
            RuleFor(j => j.Status, f => f.PickRandom<Status>());

            RuleFor(j => j.StartTime, (f, j) =>
            {
                if (j.Status != Status.Pending)
                {
                    return f.Date.RecentOffset();
                }
                return null;
            });

            RuleFor(j => j.EndTime, (f, j) =>
            {
                if (j.Status == Status.Failed || j.Status == Status.Completed)
                {
                    return f.Date.SoonOffset();
                }
                return null;
            });

            RuleFor(j => j.Meta,
                f => _metadata ?? new Dictionary<string, object>()
                {
                    {$"{f.Lorem.Slug()}-1", f.Random.Int()},
                    {$"{f.Lorem.Slug()}-2", f.Lorem.Sentence()},
                    {$"{f.Lorem.Slug()}-3", f.Random.Bool()}
                });

            RuleSet("Pending", set =>
            {
                set.RuleFor(j => j.Status, Status.Pending);
                set.RuleFor(j => j.StartTime, f => null);
                set.RuleFor(j => j.EndTime, f => null);
            });

            RuleSet("InProgress", set =>
            {
                set.RuleFor(j => j.Status, Status.InProgress);
                set.RuleFor(j => j.StartTime, f => f.Date.RecentOffset());
                set.RuleFor(j => j.EndTime, f => null);
            });
            
            RuleSet("Completed", set =>
            {
                set.RuleFor(j => j.Status, Status.Completed);
                set.RuleFor(j => j.StartTime, f => f.Date.RecentOffset());
                set.RuleFor(j => j.EndTime, f => f.Date.SoonOffset());
            });

            RuleSet("Failed", set =>
            {
                set.RuleFor(j => j.Status, Status.Failed);
                set.RuleFor(j => j.StartTime, f => f.Date.RecentOffset());
                set.RuleFor(j => j.EndTime, f => f.Date.SoonOffset());
            });
        }

        /// <summary>
        /// Explicitly set parent job ID.
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public SubtaskFaker SetJobId(long jobId)
        {
            _jobId = jobId;
            return this;
        }

        /// <summary>
        /// Explicitly sets the metadata for generated jobs.
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public SubtaskFaker SetMeta(Dictionary<string, object> metadata)
        {
            _metadata = metadata;
            return this;
        }
    }
}
