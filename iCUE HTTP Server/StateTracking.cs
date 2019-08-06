using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

//using CgSDK_Interop;
//using Cg = CgSDK_Interop.CgSDK;

namespace iCUE_HTTP_Server
{
    public class StateTracking
    {
        public static string CurrentGame;
        public static bool Locked = false;
        public static Dictionary<string, GameState> Games = new Dictionary<string, GameState>();

        private static string pre { get { return string.Format("{0}    Tracker -- ", DateTime.Now.ToString("[HH:mm:ss]")); } }

        public static bool ResetGame(string gameName)
        {
            bool success = ClearAllStates(gameName) & ClearAllEvents(gameName);
            Games[gameName].Reset();
            Console.WriteLine(pre + "Reset the game: " + gameName);
            return success;
        }

        public static bool LockGame(string gameName, bool state)
        {
            if (Locked && CurrentGame == gameName && !state)
            {
                Locked = false;
                Console.WriteLine(pre + "Unlocked SetGame from: " + gameName);
                return true;
            }
            else if (!Locked && CurrentGame == gameName && state)
            {
                Locked = true;
                Console.WriteLine(pre + "Locked SetGame to: " + gameName);
                return true;
            }

            return false;
        }

        public static bool ValidateGame(string gameName)
        {
            // Check if setting to a valid game
            if (!System.IO.Directory.GetDirectories(Program.CgSDKGamesDir).Contains(string.Format("{0}\\{1}", Program.CgSDKGamesDir, gameName)))
            {
                Console.WriteLine(pre + "Unable to validate game \"{0}\" because it lacks profiles", gameName);
                return false;
            }

            if (!Games.ContainsKey(gameName))
            {
                Games[gameName] = new GameState(gameName);
            }

            return true;
        }

        public static string ErrorToString(int error)
        {
            switch (error)
            {
                case 0:
                    return "CE_Success";
                case 1:
                    return "CE_ServerNotFound";
                case 2:
                    return "CE_NoControl";
                case 3:
                    return "CE_ProtocolHandshakeMissing";
                case 4:
                    return "CE_IncompatibleProtocol";
                case 5:
                    return "CE_InvalidArguments";
                case 6:
                    return "THE MYSTICAL ERROR 6 -- WTF CAUSED THIS!!";
                case 7:
                    return "CE_MissingPrioritiesFile";
                default:
                    return String.Format("Unknown Error Code [{0}]", error);
            }
        }

        public static int GetLastError() { return PipeServer.IntFunction("getlasterror"); }
        public static string GetLastErrorString(string foreword = "CgSDK Error: {0}")
        {
            int error = GetLastError();
            if (error != 0)
            {
                return string.Format(foreword, ErrorToString(error));
            }

            return "";
        }
        public static bool PrintIfLastError(string foreword = "CgSDK Error: {0}")
        {
            int error = GetLastError();
            if (error != 0)
            {
                Console.WriteLine(string.Format(foreword, ErrorToString(error)));
                return true;
            }

            return false;
        }

        public static void PerformProtocolHandshake() { PipeServer.BoolFunction("performprotocolhandshake"); }
        public static bool RequestControl() { return PipeServer.BoolFunction("requestcontrol"); }
        public static bool ReleaseControl() { return PipeServer.BoolFunction("releasecontrol"); }

        public static void SetupCgSDK()
        {
            Console.WriteLine(pre + "Setting up CgSDK");
            PerformProtocolHandshake();

            if (!PrintIfLastError("Failed to access iCUE, reason: {0}"))
            {
                Console.WriteLine(pre + "Successfully connected to iCUE");
                RequestControl();
            }
        }

        public static bool SetGame(string gameName)
        {
            if (Locked && CurrentGame != gameName)
            {
                return false;
            }

            if (gameName.ToUpper() == "ICUE")
            {
                // Use gameName = "iCUE" to not use the SDK -> Freeing up the iCUE Software to work as normal
                Console.WriteLine(pre + "Set Game to: iCUE - Releasing software");
                PipeServer.ResetClient();

                if (!Games.ContainsKey("iCUE"))
                {
                    Games["iCUE"] = new GameState("iCUE");
                }

                CurrentGame = "iCUE";
                return true;
            }
            else if (!ValidateGame(gameName))
            {
                return false;
            }
            else
            {
                PipeServer.ResetClient();
                SetupCgSDK();
            }

            bool success = PipeServer.BoolFunction(string.Format("setgame|{0}", gameName));

            CurrentGame = (success) ? gameName : CurrentGame;
            if (success)
            {
                if (!Games.ContainsKey(gameName))
                {
                    // Add game and validate it
                    Games[gameName] = new GameState(gameName);
                }
                else
                {
                    // Set game and validate all states
                    List<string> invalidStates = new List<string>();
                    foreach (KeyValuePair<string, bool> state in Games[gameName].currentStates)
                    {
                        bool valid = false;

                        // If state is true and validated with iCUE, keep it
                        if (state.Value == true && PipeServer.BoolFunction(string.Format("setstate|{0}", state.Key)))
                        {
                            valid = true;
                        }

                        if (!valid)
                        {
                            // Delete invalid states
                            invalidStates.Add(state.Key);
                        }
                    }

                    foreach (string stateName in invalidStates)
                    {
                        Games[gameName].currentStates.Remove(stateName);
                    }
                }
                Console.WriteLine(pre + "Set Game to: " + gameName);
            }
            else
            {
                PrintIfLastError(string.Format("Failed to set game to \"{0}\", error: {{0}}", gameName));
            }
            return success;
        }

        public static bool SetState(string gameName, string stateName)
        {
            if (!ValidateGame(gameName))
            {
                return false;
            }

            // If the key exists and is true, it has been (or will be) validated, so no need to validate it now
            if (Games[gameName].currentStates.ContainsKey(stateName) && Games[gameName].currentStates[stateName])
            {
                return true;
            }

            Console.WriteLine(pre + "Set State ({0}): {1}", gameName, stateName);

            if (CurrentGame == gameName)
            {
                // If current game, then validate with iCUE
                bool success = PipeServer.BoolFunction(string.Format("setstate|{0}", stateName));
                if (success)
                {
                    Games[gameName].SetState(stateName, true);
                }
                return success;
            }
            else
            {
                // Set it now, validate on change games
                Games[gameName].SetState(stateName, true);
                return true;
            }
        }

        public static bool SetEvent(string gameName, string eventName)
        {
            if (!ValidateGame(gameName))
            {
                return false;
            }

            Console.WriteLine(pre + "Triggered Event ({0}): {1}", gameName, eventName);

            // Only set event if current game
            if (CurrentGame == gameName)
            {
                bool success = PipeServer.BoolFunction(string.Format("setevent|{0}", eventName));
                if (success)
                {
                    Games[gameName].TriggerEvent(eventName);
                }
                return success;
            }
            else
            {
                Games[gameName].TriggerEvent(eventName);
                return true;
            }
        }

        public static bool ClearState(string gameName, string stateName)
        {
            if (!ValidateGame(gameName))
            {
                return false;
            }

            // If the key exists and is false, it has been (or will be) validated, so no need to validate it now
            if (Games[gameName].currentStates.ContainsKey(stateName) && !Games[gameName].currentStates[stateName])
            {
                return true;
            }

            Console.WriteLine(pre + "Clear State ({0}): {1}", gameName, stateName);

            if (CurrentGame == gameName)
            {
                // If current game, then validate with iCUE
                bool success = PipeServer.BoolFunction(string.Format("clearstate|{0}", stateName));
                if (success)
                {
                    Games[gameName].SetState(stateName, false);
                }
                return success;
            }
            else
            {
                // Set it now, validate on change games
                Games[gameName].SetState(stateName, false);
                return true;
            }
        }

        public static bool ClearAllStates(string gameName)
        {
            if (!ValidateGame(gameName))
            {
                return false;
            }

            Console.WriteLine(pre + "Clearing All States ({0})", gameName);

            if (CurrentGame == gameName)
            {
                bool success = PipeServer.BoolFunction("clearallstates");
                if (success)
                {
                    List<string> keys = Games[gameName].currentStates.Keys.ToList();
                    foreach (string key in keys)
                    {
                        Games[gameName].SetState(key, false);
                    }
                }
                return success;
            }
            else
            {
                List<string> keys = Games[gameName].currentStates.Keys.ToList();
                foreach (string key in keys)
                {
                    Games[gameName].SetState(key, false);
                }
                return true;
            }
        }

        public static bool ClearAllEvents(string gameName)
        {
            if (!ValidateGame(gameName))
            {
                return false;
            }

            if (CurrentGame == gameName)
            {
                Console.WriteLine(pre + "Clearing All Events ({0})", gameName);
                return PipeServer.BoolFunction("clearallevents");
            }

            return true;
        }

        public class GameState
        {
            public string name;
            public Dictionary<string, bool> currentStates;
            public string lastEventTriggered;

            public GameState(string gameName)
            {
                name = gameName;
                currentStates = new Dictionary<string, bool>();
                lastEventTriggered = "";
            }

            public void SetState(string stateName, bool state)
            {
                currentStates[stateName] = state;
            }

            public void TriggerEvent(string eventName)
            {
                lastEventTriggered = eventName;
            }

            public void Reset()
            {
                currentStates = new Dictionary<string, bool>();
                lastEventTriggered = "";
            }
        }
    }
}
