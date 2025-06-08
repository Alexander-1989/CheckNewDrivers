using CsQuery;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using CheckNewDrivers.Service;
using CheckNewDrivers.Service.Serializer;

namespace CheckNewDrivers
{
    class Program
    {
        private static readonly WebClient webClient = new WebClient();
        private static readonly Configuration config = new Configuration();
        private static readonly IntPtr hWnd = NativeMethods.GetConsoleWindow();
        private static string address = string.Empty;

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

        private static int OrderByAscending<T>(T a, T b) where T : IComparable<T>
        {
            return a.CompareTo(b);
        }

        private static int OrderByDescending<T>(T a, T b) where T : IComparable<T>
        {
            return b.CompareTo(a);
        }

        private static List<string> GetFileVersions(string path)
        {
            const string partOfName = "MOTU M Series";
            List<string> fileVersion = new List<string>();

            foreach (string fileName in Directory.GetFiles(path, "*.exe", SearchOption.AllDirectories))
            {
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(fileName);
                if (fileVersionInfo.ProductName != null && fileVersionInfo.ProductName.Contains(partOfName))
                {
                    string productVersion = NormalizeVersion(fileVersionInfo.ProductVersion);
                    fileVersion.Add(productVersion);
                }
            }

            fileVersion.Sort(OrderByAscending);
            return fileVersion;
        }

        private static void GetContent(IDomElement element, List<Driver> driversList)
        {
            string className = "mobile-only";
            string prefix = "PC v";
            string content = element.InnerText;

            if (content.Contains(prefix) && !content.Contains("."))
            {
                string name = "Motu";
                string version = content.Replace(prefix, null);
                string href = string.Empty;

                for (IDomElement currentElement = element; currentElement != null; currentElement = currentElement.NextElementSibling)
                {
                    if (currentElement.ClassName.Equals(className) && currentElement.FirstElementChild.HasAttribute("href"))
                    {
                        href = currentElement.FirstElementChild.GetAttribute("href");
                        break;
                    }
                }

                Driver driver = new Driver(name, version, href);
                driversList.Add(driver);
            }
        }

        private static List<Driver> GetDriverVersion(string source)
        {
            string selector = ".platform-logos";
            List<Driver> driversList = new List<Driver>();
            CQ cq = new CQ(selector, source);

            foreach (IDomObject item in cq)
            {
                GetContent(item.NextElementSibling, driversList);
            }

            driversList.Sort(OrderByDescending);
            return driversList;
        }

        private static string GetProgressLine(int percentage)
        {
            int lineLength = 25;
            int percentPerSymbol = 100 / lineLength;
            StringBuilder percentLine = new StringBuilder(lineLength);

            for (int i = 0; i < lineLength; i++)
            {
                char symbol = i < percentage / percentPerSymbol ? '#' : '-';
                percentLine.Append(symbol);
            }

            return percentLine.ToString();
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

        private static void ShowDriversList<T>(IEnumerable<T> items)
        {
            bool first = true;
            foreach (T item in items)
            {
                if (first)
                {
                    first = false;
                    Console.WriteLine($"All your versions: {item, 10}");
                }
                else
                {
                    Console.WriteLine($"{item, 29}");
                }
            }
        }

        private static void CheckVersion(string url)
        {
            try
            {
                string source = webClient.DownloadString(url);

                List<Driver> productVersionsArray = GetDriverVersion(source);
                List<string> fileVersionsArray = GetFileVersions(Environment.CurrentDirectory);

                Driver firstProductVersion = productVersionsArray.GetFirst();
                string lastFileVersion = fileVersionsArray.GetLast("00000");

                if (firstProductVersion == null || firstProductVersion.IsEmpty())
                {
                    Console.WriteLine("Unable to find new driver version.");
                }
                else
                {
                    if (fileVersionsArray.Count > 1)
                    {
                        ShowDriversList(fileVersionsArray);
                    }

                    Console.WriteLine($"Your Last Version: {lastFileVersion, 10}");
                    Console.WriteLine($"New Version:  {firstProductVersion.Version, 15}");

                    if (firstProductVersion.Compare(lastFileVersion))
                    {
                        System.Media.SystemSounds.Beep.Play();
                        ConsoleColor lastForegroundColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("There is a NEW VERSION drivers!!!");
                        Console.ForegroundColor = lastForegroundColor;
                        DownloadFileDialog(url, firstProductVersion);
                    }
                    else
                    {
                        Console.WriteLine("You already have the LATEST drivers.");
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

            Task exitTask = new Task(() =>
            {
                for (int i = seconds; i > 0; i--)
                {
                    Console.SetCursorPosition(left, top);
                    Console.Write($"Waiting {i} seconds for Exit...");
                    Thread.Sleep(1000);
                }

                WriteProperties();
                Environment.Exit(0);
            });

            exitTask.Start();
        }

        private static void SetSecurityProtocol()
        {
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls12 |
                SecurityProtocolType.Ssl3;
        }

        private static void ReadProperties()
        {
            if (config.Read())
            {
                NativeMethods.SetWindowPos
				(
                    hWnd,
                    NativeMethods.SWPInsertAfter.TOP,
                    config.Properties.Location.X,
                    config.Properties.Location.Y,
                    config.Properties.Size.Width,
                    config.Properties.Size.Height,
                    NativeMethods.SWPFlags.SHOWWINDOW
				);
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), config.Properties.ForegroundColor);
                Console.BackgroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), config.Properties.BackgroundColor);
            }
            address = config.Properties.Address;
        }

        private static void WriteProperties()
        {
            NativeMethods.GetWindowRect(hWnd, out Rectangle rectangle);
            config.Properties.Location = rectangle.Location;
            config.Properties.Size = rectangle.Size;
            config.Properties.ForegroundColor = Console.ForegroundColor.ToString();
            config.Properties.BackgroundColor = Console.BackgroundColor.ToString();
            config.Write();
        }

        private static void Main()
        {
            SetSecurityProtocol();

            try
            {
                ReadProperties();
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
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
            WriteProperties();
        }
    }
}