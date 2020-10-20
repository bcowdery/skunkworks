using System;
using System.ComponentModel.DataAnnotations;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Forms
{
    public class EndSubtaskForm
    {
        public bool Success { get; set; }
        
        [Required]
        public DateTimeOffset EndTime { get; set; }

        public override string ToString()
        {
            return $"{nameof(Success)}: {Success}, {nameof(EndTime)}: {EndTime}";
        }
    }
}
