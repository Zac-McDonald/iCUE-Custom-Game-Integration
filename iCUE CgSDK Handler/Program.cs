using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cg = CgSDK_Interop.CgSDK;

namespace iCUE_CgSDK_Handler
{
    class Program
    {
        public static bool running;

        private static string pre { get { return string.Format("{0}    Handler -- ", DateTime.Now.ToString("[HH:mm:ss]")); } }

        static void Main(string[] args)
        {
            running = true;
            PipeClient.Run();
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
                default:
                    return "unknown error";
            }
        }

        public static int GetLastError() { return Cg.GetLastError(); }
        public static void PerformProtocolHandshake() { Cg.PerformProtocolHandshake(); }
        public static bool RequestControl() { return Cg.RequestControl(); }
        public static bool ReleaseControl() { return Cg.ReleaseControl(); }

        public static bool SetGame(string gameName)
        {
            return Cg.SetGame(gameName);
        }

        public static bool SetState(string stateName)
        {
            return Cg.SetState(stateName);
        }

        public static bool SetEvent(string eventName)
        {
            return Cg.SetEvent(eventName);
        }

        public static bool ClearState(string stateName)
        {
            return Cg.ClearState(stateName);
        }

        public static bool ClearAllStates()
        {
            return Cg.ClearAllStates();
        }

        public static bool ClearAllEvents()
        {
            return Cg.ClearAllEvents();
        }
    }
}
