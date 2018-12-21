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
        Dictionary<string, string> headerLines; //Done

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion; //Done
        string requestString;   //Done  
        string[] contentLines;
        string[] tempLines; // Done
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
            requestLines = requestString.Split(new char[] {'\r', '\n'});
            tempLines = requestLines[0].Split(' ');
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 3)
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
            if (tempLines[0] == "GET")
                method = RequestMethod.GET;
            else if (tempLines[0] == "POST")
                method = RequestMethod.POST;
            else if (tempLines[0] == "HEAD")
                method = RequestMethod.HEAD;
            else
                return false;
            if (ValidateIsURI(tempLines[1]))
            {
                int cnt = 0, ptr;
                for(ptr = 0; ptr < tempLines[1].Length && cnt < 3; ptr++)
                {
                    bool f = (tempLines[1][ptr] == '/');
                    if (f)
                        cnt++;
                }
                relativeURI = "/" + tempLines[1].Substring(ptr);
            }
            else
                return false;
            if (tempLines[2] == "HTTP/1.1")
                httpVersion = HTTPVersion.HTTP11;
            else if (tempLines[2] == "HTTP/1.0")
                httpVersion = HTTPVersion.HTTP10;
            else if (tempLines[2] == "HTTP/0.9")
                httpVersion = HTTPVersion.HTTP09;
            else
                return false;
            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }
        private int getSeprator(string header)
        {
            for(int i = 0; i < header.Length; i++)
            {
                if (header[i] == ':')
                    return i + 1;
            }
            return -1;
        }
        private bool LoadHeaderLines()
        {
            for(int i = 1; i < requestLines.Length - 1; i++)
            {
                int x = getSeprator(requestLines[i]);
                if (x == -1)
                    return false;
                headerLines[requestLines[i].Substring(0, x)] = requestLines[i].Substring(x);
            }
            return true;
        }

        private bool ValidateBlankLine()
        {
            if (requestLines[requestLines.Length - 1] != "")
                return false;
            return true;
        }

    }
}
