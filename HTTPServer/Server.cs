﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket, tmpSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portNumber);
            serverSocket.Bind(hostEndPoint);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(1000);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSock = serverSocket.Accept();
                Thread thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                thread.Start(clientSock);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket
            Socket clientSocket = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] clientData = new byte[1024];
                    // TODO: break the while loop if receivedLen==0
                    int Length = clientSocket.Receive(clientData);
                    if (Length == 0)
                        break;
                    // TODO: Create a Request object using received request string
                    string msg = Encoding.ASCII.GetString(clientData);
                    Request request = new Request(msg);
                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);
                    // TODO: Send Response back to client
                    clientSocket.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSocket.Close();
        }

        Response HandleRequest(Request request)
        {
            // throw new NotImplementedException();
            string content;
            try
            {
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    return new Response(StatusCode.BadRequest, "text/html", content, "");
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                string physicalPath = Path.Combine(Configuration.RootPath, request.relativeURI);
                //TODO: check for redirect
                string relativePath = Path.GetFileName(request.relativeURI);
                string redirectedPath = GetRedirectionPagePathIFExist(relativePath);
                if (redirectedPath != string.Empty)
                {
                    string physicalRedirectedPath = Path.Combine(Configuration.RootPath, redirectedPath);
                    content = LoadDefaultPage(physicalRedirectedPath);
                    return new Response(StatusCode.Redirect, "text/html", content, redirectedPath);
                }
                //TODO: check file exists
                if (!File.Exists(physicalPath))
                {
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    return new Response(StatusCode.NotFound, "text/html", content, "");
                }
                //TODO: read the physical file
                else
                {
                    content = LoadDefaultPage(physicalPath);
                    return new Response(StatusCode.OK, "text/html", content, "");
                }
                // Create OK response
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                return new Response(StatusCode.InternalServerError, "text/html", content, "");
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
                return Configuration.RedirectionRules[relativePath];
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (!File.Exists(filePath))
            {
                Logger.LogException(new Exception());
                return string.Empty;
            }
            // else read file and return its content
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string fileContent = sr.ReadToEnd();
            return fileContent;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                string fileContent = "";
                // then fill Configuration.RedirectionRules dictionary
                Configuration.RedirectionRules = new Dictionary<string, string>();
                while ((fileContent = sr.ReadLine()) != null)
                {
                    string[] splitedFile = fileContent.Split(',');
                    Configuration.RedirectionRules.Add(splitedFile[0], splitedFile[1]);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
