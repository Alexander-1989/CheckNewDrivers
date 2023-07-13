using CsQuery;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CheckNewDrivers
{
    class Program
    {
        private static readonly WebClient webClient = new WebClient();

        private static string GetFileVersions(string path)
        {
            const string partOfName = "MOTU M Series Installer";
            List<string> files = new List<string>();
            foreach (string file in Directory.GetFiles(path, "*.exe"))
            {
                if (file.Contains(partOfName))
                {
                    int startIndex = file.LastIndexOf('(');
                    int endIndex = file.LastIndexOf(')');

                    if (startIndex > -1 && endIndex > startIndex)
                    {
                        files.Add(file.Substring(startIndex + 1, endIndex - startIndex - 1));
                    }
                }
            }

            files.Sort((str1, str2) => string.Compare(str2, str1));
            return files.First() ?? string.Empty;
        }

        private static string FindHref(IDomElement element)
        {
            const string className = "mobile-only";
            const string address = "https://motu.com";
            for (IDomElement sibling = element; sibling != null; sibling = sibling.NextElementSibling)
            {
                if (sibling.ClassName == className)
                {
                    return address + sibling.FirstElementChild.GetAttribute("href");
                }
            }
            return string.Empty;
        }

        private static void GetContent(IDomElement element, List<Item> drivers)
        {
            const string prefix = "PC v";
            string content = element.InnerText;

            if (content.Contains(prefix) && !content.Contains("."))
            {
                string version = content.Replace(prefix, null);
                string href = FindHref(element);
                drivers.Add(new Item(version, href));
            }
        }

        private static Item GetDriverVersion(string source)
        {
            const string selector = ".platform-logos";
            List<Item> drivers = new List<Item>();
            CQ cq = new CQ(selector, source);

            foreach (IDomObject item in cq)
            {
                GetContent(item.NextElementSibling, drivers);
            }

            drivers.Sort((item1, item2) => string.Compare(item2.Version, item1.Version));
            return drivers.First();
        }

        private static void Download(string address, string fileName)
        {
            int left = 0;
            int top = Console.CursorTop;
            int prevPercentage = -1;

            webClient.DownloadProgressChanged += (s, e) =>
            {
                int currentPercentage = e.ProgressPercentage;
                if (currentPercentage != prevPercentage)
                {
                    Console.WriteLine($"Downloading {currentPercentage} %");
                    Console.SetCursorPosition(left, top);
                    prevPercentage = currentPercentage;
                }
            };

            webClient.DownloadFileTaskAsync(address, fileName).Wait();
            Console.WriteLine("\nDownload completed.");
        }

        static void CheckVersion(string url)
        {
            try
            {
                string source = webClient.DownloadString(url);
                Item productVersion = GetDriverVersion(source);
                string fileProductVersion = GetFileVersions(Environment.CurrentDirectory);

                if (string.IsNullOrEmpty(productVersion?.Version))
                {
                    Console.WriteLine("Unable to find new driver version.");
                }
                else if (string.Compare(productVersion.Version, fileProductVersion) > 0)
                {
                    Console.WriteLine("There is a NEW VERSION of drivers!!!");
                    Console.WriteLine("Press 'D' for download a new version or 'O' for visit a website page.");

                    char inputChar = Console.ReadKey(true).KeyChar;
                    if (inputChar == 'd' || inputChar == 'в' || inputChar == 'D' || inputChar == 'В')
                    {
                        string fileName = $"MOTU M Series Installer ({productVersion.Version}).exe";
                        Download(productVersion.Href, fileName);
                    }
                    else if (inputChar == 'o' || inputChar == 'щ' || inputChar == 'O' || inputChar == 'Щ')
                    {
                        Console.WriteLine("Opening website.");
                        Process.Start(url);
                    }
                }
                else
                {
                    Console.WriteLine("You already have the LATEST drivers.");
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        static void WaitExit(int seconds)
        {
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            new Task(() =>
            {
                for (int i = seconds; i > 0; i--)
                {
                    Console.SetCursorPosition(left, top);
                    Console.Write($"Waiting {i} seconds for Exit...");
                    Thread.Sleep(1000);
                }
                Environment.Exit(0);
            }).Start();
        }

        static void Main()
        {
            const string url = "https://motu.com/en-us/download/product/408/?download_type=driver&platform_family=win";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            Console.WriteLine("Checking for new drivers. Waiting...");
            CheckVersion(url);
            WaitExit(5);
            Console.ReadKey();
        }
    }
}