using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using ProcessSuspend;
using JSON;

namespace FortniteLauncher
{
    public class Program
    {
        static readonly string LocalAppData = Environment.GetEnvironmentVariable("LocalAppData");
        static readonly bool WinCheck = Environment.OSVersion.Version < new Version(6, 2);
        static string Arguments = "TBD";

        static void Main()
        {
            Console.Title = "Fortnite Launcher";
            if (WinCheck)
            {
                Console.Title = "Error!";
                Console.WriteLine("A version of Windows other than Windows 10 has been detected, please upgrade as it is completely free!\nhttps://www.microsoft.com/en-us/software-download/windows10 :PeepoGlad:");
                Console.ReadKey();
                Environment.Exit(0);
            }
            if (File.Exists(LocalAppData + @"\uFortniteLauncher\Settings.json"))
            { }
            else { ConfigurateFortnitePath(); }

            WebClient webClient = new WebClient();
            var FLToken = webClient.DownloadString("https://api.nitestats.com/v1/epic/builds/fltoken");
            webClient.Dispose();
            FLTokenJson json = JsonConvert.DeserializeObject<FLTokenJson>(FLToken);

            Console.Clear();
            Console.WriteLine("FL Token: " + json.fltoken);
            Console.WriteLine("Fortnite Build: " + json.version);
            Console.WriteLine("\nIf you would like to change your Fortnite path, please enter the letter \'c\' instead of your Exchange Code.");
            Console.WriteLine("\n\nExchange Code");
            Console.Write("> ");
            string exchange = Console.ReadLine();
            if (exchange == "" || exchange.Contains(" ") || exchange.Contains("~") || exchange.Contains("-") || exchange.Contains("+") || exchange.Contains("`") || exchange.Contains("!") || exchange.Contains("@") || exchange.Contains("#") || exchange.Contains("$") || exchange.Contains("%") || exchange.Contains("^") || exchange.Contains("&") || exchange.Contains("*") || exchange.Contains("(") || exchange.Contains(")") || exchange.Contains("_") || exchange.Contains("=") || exchange.Contains("[") || exchange.Contains("]") || exchange.Contains("{") || exchange.Contains("}") || exchange.Contains("|") || exchange.Contains(@"\") || exchange.Contains(";") || exchange.Contains(":") || exchange.Contains("\'") || exchange.Contains("\"") || exchange.Contains(",") || exchange.Contains(".") || exchange.Contains("<") || exchange.Contains(">") || exchange.Contains("?") || exchange.Contains("/"))
            {
                Console.WriteLine("Invalid Exchange Code!");
                Thread.Sleep(1300);
                Environment.Exit(0);
            }
            if (exchange == "c")
            {
                Console.Clear();
                ConfigurateFortnitePath();
                Main();
                Environment.Exit(0);
            }

            string GetPath = File.ReadAllText(LocalAppData + @"\uFortniteLauncher\Settings.json");
            Settings settings = JsonConvert.DeserializeObject<Settings>(GetPath);

            Arguments = $"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -NOSSLPINNING -noeac -fromfl=be -fltoken={json.fltoken} -frombe AUTH_TYPE=exchangecode -AUTH_LOGIN=unused -AUTH_PASSWORD={exchange}";
            var client = new Process
            {
                StartInfo =
                {
                    FileName = settings.Path + @"\FortniteGame\Binaries\Win64\FortniteClient-Win64-Shipping.exe",
                    Arguments = Arguments,
                    UseShellExecute = false
                }
            };
            var eac = new Process
            {
                StartInfo =
                {
                    FileName = settings.Path + @"\FortniteGame\Binaries\Win64\FortniteClient-Win64-Shipping_EAC.exe",
                    Arguments = Arguments,
                    UseShellExecute = false
                }
            };
            var launcher = new Process
            {
                StartInfo =
                {
                    FileName = settings.Path + @"\FortniteGame\Binaries\Win64\FortniteLauncher.exe",
                    UseShellExecute = false
                }
            };

            try
            {
                client.Start();
            }
            catch
            {
                Console.Clear();
                Console.WriteLine("\nInvalid Fortnite Path!\n");
                Thread.Sleep(1100);
                Console.Clear();
                ConfigurateFortnitePath();
                Console.Clear();
                Main();
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

        public static void ConfigurateFortnitePath()
        {
            try
            { Directory.Delete(LocalAppData + @"\uFortniteLauncher", true); }
            catch { }
            Directory.CreateDirectory(LocalAppData + @"\uFortniteLauncher");
            Console.WriteLine("Fortnite Path (Directory that contains Engine and FortniteGame)");
            Console.Write("> ");
            string ReadPath = Console.ReadLine();
            using (StreamWriter sw = File.CreateText(LocalAppData + @"\uFortniteLauncher\Settings.json"))
            {
                sw.WriteLine("{");
                sw.WriteLine($"  \"Path\": \"{ReadPath}\"");
                sw.Write("}");
                sw.Close();
            }
            string ReplaceThings = File.ReadAllText(LocalAppData + @"\uFortniteLauncher\Settings.json");
            ReplaceThings = ReplaceThings.Replace(@"\", @"\\");
            File.WriteAllText(LocalAppData + @"\uFortniteLauncher\Settings.json", ReplaceThings);
            return;
        }
    }
}
