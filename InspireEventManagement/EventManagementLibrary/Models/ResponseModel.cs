using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BookstopNetModels.Models
{
    public class ResponseModel
    {
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string Message { get; set; }

        public ResponseModel() { }

        public ResponseModel(Exception e)
        {
            this.StatusCode = "Exception";
            this.StatusDescription = String.IsNullOrEmpty(e.InnerException.Message) ? "Error" : e.InnerException.Message;
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
}
