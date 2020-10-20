using System;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Forms
{
    public class StartSubtaskForm
    {
        public DateTimeOffset StartTime { get; set; }

        public override string ToString()
        {
            return $"{nameof(StartTime)}: {StartTime}";
        }
    }
}
