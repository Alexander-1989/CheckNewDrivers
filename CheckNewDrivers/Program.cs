using CsQuery;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

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

        private static bool IsDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        private static string NormalizeVersion(string version)
        {
            StringBuilder newVersion = new StringBuilder(10);
            foreach (char ch in version)
            {
                if (IsDigit(ch))
                {
                    newVersion.Append(ch);
                }
                else if (ch == ' ')
                {
                    break;
                }
            }

            int maxLength = 5;
            int currentLength = newVersion.Length;

            return currentLength <= maxLength ? newVersion.ToString() : newVersion.ToString(currentLength - maxLength, maxLength);
        }

        private static int OrderByDescending<T>(T a, T b) where T : IComparable<T>
        {
            return b.CompareTo(a);
        }

        private static string GetFileVersions(string path)
        {
            const string partOfName = "MOTU M Series Installer";
            List<string> fileVersion = new List<string>();

            foreach (string file in Directory.GetFiles(path, "*.exe"))
            {
                if (file.Contains(partOfName))
                {
                    string version = NormalizeVersion(FileVersionInfo.GetVersionInfo(file).ProductVersion);
                    fileVersion.Add(version);
                }
            }

            fileVersion.Sort(OrderByDescending);
            return fileVersion.GetFirst() ?? "00000";
        }

        private static string FindHref(IDomElement element)
        {
            const string className = "mobile-only";

            for (IDomElement current = element; current != null; current = current.NextElementSibling)
            {
                if (current.ClassName.Equals(className) && current.FirstElementChild.HasAttribute("href"))
                {
                    return current.FirstElementChild.GetAttribute("href");
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

            drivers.Sort(OrderByDescending);
            return drivers.GetFirst();
        }

        static string GetProgressLine(int percentage)
        {
            int lineLength = 25;
            int percentPerSymbol = 100 / lineLength;
            char[] percentLine = new char[lineLength];

            for (int i = 0; i < lineLength; i++)
            {
                percentLine[i] = (i < (percentage / percentPerSymbol)) ? '#' : '-';
            }

            return new string(percentLine);
        }

        private static void Download(string address, string fileName)
        {
            int prevPercentage = -1;
            int top = Console.CursorTop;
            int left = Console.CursorLeft;

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
                    Console.WriteLine($"Your Version: {fileProductVersion}");
                    Console.WriteLine($"New Version:  {productVersion.Version}");
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
            int top = Console.CursorTop;
            int left = Console.CursorLeft;

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
            Console.Write($"Read url address from \"{fileName}\" file... ");

            try
            {
                string url = null;
                if (File.Exists(fileName))
                {
                    url = ReadUrlFromFile(fileName);
                }
                else
                {
                    Console.Write($"File not found. Set default url address...");
                    url = defaultURL;
                }

                Console.WriteLine("\nChecking for new drivers. Waiting...");
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