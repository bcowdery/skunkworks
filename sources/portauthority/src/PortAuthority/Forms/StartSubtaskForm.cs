﻿using System;
using System.ComponentModel.DataAnnotations;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Forms
{
    public class StartSubtaskForm
    {
        [Required]
        public DateTimeOffset StartTime { get; set; }

        public override string ToString()
        {
            return $"{nameof(StartTime)}: {StartTime}";
        }
    }
}
