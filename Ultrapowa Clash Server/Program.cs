using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using UCS.Core;
using UCS.Helpers;
using UCS.Network;
using Debugger = UCS.Core.Debugger;
using Menu = UCS.Core.Menu;

namespace UCS
{
    internal class Program
    {
        public static readonly int port = Utils.parseConfigInt("serverPort");
        private static bool isclosing = false;
        [DllImport("Kernel32")]
        public static extern int GetConsoleWindow();
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(ExitHandler.CtrlType sig);
        static EventHandler _handler;

        private static void InitConsoleStuff()
        {
            Console.Title = GlobalStrings.__UCS;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(GlobalStrings.__ASCIIART);
            Console.WriteLine(GlobalStrings.__UCS);
            Console.WriteLine("Codename : {0}", GlobalStrings.__Codename);
            Console.WriteLine("www.ultrapowa.com");
            Console.WriteLine("");
            Console.WriteLine("Server starting...");
            Console.ResetColor();
        }

        private static void InitProgramThreads()
        {
            Debugger.WriteLine("\t", null, 5);
            Debugger.WriteLine("Server Thread's:", null, 5, ConsoleColor.Blue);
            var programThreads = new List<Thread>();
            for (var i = 0; i < int.Parse(ConfigurationManager.AppSettings["programThreadCount"]); i++)
            {
                var pt = new ProgramThread();
                programThreads.Add(new Thread(pt.Start));
                programThreads[i].Start();
                Debugger.WriteLine("\tServer Running On Thread " + i, null, 5, ConsoleColor.Blue);
            }
            Console.ResetColor();
        }

        public static void ExitProgram()
        {
            Debugger.WriteLine("Starting saving all player to database", null, 0, ConsoleColor.Cyan);
            foreach (var l in ResourcesManager.GetOnlinePlayers())
            {
                DatabaseManager.Singelton.Save(l);
            }
            Environment.Exit(1);
        }

        public static void RestartProgram()
        {
            Debugger.WriteLine("Starting saving all player to database", null, 0, ConsoleColor.Cyan);
            foreach (var l in ResourcesManager.GetOnlinePlayers())
            {
                DatabaseManager.Singelton.Save(l);
            }
            Process.Start(@"tools\ucs-restart.bat");
        }

        private static void InitUCS()
        {
            new ResourcesManager();
            new ObjectManager();
            new Gateway().Start();
            new ApiManager();
            if (!Directory.Exists("logs"))
            {
                Console.WriteLine("Folder \"logs/\" does not exist. Let me create one..");
                Directory.CreateDirectory("logs");
            }

            if (Convert.ToBoolean(Utils.parseConfigString("apiManagerPro")))
            {
                if (ConfigurationManager.AppSettings["ApiKey"] == "random")
                {
                    var api = new byte[201512222];
                    new Random().NextBytes(api);
                    SHA1 sha = new SHA1CryptoServiceProvider();
                    var ch = BitConverter.ToString(sha.ComputeHash(api)).Replace("-", "");
                    /*var random = new Random();
                    var ch = Convert.ToString(chars[random.Next(chars.Length)]);*/
                    ConfigurationManager.AppSettings.Set("ApiKey", ch);
                    Console.WriteLine("Your API key : {0}", ch);
                }
                var ws = new ApiManagerPro(ApiManagerPro.SendResponse,
                    "http://+:" + Utils.parseConfigInt("proDebugPort") + "/" + Utils.parseConfigString("ApiKey") + "/");
                Console.WriteLine("Your API key : {0}", Utils.parseConfigString("ApiKey"));
                ws.Run();
            }

            Debugger.SetLogLevel(Utils.parseConfigInt("loggingLevel"));
            Logger.SetLogLevel(Utils.parseConfigInt("loggingLevel"));

            InitProgramThreads();

            if (Utils.parseConfigInt("loggingLevel") >= 5)
            {
                Debugger.WriteLine("\t", null, 5);
                Debugger.WriteLine("Played ID's:", null, 5, ConsoleColor.Cyan);
                foreach (var id in ResourcesManager.GetAllPlayerIds())
                {
                    Debugger.WriteLine("\t" + id, null, 5, ConsoleColor.Cyan);
                }
                Debugger.WriteLine("\t", null, 5);
            }
            Console.WriteLine("Server started on port " + port + ". Let's play Clash of Clans!");     

            if (Convert.ToBoolean(Utils.parseConfigString("consoleCommand")))
            {
                new Menu();
            }
            else
            {
                Application.Run(new UCSManager());
            }
        }

        private static void Main(string[] args)
        {
            var win = GetConsoleWindow();
            if (Convert.ToBoolean(Utils.parseConfigString("guiMode")))
            {
                ShowWindow(win, 0);
                Application.Run(new UCSGui());
            }
            else
            {
                ShowWindow(win, 5);
                _handler += new EventHandler(ExitHandler.Handler);
                SetConsoleCtrlHandler(_handler, true);
                InitConsoleStuff();
                InitUCS();

            }
        }

        [DllImport("user32.dll")]
        private static extern int ShowWindow(int Handle, int showState);
    }
}