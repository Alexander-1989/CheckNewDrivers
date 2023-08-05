using CsQuery;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CheckNewDrivers
{
    class Program
    {
        private static readonly WebClient webClient = new WebClient();

        private static string GetRootAddress(string url)
        {
            string domain = ".com";
            int index = url.IndexOf(domain);

            if (index > -1)
            {
                return url.Substring(0, index + domain.Length);
            }

            return string.Empty;
        }

        private static string CombineAddress(string url, string path)
        {
            string rootUrl = GetRootAddress(url);
            return path.StartsWith('/') ? rootUrl + path : rootUrl + '/' + path;
        }

        private static string ReadUrlFromFile(string fileName)
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(fileName))
                {
                    if (!streamReader.EndOfStream)
                    {
                        return streamReader.ReadLine();
                    }
                }
            }
            catch (Exception) { }
            return null;
        }

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
            return files.GetFirst() ?? string.Empty;
        }

        private static string FindHref(IDomElement element)
        {
            const string className = "mobile-only";
            IDomElement current = element;

            while (current != null)
            {
                if (current.ClassName.Equals(className))
                {
                    return current.FirstElementChild.GetAttribute("href");
                }
                current = current.NextElementSibling;
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
            return drivers.GetFirst();
        }

        static string GetProgressLine(int percentage)
        {
            StringBuilder persentLine = new StringBuilder(20);

            for (int i = 0; i < 100 / 5; i++)
            {
                char ch = (i < percentage / 5) ? '#' : '-';
                persentLine.Append(ch);
            }

            return persentLine.ToString();
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
                    Console.WriteLine($"Downloading {currentPercentage}% {GetProgressLine(currentPercentage)}");
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
                    switch (inputChar)
                    {
                        case 'd':
                        case 'в':
                        case 'D':
                        case 'В':
                            string href = CombineAddress(url, productVersion.Href);
                            string fileName = $"MOTU M Series Installer ({productVersion.Version}).exe";
                            Download(href, fileName);
                            break;
                        case 'o':
                        case 'щ':
                        case 'O':
                        case 'Щ':
                            Console.WriteLine("Opening website.");
                            Process.Start(url);
                            break;
                        default:
                            break;
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
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            string fileName = "URL.txt";
            string defaultURL = "https://motu.com/en-us/download/product/408/";
            Console.WriteLine($"Read url address from \"{fileName}\" file...");

            try
            {
                string url = null;
                if (File.Exists(fileName))
                {
                    url = ReadUrlFromFile(fileName);
                }
                else
                {
                    Console.WriteLine($"File \"{fileName}\" not found. Set default url address...");
                    url = defaultURL;
                }

                Console.WriteLine("Checking for new drivers. Waiting...");
                CheckVersion(url);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            WaitExit(5);
            Console.ReadKey();
        }
    }
}