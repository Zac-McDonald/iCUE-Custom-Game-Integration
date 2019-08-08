using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace iCUE_HTTP_Server
{
    class Linker
    {
        private static string pre { get { return string.Format("{0}     Linker -- ", DateTime.Now.ToString("[HH:mm:ss]")); } }

        private static string currentLinkedProcess = "";
        private static string previousLinkedProcess = "";
        private static bool linkedProcessClosed = true;
        // Key: Process name, Value: Active controller - null if none
        private static Dictionary<string, Process> activeControllers;

        // For logic related to focussed process
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static void Run()
        {
            // Create and populate our tracked controllers list
            activeControllers = new Dictionary<string, Process>();
            foreach (string processName in Settings.processControllers.Keys)
            {
                // Only add processes that have controllers associated with them -- processes without controllers are used to block other controllers in the case that their controller is external, ie. FarCry5
                if (!String.IsNullOrWhiteSpace(Settings.processControllers[processName].controller))
                {
                    activeControllers[processName] = null;
                }
            }

            while (Program.isRunning)
            {
                UpdateProcessLinkedProfiles();

                string[] activeProcesses = GetActiveProcesses();
                CleanupControllersWithoutProcesses(activeProcesses);
                CheckForClosedProcessLogic(activeProcesses);

                previousLinkedProcess = currentLinkedProcess;
                Thread.Sleep(200);
            }
        }

        // Checks periodically if the currently focussed window is a different linked process to the previously focussed + linked process
        // The aim is to only switch lighting effects when we refocus to another linked process, otherwise we let the last linked process keep its profile running
        private static void UpdateProcessLinkedProfiles()
        {
            string currentFocus = GetFocussedProcess();

            // If we have switched focus to another process
            // Note:    If a controller is manually launched (e.g. the documentation Javascript), refocussing to a linked process WILL NOT trigger AutoSetGame.
            //          This is by design to allow switching between unlinked processes without interupting lighting. We could trigger AutoSetGame whenever we
            //          refocus, however this unfortunently causes an undesirable flickering effect. Therefore use unlinked/manual controllers at your own risk.
            if (currentFocus != currentLinkedProcess)
            {
                // Is the new process another linked process
                if (Settings.processControllers.ContainsKey(currentFocus))
                {
                    Console.WriteLine(pre + "Switched focus to linked process: {0}", currentFocus);

                    if (!String.IsNullOrWhiteSpace(currentLinkedProcess) && activeControllers.ContainsKey(currentLinkedProcess) && Settings.processControllers[currentLinkedProcess].closeOnProfileSwitch)
                    {
                        KillController(currentLinkedProcess);
                    }

                    StartController(currentFocus);

                    // If we should AutoSetGame, do it now
                    if (!String.IsNullOrWhiteSpace(Settings.processControllers[currentFocus].autoSetGame))
                    {
                        StateTracking.SetGame(Settings.processControllers[currentFocus].autoSetGame);
                    }

                    // If we should enable LockSetGame
                    if (!String.IsNullOrWhiteSpace(Settings.processControllers[currentFocus].lockSetGame))
                    {
                        StateTracking.LockGame(Settings.processControllers[currentFocus].lockSetGame, true);
                    }

                    currentLinkedProcess = currentFocus;
                }
                // Switched to an unlinked process
                else
                {
                    // Nothing to do here... wait... Default controllers are AIDS!!
                }
            }
        }

        // Closes any controllers that no longer have their linked controller running
        private static void CleanupControllersWithoutProcesses (string[] activeProcesses)
        {
            List<string> keys = new List<string>(activeControllers.Keys);
            foreach (string processName in keys)
            {   
                // Close the controller if it is currently running AND we should close the controller with the process AND the process is not running
                if (Settings.processControllers.ContainsKey(processName) && Settings.processControllers[processName].closeWithProcess && !activeProcesses.Contains(processName))
                {
                    KillController(processName, "Cleaned up controller for: {0}");
                }  
            }
        }

        private static void CheckForClosedProcessLogic (string[] activeProcesses)
        {
            // If the previously focussed linked process was closed
            if (!linkedProcessClosed && !activeProcesses.Contains(previousLinkedProcess) && Settings.processControllers.ContainsKey(previousLinkedProcess))
            {
                // If we should disable LockSetGame
                if (!String.IsNullOrWhiteSpace(Settings.processControllers[previousLinkedProcess].lockSetGame))
                {
                    StateTracking.LockGame(Settings.processControllers[previousLinkedProcess].lockSetGame, false);
                }

                // If we should AutoSetGameOnClose
                if (!String.IsNullOrWhiteSpace(Settings.processControllers[previousLinkedProcess].autoSetGameOnClose))
                {
                    StateTracking.SetGame(Settings.processControllers[previousLinkedProcess].autoSetGameOnClose);
                }

                linkedProcessClosed = true;

                // If we have closed the linked process and have NOT switched to another, set currentLinkedProcess to none
                if (previousLinkedProcess == currentLinkedProcess)
                {
                    currentLinkedProcess = "";
                }
            }

            // Update linkedProcessClosed if we have switched to a new linked process
            if (previousLinkedProcess != currentLinkedProcess)
            {
                linkedProcessClosed = false;
            }
        }

        private static bool StartController (string processName)
        {
            if (activeControllers.ContainsKey(processName) && activeControllers[processName] == null)
            {
                // Start controller
                activeControllers[processName] = new Process();
                activeControllers[processName].StartInfo.Arguments = "";

                // Detect if controller is a python script
                if (Settings.processControllers[processName].controller.EndsWith(".py"))
                {
                    activeControllers[processName].StartInfo.FileName = "python";

                    // Setup arguments
                    activeControllers[processName].StartInfo.Arguments += string.Format(" {0} ", Settings.processControllers[processName].controller);
                }
                else
                {
                    activeControllers[processName].StartInfo.FileName = Settings.processControllers[processName].controller;
                }

                if (Settings.processControllers[processName].embedController)
                {
                    activeControllers[processName].StartInfo.UseShellExecute = false;
                }

                if (!String.IsNullOrWhiteSpace(Settings.processControllers[processName].commandLineArgs))
                {
                    activeControllers[processName].StartInfo.Arguments += Settings.processControllers[processName].commandLineArgs;
                }

                try
                {
                    activeControllers[processName].Start();
                    Console.WriteLine(pre + "Started controller for: {0}", processName);
                }
                catch (System.ComponentModel.Win32Exception w)
                {
                    Console.WriteLine(pre + "Failed to launch controller for: {0}", processName);

                    Console.WriteLine(w.Message);
                    Console.WriteLine(w.ErrorCode.ToString());
                    Console.WriteLine(w.NativeErrorCode.ToString());
                    Console.WriteLine(w.StackTrace);
                    Console.WriteLine(w.Source);
                    Exception e = w.GetBaseException();
                    Console.WriteLine(e.Message);

                    Console.WriteLine(pre + "End Error log");

                    activeControllers[processName] = null;
                    return false;
                }

                return true;
            }

            return false;
        }

        private static bool KillController(string processName, string reason = "Closed controller for: {0}")
        {
            if (activeControllers.ContainsKey(processName) && activeControllers[processName] != null)
            {
                if (!activeControllers[processName].HasExited)
                {
                    activeControllers[processName].Kill();
                }
                
                activeControllers[processName] = null;

                Console.WriteLine(pre + reason, processName);
                return true;
            }

            return false;
        }

        // Close all active controllers, ie. on application close
        public static void KillAllControllers ()
        {
            List<string> keys = new List<string>(activeControllers.Keys);
            foreach (string processName in keys)
            {
                if (activeControllers[processName] != null && !activeControllers[processName].HasExited)
                {
                    activeControllers[processName].Kill();
                }
            }
        }

        // Get the name of the currently focussed window
        private static string GetFocussedProcess()
        {
            // All in a try/catch because it's gross and often crashes the program
            try
            {
                IntPtr hwnd = GetForegroundWindow();

                if (hwnd == null || hwnd == IntPtr.Zero)
                {
                    return "Unknown";
                }

                uint pid;
                GetWindowThreadProcessId(hwnd, out pid);

                Process p = Process.GetProcessById((int)pid);
                if (!String.IsNullOrWhiteSpace(p.ProcessName))
                {
                    return p.ProcessName;
                }
                else
                {
                    return "Unknown";
                }
            }
            catch
            {
                return "Unknown";
            }
        }

        // Gets a list of all active processes
        private static string[] GetActiveProcesses()
        {
            Process[] processes = Process.GetProcesses();
            string[] processNames = new string[processes.Length];

            for (int i = 0; i < processes.Length; i++)
            {
                processNames[i] = processes[i].ProcessName;
            }

            return processNames;
        }
    }
}
