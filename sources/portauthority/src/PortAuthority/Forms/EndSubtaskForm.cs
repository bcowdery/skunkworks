using System;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Forms
{
    public class EndSubtaskForm
    {
        public bool Success { get; set; }
        public DateTimeOffset EndTime { get; set; }

        public override string ToString()
        {
            return $"{nameof(Success)}: {Success}, {nameof(EndTime)}: {EndTime}";
        }
    }
}
