using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines; //Done
        RequestMethod method; //Done
        public string relativeURI; //Done
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion; //Done
        string requestString;   //Done  
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //GET http://www.w3.org/pub/WWW/TheProject.html HTTP/1.1
            //TODO: parse the receivedRequest using the \r\n delimeter
            string[] requestLines = requestString.Split(new char[] {'\r', '\n', ' '});

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length != 4)
                return false;
            // Parse Request line
            if (!ParseRequestLine())
                return false;
            // Validate blank line exists
            if (!ValidateBlankLine())
                return false;
            // Load header lines into HeaderLines dictionary
            if (!LoadHeaderLines())
                return false;
            return true;
        }

        private bool ParseRequestLine()
        {
            if (requestLines[0] == "GET")
                method = RequestMethod.GET;
            else if (requestLines[0] == "POST")
                method = RequestMethod.POST;
            else if (requestLines[0] == "HEAD")
                method = RequestMethod.HEAD;
            else
                return false;
            if (ValidateIsURI(requestLines[1]))
            {
                int cnt = 0, ptr;
                for(ptr = 0; ptr < requestLines[1].Length && cnt < 3; ptr++)
                {
                    bool f = (requestLines[1][ptr] == '/');
                    if (f)
                        cnt++;
                }
                relativeURI = "/" + requestLines[1].Substring(ptr);
            }
            else
                return false;
            if (requestLines[2] == "HTTP/1.1")
                httpVersion = HTTPVersion.HTTP11;
            else if (requestLines[2] == "HTTP/1.0")
                httpVersion = HTTPVersion.HTTP10;
            else if (requestLines[2] == "HTTP/0.9")
                httpVersion = HTTPVersion.HTTP09;
            else
                return false;
            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            string type = relativeURI.Split('.')[relativeURI.Split('.').Length - 1];
            if (type == "html" || type == "text")
            {
                headerLines["Content-Type"] = type;
            }
            else
                return false;
            headerLines["Date"] = DateTime.UtcNow.Date.ToString("dd/MM/yyyy");
            return true;
        }

        private bool ValidateBlankLine()
        {
            if (requestLines[3] != "")
                return false;
            return true;
        }

    }
}
