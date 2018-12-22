using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {

            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            string statusline = GetStatusLine(code);
            headerLines.Add(contentType);
            headerLines.Add(content.Length.ToString());
            headerLines.Add(DateTime.Now.ToString());
            if (redirectoinPath.Count() != 0)
                headerLines.Add(redirectoinPath);
            // TODO: Create the request string
            if (redirectoinPath != string.Empty)
                responseString = statusline + "\ncontentType:" + headerLines[0] + "\ncontentLength:" + headerLines[1] + "\nDate:" + headerLines[2] + "\nredirectoinPath:" + headerLines[3] + "\n" + content;
            else
                responseString = statusline + "\ncontentType:" + headerLines[0] + "\ncontentLength:" + headerLines[1] + "\nDate:" + headerLines[2] + "\n" + content;
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;
            if (StatusCode.OK == code)
            {
                statusLine = Configuration.ServerHTTPVersion + code + "OK";
            }
            else if (StatusCode.BadRequest == code)
            {
                statusLine = Configuration.ServerHTTPVersion + code + "BadRequest";
            }
            else if (StatusCode.NotFound == code)
            {
                statusLine = Configuration.ServerHTTPVersion + code + "NotFound";
            }
            else if (StatusCode.InternalServerError == code)
            {
                statusLine = Configuration.ServerHTTPVersion + code + "InternalServerError";
            }
            else if (StatusCode.Redirect == code)
            {
                statusLine = Configuration.ServerHTTPVersion + code + "Redirect";
            }
            return statusLine;
        }
    }
}
