using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace WebCrawler
{
    class Program
    {
        static string hrefPattern = "<a.*?href=\"(.*?)\".*?>";
        static void Main(string[] args)
        {
            Stack<string> s = new Stack<string>();
            LinkedList<string> a = new LinkedList<string>();
            if (args.Length > 1)
            {
                Console.WriteLine("No URL to Parse!");
                Environment.Exit(0);
            }
            string urlToParse = args[0];
            string htmlUnParsed = GetWebText(urlToParse);
            a.AddLast(urlToParse);
            while (urlToParse != null)
            {
                var r = new Regex(hrefPattern);
                var output = r.Matches(htmlUnParsed);
                var urls = new List<string>();
                foreach (var item in output)
                {
                    urls.Add((item as Match).Groups[1].Value);
                }
                foreach (var v in urls)
                {
                    if(a.Find(v) != null)
                    {
                        continue;
                    }
                    if(v.Length < 4)
                    {
                        continue;
                    }

                    if (v[0] == '/')
                    {
                        if (a.Find(args[0] + v) == null)
                        {
                            s.Push(args[0] + v);
                            Console.WriteLine(args[0] + v);
                        }
                    }
                    else if (v[0] == '.' || v[0] == '#' || v.Substring(v.Length - 4,1) == "." )
                    {
                        Console.WriteLine($"Skipping {v}.");
                        continue;
                    }
                    else
                    {
                        if (a.Find(v) == null)
                        {
                            s.Push(v);
                            Console.WriteLine(v);
                        }
                    }

                    


                }
                Console.WriteLine($"\nFinished Parsing page {urlToParse}!\n");
                if (s.Count == 0)
                {
                    Console.WriteLine("No Links to Follow!");
                    htmlUnParsed = null;
                    urlToParse = null;
                }
                else
                {
                    urlToParse = s.Pop();
                    a.AddLast(urlToParse);
                    htmlUnParsed = GetWebText(urlToParse);
                }


            }
            Console.WriteLine("All links followed.");
        }
        private static string GetWebText(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.UserAgent = "";
            WebResponse response;
            try
            {
                 response = request.GetResponse();
            }
            catch(Exception e)
            {
                return null;
            }
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string htmlText = reader.ReadToEnd();
            return htmlText;
        }

    }
}
