using EventManagementLibrary.Models;

namespace EventManagementUI.Models;

public class EventViewModel
{
    public Event Event { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public List<IFormFile> detailsImageList { get; set; }
}
