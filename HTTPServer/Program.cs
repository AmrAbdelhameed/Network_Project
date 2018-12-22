using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
            //Start server
            // 1) Make server object on port 1000
            // 2) Start Server
            string redirectionRulesPath = "D:\\_FCIS\\Sna 4\\Semester 1\\Network\\Sections\\Network_Project\\HTTPServer\\bin\\Debug\\redirectionRules.txt";
            Server s = new Server(1000, redirectionRulesPath);
            s.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
            FileStream fw = new FileStream("redirectionRules.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fw);
            sw.WriteLine("aboutus.html,aboutus2.html");
            sw.WriteLine("aboutus2.html,aboutus2.html");
            sw.WriteLine("BadRequest.html,NotFound.html");
            sw.Close();
        }
    }
}
