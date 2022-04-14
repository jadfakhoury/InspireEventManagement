namespace EventManagementUI.Models;

public class ImageListUploadViewModel
{
    public HttpResponseMessage Response { get; set; }
    public List<string> ImagesNames { get; set; }

    public ImageListUploadViewModel()
    {
        ImagesNames = new List<string>();
    }
}
