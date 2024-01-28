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
        private static readonly Configuration config = new Configuration();

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
            string rootAddress = GetRootAddress(url);
            return path.StartsWith('/') ? rootAddress + path : rootAddress + '/' + path;
        }

        private static bool IsDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        private static string NormalizeVersion(string version)
        {
            StringBuilder result = new StringBuilder(10);
            foreach (char ch in version)
            {
                if (IsDigit(ch))
                {
                    result.Append(ch);
                }
            }
            return result.ToString();
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
            return fileVersion.GetFirst("00000");
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

        private static void GetContent(IDomElement element, List<Driver> driversList)
        {
            const string prefix = "PC v";
            string content = element.InnerText;

            if (content.Contains(prefix) && !content.Contains("."))
            {
                string name = "Motu";
                string version = content.Replace(prefix, null);
                string href = FindHref(element);
                driversList.Add(new Driver(name, version, href));
            }
        }

        private static Driver GetDriverVersion(string source)
        {
            const string selector = ".platform-logos";
            List<Driver> driversList = new List<Driver>();
            CQ cq = new CQ(selector, source);

            foreach (IDomObject item in cq)
            {
                GetContent(item.NextElementSibling, driversList);
            }

            driversList.Sort(OrderByDescending);
            return driversList.GetFirst();
        }

        private static string GetProgressLine(int percentage)
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

        private static bool Download(string address, string fileName)
        {
            int prevPercentage = 0;
            int currentPercentage = 0;
            int top = Console.CursorTop;
            int left = Console.CursorLeft;

            webClient.DownloadProgressChanged += (s, e) =>
            {
                currentPercentage = e.ProgressPercentage;
                if (currentPercentage != prevPercentage)
                {
                    Console.WriteLine($"Downloading {currentPercentage}% {GetProgressLine(currentPercentage)}");
                    Console.SetCursorPosition(left, top);
                    prevPercentage = currentPercentage;
                }
            };

            webClient.DownloadFileTaskAsync(address, fileName).Wait();
            return currentPercentage == 100;
        }

        private static void DownloadFileDialog(string url, Driver productVersion)
        {
            Console.WriteLine("Press 'D' for download a new version or 'O' for visit a website page.");
            char inputKey = Console.ReadKey(true).KeyChar;

            switch (inputKey)
            {
                case 'd':
                case 'в':
                case 'D':
                case 'В':
                    string href = CombineAddress(url, productVersion.Href);
                    string fileName = $"MOTU M Series Installer ({productVersion.Version}).exe";
                    if (Download(href, fileName))
                    {
                        Console.WriteLine("\nDownload completed.");
                        OpenFileDialog(fileName);
                    }
                    break;
                case 'o':
                case 'щ':
                case 'O':
                case 'Щ':
                    Console.WriteLine("Opening website.");
                    using (Process.Start(url)) { };
                    break;
                default:
                    break;
            }
        }

        private static void OpenFileDialog(string fileName)
        {
            Console.WriteLine("Press 'O' for opening downloaded file.");
            char inputKey = Console.ReadKey(true).KeyChar;

            switch (inputKey)
            {
                case 'o':
                case 'щ':
                case 'O':
                case 'Щ':
                    using (Process.Start(fileName)) { };
                    break;
                default:
                    break;
            }
        }

        private static void CheckVersion(string url)
        {
            try
            {
                string source = webClient.DownloadString(url);
                Driver productVersion = GetDriverVersion(source);
                string fileVersion = GetFileVersions(Environment.CurrentDirectory);

                if (productVersion == null || productVersion.IsEmpty())
                {
                    Console.WriteLine("Unable to find new driver version.");
                }
                else
                {
                    Console.WriteLine($"Your Version: {fileVersion}");
                    Console.WriteLine($"New Version:  {productVersion.Version}");

                    if (productVersion.CompareTo(fileVersion) < 1)
                    {
                        Console.WriteLine("You already have the LATEST drivers.");
                    }
                    else
                    {
                        Console.WriteLine("There is a NEW VERSION drivers!!!");
                        DownloadFileDialog(url, productVersion);
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        private static void WaitForExit(int seconds)
        {
            int top = Console.CursorTop;
            int left = Console.CursorLeft;

            Task task = new Task(() =>
            {
                for (int i = seconds; i > 0; i--)
                {
                    Console.SetCursorPosition(left, top);
                    Console.Write($"Waiting {i} seconds for Exit...");
                    Thread.Sleep(1000);
                }
                Environment.Exit(0);
            });

            task.Start();
        }

        private static void Main()
        {
            string address = string.Empty;
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls12 |
                SecurityProtocolType.Ssl3;

            try
            {
                config.Read();
                address = config.Properties.Address;
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), config.Properties.ForegroundColor);
                Console.BackgroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), config.Properties.BackgroundColor);
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
            }
            finally
            {
                config.Properties.ForegroundColor = Console.ForegroundColor.ToString();
                config.Properties.BackgroundColor = Console.BackgroundColor.ToString();
                config.Write();
            }

            try
            {
                Console.WriteLine("Checking for a new version drivers. Waiting...");
                CheckVersion(address);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            WaitForExit(5);
            Console.ReadKey();
        }
    }
}