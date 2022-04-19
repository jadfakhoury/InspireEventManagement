using System.Text;

namespace EventManagementLibrary.Models;

public class ResponseModel
{
    public string StatusCode { get; set; }
    public string StatusDescription { get; set; }
    public string Message { get; set; }


    public ResponseModel(Exception e)
    {
        this.StatusCode = "Exception";
        this.StatusDescription = "Something went wrong!";
        this.Message = e.Message;
    }

    public override string ToString()
    {
        StringBuilder stringResponse = new StringBuilder();
        stringResponse.Append("\nStatus Code: " + this.StatusCode + "\n");
        stringResponse.Append("Status Description: " + this.StatusDescription + "\n");
        stringResponse.Append("Message: " + this.Message);
        return stringResponse.ToString(); ;
    }
}
