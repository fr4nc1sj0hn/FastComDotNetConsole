using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace fast_com
{
    public class FastCodeApp
    {
        private int[] results;
        private const int BUFFER_SIZE = 16 * 1024;

        public FastCodeApp()
        {
            results = new int[2];

        }
        private void GetDownloadResult(Uri url, int index)
        {
            var req = WebRequest.Create(url);
            req.Credentials = CredentialCache.DefaultCredentials;
            using (var response = req.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    var buffer = new byte[BUFFER_SIZE];
                    int bytesRead;
                    do
                    {
                        bytesRead = responseStream.Read(buffer, 0, BUFFER_SIZE);
                        results[index] += bytesRead;
                        //Console.WriteLine("Total Bytes {0} from thread {1}", totalbytes, Thread.CurrentThread.ManagedThreadId);
                    } while (bytesRead > 0);

                }
            }
        }
        private void OpenUrl()
        {
            string url = "https://fast.com";
            string result;
            using (WebClient client = new WebClient())
            {
                result = client.DownloadString(url);
            }
        }
    }
}
