using EventManagementLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EventManagementLibrary.Models
{
    public partial class Event : IDBObject
    {
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("title")]
        [MaxLength(50, ErrorMessage = "{0} can not exceed {1} charaters")]
        public string Title { get; set; } = null!;

        [Required]

        [MaxLength(350, ErrorMessage = "{0} can not exceed {1} charaters")]
        public string Description { get; set; } = null!;

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        public string Images { get; set; } = null!;
    }
}
