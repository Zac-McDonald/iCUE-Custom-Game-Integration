using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;

namespace iCUE_HTTP_Server
{
    class Program
    {
#if Debug
        public const string CgSDKHandlerDir = "../../../iCUE CgSDK Handler/bin/Debug/iCUE CgSDK Handler.exe";
#else
        public const string CgSDKHandlerDir = "./iCUE CgSDK Handler.exe";
#endif

        public const string CgSDKGamesDir = "C:/ProgramData/Corsair/CUE/GameSdkEffects";
        public static bool isRunning;

        private static string pre { get { return string.Format("{0}       Main -- ", DateTime.Now.ToString("[HH:mm:ss]"));  } }

        // For logic when the window is closed
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
        private delegate bool EventHandler(CtrlType sig);
        private static EventHandler exitHandler;

        private static void Main (string[] args)
        {
            Console.WriteLine(pre + "Starting up iCUE (unofficial) Game Integration");
            
            // If any required files are missing, alert the user of which ones and terminate the application
            if (!CheckRequiredFiles())
            {
                Console.WriteLine(pre + "Missing Files or Privilege, press any key to exit...");
                Console.ReadKey();
                return;
            }

            // Setup logic for handling of closing the window
            exitHandler += new EventHandler(OnAppExit);
            SetConsoleCtrlHandler(exitHandler, true);

            // Load the settings file to the static fields of Settings.cs
            Settings.LoadSettings();

            // Start all of the threads responsible for the program:
            // - linkCheck keeps track of currently open processes, used for linking controllers to processes
            // - pipeServer establishes a connection to the CgSDK Handler program which interacts with CgSDK and by extension iCUE
            // - httpServer handles http requests made by controllers (or the user) to control profiles
            isRunning = true;

            Thread linkCheck = new Thread(new ThreadStart(Linker.Run));
            linkCheck.Start();

            Thread pipeServer = new Thread(new ThreadStart(PipeServer.Run));
            pipeServer.Start();

            Thread httpServer = new Thread(new ThreadStart(Server.Run));
            httpServer.Start();

            StateTracking.SetGame("iCUE");
        }

        // Throw error messages if any required files are missing due to user error
        private static bool CheckRequiredFiles ()
        {
            bool success = true;
            Action<string> error = (string input) => { Console.WriteLine(pre + "Missing File: {0}", input); };

            // Check privilege
            bool IsAdmin = new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent()).IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            if (!IsAdmin)
            {
                Console.WriteLine(pre + "This program needs to be run as an administrator to be able to host the HTTP and Pipes servers responsible for its operation. Please relaunch as an administrator.");
                success = false;
            }

            // Check files
            if (!File.Exists(CgSDKHandlerDir))
            {
                error("CgSDK Handler program");
                success = false;
            }
            if (!File.Exists("./CgSDK.x64_2015.dll"))
            {
                error("CgSDK library");
                success = false;
            }
            if (!File.Exists("./CgSDK_Interop.dll"))
            {
                error("CgSDK Interoperability library");
                success = false;
            }
            if (!File.Exists("./Newtonsoft.Json.dll"))
            {
                error("Newtonsoft JSON library");
                success = false;
            }

            return success;
        }

        // Runs when the application terminates, used to close all the controllers and current CgSDK Handler
        private static bool OnAppExit(CtrlType sig)
        {
            Linker.KillAllControllers();
            PipeServer.CleanUpClients();  
            isRunning = false;

            bool handled = true;
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    handled = true;
                    break;
                default:
                    return handled;
            }

            return handled;
        }

        // Codes for application termination
        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
    }
}
