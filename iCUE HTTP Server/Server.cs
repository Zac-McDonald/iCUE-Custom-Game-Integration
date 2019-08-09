using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace iCUE_HTTP_Server
{
    class Server
    {
        // Set connection locations
        public static string boundIP = "127.0.0.1";
        public static string port = "25555";

        private static string pre { get { return string.Format("{0}       HTTP -- ", DateTime.Now.ToString("[HH:mm:ss]")); } }

        public static void Run ()
        {          
            // Connect to iCUE using CgSDK -- Now done via the pipe server
            //StateTracking.SetupCgSDK();

            // Setup and start the listener, add more prefixes for multiple access points
            HttpListener listener = new HttpListener();
            string prefix = string.Format("http://{0}:{1}/", boundIP, port);
            listener.Prefixes.Add(prefix);
            listener.Start();

            Console.WriteLine(pre + "HTTP Server listening to {0}", prefix);

            // Core processing loop for HTTP Requests
            while (Program.isRunning)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    // Process Request
                    //ShowRequestProperties(request);

                    // Extract information from url
                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    List<string> suburls = new List<string>();

                    foreach (string key in request.QueryString.AllKeys)
                    {
                        parameters.Add(key, request.QueryString.Get(key));
                    }

                    // Splitting the url up by '/' characters
                    Regex rx = new Regex(@"(?:\/([^\/^\?]+))");
                    MatchCollection matches = rx.Matches(request.RawUrl);
                    foreach (Match match in matches)
                    {
                        GroupCollection groups = match.Groups;
                        suburls.Add(groups[1].Value);
                    }

                    // Logic for handling the request
                    string responseString = ProcessRequest(suburls, parameters);

                    // Send the appropriate response
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);

                    output.Close();
                }
                catch (HttpListenerException e)
                {
                    // Do fuck all, we don't really care, but it probably will break the program
                }
            }

            listener.Stop();

            StateTracking.ReleaseControl();
            Console.WriteLine(pre + "Disconnecting from iCUE");
        }

        static string ProcessRequest(List<string> suburls, Dictionary<string, string> paramaters)
        {
            switch (suburls[0])
            {
                case "icue":
                    if (suburls.Count == 1)
                    {
                        return HandleICue(paramaters);
                    }
                    break;

                default:
                    break;
            }

            return "";
        }

        static string HandleICue(Dictionary<string, string> paramaters)
        {
            switch (paramaters["func"].ToLower())
            {
                case "getgame":
                    if (StateTracking.Games.Keys.Count > 0 && !String.IsNullOrWhiteSpace(StateTracking.CurrentGame))
                    {
                        string gameName = (paramaters.ContainsKey("game")) ? paramaters["game"] : StateTracking.CurrentGame;
                        if (StateTracking.Games.ContainsKey(gameName))
                        {
                            return JsonConvert.SerializeObject(StateTracking.Games[gameName], Formatting.Indented);
                        }
                    }
                    break;

                case "getallgames":
                    if (StateTracking.Games.Keys.Count > 0)
                    {
                        return JsonConvert.SerializeObject(StateTracking.Games, Formatting.Indented);
                    }
                    break;

                case "setgame":
                    if (paramaters.ContainsKey("game"))
                    {
                        if (StateTracking.SetGame(paramaters["game"]))
                        {
                            return StateTracking.ErrorToString(0);
                        }
                        else
                        {
                            return StateTracking.GetLastErrorString();
                        }
                    }
                    break;

                case "reset":
                    if (paramaters.ContainsKey("game"))
                    {
                        if (StateTracking.ResetGame(paramaters["game"]))
                        {
                            return StateTracking.ErrorToString(0);
                        }
                        else
                        {
                            return StateTracking.GetLastErrorString();
                        }
                    }
                    break;

                case "lock":
                    if (paramaters.ContainsKey("game"))
                    {
                        if (StateTracking.LockGame(paramaters["game"], true))
                        {
                            return StateTracking.ErrorToString(0);
                        }
                        else
                        {
                            return StateTracking.GetLastErrorString();
                        }
                    }
                    break;

                case "unlock":
                    if (paramaters.ContainsKey("game"))
                    {
                        if (StateTracking.LockGame(paramaters["game"], false))
                        {
                            return StateTracking.ErrorToString(0);
                        }
                        else
                        {
                            return StateTracking.GetLastErrorString();
                        }
                    }
                    break;

                case "setstate":
                    if (paramaters.ContainsKey("game") && paramaters.ContainsKey("state"))
                    {
                        if (StateTracking.SetState(paramaters["game"], paramaters["state"]))
                        {
                            return StateTracking.ErrorToString(0);
                        }
                        else
                        {
                            return StateTracking.GetLastErrorString();
                        }
                    }
                    break;

                case "clearstate":
                    if (paramaters.ContainsKey("game") && paramaters.ContainsKey("state"))
                    {
                        if (StateTracking.ClearState(paramaters["game"], paramaters["state"]))
                        {
                            return StateTracking.ErrorToString(0);
                        }
                        else
                        {
                            return StateTracking.GetLastErrorString();
                        }
                    }
                    break;

                case "clearallstates":
                    if (paramaters.ContainsKey("game"))
                    {
                        if (StateTracking.ClearAllStates(paramaters["game"]))
                        {
                            return StateTracking.ErrorToString(0);
                        }
                        else
                        {
                            return StateTracking.GetLastErrorString();
                        }
                    }
                    break;

                case "setevent":
                    if (paramaters.ContainsKey("game") && paramaters.ContainsKey("event"))
                    {
                        if (StateTracking.SetEvent(paramaters["game"], paramaters["event"]))
                        {
                            return StateTracking.ErrorToString(0);
                        }
                        else
                        {
                            return StateTracking.GetLastErrorString();
                        }
                    }
                    break;

                case "clearallevents":
                    if (paramaters.ContainsKey("game"))
                    {
                        if (StateTracking.ClearAllEvents(paramaters["game"]))
                        {
                            return StateTracking.ErrorToString(0);
                        }
                        else
                        {
                            return StateTracking.GetLastErrorString();
                        }
                    }
                    break;

                default:
                    // No function provided
                    Console.WriteLine(pre + "Error - No valid function provided, requested funtion ({0})", paramaters["func"].ToLower());
                    return StateTracking.ErrorToString(5);
            }

            return "";
        }

        static void ShowRequestProperties(HttpListenerRequest request)
        {
            // Display the MIME types that can be used in the response.
            string[] types = request.AcceptTypes;
            if (types != null)
            {
                Console.WriteLine(pre + "Acceptable MIME types:");
                foreach (string s in types)
                {
                    Console.WriteLine(s);
                }
            }
            // Display the language preferences for the response.
            types = request.UserLanguages;
            if (types != null)
            {
                Console.WriteLine(pre + "Acceptable natural languages:");
                foreach (string l in types)
                {
                    Console.WriteLine(l);
                }
            }

            // Display the URL used by the client.
            Console.WriteLine(pre + "URL: {0}", request.Url.OriginalString);
            Console.WriteLine(pre + "Raw URL: {0}", request.RawUrl);

            Regex rx = new Regex(@"(?:\/([^\/^\?]+))");
            MatchCollection matches = rx.Matches(request.RawUrl);

            Console.WriteLine(pre + "{0} Subs:", matches.Count);
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                Console.WriteLine(pre + "{0}", groups[1]);
            }

            Console.WriteLine(pre + "Query: {0}", request.QueryString);
            foreach (string key in request.QueryString.AllKeys)
            {
                Console.WriteLine(pre + "    {0} = {1}", key, request.QueryString.Get(key));
            }

            // Display the referring URI.
            Console.WriteLine(pre + "Referred by: {0}", request.UrlReferrer);

            //Display the HTTP method.
            Console.WriteLine(pre + "HTTP Method: {0}", request.HttpMethod);
            //Display the host information specified by the client;
            Console.WriteLine(pre + "Host name: {0}", request.UserHostName);
            Console.WriteLine(pre + "Host address: {0}", request.UserHostAddress);
            Console.WriteLine(pre + "User agent: {0}", request.UserAgent);
            Console.WriteLine(pre + " ----- ");
        }
    }
}
