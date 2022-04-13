using System;
using System.Collections.Generic;

namespace EventManagementLibrary.Models
{
    public partial class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime EventStart { get; set; }
        public DateTime EventEnd { get; set; }
        public string Images { get; set; } = null!;
    }
}
