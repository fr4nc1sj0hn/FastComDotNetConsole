using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;

namespace fast_com
{
    public class FastCodeApp
    {

        private const int BUFFER_SIZE = 16 * 1024;
        private string fastcomhtmlresult;
        private const string url = "https://fast.com";
        private string jsfile = "";
        private string token = "";
        private const string baseurl = "https://api.fast.com/";
        private Uri[] Links;
        private double HighestSpeed = 0;
        private double RunningTotal = 0;
        public FastCodeApp()
        {

        }
        public void Run()
        {
            OpenUrl();
            OpenJSFile();
            GetTokenFromJSFile();
            GetISPUrls();
            ExecuteDownloads();
        }
       
        private void OpenUrl(bool verbose=false)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    fastcomhtmlresult = client.DownloadString(url);
                    if (verbose) Console.WriteLine(fastcomhtmlresult);
                }
                catch (WebException ex)
                {
                    Console.WriteLine("An error has occured or no connection is detected.\n\nError Message:\n{0}", ex.Message);
                    return;
                }
            }
        }
        private void OpenJSFile(bool verbose = false)
        {
            string jsfileurl = "";
            foreach (var line in fastcomhtmlresult.Split("\n"))
            {
                if (line.Contains("script src"))
                {
                    jsfileurl = line.Split('"')[1];
                    break;
                }
            }
            using (WebClient client = new WebClient())
            {
                try
                {
                    jsfile = client.DownloadString(url + jsfileurl);
                    if (verbose) Console.WriteLine(jsfile);
                }
                catch (WebException ex)
                {
                    Console.WriteLine("An error has occured or no connection is detected.\n\nError Message:\n{0}", ex.Message);
                    return;
                }
            }
        }
        private void GetTokenFromJSFile()
        {
            int tokenlocation = jsfile.IndexOf("token", 1);
            int tokenend = jsfile.IndexOf(",", tokenlocation + 1);

            token = jsfile.Substring(tokenlocation + 7, tokenend - tokenlocation - 8);

            Console.WriteLine("Token: {0}", token);
        }
        private void GetISPUrls()
        {
            string finalurl = baseurl + "netflix/speedtest?https=true&token=" + token + "&urlCount=3";
            int count = 0;
            using (WebClient wc = new WebClient())
            {
                try
                {
                    var json = wc.DownloadString(finalurl);

                    count = json.Split("url").Length - 1;

                    string urlparsed = "";
                    int start = 1;
                    Links = new Uri[count];
                    for (var i = 1; i <= count; i++)
                    {
                        int urlloc = json.IndexOf("url", start);
                        int urlend = json.IndexOf('"', urlloc + 7);

                        urlparsed = json.Substring(urlloc + 6, urlend - urlloc - 6);
                        Links[i - 1] = new Uri(urlparsed);
                        start = urlend;

                    }
                }
                catch (WebException ex)
                {
                    Console.WriteLine("An error has occured or no connection is detected.\n\nError Message:\n{0}", ex.Message);
                    return;
                }
            }
        }
        private void ExecuteDownloads()
        {
            int i = 0;
            foreach(var uri in Links)
            {
                Console.WriteLine("Test File {0}:\n{1}\n", i + 1, Links[i]);
                Console.WriteLine("-------------------------------------------");
                for (var j = 1; j <= 5; j++)
                {
                    DownloadTestFile(Links[i], j);
                }
                i++;
            }
            Console.WriteLine("\n-------------------------------------------");
            Console.WriteLine("\nAverage Speed: {0} Kpbs", Math.Round(RunningTotal/10.0,2));
            Console.WriteLine("Highest Speed: {0} Kpbs", Math.Round(HighestSpeed,2));
        }
        private void DownloadTestFile(Uri url,int trial)
        {

            try
            {
                var req = WebRequest.Create(url);
                req.Credentials = CredentialCache.DefaultCredentials;
                int totaldownloaded = 0;
                using (var response = req.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        var buffer = new byte[BUFFER_SIZE];
                        int bytesRead;
                        Stopwatch s = new Stopwatch();
                        s.Start();
                        while (s.Elapsed < TimeSpan.FromSeconds(3))
                        {
                            bytesRead = responseStream.Read(buffer, 0, BUFFER_SIZE);
                            totaldownloaded += bytesRead;
                            
                        }
                        s.Stop();
                        
                        double speed = Math.Round(totaldownloaded * 8 / 3.0 / 1024.0,2);
                        RunningTotal += speed;
                        HighestSpeed = speed > HighestSpeed ? speed : HighestSpeed;
                        Console.WriteLine("Try {0} Speed: {1} Kbps", trial, speed);

                    }
                }
            } catch (Exception ex)
            {
                Console.WriteLine("An error has occured or no connection is detected.\n\nError Message:\n{0}", ex.Message);
                return;
            }
        }
    }
}
