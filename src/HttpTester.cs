using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpTester
{
    class HttpTester
    {
        private string _webApiBase = "http://your.webapi.site.com/";

        public HttpTester()
        {
            Console.WriteLine("H for Help\n\n");
            Console.Write(">");
            ServicePointManager.MaxServicePointIdleTime = 10000;
        }

        static void Main(string[] args)
        {
            var thisTester = new HttpTester();

            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine("");

                switch (key.Key)
                {
                    case ConsoleKey.H:
                        thisTester.PrintHelp();
                        break;

                    case ConsoleKey.Escape:
                    case ConsoleKey.Q:
                        return;

                    case ConsoleKey.E:
                        thisTester.PostDataHttpClient(true);
                        break;

                    case ConsoleKey.D:
                        thisTester.PostDataHttpClient(false);
                        break;

                    case ConsoleKey.R:
                        thisTester.PostDataWebRequest(true);
                        break;

                    case ConsoleKey.F:
                        thisTester.PostDataWebRequest(false);
                        break;

                    case ConsoleKey.P:
                        thisTester.PrintServicePointManagerDefaults();
                        break;

                    case ConsoleKey.S:
                        thisTester.PrintServicePointSettings();
                        break;

                    case ConsoleKey.U:
                        thisTester.UpdateServicePointManagerDefaults();
                        break;

                    case ConsoleKey.W:
                        Console.WriteLine("Enter base webb address in form http://yoursite.com/");
                        Console.Write(">");
                        thisTester._webApiBase = Console.ReadLine();
                        Console.WriteLine("You entered: " + thisTester._webApiBase);
                        Console.Write(">");
                        break;

                    default:
                        Console.WriteLine(key.KeyChar + " is not an option.");
                        break;
                }
            }
        }

        private void UpdateServicePointManagerDefaults()
        {
            ServicePointManager.MaxServicePointIdleTime = 10000;
            ServicePointManager.MaxServicePoints = 3;
            ServicePointManager.DefaultConnectionLimit = 10;
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.SetTcpKeepAlive(true, 10000, 1000);
            ServicePointManager.Expect100Continue = false;

            PrintServicePointManagerDefaults();
        }

        private void PrintHelp()
        {
            StringBuilder helpText = new StringBuilder();
            helpText.AppendLine("** Help **");
            helpText.AppendLine();

            helpText.AppendLine("Q or ESC:\tQuit");
            helpText.AppendLine("E:\t\tPost via HttpClient - Expect 100-Continue TRUE");
            helpText.AppendLine("D:\t\tPost via HttpClient - Expect 100-Continue FALSE");
            helpText.AppendLine("R:\t\tPost via HttpWebRequest - Expect 100-Continue TRUE");
            helpText.AppendLine("F:\t\tPost via HttpWebRequest - Expect 100-Continue FALSE");
            helpText.AppendLine("P:\t\tPrint current ServicePointManager settings");
            helpText.AppendLine("S:\t\tPrint current ServicePoint settings");
            helpText.AppendLine("U:\t\tUpdate ServicePointManager settings");
            helpText.AppendLine("W:\t\tEnter base web api address (format like: http://yoursite.com/)");

            Console.WriteLine(helpText.ToString());
            Console.Write(">");
        }

        public void PrintServicePointManagerDefaults()
        {
            StringBuilder settingsText = new StringBuilder();

            settingsText.AppendLine("** Current ServicePointManager Settings **\n");
            settingsText.AppendLine("CheckCertificateRevocationList: \t" + ServicePointManager.CheckCertificateRevocationList.ToString());
            settingsText.AppendLine("DefaultConnectionLimit: \t\t" + ServicePointManager.DefaultConnectionLimit.ToString());
            settingsText.AppendLine("DefaultNonPersistentConnectionLimit: \t" + ServicePointManager.DefaultNonPersistentConnectionLimit.ToString());
            settingsText.AppendLine("DefaultPersistentConnectionLimit: \t" + ServicePointManager.DefaultPersistentConnectionLimit.ToString());
            settingsText.AppendLine("DnsRefreshTimeout: \t\t\t" + ServicePointManager.DnsRefreshTimeout.ToString());
            settingsText.AppendLine("EnableDnsRoundRobin: \t\t\t" + ServicePointManager.EnableDnsRoundRobin.ToString());
            settingsText.AppendLine("EncryptionPolicy: \t\t\t" + ServicePointManager.EncryptionPolicy.ToString());
            settingsText.AppendLine("Expect100Continue: \t\t\t" + ServicePointManager.Expect100Continue.ToString());
            settingsText.AppendLine("MaxServicePointIdleTime: \t\t" + ServicePointManager.MaxServicePointIdleTime.ToString());
            settingsText.AppendLine("MaxServicePoints: \t\t\t" + ServicePointManager.MaxServicePoints.ToString());
            settingsText.AppendLine("SecurityProtocol: \t\t\t" + ServicePointManager.SecurityProtocol.ToString());
            settingsText.AppendLine("UseNagleAlgorithm: \t\t\t" + ServicePointManager.UseNagleAlgorithm.ToString());
            
            Console.WriteLine(settingsText.ToString());
            Console.Write(">");
        }

        public void PrintServicePointSettings()
        {
            ServicePoint sp = ServicePointManager.FindServicePoint(new Uri(_webApiBase));
            PrintServicePointSettings(sp);
        }

        public void PrintServicePointSettings(ServicePoint sp)
        {
            StringBuilder spSettings = new StringBuilder();

            spSettings.AppendLine("** Current ServicePoint Settings **\n");
            spSettings.AppendLine("Address\t\t\t" + sp.Address);
            spSettings.AppendLine("BindIPEndPointDelegate\t" + sp.BindIPEndPointDelegate);
            spSettings.AppendLine("Certificate\t\t" + sp.Certificate);
            spSettings.AppendLine("ClientCertificate\t\t" + sp.ClientCertificate);
            spSettings.AppendLine("ConnectionLeaseTimeout\t" + sp.ConnectionLeaseTimeout);
            spSettings.AppendLine("ConnectionLimit\t\t" + sp.ConnectionLimit);
            spSettings.AppendLine("ConnectionName\t\t" + sp.ConnectionName);
            spSettings.AppendLine("CurrentConnections\t" + sp.CurrentConnections);
            spSettings.AppendLine("Expect100Continue\t" + sp.Expect100Continue);
            spSettings.AppendLine("IdleSince\t\t" + sp.IdleSince);
            spSettings.AppendLine("MaxIdleTime\t\t" + sp.MaxIdleTime);
            spSettings.AppendLine("ProtocolVersion\t\t" + sp.ProtocolVersion);
            spSettings.AppendLine("ReceiveBufferSize\t" + sp.ReceiveBufferSize);
            spSettings.AppendLine("SupportsPipelining\t" + sp.SupportsPipelining);
            spSettings.AppendLine("UseNagleAlgorithm\t" + sp.UseNagleAlgorithm);

            Console.WriteLine(spSettings.ToString());
            Console.Write(">");
        }

        public async void PostDataWebRequest(bool Is100Expected)
        {
            Console.WriteLine("Posting via WebRequest. Is100Expected: {0}", Is100Expected);
            Console.Write(">");

            Stream dataStream = null;
            WebResponse webResponse = null;
            ServicePointManager.Expect100Continue = Is100Expected;
            try
            {
                HttpWebRequest webRequestClient = (HttpWebRequest)WebRequest.Create(_webApiBase + "api/Values/Post");

                // NOTE: If you set the proxy to null, you will bypass Fiddler.
                //       If you don't set the proxy to null, then ALL traffic
                //       will resolve as the Fiddler proxy 127.0.0.1:8888 and
                //       therefore use the same ServicePoint.
                //webRequestClient.Proxy = null;

                webRequestClient.UserAgent = ".NET Framework Example Client";
                webRequestClient.Method = "POST";
                webRequestClient.ContentType = "application/json; charset=utf-8";
                webRequestClient.Accept = "application/json";
                webRequestClient.KeepAlive = false;

                //webRequestClient.ServicePoint.Expect100Continue = Is100Expected;
                webRequestClient.ServicePoint.UseNagleAlgorithm = true;
                webRequestClient.ServicePoint.ConnectionLimit = 50;

                PrintServicePointSettings(webRequestClient.ServicePoint);

                byte[] contentArr = Encoding.UTF8.GetBytes("\"Foo\"");
                webRequestClient.ContentLength = contentArr.Length;

                dataStream = webRequestClient.GetRequestStream();
                dataStream.Write(contentArr, 0, contentArr.Length);
                dataStream.Close();

                webResponse = await webRequestClient.GetResponseAsync();

                var responseMessage = new HttpResponseMessage(((HttpWebResponse)webResponse).StatusCode);

                Console.WriteLine("Request successful? {0}", responseMessage.IsSuccessStatusCode);
                Console.Write(">");
            }
            finally
            {
                if (dataStream != null)
                    dataStream.Close();

                if (webResponse != null)
                    webResponse.Close();
            }
        }

        public async void PostDataHttpClient(bool is100Expected)
        {
            Console.WriteLine("Posting via HttpClient. Is100Expected: {0}", is100Expected);
            Console.Write(">");
            ServicePointManager.Expect100Continue = is100Expected;
            WebClient foo = new WebClient();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_webApiBase);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Framework Example Client");

                //client.DefaultRequestHeaders.ExpectContinue = Is100Expected;
                client.DefaultRequestHeaders.ConnectionClose = true;

                HttpResponseMessage response = await client.PostAsJsonAsync<string>("api/Values/Post", "Foo");

                Console.WriteLine("Request successful? {0}", response.IsSuccessStatusCode);
                Console.Write(">");
            }
        }
    }
}