using EventManagementLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventManagementLibrary.Models
{
    public partial class Event :IDBObject
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "{0} can not exceed {1} charaters")]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(350, ErrorMessage = "{0} can not exceed {1} charaters")]
        public string Description { get; set; } = null!;

        [Required]
        public DateTime EventStart { get; set; }

        [Required]
        public DateTime EventEnd { get; set; }
        public string Images { get; set; } = null!;
    }
}
