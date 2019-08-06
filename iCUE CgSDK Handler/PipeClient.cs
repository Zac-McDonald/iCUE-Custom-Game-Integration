using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;

namespace iCUE_CgSDK_Handler
{
    class PipeClient
    {
        private static string pre { get { return string.Format("{0} PipeClient -- ", DateTime.Now.ToString("[HH:mm:ss]")); } }

        public static void Run()
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "CgPipe", PipeDirection.InOut))
            {
                Console.WriteLine(pre + "Connecting to pipe...");
                pipeClient.Connect();

                try
                {
                    while (Program.running)
                    {
                        string response;

                        // Wait for data from the server
                        using (StreamReader sr = new StreamReader(pipeClient, Encoding.UTF8, false, 512, true))
                        {
                            string msg;
                            while ((msg = sr.ReadLine()) == null)
                            {
                                System.Threading.Thread.Sleep(10);
                            }

                            Console.WriteLine(pre + "Received message: {0}", msg);

                            // MESSAGE FORMAT: function_name|parameter
                            string[] parsedMsg = msg.Split('|');
                            response = PerformAction(parsedMsg);
                        }

                        // Send response to the server
                        using (StreamWriter sw = new StreamWriter(pipeClient, Encoding.UTF8, 512, true))
                        {
                            sw.WriteLine(response);
                            sw.Flush();
                        }
                    }
                }
                // Catch the error thrown when the pipe is broken or disconnected
                catch (IOException e)
                {
                    Console.WriteLine(pre + e.ToString());
                }
                catch (ObjectDisposedException e)
                {
                    Console.WriteLine(pre + e.ToString());
                }
            }
        }

        private static string PerformAction (string[] parameters)
        {
            switch (parameters[0].ToLower())
            {
                case "getlasterror":
                    return Program.GetLastError().ToString();

                case "performprotocolhandshake":
                    Program.PerformProtocolHandshake();
                    return BoolToString(true);

                case "requestcontrol":
                    return BoolToString(Program.RequestControl());

                case "releasecontrol":
                    return BoolToString(Program.ReleaseControl());

                case "setgame":
                    return BoolToString(Program.SetGame(parameters[1]));

                case "setstate":
                    return BoolToString(Program.SetState(parameters[1]));

                case "clearstate":
                    return BoolToString(Program.ClearState(parameters[1]));

                case "clearallstates":
                    return BoolToString(Program.ClearAllStates());

                case "setevent":
                    return BoolToString(Program.SetEvent(parameters[1]));

                case "clearallevents":
                    return BoolToString(Program.ClearAllEvents());
                
                default:
                    // No function provided
                    Console.WriteLine(pre + "Error - No valid function provided");
                    return BoolToString(false);
            }
        }

        private static string BoolToString (bool state)
        {
            if (state)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }
    }
}
