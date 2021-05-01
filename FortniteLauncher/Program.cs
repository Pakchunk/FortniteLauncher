using Newtonsoft.Json;
using ProcessSuspend;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace FortniteLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Launch();
            }
            catch
            {
                Console.WriteLine("Something went wrong, the cause could not be specified.");
                Thread.Sleep(1400);
                Environment.Exit(0);
            }
        }


        public static void Launch()
        {
            Console.Title = "Fortnite Launcher by ozne";
            bool wincheck = Environment.OSVersion.Version < new Version(6, 2);
            if (wincheck)
            {
                Console.Title = "Error!";
                Console.WriteLine("A version of Windows other than Windows 10 has been detected, please upgrade as it is completely free!\nhttps://www.microsoft.com/en-us/software-download/windows10 :PeepoGlad:");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else { }
            string LAD = Environment.GetEnvironmentVariable("LocalAppData");

            if (Directory.Exists(LAD + @"\uFortniteLauncher"))
            {
            }
            else
            {
                Directory.CreateDirectory(LAD + @"\uFortniteLauncher");
                Console.WriteLine("Fortnite Path (directory that contains Engine and FortniteGame)");
                Console.Write("> ");
                string ReadPath = Console.ReadLine().ToLower();
                using (StreamWriter sw = File.CreateText(LAD + @"\uFortniteLauncher\Settings.config"))
                {
                    sw.WriteLine("Path");
                    sw.Write(ReadPath);
                    sw.Close();
                }
            }
            StreamReader sr = new StreamReader(LAD + @"\uFortniteLauncher\Settings.config");
            sr.ReadLine();
            WebClient webClient = new WebClient();
            var FLToken = webClient.DownloadString("https://api.nitestats.com/v1/epic/builds/fltoken");
            FLTokenJson json = JsonConvert.DeserializeObject<FLTokenJson>(FLToken);
            
            Console.Clear();
            Console.Write("FL Token: ");
            Console.Write(json.fltoken);
            Console.Write("\nFortnite Build: ");
            Console.Write(json.version);
            Console.WriteLine("\n\nExchange Code");
            Console.Write("> ");
            string exchange = Console.ReadLine().ToLower();
            if (exchange == "")
            {
                Console.WriteLine("Invalid exchange code, exiting...");
                Thread.Sleep(1200);
                Environment.Exit(0);
            }
            else { }
            string clientargs = $"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -NOSSLPINNING -noeac -fromfl=be -fltoken={json.fltoken} -frombe AUTH_TYPE=exchangecode -AUTH_LOGIN=unused -AUTH_PASSWORD={exchange}";
            string FortnitePath = sr.ReadLine();
            sr.Close();
            var client = new Process
            {
                StartInfo =
                {
                    FileName = FortnitePath + @"\FortniteGame\Binaries\Win64\FortniteClient-Win64-Shipping.exe",
                    Arguments = clientargs
                }
            };
            var eac = new Process
            {
                StartInfo =
                {
                    FileName = FortnitePath + @"\FortniteGame\Binaries\Win64\FortniteClient-Win64-Shipping_EAC.exe",
                    Arguments = clientargs
                }
            };
            var launcher = new Process
            {
                StartInfo =
                {
                    FileName = FortnitePath + @"\FortniteGame\Binaries\Win64\FortniteLauncher.exe",
                }
            };

            try
            {
                client.Start();
            }
            catch
            {
                Console.WriteLine("\nInvalid Fortnite Path!\n");
                Thread.Sleep(1100);
                Console.Clear();
                Console.WriteLine("Fortnite Path (directory that contains Engine and FortniteGame)");
                Console.Write("> ");
                string paththing = Console.ReadLine().ToLower();
                using (StreamWriter sw = File.CreateText(LAD + @"\uFortniteLauncher\Settings.config"))
                {
                    sw.WriteLine("Path");
                    sw.Write($"{paththing}");
                    sw.Close();
                }
                Console.Clear();
                Program.Launch();
                Environment.Exit(0);
            }
            launcher.Start();
            SuspendProcess.Suspend("FortniteLauncher");
            eac.Start();
            SuspendProcess.Suspend("FortniteClient-Win64-Shipping_EAC");

            client.WaitForExit();
            eac.Kill();
            launcher.Kill();

            Console.WriteLine("\nozne: This launcher");
            Console.WriteLine("VastBlast: NiteStats which is used for grabbing the FL Token");
            Thread.Sleep(2600);
        }
        
        
        public class FLTokenJson
        {
            public string version { get; set; }
            public string fltoken { get; set; }
        }
    }
}