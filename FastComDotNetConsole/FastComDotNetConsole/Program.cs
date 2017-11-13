using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Fast_Com
{
    class Program
    {
        private static int[] results;
        static void Main(string[] args)
        {
            // A simple source for demonstration purposes. Modify this path as necessary.
            /*Uri[] files = {
                new Uri("https://ipv4-c001-mnl001-globetelecom-isp.1.oca.nflxvideo.net/speedtest?c=ph&n=132199&v=3&e=1510532837&t=1t9sVNjxy8PXMhnCVO8tcvBLA7A"),
                new Uri("https://ipv4-c001-ceb001-globetelecom-isp.1.oca.nflxvideo.net/speedtest?c=ph&n=132199&v=3&e=1510532837&t=8NH3CdzCbNREj-55kKxyp5VXkAU")
            };
            // Method signature: Parallel.ForEach(IEnumerable<TSource> source, Action<TSource> body)
            // Be sure to add a reference to System.Drawing.dll.
            Parallel.ForEach(files, currentFile => MyDownloadFile(currentFile));


            // Keep the console window open in debug mode.
            Console.WriteLine("Processing complete. Press any key to exit.");
            Console.ReadKey();
            */
            OpenUrl();
            //GetIpAddressList("api.fast.com");
            Console.WriteLine("Processing complete. Press any key to exit.");
            Console.ReadKey();
        }
        private static void TestMethod(string args)
        {
            Console.WriteLine("Processing {0} on thread {1}", args, Thread.CurrentThread.ManagedThreadId);
            System.Threading.Thread.Sleep(5000);
        }
        public static void MyDownloadFile(Uri url, int index)
        {
            const int BUFFER_SIZE = 16 * 1024;
            var req = WebRequest.Create(url);
            req.Credentials = CredentialCache.DefaultCredentials;
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
                        results[index] += bytesRead;
                    }

                    s.Stop();
                    Console.WriteLine("Downloaded: {0} Kilobytes",results[index]/1024.0);
                    Console.WriteLine("Speed: {0} Kbps", results[index]*8/3.0/1024.0);
                    /*do
                    {
                        bytesRead = responseStream.Read(buffer, 0, BUFFER_SIZE);
                        results[index] += bytesRead;
                        //outputFileStream.Write(buffer, 0, bytesRead);
                        //Console.WriteLine("Total Bytes {0} from {1} Index {2}", results[index], url, index);
                    } while (bytesRead > 0);*/

                }
            }
        }
        private static void OpenUrl()
        {
            Uri[] links;
            string url = "https://fast.com";
            string result;
            string jsfileurl = "";
            string jsfile;
            string token = "";
            string finalurl = "";
            string baseurl = "https://api.fast.com/";
            using (WebClient client = new WebClient())
            {
                result = client.DownloadString(url);
            }
            //Console.WriteLine(result);
            foreach (var line in result.Split("\n"))
            {
                if (line.Contains("script src"))
                {
                    jsfileurl = line.Split('"')[1];
                    break;
                }
            }
            using (WebClient client = new WebClient())
            {
                jsfile = client.DownloadString(url + jsfileurl);
            }
            int tokenlocation = jsfile.IndexOf("token", 1);
            int tokenend = jsfile.IndexOf(",", tokenlocation + 1);
            token = jsfile.Substring(tokenlocation + 7, tokenend - tokenlocation - 8);
            /*foreach (var line in jsfile.Split("\n"))
            {
                if (line.Contains("token:"))
                {
                    Console.WriteLine(line);
                    token = line.Split('"')[1];
                    break;
                }
            }*/
            int count = 0;
            Console.WriteLine(token);
            finalurl = baseurl + "netflix/speedtest?https=true&token=" + token + "&urlCount=3";
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString(finalurl);
                //Console.WriteLine(json);

                count = json.Split("url").Length - 1;
                links = new Uri[count];
                //Console.WriteLine(count);
                string urlparsed = "";
                int start = 1;
                for (var i = 1; i <= count; i++)
                {
                    int urlloc = json.IndexOf("url", start);
                    int urlend = json.IndexOf('"', urlloc + 7);

                    urlparsed = json.Substring(urlloc + 6, urlend - urlloc - 6);
                    links[i - 1] = new Uri(urlparsed);
                    //Console.WriteLine(urlparsed);
                    start = urlend;

                }

            }
            results = new int[count];
            MyDownloadFile(links[0], 0);
            

            //Thread.Sleep(2000);
            //int baseline = results[0];
            //Console.WriteLine("Initial (kbps) {0}", baseline);
            //Thread.Sleep(5000);
            //Console.WriteLine("Now (kbps) {0}", baseline);
            //double speed = (results[0] - baseline) / 5.0;

            //Console.WriteLine("Speed (kbps) {0}", speed);

            //Console.WriteLine(links[0].ToString());
        }
        public static void GetIpAddressList(String hostString)
        {
            try
            {
                // Get 'IPHostEntry' object containing information like host name, IP addresses, aliases for a host.
                IPHostEntry hostInfo = Dns.GetHostEntry(hostString);
                Console.WriteLine("Host name : " + hostInfo.HostName);
                Console.WriteLine("IP address List : ");
                for (int index = 0; index < hostInfo.AddressList.Length; index++)
                {
                    Console.WriteLine(hostInfo.AddressList[index]);
                    Console.WriteLine(hostInfo.AddressList[index].AddressFamily.ToString());
                    Console.WriteLine(ProtocolFamily.InterNetworkV6.ToString());
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
        }

    }
}
